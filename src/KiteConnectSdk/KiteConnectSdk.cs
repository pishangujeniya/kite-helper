using System;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using KiteConnect;
using Newtonsoft.Json.Linq;

namespace KiteConnectSdk
{

    public class KiteConnectSdk : Kite
    {
        private string UserAgentHeader = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36";
        private string AcceptHeader = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
        private string UserId = string.Empty;
        private string EncToken = string.Empty;
        public bool IsAuthorized => (!string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(EncToken));

        private static readonly HttpClient client = new HttpClient();

        public KiteConnectSdk() : base(string.Empty)
        {

        }

        public void Login(string userId, string password, string appCode)
        {
            this.UserId = userId;

            string loginUrl = "https://kite.zerodha.com/api/login";

            string loginPostData = "";
            Dictionary<string, string> postParameters = new() { { "user_id", userId }, { "password", password } };

            foreach (string key in postParameters.Keys)
            {
                loginPostData += HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(postParameters[key]) + "&";
            }

            HttpWebRequest loginHttpWebRequest = (HttpWebRequest)WebRequest.Create(loginUrl);
            loginHttpWebRequest.Method = "POST";

            byte[] loginData = Encoding.ASCII.GetBytes(loginPostData);

            loginHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            loginHttpWebRequest.ContentLength = loginData.Length;

            AddExtraHeaders(ref loginHttpWebRequest);

            Stream requestStream = loginHttpWebRequest.GetRequestStream();
            requestStream.Write(loginData, 0, loginData.Length);
            requestStream.Close();

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)loginHttpWebRequest.GetResponse();

            Stream responseStream = myHttpWebResponse.GetResponseStream();

            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

            string responseContent = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            responseStream.Close();

            myHttpWebResponse.Close();

            JToken token = JObject.Parse(responseContent);

            string? status = (string?)(token.SelectToken("status"));
            if (!string.IsNullOrWhiteSpace(status) && status.ToLower().Equals("success"))
            {
                string? requestId = (string?)token.SelectToken("data")?.SelectToken("request_id");
                if (!string.IsNullOrWhiteSpace(requestId))
                {
                    this.TwoFactorAuthentication(userId, requestId, appCode);
                }
                else
                {
                    throw new GeneralException("request id not found in the login attempt", myHttpWebResponse.StatusCode);
                }
            }
            else
            {
                throw new TokenException("login attempt failed", myHttpWebResponse.StatusCode);
            }
        }

        private void TwoFactorAuthentication(string userId, string requestId, string appCode)
        {
            string twoFactorAuthenticationUrl = "https://kite.zerodha.com/api/twofa";

            string twofaPostData = "";
            Dictionary<string, string> postParameters = new()
            {
                { "user_id", userId },
                { "request_id", requestId },
                { "twofa_type", "app_code" },
                { "twofa_value", appCode }
            };

            foreach (string key in postParameters.Keys)
            {
                twofaPostData += HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(postParameters[key]) + "&";
            }

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(twoFactorAuthenticationUrl);
            httpWebRequest.Method = "POST";

            byte[] data = Encoding.ASCII.GetBytes(twofaPostData);

            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = data.Length;

            AddExtraHeaders(ref httpWebRequest);

            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            Stream responseStream = myHttpWebResponse.GetResponseStream();

            StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

            string responseContent = myStreamReader.ReadToEnd();

            myStreamReader.Close();
            responseStream.Close();

            myHttpWebResponse.Close();

            JToken token = JObject.Parse(responseContent);

            string? status = (string?)(token.SelectToken("status"));
            if (!string.IsNullOrWhiteSpace(status) && status.ToLower().Equals("success"))
            {
                // Means succeeded
                this.EncToken = myHttpWebResponse.Cookies.ToList().FirstOrDefault(x => x.Name.ToLower().Equals("enctoken"))?.Value ?? string.Empty;
            }
            else
            {
                throw new TokenException("app code verification failed", myHttpWebResponse.StatusCode);
            }
        }

        public override void AddExtraHeaders(ref HttpWebRequest Req)
        {
            base.AddExtraHeaders(ref Req);

            #region Custom Headers
            Req.UserAgent = UserAgentHeader;
            Req.Headers.Add(HttpRequestHeader.Accept, AcceptHeader);
            Req.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-GB,en-US;q=0.9,en;q=0.8");
            Req.Headers.Add("X-Kite-Version", "3.0.6");
            Req.Headers.Add("sec-fetch-site", "same-origin");
            Req.Headers.Add("sec-fetch-mode", "cors");
            Req.Headers.Add("sec-fetch-dest", "empty");
            Req.Headers.Add(HttpRequestHeader.Referer, "https://kite.zerodha.com/dashboard");

            if (!string.IsNullOrWhiteSpace(this.UserId))
            {
                Req.Headers.Add("x-kite-userid", UserId);
            }
            else
            {
                Req.Headers.Remove("x-kite-userid");
            }

            if (!string.IsNullOrWhiteSpace(this.EncToken))
            {
                Req.Headers.Add(HttpRequestHeader.Authorization, $"enctoken {this.EncToken}");
            }
            else
            {
                Req.Headers.Remove(HttpRequestHeader.Authorization);
            }
            #endregion
        }

