using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using BuildRevisionCounter.Api.TransportObject.Enums;

namespace BuildRevisionCounter.Api.TransportObject
{
    public class Response
    {
        private const string ErrorStatus = "error";

        private const string SuccessStatus = "success";

        private static string Message = "message";

        public Dictionary<object, object> ValidateErrors { get; set; }

        public Dictionary<object, object> LogicErrors { get; set; }

        public Response()
        {
            ValidateErrors = new Dictionary<object, object>();
            LogicErrors = new Dictionary<object, object>();
        }

        public void AddValidateError(object key, object value)
        {
            ValidateErrors.Add(key, value);
        }

        public void AddLogicError(object key, object value)
        {
            LogicErrors.Add(key, value);
        }

        public Dictionary<object, object> GetValidateErrors()
        {
            return ValidateErrors;
        }

        public Dictionary<object, object> GetLogicErrors()
        {
            return LogicErrors;
        }

        public HttpResponseMessage CreateErrorResponse(ErrorResponceType type, HttpRequestMessage request)
        {
            HttpResponseMessage result = null;

            switch (type)
            {
                case ErrorResponceType.BadAuthToken:
                    result = request.CreateResponse(HttpStatusCode.OK, new { status = ErrorStatus, error_code = "error_auth_token", data = "User by token not found" });
                    break;
                case ErrorResponceType.BadRequestParameters:
                    result = request.CreateResponse(HttpStatusCode.OK, new { status = ErrorStatus, error_code = "error_post_data", data = ValidateErrors });
                    break;
                case ErrorResponceType.LogicError:
                    result = request.CreateResponse(HttpStatusCode.OK, new { status = ErrorStatus, error_code = "error_logic", data = LogicErrors });
                    break;
                case ErrorResponceType.NotFound:
                    result = request.CreateResponse(HttpStatusCode.OK, new { status = ErrorStatus, error_code = "not_found", data = "Data not found" });
                    break;
            }

            return result;
        }

        public HttpResponseMessage CreateSuccessResponce(object data, HttpRequestMessage request)
        {
            return request.CreateResponse(HttpStatusCode.OK, new { status = SuccessStatus, error_code = string.Empty, data = data });
        }
    }
}