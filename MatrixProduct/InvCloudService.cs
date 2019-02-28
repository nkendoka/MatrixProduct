﻿using Newtonsoft.Json;
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
            var json = GetHttps(apiUri, size.ToString());
            var response = JsonConvert.DeserializeObject<InitResponse>(json);
            return response.Success;
        }

        public double[] GetRowData(string dataset, int index)
        {
            double[] retval = new List<double>().ToArray();
            string apiUri = ConfigurationManager.AppSettings["ApiGetDataSet"];
            var uriParams = $@"{dataset}/row/{index.ToString()}";
            var json = GetHttps(apiUri, uriParams);
            var response =  JsonConvert.DeserializeObject<DataSetResponse>(json);

            if (response.Success)
                retval = response.Value;

            return retval;
        }

        public bool Validate(string hashString)
        {
            string apiUri = ConfigurationManager.AppSettings["ApiPostValidate"];
            var json = PostHttps(apiUri, hashString);
            var response = JsonConvert.DeserializeObject<ValidateResponse>(json);
            return response.Success;
        }

        private string GetHttps(string serviceRootUrl, string index)
        {
            var qUrl = serviceRootUrl + index;
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                {return true;};

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

            return json;
        }

        private string PostHttps(string serviceRootUrl, string body)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                {return true;};

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
            return json;
        }
    }
}