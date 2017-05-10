# PySSSMQ_server

## Requirements

Python 2.x (>=2.7)

Windows install file: https://www.python.org/downloads/windows/  
Linux: Use your favorite package manager  

## Usage

User executing the script, must have write access to the directory where the PySSSMQ_server.py script is located. (.pidfile)

### Linux

`chmod 755 PySSSMQ_server.py`

**Start:** `./PySSSMQ_server.py &`  
**Stop:** `./PySSSMQ_server.py stop`  

### Windows

**Start:** `x:\path\to\pythonw.exe PySSSMQ_server.py`  
**Stop:** `x:\path\to\pythonw.exe PySSSMQ_server.py stop`  

Use `pythonw.exe` and not the `python.exe` executable.

If the process is not ended normally, the unremoved `.pidfile` can cause some problems. 
Just delete this file (located in same folder as the python script), or use the stop command.
