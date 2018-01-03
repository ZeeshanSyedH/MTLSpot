using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MTLSpotScraper.Interfaces
{
    public interface IMongoRepo
    {
        Task<bool> IncOneField<T, TA>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA inc);

        Task<bool> UpdateMany<T, TA>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression, TA value);

        Task<bool> UnsetOneField<T>(Expression<Func<T, bool>> findExpression,
            Expression<Func<T, dynamic>> fieldExpression);

        Task<T> FindOne<T>(Expression<Func<T, bool>> findExpression1, Expression<Func<T, bool>> findExpression2 = null, Expression<Func<T, bool>> findExpression3 = null);

        Task<T> FindOne<T>(bool useExchangeRateDb = false);

        Task<bool> Insert<T>(T obj);

        Task<bool> InsertBatch<T>(List<T> list);

        Task<long> Count<T>(Expression<Func<T, bool>> expression);

        Task<long> CountWithTwoExpression<T>(Expression<Func<T, dynamic>> findExpression1, dynamic value1,
            Expression<Func<T, dynamic>> findExpression2, dynamic value2);

        Task<long> CollectionCount<T>();

        Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> expression, int? batchSize = null, int? skip = null,
            int? limit = null);

        Task<IEnumerable<T>> FindWithFourOrLessExpressions<T>(Expression<Func<T, bool>> expression1,
            Expression<Func<T, bool>> expression2 = null, Expression<Func<T, bool>> expression3 = null, Expression<Func<T, bool>> expression4 = null, int? batchSize = null, int? skip = null, int? limit = null);

        Task<bool> DeleteOne<T>(Expression<Func<T, bool>> findExpression);

        Task<bool> DeleteMany<T>(Expression<Func<T, bool>> findExpression);

        Task<IEnumerable<T>> FindAll<T>(bool sort = false, Expression<Func<T, object>> sortExpression = null, bool asc = true);

        Task<bool> UpdateFields<T, TA>(Expression<Func<T, bool>> findExpression, Expression<Func<T, TA>> fieldExpression,
            TA value, bool isUpsert = false);

        Task<bool> UpdateFields<T, TA, TB>(Expression<Func<T, bool>> findExpression,
            Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2,
            bool isUpsert = false);

        Task<bool> UpdateFields<T, TA, TB, TC>(Expression<Func<T, bool>> findExpression,
            Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2,
            Expression<Func<T, TC>> fieldExpression3, TC value3, bool isUpsert = false);

        Task<bool> UpdateFields<T, TA, TB, TC, TD>(Expression<Func<T, bool>> findExpression,
            Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2,
            Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4,
            bool isUpsert = false);

        Task<bool> UpdateFields<T, TA, TB, TC, TD, TE>(Expression<Func<T, bool>> findExpression,
            Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2,
            Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4,
            Expression<Func<T, TE>> fieldExpression5, TE value5, bool isUpsert = false);

        Task<bool> UpdateFields<T, TA, TB, TC, TD, TE, TF>(Expression<Func<T, bool>> findExpression,
            Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2,
            Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4,
            Expression<Func<T, TE>> fieldExpression5, TE value5, Expression<Func<T, TF>> fieldExpression6, TF value6,
            bool isUpsert = false);

        Task<bool> UpdateFields<T, TA, TB, TC, TD, TE, TF, TG>(Expression<Func<T, bool>> findExpression,
            Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2,
            Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4,
            Expression<Func<T, TE>> fieldExpression5, TE value5, Expression<Func<T, TF>> fieldExpression6, TF value6,
            Expression<Func<T, TG>> fieldExpression7, TG value7, bool isUpsert = false);

        Task<bool> UpdateFields<T, TA, TB, TC, TD, TE, TF, TG, TH>(Expression<Func<T, bool>> findExpression,
            Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2,
            Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4,
            Expression<Func<T, TE>> fieldExpression5, TE value5, Expression<Func<T, TF>> fieldExpression6, TF value6,
            Expression<Func<T, TG>> fieldExpression7, TG value7, Expression<Func<T, TH>> fieldExpression8, TH value8,
            bool isUpsert = false);

        Task<bool> UpdateFinal<T, TA, TB, TC, TD, TE, TF, TG, TH>(Expression<Func<T, bool>> findExpression,
            Expression<Func<T, TA>> fieldExpression, TA value, Expression<Func<T, TB>> fieldExpression2, TB value2,
            Expression<Func<T, TC>> fieldExpression3, TC value3, Expression<Func<T, TD>> fieldExpression4, TD value4,
            Expression<Func<T, TE>> fieldExpression5, TE value5, Expression<Func<T, TF>> fieldExpression6, TF value6,
            Expression<Func<T, TG>> fieldExpression7, TG value7, Expression<Func<T, TH>> fieldExpression8, TH value8,
            bool isUpsert = false);

        Task<bool> CollectionExists<T>(string connectionString = "");

        Task<List<string>> GetDBList(string connectionString = "");
    }
}