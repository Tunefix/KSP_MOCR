#!/usr/bin/env python
# -*- coding: utf-8 -*-

# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>
#
# PySSSMQ_server (Server daemon)
# Copyright (c) 2017 Lars Andre Landås (landas@gmail.com)
#
# TODO: Add option to make the debug message optional
#

 
import socket
import sys
import os
import signal
from random import randint
from thread import *
 
HOST = ''   # Symbolic name meaning all available interfaces
PORT = 8778 # Arbitrary non-privileged port

class PySSSMQ_server:

    server = None
    client_list = None
    data = None

    def __init__(self):
        self.server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    
    def run(self):
        print 'Socket created'
 
        #Bind socket to local host and port
        try:
            self.server.bind((HOST, PORT))
        except socket.error as msg:
            print 'Bind failed. Error Code : ' + str(msg[0]) + ' Message ' + msg[1]
            sys.exit()
     
        print 'Socket bind complete'

        self.data = PySSSMQ_data()
        self.client_list = PySSSMQ_message_queue(self.data)

        #Start listening on socket
        self.server.listen(10)
        print 'Socket now listening'

        #now keep talking with the client
        while 1:
            #wait to accept a connection - blocking call
            print "Waiting for connection..."
            conn, addr = self.server.accept()
            print 'Connected with ' + addr[0] + ':' + str(addr[1])
            
            #start new thread takes 1st argument as a function name to be run, second is the tuple of arguments to the function.
            start_new_thread(self.run_new_connection ,(conn, addr))

    def run_new_connection(self, conn, addr):
        client = PySSSMQ_client_connection(conn, addr, self.client_list)
        self.client_list.add_client(client)

        client.run()

    def close(self):
        self.server.close();

class PySSSMQ_client_connection:
    
    client_list = None

    def __init__(self, conn, addr, client_list):
        self.conn = conn
        self.addr = addr
        self.client_list = client_list
        self.name = randint(000000000000,999999999999)

    def __eq__(self, other):
        if self.name == other.name:
            return true
        return false

    def run(self):
        #Sending message to connected client
        self.conn.send('Welcome to the server. Type something and hit enter\n') #send only takes string
         
        #infinite loop so that function do not terminate and thread do not end.
        while True:
             
            #Receiving from client
            try:
                data = self.conn.recv(1024)
            except:
                self.client_list.remove_client(self)
                print "Client exited unexpected"
                return

            reply = 'Data from ' + self.addr[0] + ': ' + data 
            if (not data) or (data.strip() == 'exit'): 
                print 'Exit'
                self.client_list.remove_client(self)
                break

            while True:
                dataobj = self.parse_data(data)
                if dataobj is False:
                    print "Malformed data: " + data
                else:
                    print "Received data: " + data
                    if dataobj['command'] == "|":
                        self.client_list.message(dataobj["key"], dataobj["data"])
                    elif dataobj['command'] == "&":
                        for k in self.client_list.data.data:
                            print "List: " + k + ":" + self.client_list.data.data[k]
                            self.send(self.client_list.data.data[k],len(self.client_list.data.data[k]),k)
                        break;

                if(dataobj is False or dataobj['more_data'] == ''):
                    break;
                
                data = dataobj['more_data']

            
         
        #came out of loop
        self.conn.close()

    def send(self, string, length, skey):
        try:
            self.conn.sendall("|" + skey[:10].ljust(10) + format(len(string),'04') + string)
        except:
            print "Error sending to " , self.addr[0]

    def parse_data(self, data):
        more_data = "";
        if not type(data) is str:
            return False
        if (data[:1] != '|') and (data[:1] != '&'):
            return False
        if data[:1] == '&':
            return { 'command' : "&", 'length' : 0, 'key' : None, 'data' : None, 'more_data' : "" }

        if not self.is_int(data[11:15]):
            return False
        
        command = data[:1]
        key = data[1:11]
        length = int(data[11:15])
        data_string = data[15:(15+length)]

        if len(data) > 14+length:
            more_data = data[(15+length):]

        return { 'command' : command, 'length' : length, 'key' : key, 'data' : data_string, 'more_data' : more_data }

    def is_int(self, s):
        try: 
            int(s)
            return True
        except ValueError:
            return False


class PySSSMQ_message_queue:
    
    clients = []
    data = None

    def __init__(self, data):
        self.data = data

    def add_client(self, client):
        self.clients.append(client)

    def remove_client(self, client):
        self.clients.remove(client)

    def message(self, key, data):
        self.data.set(key[:10],data)

        for client in self.clients:
            print "Sendt to ", client.addr[0], ": ", data;
            client.send(data, len(data), key)

class PySSSMQ_data:
    data = { }

    def __init__(self):
        return

    def get(self, name):
        
        if name in self.data:
            return self.data[name]

        return None

    def set(self, name, value):
        self.data[name] = value

def file_get_contents(filename):
    with open(filename) as f:
        return f.read()

def check_pid(pid):
    try:
        os.kill(pid, 0)
    except OSError:
        return False
    else:
        return True

if __name__ == '__main__':
    pidfile = os.path.dirname(os.path.realpath(__file__)) + '/.pidfile'
    isrunning = False
    # Check if server is allready running
    if os.path.isfile(pidfile):
        pid = int(file_get_contents(pidfile))
        isrunning = check_pid(pid)

        if(len(sys.argv) > 1 and sys.argv[1] == 'stop'):
            print("Trying to kill process...")
            os.remove(pidfile)
            os.kill(pid, signal.SIGINT)
            sys.exit(0)
        elif isrunning:
            print("Process is already running. Exiting...")
            sys.exit(0)
    elif(len(sys.argv) > 1 and sys.argv[1] == 'stop'):
        print("Process not found... Exiting.")
        sys.exit(0)



    s = PySSSMQ_server()
    
    f = open(pidfile, 'w')
    f.write(str(os.getpid()))
    f.close()
    print os.getpid()

    try:
        s.run()
        s.close()
    except KeyboardInterrupt:
        s.close()
        print "Good bye"
        try:
            os.remove(pidfile)
        except:
            pass
