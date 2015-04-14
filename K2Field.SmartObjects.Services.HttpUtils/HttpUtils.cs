﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using Attributes = SourceCode.SmartObjects.Services.ServiceSDK.Attributes;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;
using System.Net;
using System.Globalization;
using Microsoft.Win32;
using System.IO;

namespace K2Field.SmartObjects.Services.HttpUtils
{
    [Attributes.ServiceObject("HttpUtils", "Http Utils", "Http Utils")]
    public class HttpUtils
    {
        public ServiceConfiguration ServiceConfiguration { get; set; }

        [Attributes.Property("Url", SoType.Text, "Url", "Url")]
        public string Url { get; set; }

        [Attributes.Property("Content Type", SoType.Text, "Content Type", "Content Type")]
        public string ContentType { get; set; }


        [Attributes.Property("FileBase64", SoType.Memo, "File Base64", "File Base64")]
        public string FileBase64 { get; set; }

        [Attributes.Property("FileUrl", SoType.Text, "File Url", "File Url")]
        public string FileUrl { get; set; }

        [Attributes.Property("File", SoType.File, "File", "File")]
        public string File { get; set; }

        [Attributes.Property("FileSize", SoType.Number, "File Size", "File Size")]
        public int FileSize { get; set; }

        [Attributes.Property("FileFilename", SoType.Text, "File Name", "File Name")]
        public string FileFilename { get; set; }

        [Attributes.Property("FileContentType", SoType.Text, "File Content Type", "File Content Type")]
        public string FileContentType { get; set; }

        [Attributes.Property("FileExtension", SoType.Text, "File Extension", "File Extension")]
        public string FileExtension { get; set; }

        [Attributes.Property("ResultStatus", SoType.Text, "Result Status", "Result Status")]
        public string ResultStatus { get; set; }

        [Attributes.Property("ResultMessage", SoType.Text, "Result Message", "Result Message")]
        public string ResultMessage { get; set; }

        [Attributes.Method("DownloadFile", SourceCode.SmartObjects.Services.ServiceSDK.Types.MethodType.Read, "Download File", "Download File",
        new string[] { "Url" }, //required property array (no required properties for this sample)
        new string[] { "Url" }, //input property array (no optional input properties for this sample)
        new string[] { "Url", "File", "FileSize", "FileExtension", "FileContentType", "FileBase64", "ResultStatus", "ResultMessage" })]
        public HttpUtils DownloadFile()
        {
            //ServiceConfiguration["BingMapsKey"].ToString();

            DownloadedFile File = DownloadFile(this.Url);

            if (string.IsNullOrWhiteSpace(File.FileName))
            {
                File.FileName = Guid.NewGuid().ToString() + "." + File.FileExtension;
            }

            FileProperty fp = new FileProperty()
            {
                Content = File.Base64,
                FileName = File.FileName
            };

            this.File = fp.Value.ToString();
            this.ContentType = File.ContentType;
            this.FileExtension = File.FileExtension;
            this.FileBase64 = File.Base64;

            this.ResultMessage = "OK";

            return this;
        }

        [Attributes.Method("GetFileDetails", SourceCode.SmartObjects.Services.ServiceSDK.Types.MethodType.Read, "Get File Details", "Get File Details",
        new string[] { "Url" }, //required property array (no required properties for this sample)
        new string[] { "Url" }, //input property array (no optional input properties for this sample)
        new string[] { "Url", "FileSize", "FileExtension", "FileContentType", "ResultStatus", "ResultMessage" })]
        public HttpUtils GetFileDetails()
        {
            //ServiceConfiguration["BingMapsKey"].ToString();

            DownloadedFile File = DownloadFile(this.Url);

            if (string.IsNullOrWhiteSpace(File.FileName))
            {
                File.FileName = Guid.NewGuid().ToString() + "." + File.FileExtension;
            }

            FileProperty fp = new FileProperty()
            {
                Content = File.Base64,
                FileName = File.FileName
            };

            this.File = fp.Value.ToString();
            this.ContentType = File.ContentType;
            this.FileExtension = File.FileExtension;
            this.FileBase64 = File.Base64;

            this.ResultMessage = "OK";

            return this;
        }

        
        private GZipWebClient GetWebClient()
        {
            GZipWebClient Client = new GZipWebClient();

            if (!string.IsNullOrWhiteSpace(this.ContentType))
            {
                Client.Headers.Add("Accept", this.ContentType);
            }

            //                request.Expect = "100-continue";
            Client.Headers.Add("Accept-Encoding", "gzip, deflate");

            if (ServiceConfiguration.ServiceAuthentication.AuthenticationMode == AuthenticationMode.Impersonate || ServiceConfiguration.ServiceAuthentication.AuthenticationMode == AuthenticationMode.ServiceAccount)
            {
                Client.UseDefaultCredentials = true;
            }
            if (ServiceConfiguration.ServiceAuthentication.AuthenticationMode == AuthenticationMode.OAuth)
            {
                string accessToken = ServiceConfiguration.ServiceAuthentication.OAuthToken;
                string headerBearer = String.Format(CultureInfo.InvariantCulture, "Bearer {0}", accessToken);

                Client.Headers.Add("Authorization", headerBearer.ToString());
            }
            if (ServiceConfiguration.ServiceAuthentication.AuthenticationMode == AuthenticationMode.Static)
            {
                char[] sp = { '\\' };
                string[] user = ServiceConfiguration.ServiceAuthentication.UserName.Split(sp);
                if (user.Length > 1)
                {
                    Client.Credentials = new NetworkCredential(user[1], ServiceConfiguration.ServiceAuthentication.Password, user[0]);
                }
                else
                {
                    Client.Credentials = new NetworkCredential(ServiceConfiguration.ServiceAuthentication.UserName, ServiceConfiguration.ServiceAuthentication.Password);
                }

            }            

            return Client;

        }

