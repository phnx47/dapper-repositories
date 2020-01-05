using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MicroOrm.Dapper.Repositories.SqlGenerator.QueryExpressions
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class Sql
    {
        /// <summary>
        /// 
        /// </summary>
        public static string VARCHAR = nameof(VARCHAR);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<object> Flatten(IEnumerable list)
        {
            var ret = new List<object>();
            if (list == null) return ret;

            foreach (var item in list)
            {
                if (item == null) continue;

                if (item is IEnumerable arr && !(item is string))
                {
                    ret.AddRange(arr.Cast<object>());
                }
                else
                {
                    ret.Add(item);
                }
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <returns></returns>
        public static bool In<T, TItem>(T value, params TItem[] list) => value != null && Flatten(list).Any(obj => obj.ToString() == value.ToString());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="asValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string As<T>(T value, object asValue) => value == null ? "" : $"{value} AS {asValue}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Sum<T>(T value) => value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Sum(string value) => $"SUM({value})";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Min<T>(T value) => value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Min(string value) => $"MIN({value})";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Max<T>(T value) => value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Max(string value) => $"MAX({value})";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Avg<T>(T value) => value;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Avg(string value) => $"AVG({value})";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T AllFields<T>(T item) => item;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customSql"></param>
        /// <returns></returns>
        public static string Custom(string customSql) => customSql;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customSql"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Custom<T>(string customSql) => default(T);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="castAs"></param>
        /// <returns></returns>
        public static string Cast(object value, string castAs) => $"CAST({value} AS {castAs})";

    }
}
