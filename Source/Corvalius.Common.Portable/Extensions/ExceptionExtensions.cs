using System;
using System.ServiceModel;
using System.Text;

namespace Corvalius.Common
{
    public static class ExceptionExtensions
    {
        public static string GetFullException(this Exception exception)
        {
            if (exception != null)
            {
                var sb = new StringBuilder();
                Exception temp = exception;

                do
                {
                    sb.AppendLine(temp.Message);
                    temp = temp.InnerException;
                } while (temp != null);

                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
