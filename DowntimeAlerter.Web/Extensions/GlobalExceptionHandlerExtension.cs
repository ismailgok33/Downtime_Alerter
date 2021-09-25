using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DowntimeAlerter.Web.Extensions
{
    public static class GlobalExceptionHandlerExtension
    {
        //This method will globally handle logging unhandled execeptions.
        //It will respond json response for ajax calls that send the json accept header
        //otherwise it will redirect to an error page
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app
                                                    , ILogger logger
                                                    , string errorPagePath
                                                    , bool respondWithJsonErrorDetails = false)
        {
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    //============================================================
                    //Log Exception
                    //============================================================
                    var exception = context.Features.Get<IExceptionHandlerFeature>().Error;

                    string errorDetails = $@"{exception.Message}
                                         {Environment.NewLine}
                                         {exception.StackTrace}";

                    int statusCode = (int)HttpStatusCode.InternalServerError;

                    context.Response.StatusCode = statusCode;

                    var problemDetails = new CustomErrorModel
                    {
                        StatusCode = statusCode,
                        Message = errorDetails,
                    };

                    var json = JsonConvert.SerializeObject(problemDetails);

                    logger.LogError(json);

                    //============================================================
                    //Return response
                    //============================================================
                    var matchText = "JSON";

                    bool requiresJsonResponse = context.Request
                                                        .GetTypedHeaders()
                                                        .Accept
                                                        .Any(t => t.Suffix.Value?.ToUpper() == matchText
                                                                  || t.SubTypeWithoutSuffix.Value?.ToUpper() == matchText);

                    if (requiresJsonResponse)
                    {
                        context.Response.ContentType = "application/json; charset=utf-8";

                        if (!respondWithJsonErrorDetails)
                            json = JsonConvert.SerializeObject(new
                            {
                                Title = "Unexpected Error"
                                                                   ,
                                Status = statusCode
                            });
                        await context.Response
                                        .WriteAsync(json, Encoding.UTF8);
                    }
                    else
                    {
                        context.Response.Redirect(errorPagePath);

                        await Task.CompletedTask;
                    }
                });
            });
        }

    }
}
