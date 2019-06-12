using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Linq
{
    /// <summary>
    /// 排序顺序
    /// </summary>
    [Serializable]
    [DataContract]
    public enum OrderingDirection
    {
        /// <summary>
        /// 升序
        /// </summary>
        [EnumMember]
        Asc = 0,
        /// <summary>
        /// 降序
        /// </summary>
        [EnumMember]
        Desc = 1
    }
    /// <summary>
    /// 分页排序
    /// </summary>
    [Serializable]
    [DataContract]
    public class Ordering
    {
        /// <summary>
        /// 排序属性、字段名
        /// </summary>
        [DataMember(Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// 排序方向
        /// </summary>
        [DataMember(Order = 2)]
        public OrderingDirection Direction { get; set; }
    }

    /// <summary>
    /// 排序扩展
    /// </summary>
    public static class OrderingExtensions
    {
        // 名字对应成员缓存表达式
        private static ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), LambdaExpression> _nameMemberExpressionCache
            = ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), LambdaExpression>.Empty;
        // 名字访问成员缓存表达式
        private static ImmutableDictionary<(Type Type, string Name), LambdaExpression> _nameMemberAccessExpressionCache
            = ImmutableDictionary<(Type Type, string Name), LambdaExpression>.Empty;
        //// OrderBy委托缓存
        //private static ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), Delegate> _orderByActionCache
        //    = ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), Delegate>.Empty;
        //// ThenBy委托缓存
        //private static ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), Delegate> _thenByActionCache
        //    = ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), Delegate>.Empty;

        /// <summary>
        /// 创建Lambda表达式"_ => _.Name"
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="order">排序信息</param>
        /// <returns>Lambda表达式</returns>
        private static LambdaExpression CreateMemberAccessExpression<T>(Ordering order)
        {
            // 根据排序的名字，找到数据源类型的对应的属性或者字段信息
            MemberInfo member = typeof(T).GetMember(order.Name, MemberTypes.Property | MemberTypes.Field, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();
            if (member == null)
            {
                throw new InvalidProgramException("Not found the member: " + order.Name + " in " + typeof(T).FullName);
            }
            // 属性或者字段的类型
            var memberType = member is PropertyInfo ? ((PropertyInfo)member).PropertyType : ((FieldInfo)member).FieldType;
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), memberType);
            var parameter = Expression.Parameter(typeof(T), "_");
            return Expression.Lambda(
                delegateType,
                Expression.MakeMemberAccess(parameter, member),
                parameter);
        }
        /// <summary>
        /// 创建Lambda表达式"_ => _.Name"
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="order">排序信息</param>
        /// <returns>Lambda表达式</returns>
        private static LambdaExpression CreateMemberAccessExpression<T>(string name)
        {
            // 根据排序的名字，找到数据源类型的对应的属性或者字段信息
            MemberInfo member = typeof(T).GetMember(name, MemberTypes.Property | MemberTypes.Field, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public).FirstOrDefault();
            if (member == null)
            {
                throw new InvalidProgramException("Not found the member: " + name + " in " + typeof(T).FullName);
            }
            // 属性或者字段的类型
            var memberType = member is PropertyInfo ? ((PropertyInfo)member).PropertyType : ((FieldInfo)member).FieldType;
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), memberType);
            var parameter = Expression.Parameter(typeof(T), "_");
            return Expression.Lambda(
                delegateType,
                Expression.MakeMemberAccess(parameter, member),
                parameter);
        }
        /// <summary>
        /// 创建排序表达式
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="memberType">排序字段类型</param>
        /// <param name="order">排序信息</param>
        /// <param name="orderMethod">排序方法名("OrderBy","ThenBy")</param>
        /// <returns>Lambda表达式</returns>
        private static LambdaExpression CreateFuncExpression<T>(Type memberType, Ordering order, string orderMethod)
        {
            string funName = orderMethod;
            if (order.Direction == OrderingDirection.Desc)
            {
                funName += "Descending";
            }

            // OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
            var orderMethodInfo = typeof(Queryable).GetMethods().FirstOrDefault(_ => _.Name == funName && _.GetParameters().Length == 2);

            // (query, orderExpression) => query.OrderBy(orderExpression);
            var query = Expression.Parameter(typeof(IQueryable<T>), "query");
            var orderExpressionType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(T), memberType));
            var orderExpression = Expression.Parameter(orderExpressionType, "orderExpression");

            return Expression.Lambda(
                Expression.Call(orderMethodInfo.MakeGenericMethod(typeof(T), memberType), query, orderExpression),
                query,
                orderExpression);
        }

        /// <summary>
        /// 创建OrderBy表达式
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="memberType">排序字段类型</param>
        /// <param name="order">排序信息</param>
        /// <param name="orderMethod">排序方法名("OrderBy","ThenBy")</param>
        /// <returns>Lambda表达式</returns>
        private static LambdaExpression CreateOrderByExpression<T>(Type memberType, Ordering order, Expression query, Expression memberAccessExpression)
        {
            string funName = "OrderBy";
            if (order.Direction == OrderingDirection.Desc)
            {
                funName += "Descending";
            }
            MethodCallExpression orderExpression = Expression.Call(typeof(Queryable), funName, new[] { typeof(T), memberType }, query, Expression.Quote(memberAccessExpression));
            return Expression.Lambda(orderExpression);
        }

        /// <summary>
        /// 创建ThenBy表达式
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="memberType">排序字段类型</param>
        /// <param name="order">排序信息</param>
        /// <param name="orderMethod">排序方法名("OrderBy","ThenBy")</param>
        /// <returns>Lambda表达式</returns>
        private static LambdaExpression CreateThenByExpression<T>(Type memberType, Ordering order, Expression query, Expression memberAccessExpression)
        {
            string funName = "ThenBy";
            if (order.Direction == OrderingDirection.Desc)
            {
                funName += "Descending";
            }
            MethodCallExpression orderExpression = Expression.Call(typeof(Queryable), funName, new[] { typeof(T), memberType }, query, Expression.Quote(memberAccessExpression));
            return Expression.Lambda(orderExpression);
        }

        /// <summary>
        /// 创建OrderBy处理委托，OrderBy(x => x.Value) 或者 OrderByDescending(x => x.Value)
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="memberType">排序字段类型</param>
        /// <param name="order">排序信息</param>
        /// <returns>委托</returns>
        private static Delegate CreateOrderByAction<T>(Type memberType, Ordering order, Expression query, Expression memberAccessExpression)
        {
            return CreateOrderByExpression<T>(memberType, order, query, memberAccessExpression).Compile();
        }

        /// <summary>
        /// 创建OrderBy处理委托，OrderBy(x => x.Value) 或者 OrderByDescending(x => x.Value)
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="memberType">排序字段类型</param>
        /// <param name="order">排序信息</param>
        /// <returns>委托</returns>
        private static Delegate GetOrderByAction<T>(Type memberType, Ordering order, Expression query, Expression memberAccessExpression)
        {
            return CreateOrderByExpression<T>(memberType, order, query, memberAccessExpression).Compile();
        }

        private static MethodInfo OrderByMethodInfo = typeof(Queryable).GetMethods().FirstOrDefault(_ => _.Name == nameof(Queryable.OrderBy) && _.GetParameters().Length == 2);
        private static MethodInfo OrderByDescendingMethodInfo = typeof(Queryable).GetMethods().FirstOrDefault(_ => _.Name == nameof(Queryable.OrderByDescending) && _.GetParameters().Length == 2);
        private static MethodInfo ThenByMethodInfo = typeof(Queryable).GetMethods().FirstOrDefault(_ => _.Name == nameof(Queryable.ThenBy) && _.GetParameters().Length == 2);
        private static MethodInfo ThenByDescendingMethodInfo = typeof(Queryable).GetMethods().FirstOrDefault(_ => _.Name == nameof(Queryable.ThenByDescending) && _.GetParameters().Length == 2);

        /// <summary>
        /// 创建ThenBy排序处理委托，ThenBy(x => x.Value) 或者 ThenByDescending(x => x.Value)
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="memberType">排序字段类型</param>
        /// <param name="order">排序信息</param>
        /// <returns>委托</returns>
        private static Delegate CreateThenByAction<T>(Type memberType, Ordering order, Expression query, Expression memberAccessExpression)
        {
            return CreateThenByExpression<T>(memberType, order, query, memberAccessExpression).Compile();
        }

        /// <summary>
        /// 排序 OrderBy
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="query">数据源</param>
        /// <param name="order">排序信息</param>
        /// <returns>排序结果</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, Ordering order)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            (Type Type, string Name) key = (typeof(T), order.Name.ToUpper());
            // 尝试从缓存中加载，不存在时创建
            if (!_nameMemberAccessExpressionCache.TryGetValue(key, out LambdaExpression expression))
            {
                expression = CreateMemberAccessExpression<T>(order.Name);
                _nameMemberAccessExpressionCache = _nameMemberAccessExpressionCache.Add(key, expression);
            }
            MethodInfo methodInfo;
            if (order.Direction == OrderingDirection.Asc)
            {
                methodInfo = OrderByMethodInfo;
            }
            else
            {
                methodInfo = OrderByDescendingMethodInfo;
            }

            Type p1 = typeof(T);
            Type p2 = expression.ReturnType;
            methodInfo = methodInfo.MakeGenericMethod(p1, p2);
            object[] parameters = { query, expression };
            return (IOrderedQueryable<T>)methodInfo.Invoke(null, parameters);
            //if (!_orderByActionCache.TryGetValue(key, out Delegate action))
            //{
            //    action = CreateOrderByAction<T>(expression.ReturnType, order, query.Expression, expression);
            //    _orderByActionCache = _orderByActionCache.Add(key, action);
            //}
            //var action = CreateOrderByAction<T>(expression.ReturnType, order, query.Expression, expression);
            //return (IOrderedQueryable<T>)query.Provider.CreateQuery(expression);
            //return (IOrderedQueryable<T>)action.DynamicInvoke();
        }

        /// <summary>
        /// 后续排序 ThenBy
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="query">数据源</param>
        /// <param name="order">排序信息</param>
        /// <returns>排序结果</returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> query, Ordering order)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            (Type Type, string Name) key = (typeof(T), order.Name.ToUpper());
            // 尝试从缓存中加载，不存在时创建
            if (!_nameMemberAccessExpressionCache.TryGetValue(key, out LambdaExpression expression))
            {
                expression = CreateMemberAccessExpression<T>(order.Name);
                _nameMemberAccessExpressionCache = _nameMemberAccessExpressionCache.Add(key, expression);
            }
            MethodInfo methodInfo;
            if (order.Direction == OrderingDirection.Asc)
            {
                methodInfo = ThenByMethodInfo;
            }
            else
            {
                methodInfo = ThenByDescendingMethodInfo;
            }
            Type p1 = typeof(T);
            Type p2 = expression.ReturnType;
            methodInfo = methodInfo.MakeGenericMethod(p1, p2);
            object[] parameters = { query, expression };
            return (IOrderedQueryable<T>)methodInfo.Invoke(null, parameters);
            //if (!_thenByActionCache.TryGetValue(key, out Delegate action))
            //{
            //    action = CreateThenByAction<T>(expression.ReturnType, order, query.Expression, expression);
            //    _thenByActionCache = _thenByActionCache.Add(key, action);
            //}
            //var action = CreateThenByAction<T>(expression.ReturnType, order, query.Expression, expression);
            //return (IOrderedQueryable<T>)action.DynamicInvoke();
            //return (IOrderedQueryable<T>)query.Provider.CreateQuery(expression);
        }

        /// <summary>
        /// 执行多重排序
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="query">数据源</param>
        /// <param name="orders">排序信息</param>
        /// <returns>排序结果</returns>
        public static IOrderedQueryable<T> OrderAndThenBy<T>(this IQueryable<T> query, IEnumerable<Ordering> orders)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (orders == null)
            {
                throw new ArgumentNullException(nameof(orders));
            }
            List<Ordering> orderList = new List<Ordering>(orders);
            if (!orderList.Any())
            {
                throw new ArgumentException(nameof(orders) + ": is empty.");
            }
            var orderedQuery = OrderBy(query, orderList[0]);
            int count = orderList.Count;
            for (int i = 1; i < count; i++)
            {
                orderedQuery = ThenBy(orderedQuery, orderList[i]);
            }
            return orderedQuery;
        }
    }
}
