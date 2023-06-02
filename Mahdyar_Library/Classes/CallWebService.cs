using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mahdyar_Library.Classes
{
    /// <summary>
    /// Ajax کلاس بازکشت خطا به 
    /// </summary>
    public class OutError
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public string errorTitle { get; set; }

        public object Object { get; set; }
        public string StrinErrorTag(bool isPertian = false)
        {
            return string.Format(
                "<div class='alert alert-danger' role='alert' style='text-align:center;margin: 10px 10px 0px;'> <i style='font-size:30pt;color:orange;' class='fa fa-exclamation-triangle'></i><br/><span style='font-family: b tehran;font-size:22pt;'> {0} </span></div>" +
                "<a class='accordion-toggle collapsed' data-toggle='collapse' data-parent='#accordion' style='margin-right: 10px;' href='#collapseError'>" +
                "<i class='bigger-110 ace-icon fa fa-angle-right' data-icon-hide='ace-icon fa fa-angle-down' data-icon-show='ace-icon fa fa-angle-right'></i>  نمایش خطا  </a><div id='collapseError' class='panel-collapse collapse' style='text-align: left;margin-right: 9px; margin-left: 10px;'><div class='panel panel-default panel-body' dir='{3}' style='overflow-y:scroll;height:250px;text-align:{2};'>{1}</div></div>"
                , errorTitle, errorMessage, isPertian ? "right" : "left", isPertian ? "rtl" : "ltr");
        }
        public OutError()
        {
            errorCode = 0;
            errorMessage = "عملیات با موفقیت انجام شد.";
            errorTitle = "پیام سیستم";
        }

        //public OutError(ModelStateDictionary modelState)
        //{
        //    //= "تمام داده ها خواسته شده را وارد کنید!";
        //    errorTitle = "تکمیل اطلاعات";
        //    errorCode = 1;
        //    errorMessage = string.Join("\n", modelState.Values
        //        .SelectMany(v => v.Errors)
        //        .Select(e => e.ErrorMessage));
        //}
        /// <summary>
        /// 2 تکمیل اطلاعات
        /// 3 اطلاعات یافت نشد
        /// 4 مغایرت در دادها
        /// 5 کاربر گرامی شما مجاز به تغییرات نمی باشید
        /// </summary>
        /// <param name="errorCode"> </param>
        public OutError(int errorCode = 2)
        {
            switch (errorCode)
            {
                case 2:
                    this.errorCode = 2;
                    errorMessage = "تمام داده ها خواسته شده را وارد کنید!";
                    errorTitle = "تکمیل اطلاعات";
                    break;
                case 3:
                    this.errorCode = 2;
                    errorMessage = "اطلاعاتی یافت نشد!.";
                    errorTitle = "پیام سیستم";
                    break;
                case 4:
                    this.errorCode = -1;
                    errorMessage = "مغایرت در دادها!.";
                    errorTitle = "پیام سیستم";
                    break;
                case 5:
                    this.errorCode = -1;
                    errorMessage = "کاربر گرامی شما مجاز به تغییرات نمی باشید! ";
                    errorTitle = "پیام سیستم";
                    break;
                default:
                    break;
            }

        }

    }
    public class CallWebService
    {
        public OutError OutError { get; set; }
        private string WebUrl { get; set; }
        public string WebMethod { get; set; }
        public string ContentType { get; set; }
        public string Accept { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public int TimeOut { get; set; }
        public int Credentials { get; set; }

        public CallWebService(string url, string header, string inputJson)
        {
            WebUrl = url;
            WebMethod = "POST";
            ContentType = @"application/json";
            TimeOut = short.MaxValue;
            Credentials = 0;
            Body = inputJson;
            Accept = null;
            Header = header;
        }
        public void CallSyncHttpWebService()
        {
            StreamReader reader = null;
            WebResponse response = null;
            OutError = new OutError();
            Stream dataStream = null;
            try
            {
                OutError.errorCode = 0;
                OutError.errorTitle = "وب سرویس";
                HttpWebRequest request = null;
                request = (HttpWebRequest)WebRequest.Create(WebUrl);

                request.Method = string.IsNullOrEmpty(WebMethod) ? "GET" : WebMethod;

                request.Credentials = Credentials == 1 ? CredentialCache.DefaultCredentials : CredentialCache.DefaultNetworkCredentials;

                if (!string.IsNullOrEmpty(ContentType))
                    request.ContentType = ContentType;

                if (!string.IsNullOrEmpty(Accept))
                    request.Accept = Accept;

                if (!string.IsNullOrEmpty(Header))
                    request.Headers.Add(Header);

                request.Timeout = TimeOut;
                request.ReadWriteTimeout = TimeOut;
                request.ContinueTimeout = TimeOut;


                dataStream = request.GetRequestStream();
                byte[] byteArray = Encoding.UTF8.GetBytes(Body);
                if (byteArray.Length > 0)
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream);
                OutError.errorMessage = reader.ReadToEnd();
                OutError.errorCode = (int)((HttpWebResponse)response).StatusCode == 200 ? 0 : 1;
            }
            catch (ArgumentNullException ex)
            {
                OutError.errorCode = -1;
                OutError.errorMessage = ex.Message;
            }
            catch (WebException wexp)
            {
                OutError.errorCode = -1;
                if (wexp.Response != null)
                {
                    var tempStatusCode = ((HttpWebResponse)wexp.Response).StatusCode;
                    OutError.errorCode = (int)tempStatusCode;
                    OutError.errorMessage = new StreamReader(wexp.Response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    OutError.errorMessage = wexp.Message + "\n InnerException" + wexp?.InnerException;
                }

            }
            catch (Exception ex)
            {
                OutError.errorCode = -1;
                OutError.errorMessage = ex.Message + "\n InnerException:" + ex?.InnerException;
            }
            reader?.Close();
            dataStream?.Close();
            response?.Close();
        }
    }
}
