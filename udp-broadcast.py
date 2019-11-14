import socket
import time

server = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
server.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
# Set a timeout so the socket does not block
# indefinitely when trying to receive data.
server.settimeout(0.2)
server.bind(("", 44444))

a = 0

while True:
    server.sendto(str.encode(str(a)), ('<broadcast>', 5000))
    a = a + 1
    print("message sent!")
    time.sleep(1)