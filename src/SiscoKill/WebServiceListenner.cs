using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SiscoKill
{
    public class WebServiceListenner
    {
        #region Attributes
            private Thread _thread = null;
        #endregion
        #region Properties
            public string Machine { set; get; }
            public int Port { set; get; }
            public bool IsRunning { set; get; }
            private HttpListener Listener { set; get; }
        #endregion

        #region Constructors
            public WebServiceListenner() 
            {
                this.Machine = "127.0.0.1";
                this.Port = 9999;
                this.Listener = new HttpListener();
            }
        #endregion

        #region Start
            public void Start() 
            {
                if (!HttpListener.IsSupported) 
                {
                    this.OnWrite("Listenner not suported");
                    return;
                }
                this.IsRunning = true;
                this.OnWrite(string.Format("Start Listenning in {0} on Port: {1}", this.Machine, this.Port));
                this._thread = new Thread(new ThreadStart(this.Run));
                this._thread.Start();
            }
        #endregion
        #region Stop
            public void Stop() 
            {

            }
        #endregion

        #region Run
            private void Run()
            {
                this.Listener.Prefixes.Add(string.Format("http://{0}:{1}/", this.Machine, this.Port));
                this.Listener.Start();
                while(this.IsRunning)
                {
                    HttpListenerContext context = this.Listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    string contentType = string.Empty;
                    string responseHtml = GetResponseHtml(request, out contentType);
                    HttpListenerResponse response = context.Response;
                    Stream output = response.OutputStream;
                    if (!string.IsNullOrEmpty(responseHtml)) 
                    {
                        response.Headers["Content-Type"] = contentType;
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseHtml);
                        output.Write(buffer, 0, buffer.Length);
                    }
                    output.Close();
                }
            }

            private string GetResponseHtml(HttpListenerRequest request, out string contentType) 
            {
                contentType = "text/html; charset=utf-8";
                //Request
                this.OnWrite(request.Url.ToString());
                //Response
                string responseHtml = "<HTML><BODY>Unknown</BODY></HTML>";
                if (request.HttpMethod == "GET")
                    responseHtml = "<HTML><BODY>GET</BODY></HTML>";
                else if ((request.HttpMethod == "POST") && (request.Headers.AllKeys.Contains("SOAPAction")))
                    responseHtml = this.GetResponseXML(request.Headers["SOAPAction"], ref contentType);
                //New Line
                this.OnWrite(string.Empty);
                return (responseHtml);
            }

            private string GetResponseXML(string soapAction, ref string contentType) 
            {
                contentType = "text/xml; charset=utf-8";
                string response = string.Empty;
                string key = soapAction.Replace("http://tempuri.org/",string.Empty);
                key = key.Replace("\"", string.Empty);
                return(SoapDictionary.ResourceManager.GetString(key));
            }
        #endregion


        #region Write
            public delegate void WriteDelegate(string message);
            public static WriteDelegate WriteEvent;
            private void OnWrite(string message) 
            {
                if (WebServiceListenner.WriteEvent != null)
                    WebServiceListenner.WriteEvent(message);
            }
        #endregion
    }
}

    