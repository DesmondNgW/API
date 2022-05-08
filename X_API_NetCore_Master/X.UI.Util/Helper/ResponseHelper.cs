﻿using Microsoft.AspNetCore.Http;

namespace X.UI.Util.Helper
{
    public class ResponseHelper
    {
        /// <summary>
        /// ResponseWrite
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="ContentType"></param>
        /// <param name="result"></param>
        public static void ResponseWrite(HttpResponse Response, string ContentType, byte[] result)
        {
            Response.ContentType = ContentType;
            Response.StatusCode = 200;
            Response.Body.WriteAsync(result, 0, result.Length);
        }

        /// <summary>
        /// ResponseWrite
        /// </summary>
        /// <param name="Response"></param>
        /// <param name="ContentType"></param>
        /// <param name="result"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void ResponseWrite(HttpResponse Response, string ContentType, byte[] result, string name, string value)
        {
            Response.ContentType = ContentType;
            Response.StatusCode = 200;
            Response.Headers.Add(name, value);
            Response.Body.WriteAsync(result, 0, result.Length);
        }
    }
}
