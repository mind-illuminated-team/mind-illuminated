import socket
import time
import random

class SensorDataBroadcasterMock:

    def __init__(self):
        self.server = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
        self.server.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
        # Set a timeout so the socket does not block
        # indefinitely when trying to receive data.
        self.server.settimeout(0.2)
        self.server.bind(("", 44444))

        self.levels = (600, 500, 400, 300, 200, 100)
        self.currentLevel = 0
        self.deviationHalf = 20
        self.levelDurationDeviationHalf = 3
        self.baseLevelDuration = 8
        self.levelDuration = self.baseLevelDuration
        self.startTime = time.time()

    def randomizeLevelDuration(self):
        lowerBound = self.baseLevelDuration - self.levelDurationDeviationHalf
        upperBound = self.baseLevelDuration + self.levelDurationDeviationHalf
        self.levelDuration = random.randint(lowerBound, upperBound)

    def switchLevel(self):
        currentTime = time.time()
        if (currentTime - self.startTime) > self.levelDuration:
            self.currentLevel = random.randint(0, len(self.levels) - 1)
            self.startTime = currentTime
            self.randomizeLevelDuration()

    def getRandomNumber(self):
        levelValue = self.levels[self.currentLevel]
        lowerBound = levelValue - self.deviationHalf
        upperBound = levelValue + self.deviationHalf
        return random.randint(lowerBound, upperBound)

    def start(self):
        print("Starting signal sending")
        while True:
            self.switchLevel()
            number = self.getRandomNumber()
            print(number)
            self.server.sendto(str.encode(str(number)), ('<broadcast>', 5000))

            time.sleep(0.05)

if __name__ == '__main__':
    SensorDataBroadcasterMock().start()