        public override object Request(string Route, string Method, dynamic Params = null, Dictionary<string, dynamic> QueryParams = null, bool json = false)
        {
            string route = _root + _routes[Route];

            if (Params is null)
                Params = new Dictionary<string, dynamic>();

            if (QueryParams is null)
                QueryParams = new Dictionary<string, dynamic>();

            if (route.Contains("{") && !json)
            {
                var routeParams = (Params as Dictionary<string, dynamic>).ToDictionary(entry => entry.Key, entry => entry.Value);

                foreach (KeyValuePair<string, dynamic> item in routeParams)
                    if (route.Contains("{" + item.Key + "}"))
                    {
                        route = route.Replace("{" + item.Key + "}", (string)item.Value);
                        Params.Remove(item.Key);
                    }
            }

            HttpWebRequest request;

            if (Method == "POST" || Method == "PUT")
            {
                string url = route;
                if (QueryParams.Count > 0)
                {
                    url += "?" + String.Join("&", QueryParams.Select(x => Utils.BuildParam(x.Key, x.Value)));
                }

                string requestBody = "";
                if (json)
                    requestBody = Utils.JsonSerialize(Params);
                else
                    requestBody = String.Join("&", (Params as Dictionary<string, dynamic>).Select(x => Utils.BuildParam(x.Key, x.Value)));

                request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                request.Method = Method;
                request.ContentType = json ? "application/json" : "application/x-www-form-urlencoded";
                request.ContentLength = requestBody.Length;
                if (_enableLogging) Console.WriteLine("DEBUG: " + Method + " " + url + "\n" + requestBody);
                AddExtraHeaders(ref request);

                using (Stream webStream = request.GetRequestStream())
                using (StreamWriter requestWriter = new StreamWriter(webStream))
                    requestWriter.Write(requestBody);
            }
            else
            {
                string url = route;
                Dictionary<string, dynamic> allParams = new Dictionary<string, dynamic>();
                // merge both params
                foreach (KeyValuePair<string, dynamic> item in QueryParams)
                {
                    allParams[item.Key] = item.Value;
                }
                foreach (KeyValuePair<string, dynamic> item in Params)
                {
                    allParams[item.Key] = item.Value;
                }
                // build final url
                if (allParams.Count > 0)
                {
                    url += "?" + String.Join("&", allParams.Select(x => Utils.BuildParam(x.Key, x.Value)));
                }

                request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = true;
                request.Method = Method;
                if (_enableLogging) Console.WriteLine("DEBUG: " + Method + " " + url);
                AddExtraHeaders(ref request);
            }

            WebResponse webResponse;
            try
            {
                webResponse = request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Response is null)
                    throw e;

                webResponse = e.Response;
            }

            using (Stream webStream = webResponse.GetResponseStream())
            {
                using (StreamReader responseReader = new StreamReader(webStream))
                {
                    string response = responseReader.ReadToEnd();
                    if (_enableLogging) Console.WriteLine("DEBUG: " + (int)((HttpWebResponse)webResponse).StatusCode + " " + response + "\n");

                    HttpStatusCode status = ((HttpWebResponse)webResponse).StatusCode;

                    if (webResponse.ContentType == "application/json")
                    {
                        Dictionary<string, dynamic> responseDictionary = Utils.JsonDeserialize(response);

                        if (status != HttpStatusCode.OK)
                        {
                            string errorType = "GeneralException";
                            string message = "";

                            if (responseDictionary.ContainsKey("error_type"))
                                errorType = responseDictionary["error_type"];

                            if (responseDictionary.ContainsKey("message"))
                                message = responseDictionary["message"];

                            switch (errorType)
                            {
                                case "GeneralException": throw new GeneralException(message, status);
                                case "TokenException":
                                    {
                                        _sessionHook?.Invoke();
                                        throw new TokenException(message, status);
                                    }
                                case "PermissionException": throw new PermissionException(message, status);
                                case "OrderException": throw new OrderException(message, status);
                                case "InputException": throw new InputException(message, status);
                                case "DataException": throw new DataException(message, status);
                                case "NetworkException": throw new NetworkException(message, status);
                                default: throw new GeneralException(message, status);
                            }
                        }

                        return responseDictionary;
                    }
                    else if (webResponse.ContentType == "text/csv")
                        return Utils.ParseCSV(response);
                    else
                        throw new DataException("Unexpected content type " + webResponse.ContentType + " " + response);
                }
            }
        }
    }
}