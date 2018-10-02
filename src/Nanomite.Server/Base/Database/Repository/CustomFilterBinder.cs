///-----------------------------------------------------------------
///   File:         CustomFilterBinder.cs
///   Author:   	Andre Laskawy           
///   Date:         02.10.2018 17:29:21
///-----------------------------------------------------------------

namespace Nanomite.Server.Base.Database.Repository
{
    using Community.OData.Linq.OData.Query.Expressions;
    using Microsoft.OData.UriParser;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Defines the <see cref="CustomFilterBinder" />
    /// </summary>
    public class CustomFilterBinder : FilterBinder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomFilterBinder"/> class.
        /// </summary>
        /// <param name="requestContainer">The request container.</param>
        public CustomFilterBinder(IServiceProvider requestContainer)
           : base(requestContainer)
        {
        }

        /// <summary>
        /// Binds the binary operator node.
        /// </summary>
        /// <param name="binaryOperatorNode">The binary operator node.</param>
        /// <returns>the expression</returns>
        public override Expression BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode)
        {
            Expression left = this.Bind(binaryOperatorNode.Left);
            Expression right = this.Bind(binaryOperatorNode.Right);

            Type leftUnderlyingType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
            Type rightUnderlyingType = Nullable.GetUnderlyingType(right.Type) ?? right.Type;

            // to the stuff they do anyway
            var result = base.BindBinaryOperatorNode(binaryOperatorNode);

            // if datetime convert to utc not local
            if ((leftUnderlyingType == typeof(DateTime) && rightUnderlyingType == typeof(DateTimeOffset))
                || (rightUnderlyingType == typeof(DateTime) && leftUnderlyingType == typeof(DateTimeOffset)))
            {
                left = DateTimeOffsetToDateTime(left);
                right = DateTimeOffsetToDateTime(right);
                return Expression.MakeBinary(result.NodeType, left, right, (result as BinaryExpression).IsLiftedToNull, method: (result as BinaryExpression).Method);
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Datetime offset to date time.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>the expression</returns>
        private Expression DateTimeOffsetToDateTime(Expression expression)
        {
            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                if (Nullable.GetUnderlyingType(unaryExpression.Type) == unaryExpression.Operand.Type)
                {
                    // this is a cast from T to Nullable<T> which is redundant.
                    expression = unaryExpression.Operand;
                }
            }
            var parameterizedConstantValue = ExtractParameterizedConstant(expression);
            var dto = parameterizedConstantValue as DateTimeOffset?;
            if (dto != null)
            {
                DateTimeOffset dateTimeOffsetValue = (DateTimeOffset)dto.Value;
                dateTimeOffsetValue = TimeZoneInfo.ConvertTime(dateTimeOffsetValue, TimeZoneInfo.Utc);
                expression = Expression.Constant(dateTimeOffsetValue.DateTime);
            }
            return expression;
        }

        /// <summary>
        /// Extract the constant that would have been encapsulated into LinqParameterContainer if this
        /// expression represents it else return null.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>the object</returns>
        private object ExtractParameterizedConstant(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression memberAccess = expression as MemberExpression;
                Contract.Assert(memberAccess != null);
                if (memberAccess.Expression.NodeType == ExpressionType.Constant)
                {
                    ConstantExpression constant = memberAccess.Expression as ConstantExpression;
                    Contract.Assert(constant != null);
                    Contract.Assert(constant.Value != null);

                    PropertyInfo property = constant.Value
                        .GetType()
                        .GetProperty("Property");

                    return property.GetValue(constant.Value);
                }
            }

            return null;
        }
    }
}
