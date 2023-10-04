#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      danny
#
# Created:     02/10/2023
# Copyright:   (c) danny 2023
# Licence:     <your licence>
#-------------------------------------------------------------------------------
import win32file
import win32pipe
import time

class ClientPipe:
    fileHandle = None
    name = None
    logging=False
    def __init__(self,name,logging):
        self.name=name
        self.logging = logging

    def connect(self):
        count =0
        while(count <20):
            try:
                pipeName= "\\\\.\\pipe\\"+self.name
                self.fileHandle = win32file.CreateFile(
                    pipeName,
                    win32file.GENERIC_READ | win32file.GENERIC_WRITE,
                    0,
                    None,
                    win32file.OPEN_EXISTING,
                    0,
                    None)
                return
            except Exception as ex:
                print(ex)
            count = count +1
            time.sleep(1)


    def readNextLine(self):
        try:

            counter=0
            left, data = win32file.ReadFile(self.fileHandle, 4096)
            for i in data:
                if i == 36:
                    return(data[counter+1:len(data)])
                counter = counter+1
            return(data)  # "hello
        except:
            return  b"{Leave}"

    def disconnect(self):
        try:
            win32file.CloseHandle(self.fileHandle)
        except Exception as ex:
            print (ex)


class ServerPipe:
    name=None
    logging = False
    fileHandle = None
    def __init__(self,name,logging):
        self.name=name
        self.logging=logging
        pipeName= "\\\\.\\pipe\\"+self.name
        pipe = win32pipe.CreateNamedPipe(
        pipeName,
        win32pipe.PIPE_ACCESS_DUPLEX,
        win32pipe.PIPE_TYPE_MESSAGE | win32pipe.PIPE_READMODE_MESSAGE,
        1, 65536, 65536,
        0,
        None)
        self.fileHandle=pipe

    def connect(self):
        try:
            print("waiting for client")
            win32pipe.ConnectNamedPipe(self.fileHandle, None)
            print("got client")
        except Exception as ex:
            print (ex)
    def writeNextLine(self,data):
        win32file.WriteFile(self.fileHandle, str.encode(data))


    def disconnect(self):
        win32file.CloseHandle(self.fileHandle)


