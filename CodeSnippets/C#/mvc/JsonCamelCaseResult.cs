using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartD.Web.Mvc
{
    public class JsonCamelCaseResult : JsonResult
    {
        #region Methods

        /// <summary>
        /// Enables processing of the result of an action method
        /// </summary>
        /// <param name="context">The context within which the result is executed</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.HttpContext == null || context.HttpContext.Response == null)
                return;

            context.HttpContext.Response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
                context.HttpContext.Response.ContentEncoding = ContentEncoding;

            //serialize data with any converters
            if (Data != null)
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                
            context.HttpContext.Response.Write(JsonConvert.SerializeObject(Data, jsonSerializerSettings));
            }
        }

        #endregion
    }
}