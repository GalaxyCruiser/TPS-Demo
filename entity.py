

class Entity(object):
    def __init__(self):
        self.name = ""
        self.hp = 100
        self.ammo = 90
        self.ammoInCartridge = 30
        self.lvl = 1
        self.exp = 0
        self.id = -1
        self.hid = -1
    def tick(self):
        pass

    def kill(self):
        self.exp += 10

