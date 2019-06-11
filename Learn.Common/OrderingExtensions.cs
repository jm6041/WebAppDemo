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
        // OrderBy委托缓存
        private static ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), Delegate> _orderByActionCache
            = ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), Delegate>.Empty;
        // ThenBy委托缓存
        private static ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), Delegate> _thenByActionCache
            = ImmutableDictionary<(Type Type, string Name, OrderingDirection Direction), Delegate>.Empty;

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
        /// 创建排序表达式
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="memberType">排序字段类型</param>
        /// <param name="order">排序信息</param>
        /// <param name="orderMethod">排序方法名("OrderBy","ThenBy")</param>
        /// <returns>Lambda表达式</returns>
        private static LambdaExpression CreateOrderFuncExpression<T>(Type memberType, Ordering order, string orderMethod, Expression memberAccessExpression)
        {
            string funName = orderMethod;
            if (order.Direction == OrderingDirection.Desc)
            {
                funName += "Descending";
            }

            // OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
            var orderMethodInfo = typeof(Queryable).GetMethods().FirstOrDefault(_ => _.Name == funName && _.GetParameters().Length == 2);

            // (query, orderExpression) => query.OrderBy(orderExpression);
            // var query = Expression.Parameter(typeof(IQueryable<T>), "query");
            var orderExpressionType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(T), memberType));
            var orderExpression = Expression.Parameter(orderExpressionType, "orderExpression");
            return Expression.Lambda(
                Expression.Call(orderMethodInfo.MakeGenericMethod(typeof(T), memberType), orderExpression),
                orderExpression);
        }

        /*public static LabelExpression DDD<T>()
        {
            string[] companies = { "Consolidated Messenger", "Alpine Ski House", "Southridge Video", "City Power & Light",
                   "Coho Winery", "Wide World Importers", "Graphic Design Institute", "Adventure Works",
                   "Humongous Insurance", "Woodgrove Bank", "Margie's Travel", "Northwind Traders",
                   "Blue Yonder Airlines", "Trey Research", "The Phone Company",
                   "Wingtip Toys", "Lucerne Publishing", "Fourth Coffee" };

            companies.Where(company => (company.ToLower() == "coho winery" || company.Length > 16)).OrderBy(company => company);

            // The IQueryable data to query.  
            IQueryable<String> queryableData = companies.AsQueryable<string>();

            // Compose the expression tree that represents the parameter to the predicate.  
            ParameterExpression pe = Expression.Parameter(typeof(string), "company");

            // ***** Where(company => (company.ToLower() == "coho winery" || company.Length > 16)) *****  
            // Create an expression tree that represents the expression 'company.ToLower() == "coho winery"'.  
            Expression left = Expression.Call(pe, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
            Expression right = Expression.Constant("coho winery");
            Expression e1 = Expression.Equal(left, right);

            // Create an expression tree that represents the expression 'company.Length > 16'.  
            left = Expression.Property(pe, typeof(string).GetProperty("Length"));
            right = Expression.Constant(16, typeof(int));
            Expression e2 = Expression.GreaterThan(left, right);

            // Combine the expression trees to create an expression tree that represents the  
            // expression '(company.ToLower() == "coho winery" || company.Length > 16)'.  
            Expression predicateBody = Expression.OrElse(e1, e2);

            // Create an expression tree that represents the expression  
            // 'queryableData.Where(company => (company.ToLower() == "coho winery" || company.Length > 16))'  
            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { queryableData.ElementType },
                queryableData.Expression,
                Expression.Lambda<Func<string, bool>>(predicateBody, new ParameterExpression[] { pe }));
            // ***** End Where *****  

            // ***** OrderBy(company => company) *****  
            // Create an expression tree that represents the expression  
            // 'whereCallExpression.OrderBy(company => company)'  
            MethodCallExpression orderByCallExpression = Expression.Call(
                typeof(Queryable),
                "OrderBy",
                new Type[] { queryableData.ElementType, queryableData.ElementType },
                whereCallExpression,
                Expression.Lambda<Func<string, string>>(pe, new ParameterExpression[] { pe }));
            // ***** End OrderBy *****  

            // Create an executable query from the expression tree.  
            IQueryable<string> results = queryableData.Provider.CreateQuery<string>(orderByCallExpression);
        }*/

        /// <summary>
        /// 创建OrderBy处理委托，OrderBy(x => x.Value) 或者 OrderByDescending(x => x.Value)
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="memberType">排序字段类型</param>
        /// <param name="order">排序信息</param>
        /// <returns>委托</returns>
        private static Delegate CreateOrderByAction<T>(Type memberType, Ordering order, Expression memberAccessExpression)
        {
            return CreateOrderFuncExpression<T>(memberType, order, "OrderBy", memberAccessExpression).Compile();
        }

        /// <summary>
        /// 创建ThenBy排序处理委托，ThenBy(x => x.Value) 或者 ThenByDescending(x => x.Value)
        /// </summary>
        /// <typeparam name="T">数据源类型</typeparam>
        /// <param name="memberType">排序字段类型</param>
        /// <param name="order">排序信息</param>
        /// <returns>委托</returns>
        private static Delegate CreateThenByAction<T>(Type memberType, Ordering order, Expression memberAccessExpression)
        {
            return CreateOrderFuncExpression<T>(memberType, order, "ThenBy", memberAccessExpression).Compile();
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
            (Type Type, string Name, OrderingDirection Direction) key = (typeof(T), order.Name.ToUpper(), order.Direction);
            // 尝试从缓存中加载，不存在时创建
            if (!_nameMemberExpressionCache.TryGetValue(key, out LambdaExpression expression))
            {
                expression = CreateMemberAccessExpression<T>(order);
                _nameMemberExpressionCache = _nameMemberExpressionCache.Add(key, expression);
            }
            var action = expression.Compile();
            return (IOrderedQueryable<T>)action.DynamicInvoke(query, expression);
            //Expression<Func<T, dynamic>> e = (Expression<Func<T, dynamic>>)expression;
            //if (order.Direction == OrderingDirection.Asc)
            //{

            //    return query.OrderBy(e);
            //}
            //else
            //{
            //    return query.OrderByDescending(e);
            //}
            //if (!_orderByActionCache.TryGetValue(key, out Delegate action))
            //{
            //    action = CreateOrderByAction<T>(expression.ReturnType, order, expression);
            //    _orderByActionCache = _orderByActionCache.Add(key, action);
            //}
            //return (IOrderedQueryable<T>)action.DynamicInvoke(query, expression);
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
            (Type Type, string Name, OrderingDirection Direction) key = (typeof(T), order.Name.ToUpper(), order.Direction);
            // 尝试从缓存中加载，不存在时创建
            if (!_nameMemberExpressionCache.TryGetValue(key, out LambdaExpression expression))
            {
                expression = CreateMemberAccessExpression<T>(order);
                _nameMemberExpressionCache = _nameMemberExpressionCache.Add(key, expression);
            }
            Expression<Func<T, dynamic>> e = (Expression<Func<T, dynamic>>)expression;
            if (order.Direction == OrderingDirection.Asc)
            {
                return query.ThenBy(e);
            }
            else
            {
                return query.ThenByDescending(e);
            }
            //if (!_thenByActionCache.TryGetValue(key, out Delegate action))
            //{
            //    action = CreateThenByAction<T>(expression.ReturnType, order, expression);
            //    _thenByActionCache = _thenByActionCache.Add(key, action);
            //}
            //return (IOrderedQueryable<T>)action.DynamicInvoke(query, expression);
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
