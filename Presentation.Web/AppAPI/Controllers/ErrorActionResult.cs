using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Presentation.Web.AppAPI.Controllers
{

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

    public class ErrorObject
    {
        public String ErrorMessage { get; set; }
        public ErrorCodes ErrorCode { get; set; }
    }

    public class CustomErrorActionResult : IActionResult
    {
        public string Message { get; private set; }
        public HttpRequest Request { get; private set; }
        public HttpStatusCode Status;
        public ErrorCodes ErrorCode;

        public CustomErrorActionResult(HttpRequest request, string message, ErrorCodes errorCode, HttpStatusCode status)
        {
            this.Request = request;
            this.Message = message;
            this.ErrorCode = errorCode;
            this.Status = status;
        }

        public HttpResponseMessage ExecuteResult()
        {
            ErrorObject errorObject = new ErrorObject() {ErrorCode = this.ErrorCode, ErrorMessage = this.Message};
            // todo how to return error ojbect?
            return new HttpResponseMessage(this.Status);
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            return Task.FromResult(ExecuteResult());
        }
    }
}