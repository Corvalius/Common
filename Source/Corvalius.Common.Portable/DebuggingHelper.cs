using System;
using System.Diagnostics;
using System.Net;

namespace Corvalius.Common
{
    public static class DebuggingHelper
    {
        public static void Break()
        {
            if (Debugger.IsAttached)
                Debugger.Break();
        }
    }
}
