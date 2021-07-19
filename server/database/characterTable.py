from table import Table
import sqlite3


class CharacterTable(Table):
    def __init__(self, connect, table_name='CharacterTable'):
        super(CharacterTable, self).__init__(connect, table_name)
        self.columns = ['name', 'account', 'hp', 'ammo', 'ammoInCartridge', 'lvl', 'exp']

    def create(self):
        cursor = self.connect.cursor()
        try:
            cursor.execute("create table CharacterTable("
                           "name char(20) primary key not null, account char(20), "
                           "hp int, ammo int, ammoInCartridge int, lvl int, exp int);")
        except sqlite3.Error as e:
            print e
        self.connect.commit()

    def create_character(self, name, account):
        return self.insert(name=name, account=account, hp=100, ammo=90, ammoInCartridge=30, lvl=1, exp=0)

    def update_character(self, name, account, hp, ammo, ammoInCartridge, lvl, exp):
        return self.update(name=name, account=account, hp=hp, ammo=ammo, ammoInCartridge=ammoInCartridge, lvl=lvl, exp=exp)

    def find_name(self, name):
        return self.find(name)

    def find_character_by_account(self, account):
        cursor = self.connect.cursor()
        res = []
        rows = cursor.execute("select * from " + self.name + " where account = '" + account + "';")
        for row in rows:
            if row[0] != "":
                res.append(row[0])
        return res
