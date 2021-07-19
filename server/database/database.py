import sqlite3
from accountTable import AccountTable
from characterTable import CharacterTable


class Database(object):
    def __init__(self):
        self.path = "game.db"
        self.connect = sqlite3.connect(self.path)
        self.account_table = AccountTable(self.connect)
        self.character_table = CharacterTable(self.connect)


db = Database()

if __name__ == '__main__':
    db.account_table.create()
    # cursor = db.connect.cursor()
    # cursor.execute("insert into Account (account, password) values ('netease1', 123);")
    # db.connect.commit()
    # print db.account_table.find_account("netease1")
    # cursor = db.account_table.connect.cursor()
    # sql = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;"
    # sql = "SELECT * FROM Account ;"

    # sql = "select account from " + self.name + " where account = '" + account + "';"
    # res = cursor.execute(sql).fetchone()
    # print res

