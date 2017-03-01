using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Expressions.Helpers
{
    public static class ExpressionHelper
    {

        public static Expression For(Expression todo, Expression count)
        {
            return For(todo, count, Expression.Constant(1));
        }
        public static Expression For(Expression todo, Expression count, Expression increment)
        {
            return For(todo, Expression.Constant(0), count, increment);
        }
        public static Expression For(Expression todo, Expression start, Expression count, Expression increment)
        {
            LabelTarget label = Expression.Label(typeof(int));
            ParameterExpression itt = Expression.Variable(typeof(int), "i");

            return Expression.Block(
                new ParameterExpression[] {itt},
                Expression.Assign(itt, start),
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(itt, count),
                        Expression.Block(
                            Expression.Increment(itt),
                            todo
                        ),
                        Expression.Break(label)
                    ),
                    label
                )
            );
        }


        public static Expression Foreach(Expression collection, Expression itterator, Expression todo)
        {
            Type elementType = itterator.Type;
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            Type enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            ParameterExpression enumVar = Expression.Variable(enumeratorType, "enumerator");
            MethodCallExpression next = Expression.Call(enumVar, typeof(IEnumerator).GetMethod("MoveNext"));
            MethodCallExpression getEnum = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator"));
            BinaryExpression enumAssign = Expression.Assign(enumVar, getEnum);
            LabelTarget breakLabel = Expression.Label("OuttaHere");

            return Expression.Block(new[] { enumVar },
                enumAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(next, Expression.Constant(true)),
                        Expression.Block(
                            Expression.Assign(itterator, Expression.Property(enumVar, "Current")),
                            todo
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );
        }
    }
}
