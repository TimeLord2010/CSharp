using LiteDB;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WpfTaskbar {

    class DBOperations {

        public enum TriggerOn {
            Delete, Insert, Update 
        }
        public enum TriggerType {
            Before, After
        }
        public readonly string DBName;

        public DBOperations(string DatabaseName) {
            DBName = DatabaseName;
        }

        public string FileName {
            get => DBName + ".db";
        }

        public void Recreate () {
            File.Delete(FileName);
            using (var db = new LiteDatabase(FileName)) { }
        }

        public bool Exists(string name) {
            using (var db = new LiteDatabase(FileName)) {
                return db.CollectionExists(name);
            }
        }

        public void Insert<T>(string table, T item) {
            using (var db = new LiteDatabase(FileName)) {
                LiteCollection<T> collection = db.GetCollection<T>(table);
                collection.Insert(item);
            }
        }

        public int Delete (string table, BsonValue id) {
            using (var db = new LiteDatabase(FileName)) {
                var collection = db.GetCollection(table);
                return collection.Delete(id) ? 1:0;
            }
        }

        public int Delete (string table, Query query) {
            using (var db = new LiteDatabase(FileName)) {
                var collection = db.GetCollection(table);
                return collection.Delete(query);
            }
        }

        public int Delete <T> (string table, Expression<Func<T,bool>> func) {
            using (var db = new LiteDatabase(FileName)) {
                var collection = db.GetCollection<T>(table);
                return collection.Delete(func);
            }
        }

        public void Update <T> (string table, T item) {
            using (var db = new LiteDatabase(FileName)) {
                var collection = db.GetCollection<T>(table);
                collection.Update(item);
            }
        }

        public void Update <T> (string table, BsonValue id, T item) {
            using (var db = new LiteDatabase(FileName)) {
                var collection = db.GetCollection<T>(table);
                collection.Update(id,item);
            }
        }

        public T Get <T> (string table, BsonValue id) {
            using (var db = new LiteDatabase(FileName)) {
                var collection = db.GetCollection<T>(table);
                return collection.FindById(id);
            }
        }

        public IEnumerable<T> Get<T>(string table, Query query, int skip = 0, int limit = Int32.MaxValue) {
            using (var db = new LiteDatabase(FileName)) {
                var collection = db.GetCollection<T>(table);
                return collection.Find(query,skip,limit);
            }
        }

        public IEnumerable<T> Get<T>(string table, Expression<Func<T, bool>> func, int skip = 0, int limit = Int32.MaxValue) {
            using (var db = new LiteDatabase(FileName)) {
                var collection = db.GetCollection<T>(table);
                return collection.Find(func, skip, limit);
            }
        }

        public IEnumerable<T> GetAll<T>(string table) {
            using (var db = new LiteDatabase(FileName)) {
                return db.GetCollection<T>(table).FindAll();
            }
        }

        public bool CreateIndex (string table, string field, bool unique) {
            using (var db = new LiteDatabase(FileName)) {
                var lc = db.GetCollection(table);
                return lc.EnsureIndex(field,unique);
            }
        }

        public bool DeleteIndex (string table,string field) {
            using (var db = new LiteDatabase(FileName)) {
                var lc = db.GetCollection(table);
                return lc.DropIndex(field);
            }
        }
    }
}
