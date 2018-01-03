using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTLSpotScraper.Helper
{
    internal class MongoHelper
    {
        public static ConnectionSettings Infos { get; set; }

        public static void ResetInfos()
        {
            Infos = null;
        }

        public static void SetInfos(string dbName)
        {
            if (Infos == null)
                Infos = new ConnectionSettings(dbName);
        }

        public class ConnectionSettings
        {
            public string ConnectionString { get; set; }
            public string DbName { get; set; }
            public string CLientId { get; set; }
            public string ClientSecret { get; set; }
            public string ApiKey { get; set; }

            public ConnectionSettings(string dbName = "")
            {
                if (string.IsNullOrEmpty(dbName))
                    DbName = ConfigurationManager.AppSettings["dbName"];
                else
                {
                    DbName = dbName;
                }
                
                ConnectionString = ConfigurationManager.AppSettings["connectionString"];
                CLientId = ConfigurationManager.AppSettings["YelpClientId"];
                ApiKey = ConfigurationManager.AppSettings["YelpApiKey"];
                ClientSecret = ConfigurationManager.AppSettings["YelpClientSecret"];
            }
        }

        private static IMongoClient _client;
        private static IMongoDatabase _database;

        public static IMongoDatabase GetDb(string specificDb = "")
        {
            _client = new MongoClient(Infos.ConnectionString);
            _database = _client.GetDatabase(specificDb);

            return _database;
        }

        public static async Task<bool> Delete<T>(Expression<Func<T, bool>> memberExpression)
        {
            var coll = GetCollection<T>();
            var result = await coll.DeleteOneAsync(memberExpression).ConfigureAwait(false);
            return result.DeletedCount > 0;
        }

        public static async Task<bool> DeleteMany<T>(Expression<Func<T, bool>> memberExpression)
        {
            var coll = GetCollection<T>();
            var result = await coll.DeleteManyAsync(memberExpression).ConfigureAwait(false);
            return result.DeletedCount > 0;
        }

        public static async Task<bool> Delete<T>(FilterDefinition<T> filterDefinition)
        {
            var coll = GetCollection<T>();
            var result = await coll.DeleteOneAsync(filterDefinition).ConfigureAwait(false);
            return result.DeletedCount > 0;
        }

        public static async Task<bool> Save<T>(Expression<Func<T, bool>> memberExpression, T obj) where T : IEntity
        {
            var coll = GetCollection<T>();
            if (obj.Id == null)
            {
                await coll.InsertOneAsync(obj).ConfigureAwait(false);
                return true;
            }

            var result = await coll.ReplaceOneAsync(memberExpression, obj, new UpdateOptions()).ConfigureAwait(false);
            return result.ModifiedCount > 0;
        }

        public static async Task<bool> ReplaceWithUpsert<T>(Expression<Func<T, bool>> memberExpression, T obj) where T : IEntity
        {
            var coll = GetCollection<T>();
            var result = await coll.ReplaceOneAsync(memberExpression, obj, new UpdateOptions() { IsUpsert = true }).ConfigureAwait(false);
            return result.ModifiedCount > 0;
        }

        public static async Task<bool> SaveDeal<T>(Expression<Func<T, bool>> memberExpression, T obj) where T : IEntity
        {
            var coll = GetCollection<T>();
            if (obj.Id == null)
            {
                await coll.InsertOneAsync(obj).ConfigureAwait(false);
                return true;
            }

            var result = await coll.ReplaceOneAsync(memberExpression, obj, new UpdateOptions()).ConfigureAwait(false);
            return result.ModifiedCount > 0;
        }

        public static async Task<long> GetCount<T>(Expression<Func<T, bool>> memberExpression)
        {
            var coll = GetCollection<T>();
            return await coll.CountAsync(memberExpression).ConfigureAwait(false);
        }

        public static async Task<T> FindOne<T>(Expression<Func<T, bool>> memberExpression, FindOptions<T> findOptions = null)
        {
            var coll = GetCollection<T>();
            if (findOptions == null)
                findOptions = new FindOptions<T>();

            findOptions.Limit = 1;
            IAsyncCursor<T> task = await coll.FindAsync(memberExpression, findOptions).ConfigureAwait(false);
            return await task.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public static async Task<T> FindOne<T>(FilterDefinition<T> filterDefinition, FindOptions<T> findOptions = null)
        {
            var coll = GetCollection<T>();
            if (findOptions == null)
                findOptions = new FindOptions<T>();

            findOptions.Limit = 1;
            IAsyncCursor<T> task = await coll.FindAsync<T>(filterDefinition, findOptions).ConfigureAwait(false);
            return await task.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public static async Task<bool> CollectionExistsAsync<T>(string connectionString = "")
        {
            Type t = typeof(T);
            var collName = t.GetField("CollectionName").GetValue(t).ToString();
            var filter = new BsonDocument("name", collName);
            var collections = await GetDb(connectionString != "" ? connectionString : null).ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });

            return (await collections.ToListAsync()).Any();
        }

        public static async Task<List<string>> GetDBList(string connectionString = "")
        {
            List<string> dbs = new List<string>();

            var client = _client;
            if (connectionString != "")
                client = new MongoClient(connectionString);

            var cursor = await client.ListDatabasesAsync();
            await cursor.ForEachAsync(db =>
            {
                dbs.Add(db.GetElement("name").Value.AsString);
            });

            return dbs;
        }

        public static async Task<IEnumerable<T>> FindAll<T>(int limit = -1)
        {
            var coll = GetCollection<T>();
            var filter = Builders<T>.Filter.Empty;

            var val = coll.Find(filter);

            if (limit > -1)
                val = val.Limit(limit);

            return await val.ToListAsync().ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> Find<T>(FilterDefinition<T> filterDefinition, FindOptions<T> findOptions)
        {
            var coll = GetCollection<T>();
            IAsyncCursor<T> task = await coll.FindAsync(filterDefinition, findOptions).ConfigureAwait(false);
            return await task.ToListAsync().ConfigureAwait(false);
        }

        public static async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> memberExpression, FindOptions<T> findOptions)
        {
            var coll = GetCollection<T>();
            IAsyncCursor<T> task = await coll.FindAsync(memberExpression, findOptions).ConfigureAwait(false);
            return await task.ToListAsync().ConfigureAwait(false);
        }

        public static async Task<IAsyncCursor<T>> FindWithCursor<T>(FilterDefinition<T> filterDefinition, FindOptions<T> findOptions)
        {
            var coll = GetCollection<T>();
            IAsyncCursor<T> task = await coll.FindAsync(filterDefinition, findOptions).ConfigureAwait(false);
            return task;
        }

        public static async Task<bool> Insert<T>(T document)
        {
            return await InsertBatch(new List<T>() { document }).ConfigureAwait(false);
        }

        public static async Task<bool> InsertBatch<T>(IEnumerable<T> documents)
        {
            var coll = GetCollection<T>();

            var task = coll.InsertManyAsync(documents);
            await task.ConfigureAwait(false);
            return !task.IsFaulted;
        }

        public static IMongoCollection<T> GetCollection<T>()
        {
            Type t = typeof(T);
            var collName = t.GetField("CollectionName").GetValue(t).ToString();
            var coll = GetDb().GetCollection<T>(collName);
            return coll;
        }

        public static async Task<long> GetCollectionCount<T>()
        {
            var coll = GetCollection<T>();
            FilterDefinition<T> filter = Builders<T>.Filter.Empty;
            return await coll.CountAsync(filter).ConfigureAwait(false);
        }

        public static async Task<T> FindAndModifyInc<T>(Expression<Func<T, bool>> memberExpression, Expression<Func<T, int>> memberUpdateExpression, int updateValue)
        {
            var coll = GetCollection<T>();
            return await coll.FindOneAndUpdateAsync(memberExpression, Builders<T>.Update.Inc(memberUpdateExpression, updateValue)).ConfigureAwait(false);
        }

        public static async Task<T> FindAndUpdate<T>(Expression<Func<T, bool>> memberExpression, UpdateDefinition<T> builders, bool isUpsert = false)
        {
            var coll = GetCollection<T>();
            return await coll.FindOneAndUpdateAsync(memberExpression, builders, new FindOneAndUpdateOptions<T> { IsUpsert = isUpsert }).ConfigureAwait(false);
        }

        public static async Task<List<T>> FindWithQuery<T>(FilterDefinition<T> filterDefinition, FindOptions<T> findOptions)
        {
            var coll = GetCollection<T>();
            IAsyncCursor<T> task = await coll.FindAsync(filterDefinition, findOptions).ConfigureAwait(false);
            return await task.ToListAsync().ConfigureAwait(false);
        }

        public static async Task<bool> UpdateOne<T>(FilterDefinition<T> filterDefinition, UpdateDefinition<T> updateDefinition,
            UpdateOptions updateOptions = null, Expression<Func<T, bool>> findExpression = null,
            Expression<Func<T, dynamic>> setExpression = null)
        {
            var coll = GetCollection<T>();
            var result =
                await coll.UpdateOneAsync(filterDefinition, updateDefinition, updateOptions).ConfigureAwait(false);
            return result.ModifiedCount > 0;
        }

        public static async Task<bool> Update<T>(FilterDefinition<T> filterDefinition, UpdateDefinition<T> updateDefinition, UpdateOptions updateOptions = null)
        {
            var coll = GetCollection<T>();
            var result = await coll.UpdateManyAsync(filterDefinition, updateDefinition, updateOptions).ConfigureAwait(false);
            return result.ModifiedCount > 0;
        }
    }
}