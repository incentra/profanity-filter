using System;
using System.Diagnostics;
using System.Text;

namespace SP.Profanity.Utilities
{
    public static class ExceptionUtils
    {
        
        public static string ExceptionMessage(this Exception ex)
        {
            return ex?.InnerException.ExTrimmedMessage() ?? ex.ExMessage();
        }

        public static string ExTrimmedMessage(this Exception ex)
        {
            return ex?.Message?.Trim();
        }

        public static string ExMessage(this Exception ex)
        {
            return ex.ExTrimmedMessage() ?? "Unknown exception. Exception message is null.";
        }

        public static string GetExceptionMessages(this Exception ex, int exceptionCount = 10)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ex.WriteExceptionMessagesToFunction(stringBuilder.WriteMessageToStringBuilder, stringBuilder.WriteMessageToStringBuilder,  exceptionCount);
            return stringBuilder.ToString();
        }

        public static void WriteExceptionMessagesToConsole(this Exception ex, int exceptionCount = 10)
        {
            ex.WriteExceptionMessagesToFunction(WriteMessageToConsole, WriteMessageToConsole, exceptionCount);
        }

        public static void WriteExceptionMessagesToDebug(this Exception ex, int exceptionCount = 10)
        {
            ex.WriteExceptionMessagesToFunction(WriteMessageToDebug, WriteMessageToDebug, exceptionCount);
        }

        public static void WriteMessageToConsole(string message)
        {
            Console.WriteLine($"******** {message}");
        }

        public static void WriteMessageToDebug(string message)
        {
            Debug.WriteLine($"******** {message}");
        }

        public static void WriteMessageToStringBuilder(this StringBuilder builder, string message)
        {
            builder.AppendLine(message);
        }

        public static void WriteExceptionMessagesToFunction(this Exception ex, Action<string> exFunc, Action<string> innerExFunc, int exceptionCount = 10)
        {
            string message = ex.ExMessage();
            if (!string.IsNullOrEmpty(message))
            {
                exFunc($"ERROR! Message: {message}");
            }
            Exception innerEx = ex?.InnerException;
            for (int i = exceptionCount; innerEx != null && i > 0; i--)
            {
                message = innerEx.ExMessage();
                if (!string.IsNullOrEmpty(message))
                {
                    innerExFunc($"Inner Exception Message: {message}");
                }
                innerEx = innerEx?.InnerException;
            }
        }

    }
}