using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class DynamodbHandler<T> {

    public async void Insert(T t) {
        var doc = new Document();
        foreach (var prop in t.GetType().GetProperties()) {
            var value = prop.GetValue(t);
            if (value == null) continue;
            doc[prop.Name] = GetEntry(value);
        }
        await GetTable.PutItemAsync(doc);
    }

    public Document GerateDocument(T g) {
        var doc = new Document();
        foreach (var prop in g.GetType().GetProperties()) {
            var value = prop.GetValue(g);
            if (value == null) continue;
            doc[prop.Name] = GetEntry(value);
        }
        return doc;
    }

    DynamoDBEntry GetEntry(object obj) {
        if (obj == null) {
            throw new ArgumentNullException($"Couldn't get dynamodb entry because object was null.");
        }
        if (obj is int integer) {
            return integer;
        } else if (obj is string str) {
            return str;
        } else if (obj is double dou) {
            return dou;
        } else if (obj is float flo) {
            return flo;
        } else if (obj is byte byt) {
            return byt;
        } else if (obj is long lon) {
            return lon;
        } else if (obj is short shor) {
            return shor;
        } else {
            throw new NotSupportedException($"Type not mapped ({obj.GetType()})");
        }
    }

    public Table GetTable {
        get {
            var client = new AmazonDynamoDBClient();
            return Table.LoadTable(client, TableName);
        }
    }
    public abstract string TableName { get; }
    public abstract T GetFromDoc(Document doc);

}