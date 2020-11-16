using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace AWS_Helper {

    public class DynamoDB_Helper : IAWSService {

        public DynamoDB_Helper(string publicKey, string secretKey, RegionEndpoint region) {
            PublicKey = publicKey;
            SecretKey = secretKey;
            Region = region;
        }

        public string PublicKey { get; }
        public string SecretKey { get; }
        // RegionEndpoint.EUWest1;
        public RegionEndpoint Region { get; }
        public BasicAWSCredentials Credentials {
            get => new BasicAWSCredentials(PublicKey, SecretKey);
        }
        public AmazonDynamoDBConfig Config {
            get => new AmazonDynamoDBConfig {
                ServiceURL = "http://localhost:8000",
                RegionEndpoint = Region
            };
        }
        public AmazonDynamoDBClient Client {
            get => new AmazonDynamoDBClient(Credentials, Config);
        }

        public Document Put(string table, params (string, DynamoDBEntry)[] fields) {
            return GetTable(table).PutItem(BuildDocument(fields));
        }

        public Document Get (string table, Primitive hash_key) {
            return GetTable(table).GetItem(hash_key);
        }

        public Document Get (string table, Primitive hash_key, Primitive range_key) {
            return GetTable(table).GetItem(hash_key, range_key);
        }

        public Document Get (string table, Dictionary<string, DynamoDBEntry> keys) {
            return GetTable(table).GetItem(keys);
        }

        public Document Get (string table, params (string, DynamoDBEntry)[] keys) {
            return Get(table, keys.ToDictionary((item) => item.Item1, (item) => item.Item2));
        }

        public Table GetTable(string table) => Table.LoadTable(Client, table);

        public Document BuildDocument (Dictionary<string, DynamoDBEntry> fields) {
            var row = new Document();
            foreach (var field in fields) 
                row[field.Key] = field.Value;
            return row;
        }

        public Document BuildDocument (IEnumerable<(string, DynamoDBEntry)> fields) {
            return BuildDocument(fields.ToDictionary((item) => item.Item1, (item) => item.Item2));
        }

        public static string Print(Document d, int i = 0) {
            string a = "";
            foreach (var key in d.Keys) {
                var item = d[key];
                a += $"{new string(' ', i)} '{key}': '{item}',\n";
            }
            return a;
        }

    }

}