        private HttpWebRequest GetHttpWebRequest(string Method)
        {
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(this.Url);
            request.Method = Method;
            if(!string.IsNullOrWhiteSpace(this.ContentType))
            {
                request.Accept = this.ContentType;
            }
            
            //                request.Expect = "100-continue";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            if (ServiceConfiguration.ServiceAuthentication.AuthenticationMode == AuthenticationMode.Impersonate || ServiceConfiguration.ServiceAuthentication.AuthenticationMode == AuthenticationMode.ServiceAccount)
            {
                request.UseDefaultCredentials = true;
            }
            if (ServiceConfiguration.ServiceAuthentication.AuthenticationMode == AuthenticationMode.OAuth)
            {
                string accessToken = ServiceConfiguration.ServiceAuthentication.OAuthToken;
                string headerBearer = String.Format(CultureInfo.InvariantCulture, "Bearer {0}", accessToken);

                request.Headers.Add("Authorization", headerBearer.ToString());
            }
            if (ServiceConfiguration.ServiceAuthentication.AuthenticationMode == AuthenticationMode.Static)
            {
                char[] sp = { '\\' };
                string[] user = ServiceConfiguration.ServiceAuthentication.UserName.Split(sp);
                if (user.Length > 1)
                {
                    request.Credentials = new NetworkCredential(user[1], ServiceConfiguration.ServiceAuthentication.Password, user[0]);
                }
                else
                {
                    request.Credentials = new NetworkCredential(ServiceConfiguration.ServiceAuthentication.UserName, ServiceConfiguration.ServiceAuthentication.Password);
                }

            }

            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return request;
        }

        private DownloadedFile HeadDownloadFile()
        {
            //System.Net.WebClient client = new System.Net.WebClient();
            //client.OpenRead("http://someURL.com/Images/MyImage.jpg");
            //Int64 bytes_total= Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
            DownloadedFile File = new DownloadedFile();

            HttpWebRequest request = GetHttpWebRequest("HEAD");
            
            using (System.Net.WebResponse response = request.GetResponse())
            {
                int ContentLength;
                if(int.TryParse(response.Headers.Get("Content-Length"), out ContentLength))
                {
                    File.Size = response.ContentLength;
                }
                File.ContentType = response.ContentType;
                File.FileExtension = GetDefaultExtension(File.ContentType).Replace(".", "");
                File.FileName = Path.GetFileName(new Uri(request.RequestUri.ToString()).AbsolutePath);
            }

            request = null;

            return File;
        }

        private DownloadedFile DownloadFile(string uri)
        {
            DownloadedFile File = new DownloadedFile();

            byte[] data;
            using (GZipWebClient Client = GetWebClient())
            {                
                data = Client.DownloadData(uri);

                File.ContentType = Client.ResponseHeaders[HttpResponseHeader.ContentType];
                int size = 0;
                if (int.TryParse(Client.ResponseHeaders[HttpResponseHeader.ContentLength], out size))
                {
                    File.Size = size;
                }
                File.Base64 = Convert.ToBase64String(data);
                File.Downloaded = DateTime.Now;
                File.FileExtension = GetDefaultExtension(File.ContentType).Replace(".", "");
                File.FileName = Path.GetFileName(new Uri(uri).AbsolutePath);
            }
            return File;

        }

        private string DownloadFileToBase64(string uri)
        {
            byte[] data;
            using (WebClient Client = new WebClient())
            {
                data = Client.DownloadData(uri);
            }
            string base64 = Convert.ToBase64String(data);
            return base64;
        }

        //http://www.cyotek.com/blog/mime-types-and-file-extensions
        public static string GetDefaultExtension(string mimeType)
        {
            string result;
            RegistryKey key;
            object value;

            key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            value = key != null ? key.GetValue("Extension", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            return result;
        }

        public static string GetMimeTypeFromExtension(string extension)
        {
            string result;
            RegistryKey key;
            object value;

            if (!extension.StartsWith("."))
                extension = "." + extension;

            key = Registry.ClassesRoot.OpenSubKey(extension, false);
            value = key != null ? key.GetValue("Content Type", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            return result;
        }
    }

    public class DownloadedFile
    {
        public string Base64 { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public DateTime Downloaded { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string Host { get; set; }
        public WebHeaderCollection ResponseHeaders { get; set; }
    }

    public class GZipWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }
}