using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace Presentation.Web.AppAPI.Controllers
{
    public static class ErrorHandler
    {
        public static ObjectResult ErrorResult(string message, ErrorCodes errorCode, HttpStatusCode httpStatusCode)
        {
            var errorObject = new ErrorObject() { ErrorMessage = message, ErrorCode = errorCode };
            var result = new ObjectResult(errorObject);
            result.StatusCode = (int)httpStatusCode;
            return result;
        }

        public enum ErrorCodes
        {
            UnknownError = 600,
            InvalidAuthorization = 610,
            SaveError = 640,
            BadPassword = 650,
            UserNotFound = 660,
            ReportAndUserDoNotMatch = 670,
            IncorrectUserNameOrPassword = 680,
            DuplicateReportFound = 690
        }

        private class ErrorObject
        {
            public String ErrorMessage { get; set; }
            public ErrorCodes ErrorCode { get; set; }
        }
    }

}
