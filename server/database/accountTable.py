import sqlite3
from table import Table


class AccountTable(Table):

    def __init__(self, connect, table_name='Account'):
        super(AccountTable, self).__init__(connect, table_name)
        self.columns = ['account', 'password']

    def create(self):
        cursor = self.connect.cursor()
        try:
            cursor.execute(
                "create table Account(account char(20) primary key not null, "
                "password char(20) not null);"
            )
        except sqlite3.Error as e:
            print e
        self.connect.commit()

    def query_password(self, account):
        cursor = self.connect.cursor()
        sql = "select account, password from " + self.name + " where account = '" + account + "';"
        res = cursor.execute(sql).fetchone()

        if res is None:
            return res
        else:
            return res[1]

    def find_account(self, account):
        cursor = self.connect.cursor()
        # sql = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;"
        # sql = "SELECT * FROM sqlite_master "

        sql = "select account from " + self.name + " where account = '" + account + "';"
        res = cursor.execute(sql).fetchone()
        # print res


        if res is None:
            return res
        else:
            return res[0]
