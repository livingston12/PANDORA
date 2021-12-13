using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;

namespace Pandora.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            context = OverrideContext(context);
            base.OnException(context);
        }

        private static ExceptionContext OverrideContext(ExceptionContext context)
        {
            context = OverrideTimeOutException(context);
            context = OverrideArgumentException(context);
            context = OverrideInvalidDataException(context);
            return context;
        }

        private static ExceptionContext OverrideTimeOutException(ExceptionContext context)
        {
            if (context != null && IsTableLocked(context.Exception))
            {
                context.ExceptionHandled = true;
                context.Result = new ObjectResult("Database is occupied")
                {
                    StatusCode = 503
                };
            }

            return context;
        }

        private static bool IsTableLocked(Exception exception)
        {
            return exception.Message
                    .Contains("The timeout period elapsed prior to completion") ||
                (exception.InnerException != null
                    && exception.InnerException.Message.Contains("The timeout period elapsed prior to completion"));
        }

        private static ExceptionContext OverrideArgumentException(ExceptionContext context)
        {
            var exception = context.Exception as ArgumentException;
            var dictionary = exception != null && exception.Data != null ? exception.Data["Error"] : null;
            if (dictionary != null)
            {
                context.ExceptionHandled = true;
                var error = new
                {
                    Error = dictionary
                };

                context.Result = new BadRequestObjectResult(error);
            }
            else if (exception != null)
            {
                context.ExceptionHandled = true;
                context.Result = new BadRequestObjectResult(exception.Message);
            }

            return context;
        }

        private static ExceptionContext OverrideInvalidDataException(ExceptionContext context)
        {
            var exception = context.Exception as InvalidDataException;
            if (exception != null)
            {
                context.ExceptionHandled = true;
                context.Result = new BadRequestObjectResult(exception.Message);
            }
            return context;
        }
    }
}