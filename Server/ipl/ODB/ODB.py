# File: O (Python 2.3)

from ZODB import FileStorage, DB
import Persistence
import ZODB.PersistentList as ZODB
Persistent = Persistence.Persistent
PersistentList = ZODB.PersistentList.PersistentList

class ODB:
    '''Our generic object database -- change this rather than all the code that 
    hangs off it (eg. if you want to move from FileStorage to BerkeleyStorage 
    or from ZODB to some other OO database
    '''
    openDBs = { }
    
    def __init__(self, dbPath):
        '''Create an instance of our object database'''
        if dbPath in ODB.openDBs:
            self.conn = ODB.openDBs[dbPath]
        else:
            storage = FileStorage.FileStorage(dbPath)
            db = DB(storage)
            self.conn = db.open()
            ODB.openDBs[dbPath] = self.conn
        self.root = self.conn.root()

    
    def __contains__(self, item):
        return item in self.root

    
    def __delitem__(self, key):
        get_transaction().commit()

    
    def __getitem__(self, key):
        return self.root[key]

    
    def __iter__(self):
        return iter(self.root)

    
    def __len__(self):
        return len(self.root)

    
    def __setitem__(self, key, value):
        self.root[key] = value
        get_transaction().commit()

    
    def has_key(self, item):
        return item in self

    
    def commit(self):
        '''Force the database to commit a transaction'''
        get_transaction().commit()


