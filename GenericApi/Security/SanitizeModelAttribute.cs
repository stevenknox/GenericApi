using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GenericApi
{
    public class SanitizeModelAttribute : TypeFilterAttribute
    {
        public SanitizeModelAttribute() : base(typeof(SanitizeModelFilter))
        {
        }

        private class SanitizeModelFilter : IActionFilter
        {
            private readonly IInputSanitizer _sanitizer;
            public SanitizeModelFilter(IInputSanitizer sanitizer)
            {
                _sanitizer = sanitizer;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context.ActionArguments != null && context.ActionArguments.Count > 0 && _sanitizer != null)
                {
                    foreach (var requestParam in context.ActionArguments)
                    {
                       
                        if (requestParam.Value.GetType() == typeof(JObject))
                        {
                            SanitizeJson((JObject)requestParam.Value);
                        }
                        else if (requestParam.Value.GetType() == typeof(string))
                        {
                            SanitizeString(requestParam);
                        }
                        else
                        {
                            SanitizeObject(requestParam.Value);
                        }

                    }

                }

            }

            private void SanitizeString(KeyValuePair<string, object> requestParam)
            {
                var sanitized = _sanitizer.Sanitize(requestParam.Value.ToString());
                if (sanitized != requestParam.Value.ToString())
                {
                    PropertyInfo prop = requestParam.GetType().GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanWrite)
                    {
                        prop.SetValue(requestParam, sanitized, null);
                    }
                }
            }

            public void SanitizeJson(JObject obj)
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());

                foreach (var input in dict)
                {
                    if (input.Value != null && input.Value.GetType() == typeof(string))
                    {
                        var sanitized = _sanitizer.Sanitize(input.Value.ToString());
                        if (sanitized != input.Value.ToString())
                        {
                            ((JValue)(obj).SelectToken(input.Key)).Value = sanitized;
                        }
                    }

                }
               
            }

            public void SanitizeObject(object requestParam)
            {
                var obj = JsonConvert.SerializeObject(requestParam);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj);

                foreach (var input in dict)
                {
                    if (input.Value != null && input.Value.GetType() == typeof(string))
                    {
                        var sanitized = _sanitizer.Sanitize(input.Value.ToString());
                        if (sanitized != input.Value.ToString())
                        {
                            PropertyInfo prop = requestParam.GetType().GetProperty(input.Key, BindingFlags.Public | BindingFlags.Instance);
                            if (null != prop && prop.CanWrite)
                            {
                                prop.SetValue(requestParam, sanitized, null);
                            }
                        }
                    }

                }

            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }
        }
    }
}
