import sqlite3


class Table(object):

    def __init__(self, connect, table_name):
        self.connect = connect
        self.name = table_name
        self.columns = []

    def drop(self):
        cursor = self.connect.cursor()
        try:
            cursor.execute("drop table " + self.name + ";")
        except sqlite3.Error as e:
            print e
        self.connect.commit()

    def create(self):
        raise NotImplementedError

    def find(self, name):
        cursor = self.connect.cursor()
        res = cursor.execute("select * from " + self.name + " where name = '" + name + "';")
        return res.fetchone()

    def insert(self, **kwargs):
        key = 'insert into ' + self.name + '('
        value = 'values ( '
        for k, v in kwargs.iteritems():
            key += k + ', '
            if isinstance(v, str):
                value += "'" + v + "', "
            elif isinstance(v, int) or isinstance(v, float):
                value += str(v) + ', '
        sql = key[:-2] + ')' + value[:-2] + ');'
        cursor = self.connect.cursor()
        try:
            cursor.execute(sql)
            self.connect.commit()
        except sqlite3.Error as e:
            print e
            return False
        return True

    def update(self, name, **kwargs):
        res = self.find(name)
        if res is None:
            print name
            print kwargs
            return self.insert(self, name=name, **kwargs)

        sql = 'update ' + self.name + ' set '

        for k, v in kwargs.iteritems():
            if k in self.columns:
                sql += k + ' = '
                if isinstance(v, str):
                    sql += "'" + v + "', "
                elif isinstance(v, int) or isinstance(v, float):
                    sql += str(v) + ", "
        sql = sql[:-2]
        sql += " where name = '" + name + "';"
        cursor = self.connect.cursor()
        try:
            cursor.execute(sql)
            self.connect.commit()
        except sqlite3.Error as e:
            print e
            return False
        return True
