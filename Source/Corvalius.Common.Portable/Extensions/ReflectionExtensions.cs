using System.Linq.Expressions;

namespace System.Reflection
{
    /// <summary>
    /// These methods use expression trees instead of reflection to get property names.
    /// </summary>
    public static class ReflectionHelper
    {
        public static string GetPropertyName<T>(Expression<Func<T, object>> propertyExpression)
        {
            MemberExpression memberExpression = GetMemberExpression(propertyExpression);

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo != null)
                return propertyInfo.Name;

            return string.Empty;
        }

        public static string GetPropertyName(Expression<Func<object>> propertyExpression)
        {
            MemberExpression memberExpression = GetMemberExpression(propertyExpression);

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo != null)
                return propertyInfo.Name;

            return string.Empty;
        }

        public static string GetMemberName(Expression<Func<object>> propertyExpression)
        {
            MemberExpression memberExpression = GetMemberExpression(propertyExpression);
            if (memberExpression != null)
                return memberExpression.Member.Name;

            return string.Empty;
        }

        public static object GetBindedSource(Expression<Func<object>> propertyExpression)
        {
            MemberExpression memberExpression = GetMemberExpression(propertyExpression);

            var constantExpression = memberExpression.Expression as ConstantExpression;
            if (constantExpression == null)
                throw new ArgumentException(@"The expression must be constructed as: ""() => this.Property"".", "propertyExpression");

            return constantExpression.Value;
        }

        public static object GetBindedSource<T>(Expression<Func<object>> propertyExpression)
        {
            MemberExpression memberExpression = GetMemberExpression(propertyExpression);

            var constantExpression = memberExpression.Expression as ConstantExpression;
            if (constantExpression == null)
                throw new ArgumentException(@"The expression must be constructed as: ""() => this.Property"".", "propertyExpression");

            return (T)constantExpression.Value;
        }

        private static MemberExpression GetMemberExpression(Expression propertyExpression)
        {
            var lambda = (LambdaExpression) propertyExpression;
            
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) lambda.Body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            return memberExpression;
        }
    }
}
