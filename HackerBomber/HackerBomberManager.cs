using ScriptInterpreter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace 专治骗子
{
    public class BomberPerformer {
        private int mSuccessCount = 0;
        private int mThreadCount = 16;
        public int ThreadCount {
            get { return mThreadCount; }
            set { mThreadCount = value; }
        }
        public int SuccessCount {
            get {
                return mSuccessCount;
            }
        }
        private IBomber mBomber;
        private bool isStarted = false;
        private List<Thread> BomberThreads = new List<Thread>();
        public BomberPerformer(IBomber bomber) {
            mBomber = bomber;
        }
        public void StartBomber() {
            new Thread(mStartBomber).Start();
        }
        private void mStartBomber() {
            isStarted = true;
            while (BomberThreads.Count < mThreadCount)
            {
                Thread t = new Thread(run);
                Thread.Sleep(1);
                BomberThreads.Add(t);
                t.Start();
            }
        }
        public void StopBomber() {
            isStarted = false;
            BomberThreads.Clear();
        }
        private void run() {
            while (isStarted) {
                if (mBomber.perform())
                {
                    mSuccessCount++;
                }
                else {
                    Thread.Sleep(InstructionUtils.Rnd().Next() % 1500 + 500);
                }
            }
        }
    }
    public static class BomberUtils
    {

        public static bool showEcho = true;

        public static string useragent = "Mozilla/5.0 (Linux; Android 9; PH-1 Build/PPR1.180610.091; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/66.0.3359.126 MQQBrowser/6.2 TBS/044807 Mobile Safari/537.36 V1_AND_SQ_8.0.8_1218_YYB_D QQ/8.0.8.4115 NetType/WIFI WebP/0.3.0 Pixel/1312 StatusBarHeight/151";

        public static event EventHandler<HttpRequestCreatedEventArgs> BeforeRequestSend;
        public static long GetTimestamp()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (long)ts.TotalMilliseconds;
        }
        public static string DictionaryToHttpKeyValue(Dictionary<string, string> dic) {
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            return builder.ToString();
        }
        public static HttpWebRequest MakeHttpRequest(string url, string httpKeyValue, string method) {
            if (method == "GET") {
                return MakeHttpGet(url, httpKeyValue);
            }
            return MakeHttpPost(url, httpKeyValue);
        }
        public static HttpWebRequest MakeHttpGet(string url, string httpKeyValue)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "?" + httpKeyValue);
            req.Timeout = 5000;
            req.Method = "GET";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            req.UserAgent = useragent;
            req.Headers.Add("Accept-Language", "zh,zh-CN;q=0.9;q=0.9");
            //req.Connection = "Keep-Alive";
            req.AllowAutoRedirect = false;
            if (null != BeforeRequestSend)
            {
                BeforeRequestSend.Invoke(req, new HttpRequestCreatedEventArgs(req));
            }
            return req;
        }
        public static HttpWebRequest MakeHttpPost(string url, string httpKeyValue)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 5000;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            req.UserAgent = useragent;
            req.Headers.Add("Accept-Language", "zh,zh-CN;q=0.9;q=0.9");
            //req.Connection = "Keep-Alive";
            byte[] data = Encoding.UTF8.GetBytes(httpKeyValue);
            req.ContentLength = data.Length;
            req.AllowAutoRedirect = false;
            if (null != BeforeRequestSend) {
                BeforeRequestSend.Invoke(req, new HttpRequestCreatedEventArgs(req));
            }
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            return req;
        }
        public static string GetHttpResponse(HttpWebRequest req) {
            string result = "";
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    if (showEcho)
                    {
                        result = reader.ReadToEnd();
                    }
                    else {
                        result = "";
                    }
                }
                try
                {
                    //resp.Close();
                }
                catch { }
                return "[" + resp.StatusCode.ToString() + "] " + result;
            }
            catch (WebException ex) {
                if (null == ex.Response) {
                    throw ex;
                }
                if ((int)((HttpWebResponse)ex.Response).StatusCode < 400)
                {
                    return ex.Message;
                }
                throw ex;
            }
        }
        
        
        
    }
    public class HttpRequestCreatedEventArgs : EventArgs
    {
        private HttpWebRequest request;
        public HttpWebRequest Request { get { return request; } }
        public HttpRequestCreatedEventArgs(HttpWebRequest req) { this.request = req; }
    }
    public interface IBomber {
        event EventHandler<BomberResultEventArgs> OnBomberComplete;
        bool perform();
    }

    public class BomberResultEventArgs : EventArgs {
        bool mBomberResult;
        string mUsesUser,mUsesPassword,mUsesUrl,mReturnValue;
        Exception mException;

        public bool BomberResult
        {
            get
            {
                return mBomberResult;
            }

            set
            {
                mBomberResult = value;
            }
        }

        public string UsesUser
        {
            get
            {
                return mUsesUser;
            }

            set
            {
                mUsesUser = value;
            }
        }

        public string UsesPassword
        {
            get
            {
                return mUsesPassword;
            }

            set
            {
                mUsesPassword = value;
            }
        }

        public string UsesUrl
        {
            get
            {
                return mUsesUrl;
            }

            set
            {
                mUsesUrl = value;
            }
        }

        public Exception Exception
        {
            get
            {
                return mException;
            }

            set
            {
                mException = value;
            }
        }

        public string ReturnValue
        {
            get
            {
                return mReturnValue;
            }

            set
            {
                mReturnValue = value;
            }
        }

        public BomberResultEventArgs(bool bomberResult,string usesUser,string usesPassword,string usesUrl,string returnValue,Exception ex) {
            this.mBomberResult = bomberResult;
            this.mUsesUser = usesUser;
            this.mUsesPassword = usesPassword;
            this.mUsesUrl = usesUrl;
            this.mReturnValue = returnValue;
            this.mException = ex;
        }


    }


    public class ScriptedBomber : IBomber
    {
        StackStateMachine machine = new StackStateMachine();
        public ScriptedBomber(String script)
        {
            machine.Compile(script);
            machine.OnProgramPrint += Machine_OnProgramPrint;
        }
        StringBuilder printContent = new StringBuilder();
        private void Machine_OnProgramPrint(object sender, string e)
        {
            printContent.Append(e);
        }

        public event EventHandler<BomberResultEventArgs> OnBomberComplete;

        public bool perform()
        {
            try
            {
                string url;
                string method;
                string content;

                string printcontent;

                lock (machine)
                {
                    machine.Reset();
                    machine.Run();
                    url = machine.runtimeRegister["提交URL"];
                    method = machine.runtimeRegister["提交方法"];
                    content = machine.runtimeRegister["提交内容"];
                    printcontent = this.printContent.ToString();
                    printContent.Clear();
                }
                HttpWebRequest req = BomberUtils.MakeHttpRequest(url, content, method);
                string result = BomberUtils.GetHttpResponse(req);
                BomberResultEventArgs args = new BomberResultEventArgs(true, "", "", printcontent, result, null);
                if (null != OnBomberComplete)
                {
                    OnBomberComplete.Invoke(this, args);
                }
                return true;
            }
            catch (Exception ex)
            {
                BomberResultEventArgs args = new BomberResultEventArgs(false, "", "", "", ex.Message, ex);
                if (null != OnBomberComplete)
                {
                    OnBomberComplete.Invoke(this, args);
                }
                return false;
            }
        }
    }

}
