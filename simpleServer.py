# -*- coding: GBK -*-

import sys
import time
from common_server.timer import TimerManager
from common import conf
from database.database import db
sys.path.append('./common')

from network.simpleHost import SimpleHost
from network.netStream import NetStream, RpcProxy
from entity import Entity


def EXPOSED(func):
    func.__exposed__ = True
    return func


class SimpleServer(object):

    def __init__(self):
        super(SimpleServer, self).__init__()
        self.entities = {}
        self.host = SimpleHost()
        self.running = False
        self.next_entity_id = 0
        self.game_start = False
        self.host.startup(conf.SERVER_PORT)
        self.caller = RpcProxy(self, None)
        self.connected_client = []
        self.db = db
        return

    # 用户注册
    @EXPOSED
    def register(self, account, password):
        if db.account_table.find_account(account) is not None:
            self.caller.accountExist(account)
        elif db.account_table.insert(account=account, password=password):
            self.caller.registerSuccess(account)
        else:
            self.caller.databaseError(account)

    # 用户登录
    @EXPOSED
    def login(self, account, password):
        db_password = db.account_table.query_password(account)
        if db_password is None or db_password != password:
            self.caller.loginFail(account)
        else:
            self.caller.loginSuccess(account)

    # 创建角色
    @EXPOSED
    def createCharacter(self, name, account):

        if db.character_table.find_name(name) is not None:
            self.caller.characterExist(account)
        elif db.character_table.create_character(name=name, account=account):
            self.caller.registerSuccess(name)
            self.getCharacters(account)
        else:
            self.caller.databaseError(name)

    # 用户登录后获取账号中的角色
    @EXPOSED
    def getCharacters(self, account):
        characters = db.character_table.find_character_by_account(account)
        if characters:
            self.caller.showCharacter(*characters)

    # 获取角色信息
    @EXPOSED
    def getCharacterInfo(self, name):
        character = db.character_table.find_name(name)
        self.caller.loadCharacterInfo(*character)

    # 角色注册
    @EXPOSED
    def playerRegister(self, name):
        character = Entity()
        character.hid = self.caller.netstream.hid
        character_data_dict = db.character_table.find(name)
        if character_data_dict is not None:
            character.name = character_data_dict[0]
            character.account = character_data_dict[1]
            character.hp = character_data_dict[2]
            character.ammo = character_data_dict[3]
            character.ammoInCartridge = character_data_dict[4]
            character.lvl = character_data_dict[5]
            character.exp = character_data_dict[6]
        self.registerEntity(character)
        self.caller.setEntityID(character.id)

    # 更新角色信息
    @EXPOSED
    def updateCharacter(self, name, eid, key, value):
        value = int(value)
        setattr(self.entities[int(eid)], key, value)
        if key == "hp":
            db.character_table.update(name, hp=value)
        elif key == "ammo":
            db.character_table.update(name, ammo=value)
        elif key == "ammoInCartridge":
            db.character_table.update(name, ammoInCartridge=value)
        elif key == "exp":
            db.character_table.update(name, exp=value)
        elif key == "lvl":
            db.character_table.update(name, lvl=value)

    def generateEntityID(self):
        self.next_entity_id += 1
        return self.next_entity_id

    def registerEntity(self, entity):
        eid = self.generateEntityID()
        entity.id = eid
        self.caller.setEntityID(eid)
        self.entities[eid] = entity

        return

    def deleteClient(self, hid):
        if hid in self.connected_client:
            self.connected_client.remove(hid)

    def tick(self):
        self.host.process()
        event, hid, data = self.host.read()
        while event >= 0:
            if event == conf.NET_CONNECTION_NEW:
                code, client_netstream = self.host.getClient(hid)
                self.caller.netstream = client_netstream    #设置netstream
                self.caller.setClientID(hid)

            elif event == conf.NET_CONNECTION_LEAVE:
                self.deleteClient(hid)
            elif event == conf.NET_CONNECTION_DATA:
                code, self.caller.netstream = self.host.getClient(hid)
                if code == 0:
                    self.caller.parse_rpc(data) #解析RPC
            event, hid, data = self.host.read()

        for eid, entity in self.entities.iteritems():
            # Note: you can not delete entity in tick.
            # you may cache delete items and delete in next frame
            # or just use items.
            entity.tick()

        return

    def start(self):
        if not self.running:
            self.running = True
            TimerManager.addRepeatTimer(0.1, self.tick)
            while self.running:
                time.sleep(0.01)
                TimerManager.scheduler()
        return

    def stop(self):
        self.running = False
