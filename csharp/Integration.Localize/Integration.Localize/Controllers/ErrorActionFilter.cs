// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Integration.Localize.Controllers
{
    public sealed class ErrorActionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            void SetError(string message, int statusCode)
            {
                context.Result = new ObjectResult(new ApiError
                {
                    ErrorCode = statusCode,
                    Message = message,
                })
                {
                    StatusCode = statusCode
                };
            }

            if (context.Exception is InvalidOperationException ex)
            {
                SetError(ex.Message, 400);
            }
            else if (context.Exception is SecurityException ex2)
            {
                SetError(ex2.Message, 403);
            }
            else
            {
                SetError(context.Exception.Message, 500);
            }
        }
    }
}
