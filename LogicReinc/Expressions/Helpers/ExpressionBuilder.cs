using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Expressions.Helpers
{
    public class ExpressionBuilder
    {
        public List<ParameterExpression> Parameters { get; private set; } = new List<ParameterExpression>();
        public List<Expression> Expressions { get; private set; } = new List<Expression>();

        public ParameterExpression Add(ParameterExpression para)
        {
            Parameters.Add(para);
            return para;
        }

        public T Add<T>(Expression expression) where T : Expression
        {
            Expressions.Add(expression);
            return (T)expression;
        }

        public Expression Add(Expression expression)
        {
            Expressions.Add(expression);
            return expression;
        }

        public void Add(Expression[] expressions)
        {
            Expressions.AddRange(expressions);
        }

        public BlockExpression Build()
        {
            return Expression.Block(Parameters.ToArray(), Expressions);
        }
    }
}
