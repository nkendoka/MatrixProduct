using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MatrixProduct
{
    public class InvCloudService
    {
        public bool Init(int size)
        {
            string apiUri = ConfigurationManager.AppSettings["ApiGetInitSize"];
            var rspns = GetHttps(apiUri, size.ToString());
            return rspns.Success;
        }

        public List<int> GetRowData(string dataset, int index)
        {
            var retval = new List<int>();
            string apiUri = ConfigurationManager.AppSettings["ApiGetDataSet"];
            var uriParams = $@"{dataset}/row/{index.ToString()}";
            var rspns = GetHttps(apiUri, uriParams);
            if (rspns.Success)
                retval = rspns.Value.Split(',').Select(x=>int.Parse(x)).ToList();
            return retval;
        }

        public bool Validate(string hashString)
        {
            string apiUri = ConfigurationManager.AppSettings["ApiPostValidate"];
            var rspns = PostHttps(apiUri, hashString);
            return rspns.Success;
        }

        private static Response GetHttps(string serviceRootUrl, string index)
        {
            var qUrl = serviceRootUrl + index;

            ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                return true;
            };

            HttpWebRequest httpWRequest = (HttpWebRequest)WebRequest.Create(qUrl);
            //httpWRequest.Credentials = new NetworkCredential(this.txtUsername.Text, this.txtPassword.Text);
            httpWRequest.KeepAlive = false;
            httpWRequest.PreAuthenticate = true;
            httpWRequest.Headers.Set("Pragma", "no-cache");
            httpWRequest.ContentType = "text/xml";
            httpWRequest.Method = "GET";
            httpWRequest.Headers.Add("Translate", "t");
            httpWRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; InfoPath.1)";
            httpWRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            HttpWebResponse httpWResponse = (HttpWebResponse)httpWRequest.GetResponse();
            Stream strm = httpWResponse.GetResponseStream();
            StreamReader sr = new StreamReader(strm);
            var json = sr.ReadToEnd();
            sr.Close();

            return JsonConvert.DeserializeObject<Response>(json);
        }

        private static Response PostHttps(string serviceRootUrl, string body)
        {
            var retval = string.Empty;

            ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                return true;
            };

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(serviceRootUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            httpWebRequest.KeepAlive = false;
            httpWebRequest.PreAuthenticate = true;
            httpWebRequest.Headers.Set("Pragma", "no-cache");
            httpWebRequest.Headers.Add("Translate", "t");
            httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; InfoPath.1)";
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;


            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //string json = "{\"data\":\"body\"}";

                streamWriter.Write(body);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var json = string.Empty;
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                json = streamReader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<Response>(json);
        }
    }
}
