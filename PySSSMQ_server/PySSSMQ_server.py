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
# Pling (Application)
# Copyright (c) 2017 Lars Andre Landås (landas@gmail.com)


 
import socket
import sys
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
    
    conn = None
    addr = None
    client_list = None

    def __init__(self, conn, addr, client_list):
        self.conn = conn
        self.addr = addr
        self.client_list = client_list

    def run(self):
        #Sending message to connected client
        self.conn.send('Welcome to the server. Type something and hit enter\n') #send only takes string
         
        #infinite loop so that function do not terminate and thread do not end.
        while True:
             
            #Receiving from client
            data = self.conn.recv(1024)
            reply = 'Data from ' + self.addr[0] + ': ' + data 
            if (not data) or (data.strip() == 'exit'): 
                print 'Exit'
                break

            dataobj = self.parse_data(data)
            
            if dataobj is False:
                print "Malformed data: " + data
            else:
                if dataobj['command'] == "|":
                    self.client_list.message(dataobj["key"], dataobj["data"])
                elif dataobj['command'] == "&":
                    for k in self.client_list.data.data:
                        print "List: " + self.client_list.data.data[k]
                        self.send(self.client_list.data.data[k],len(self.client_list.data.data[k]),k)
            
         
        #came out of loop
        self.conn.close()

    def send(self, string, length, skey):
        try:
            self.conn.sendall("|" + skey[:10].ljust(10) + format(len(string),'04') + string)
        except:
            print "Error sending to " , self.addr[0]

    def parse_data(self, data):
        if not type(data) is str:
            return False
        if (data[:1] != '|') and (data[:1] != '&'):
            return False
        if data[:1] == '&':
            return { 'command' : "&", 'length' : 0, 'key' : None, 'data' : None }

        print "HER"

        if not self.is_int(data[11:15]):
            return False
        
        command = data[:1]
        key = data[1:11]
        length = int(data[11:15])
        data_string = data[15:(15+length)]

        return { 'command' : command, 'length' : length, 'key' : key, 'data' : data_string }

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

if __name__ == '__main__':
    
    s = PySSSMQ_server()
    
    try:
        s.run()
        s.close()
    except KeyboardInterrupt:
        s.close()
        print "Good bye"
