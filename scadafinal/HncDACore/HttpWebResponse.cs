using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization;
using System.Collections;   //zb:20150427
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.Linq.SqlClient;
using System.Text;
using HncDataInterfaces;
using hncData;
using System.Configuration;
using System.Threading;
using System.Net;
using System.Xml;
using System.Web;
using System.Web.Services;
using System.IO;
using System.Timers;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HNCBase.Models;
using System.Data.SQLite;   //zb:20150424
using Newtonsoft.Json;  //zb:20150428
using System.Runtime.InteropServices; //zb:20150610
//----------------------------------------
using System.Transactions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Services.Protocols;
using System.ServiceModel.Security;
using System.IO.Compression;
using System.Text.RegularExpressions;
namespace HncDACore
{

    //--------------------------------------------------------------http请求start
    /*  
     * 作者：周公(zhoufoxcn)  
     * 日期：2011-05-08  
     * 原文出处：http://blog.csdn.net/zhoufoxcn 或http://zhoufoxcn.blog.51cto.com  
     * 版权说明：本文可以在保留原文出处的情况下使用于非商业用途，周公对此不作任何担保或承诺。  
     * */

    /// <summary>
    /// 有关HTTP请求的辅助类
    /// </summary>
    public class HttpWebResponseUtility
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        //private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="timeout">请求的超时时间</param>
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param>
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param>
        /// <returns></returns>
        public static HttpWebResponse CreateGetHttpResponse(string url, int? timeout, string userAgent, CookieCollection cookies)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentNullException("url");
                }
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.UserAgent = DefaultUserAgent;
                if (!string.IsNullOrEmpty(userAgent))
                {
                    request.UserAgent = userAgent;
                }
                if (timeout.HasValue)
                {
                    request.Timeout = timeout.Value;
                }
                if (cookies != null)
                {
                    request.CookieContainer = new CookieContainer();
                    request.CookieContainer.Add(cookies);
                }
                return request.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateGetHttpResponse" + ex.Message);
            }
            return null;
        }
    }
}



