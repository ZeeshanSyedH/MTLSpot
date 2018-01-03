using log4net;
using log4net.Config;
using MongoDB.Driver;
using MTLSpotScraper.Helper;
using MTLSpotScraper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTLSpotScraper.Repos
{
    public class MongoRepo : IMongoRepo
    {
        private static ILog _logMongoRepo;

        public MongoRepo(string dbName = "")
        {
            MongoHelper.SetInfos(dbName);

            if (_logMongoRepo != null) return;
            _logMongoRepo = LogManager.GetLogger(typeof(MongoRepo));

            XmlConfigurator.Configure();
            Logger.SetLevel("ParserLog", "DEBUG");

            Logger.AddAppender2(_logMongoRepo, Logger.CreateFileAppender("MongoRepoDebugAppender", "MongoRepo-Debug.txt", "debug"));
            Logger.AddAppender2(_logMongoRepo, Logger.CreateFileAppender("MongoRepoErrorAppender", "MongoRepo-Error.txt", "error"));
        }

        public async Task<bool> IncOneField<T, TA>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA inc)
        {
            try
            {
                var updateDefinition = Builders<T>.Update.Inc(fieldExpression, inc);

                return await MongoHelper.Update(findExpression, updateDefinition).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logMongoRepo.ErrorFormat("(MongoRepo) Error on IncOneField : {0}", ex);
                return false;
            }
        }

        public virtual async Task<bool> UpdateMany<T, TA>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value)
        {
            try
            {
                var filter = Builders<T>.Filter.Where(findExpression);
                var updateFilter = Builders<T>.Update.Set(fieldExpression, value);

                var updateResult = await MongoHelper.Update(filter, updateFilter).ConfigureAwait(false);
                return updateResult;
            }
            catch (Exception ex)
            {
                _logMongoRepo.ErrorFormat("(MongoRepo) Error on UpdateMany : {0}", ex);
                return false;
            }
        }

        public async Task<bool> UnsetOneField<T>(Expression<Func<T, bool>> findExpression, Expression<Func<T, dynamic>> fieldExpression)
        {
            try
            {
                var filter = Builders<T>.Filter.Where(findExpression);
                var updateFilter = Builders<T>.Update.Unset(fieldExpression);

                var updateResult = await MongoHelper.UpdateOne(filter, updateFilter).ConfigureAwait(false);
                return updateResult;
            }
            catch (Exception ex)
            {
                _logMongoRepo.ErrorFormat("(MongoRepo) Error on UnsetOneField : {0}", ex);
                return false;
            }
        }

        public virtual async Task<T> FindOne<T>(Expression<Func<T, bool>> findExpression1, Expression<Func<T, bool>> findExpression2 = null, Expression<Func<T, bool>> findExpression3 = null)
        {
            var builder2 = Builders<T>.Filter.Empty;
            var builder3 = Builders<T>.Filter.Empty;

            if (findExpression2 != null)
                builder2 = Builders<T>.Filter.Where(findExpression2);

            if (findExpression3 != null)
                builder3 = Builders<T>.Filter.Where(findExpression3);

            var filterDefinition = Builders<T>.Filter.And(Builders<T>.Filter.Where(findExpression1), builder2, builder3);

            return await MongoHelper.FindOne(filterDefinition).ConfigureAwait(false);
        }

        public virtual async Task<T> FindOne<T>(bool useExchangeRateDb = false)
        {
            var filterDefinition = Builders<T>.Filter.Empty;
            return await MongoHelper.FindOne(filterDefinition).ConfigureAwait(false);
        }

        public async Task<bool> DeleteOne<T>(Expression<Func<T, bool>> findExpression)
        {
            return await MongoHelper.Delete(findExpression).ConfigureAwait(false);
        }

        public async Task<bool> DeleteMany<T>(Expression<Func<T, bool>> findExpression)
        {
            return await MongoHelper.DeleteMany(findExpression).ConfigureAwait(false);
        }

        public async Task<bool> Insert<T>(T obj)
        {
            return await MongoHelper.Insert(obj).ConfigureAwait(false);
        }

        public async Task<bool> InsertBatch<T>(List<T> list)
        {
            if (!list.Any()) return false;

            await MongoHelper.InsertBatch(list).ConfigureAwait(false);
            return true;
        }

        public virtual async Task<long> Count<T>(Expression<Func<T, bool>> expression)
        {
            return await MongoHelper.GetCount(expression).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> expression, int? batchSize = null, int? skip = null, int? limit = null)
        {
            var findOptions = new FindOptions<T>();
            if (batchSize.HasValue)
                findOptions.BatchSize = batchSize;
            if (skip.HasValue)
                findOptions.Skip = skip;
            if (limit.HasValue)
                findOptions.Limit = limit;

            return await MongoHelper.Find<T>(expression, findOptions).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<T>> FindWithFourOrLessExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2 = null, Expression<Func<T, bool>> expression3 = null, Expression<Func<T, bool>> expression4 = null, int? batchSize = null, int? skip = null, int? limit = null)
        {
            var builder2 = Builders<T>.Filter.Empty;
            var builder3 = Builders<T>.Filter.Empty;
            var builder4 = Builders<T>.Filter.Empty;

            if (expression2 != null)
                builder2 = Builders<T>.Filter.Where(expression2);

            if (expression3 != null)
                builder3 = Builders<T>.Filter.Where(expression3);

            if (expression4 != null)
                builder4 = Builders<T>.Filter.Where(expression4);

            var filterDefinition = Builders<T>.Filter.And(Builders<T>.Filter.Where(expression1), builder2, builder3, builder4);

            var findOptions = new FindOptions<T>();
            if (batchSize.HasValue)
                findOptions.BatchSize = batchSize;
            if (skip.HasValue)
                findOptions.Skip = skip;
            if (limit.HasValue)
                findOptions.Limit = limit;

            return await MongoHelper.Find<T>(filterDefinition, findOptions).ConfigureAwait(false);
        }

        public virtual async Task<long> CountWithTwoExpression<T>(Expression<Func<T, dynamic>> findExpression1, dynamic value1, Expression<Func<T, dynamic>> findExpression2, dynamic value2)
        {
            var filterDefinition = Builders<T>.Filter.And(Builders<T>.Filter.Eq(findExpression1, value1), Builders<T>.Filter.Eq(findExpression2, value2));
            return await MongoHelper.GetCount(filterDefinition).ConfigureAwait(false);
        }

        public virtual async Task<long> CollectionCount<T>()
        {
            return await MongoHelper.GetCollectionCount<T>().ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<T>> FindAll<T>(bool sort = false, Expression<Func<T, object>> sortExpression = null, bool asc = true)
        {
            var filterDefinition = Builders<T>.Filter.Empty;
            FindOptions<T> findOptions = new FindOptions<T>();

            if (sortExpression != null && sort)
            {
                if (asc)
                    findOptions.Sort = Builders<T>.Sort.Ascending(sortExpression);
                else
                    findOptions.Sort = Builders<T>.Sort.Descending(sortExpression);
            }

            return await MongoHelper.Find(filterDefinition, findOptions).ConfigureAwait(false);
        }

        public async Task<bool> UpdateFields<T, TA>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value, bool isUpsert = false)
        {
            return await UpdateFinal(findExpression, fieldExpression, value, null, (string)null, null, (string)null,
                null, (string)null, null, (string)null, null, (string)null, null, (string)null, null, (string)null,
                isUpsert);
        }

        public async Task<bool> UpdateFields<T, TA, TB>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2, bool isUpsert = false)
        {
            return await UpdateFinal(findExpression, fieldExpression, value, fieldExpression2, value2, null,
                (string)null, null, (string)null, null, (string)null, null, (string)null, null,
                (string)null, null, (string)null, isUpsert);
        }

        public async Task<bool> UpdateFields<T, TA, TB, TC>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2, Expression<Func<T, TC>> fieldExpression3, TC value3, bool isUpsert = false)
        {
            return await UpdateFinal(findExpression, fieldExpression, value, fieldExpression2, value2, fieldExpression3,
                value3, null, (string)null, null, (string)null, null, (string)null, null, (string)null, null,
                (string)null, isUpsert);
        }

        public async Task<bool> UpdateFields<T, TA, TB, TC, TD>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2, Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4, bool isUpsert = false)
        {
            return await UpdateFinal(findExpression, fieldExpression, value, fieldExpression2, value2, fieldExpression3,
                value3, fieldExpression4, value4, null, (string)null, null, (string)null, null, (string)null, null,
                (string)null, isUpsert);
        }

        public async Task<bool> UpdateFields<T, TA, TB, TC, TD, TE>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2, Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4, Expression<Func<T, TE>> fieldExpression5, TE value5, bool isUpsert = false)
        {
            return await UpdateFinal(findExpression, fieldExpression, value, fieldExpression2, value2, fieldExpression3,
                value3, fieldExpression4, value4, fieldExpression5, value5, null, (string)null, null,
                (string)null, null, (string)null, isUpsert);
        }

        public async Task<bool> UpdateFields<T, TA, TB, TC, TD, TE, TF>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2, Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4, Expression<Func<T, TE>> fieldExpression5, TE value5, Expression<Func<T, TF>> fieldExpression6, TF value6, bool isUpsert = false)
        {
            return await UpdateFinal(findExpression, fieldExpression, value, fieldExpression2, value2, fieldExpression3,
                value3, fieldExpression4, value4, fieldExpression5, value5, fieldExpression6, value6, null,
                (string)null, null, (string)null, isUpsert);
        }

        public async Task<bool> UpdateFields<T, TA, TB, TC, TD, TE, TF, TG>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2, Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4, Expression<Func<T, TE>> fieldExpression5, TE value5, Expression<Func<T, TF>> fieldExpression6, TF value6, Expression<Func<T, TG>> fieldExpression7, TG value7, bool isUpsert = false)
        {
            return await UpdateFinal(findExpression, fieldExpression, value, fieldExpression2, value2, fieldExpression3,
                value3, fieldExpression4, value4, fieldExpression5, value5, fieldExpression6, value6,
                fieldExpression7, value7, null, (string)null, isUpsert);
        }

        public async Task<bool> UpdateFields<T, TA, TB, TC, TD, TE, TF, TG, TH>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2, Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4, Expression<Func<T, TE>> fieldExpression5, TE value5, Expression<Func<T, TF>> fieldExpression6, TF value6, Expression<Func<T, TG>> fieldExpression7, TG value7, Expression<Func<T, TH>> fieldExpression8, TH value8, bool isUpsert = false)
        {
            return await UpdateFinal(findExpression, fieldExpression, value, fieldExpression2, value2, fieldExpression3,
                value3, fieldExpression4, value4, fieldExpression5, value5, fieldExpression6, value6,
                fieldExpression7, value7, fieldExpression8, value8, isUpsert);
        }

        public async Task<bool> UpdateFinal<T, TA, TB, TC, TD, TE, TF, TG, TH>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2, Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4, Expression<Func<T, TE>> fieldExpression5, TE value5, Expression<Func<T, TF>> fieldExpression6, TF value6, Expression<Func<T, TG>> fieldExpression7, TG value7, Expression<Func<T, TH>> fieldExpression8, TH value8, bool isUpsert = false)
        {
            var findDefinition = Builders<T>.Filter.Where(findExpression);
            UpdateDefinition<T> updateDefinition = Builders<T>.Update.Set(fieldExpression, value);

            if (fieldExpression2 != null)
                updateDefinition = updateDefinition.Set(fieldExpression2, value2);

            if (fieldExpression3 != null)
                updateDefinition = updateDefinition.Set(fieldExpression3, value3);

            if (fieldExpression4 != null)
                updateDefinition = updateDefinition.Set(fieldExpression4, value4);

            if (fieldExpression5 != null)
                updateDefinition = updateDefinition.Set(fieldExpression5, value5);

            if (fieldExpression6 != null)
                updateDefinition = updateDefinition.Set(fieldExpression6, value6);

            if (fieldExpression7 != null)
                updateDefinition = updateDefinition.Set(fieldExpression7, value7);

            if (fieldExpression8 != null)
                updateDefinition = updateDefinition.Set(fieldExpression8, value8);

            UpdateOptions updateOptions = new UpdateOptions() { IsUpsert = isUpsert };

            var updateResult = await MongoHelper.UpdateOne(findDefinition, updateDefinition, updateOptions);

            return updateResult;
        }

        public async Task<bool> CollectionExists<T>(string connectionString = "")
        {
            return await MongoHelper.CollectionExistsAsync<T>(connectionString);
        }

        public async Task<List<string>> GetDBList(string connectionString = "")
        {
            return await MongoHelper.GetDBList(connectionString);
        }
    }
}