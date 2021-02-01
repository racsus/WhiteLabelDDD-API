using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using WhiteLabel.Domain.Generic;
using WhiteLabel.WebAPI.Models;

namespace WhiteLabelDDD.Exceptions
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next) { this.next = next; }
        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = JsonConvert.SerializeObject(new Response<string>(exception.Message));
            context.Response.ContentType = "application/json"; 
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}