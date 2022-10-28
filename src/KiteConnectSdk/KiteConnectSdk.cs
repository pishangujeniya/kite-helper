
using System.Data;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;
using CsvHelper;
using KiteConnect;
using Newtonsoft.Json.Linq;
using DataException = KiteConnect.DataException;

namespace KiteConnectSdk
{
    /// <summary>
    /// This class helps to convert the API calls from APIKey to Cookie based
    /// </summary>
    public class KiteConnectSdk : Kite
    {
        /**
         *
         * 1. Original Kite class of the official library uses _root variable for API domain.
         * 2. We needed to change _root to browser based url, which is done in constructor.
         * 3. We also need to concatenate a patch to the _root url, which is done in constructor.
         * 4. We need to replace the headers with browser based headers which is overriden in AddExtraHeaders method.
         * 5. We need to first login and then two factor authentication.
         * 6. On success of both we need to read the cookie value of enctoken name and then after every request should have that token in the Authorization header.
         * 7. The _request method is overriden with one logic where the every response of it should update the value of _encToken with the last received response value.
         *
         **/

        /// <summary>
        /// Stores the User ID.
        /// </summary>
        private string _userId = string.Empty;

        /// <summary>
        /// Keeps the last received enctoken cookie value.
        /// </summary>
        private string _encToken = string.Empty;

        /// <summary>
        /// A simpler way to detect the session is active or not.
        /// </summary>
        public bool IsAuthorized => (!string.IsNullOrWhiteSpace(_userId) && !string.IsNullOrWhiteSpace(_encToken));

        /// <summary>
        /// Default constructor, which sets the browser based _root url with a concatenation patch
        /// </summary>
        public KiteConnectSdk() : base(string.Empty)
        {
            this._root = Constants.KiteBrowserApiRoot;
        }

        /// <summary>
        /// Fully functional password based login flow including two factor authentication.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="appCode"></param>
        /// <exception cref="GeneralException"></exception>
        /// <exception cref="TokenException"></exception>
        public bool Login(string userId, string password, string appCode)
        {
            this._userId = userId;

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

            Stream loginRequestStream = loginHttpWebRequest.GetRequestStream();
            loginRequestStream.Write(loginData, 0, loginData.Length);
            loginRequestStream.Close();

            HttpWebResponse loginHttpWebResponse = (HttpWebResponse)loginHttpWebRequest.GetResponse();

            Stream loginResponseStream = loginHttpWebResponse.GetResponseStream();

            StreamReader loginStreamReader = new StreamReader(loginResponseStream, Encoding.Default);

            string loginResponseContent = loginStreamReader.ReadToEnd();

            loginStreamReader.Close();
            loginResponseStream.Close();

            loginHttpWebResponse.Close();

            JToken loginResponseToken = JObject.Parse(loginResponseContent);

            string? loginResponseStatus = (string?)(loginResponseToken.SelectToken("status"));
            if (!string.IsNullOrWhiteSpace(loginResponseStatus) && loginResponseStatus.ToLower().Equals("success"))
            {
                string? loginResponseRequestId = (string?)loginResponseToken.SelectToken("data")?.SelectToken("request_id");
                if (!string.IsNullOrWhiteSpace(loginResponseRequestId))
                {
                    return TwoFactorAuthentication(loginResponseRequestId);
                }
                else
                {
                    throw new GeneralException("request id not found in the login attempt", loginHttpWebResponse.StatusCode);
                }
            }
            else
            {
                throw new TokenException("login attempt failed", loginHttpWebResponse.StatusCode);
            }

            bool TwoFactorAuthentication(string requestId)
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

                HttpWebRequest twoFahttpWebRequest = (HttpWebRequest)WebRequest.Create(twoFactorAuthenticationUrl);
                twoFahttpWebRequest.Method = "POST";

                byte[] twoFaData = Encoding.ASCII.GetBytes(twofaPostData);

                twoFahttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                twoFahttpWebRequest.ContentLength = twoFaData.Length;

                AddExtraHeaders(ref twoFahttpWebRequest);

                Stream twoFaRequestStream = twoFahttpWebRequest.GetRequestStream();
                twoFaRequestStream.Write(twoFaData, 0, twoFaData.Length);
                twoFaRequestStream.Close();

                HttpWebResponse twoFaHttpWebResponse = (HttpWebResponse)twoFahttpWebRequest.GetResponse();
                string header = twoFaHttpWebResponse.GetResponseHeader("Set-Cookie");
                Stream twoFaResponseStream = twoFaHttpWebResponse.GetResponseStream();

                StreamReader twoFaStreamReader = new StreamReader(twoFaResponseStream, Encoding.Default);

                string responseContent = twoFaStreamReader.ReadToEnd();

                SetEncTokenIfReceived(twoFaHttpWebResponse);

                twoFaStreamReader.Close();
                twoFaResponseStream.Close();

                twoFaHttpWebResponse.Close();

                JToken twoFaToken = JObject.Parse(responseContent);

                string? twoFaResponseStatus = (string?)(twoFaToken.SelectToken("status"));
                if (!string.IsNullOrWhiteSpace(twoFaResponseStatus) && twoFaResponseStatus.ToLower().Equals("success"))
                {
                    // Means succeeded
                    return true;
                }
                else
                {
                    throw new TokenException("app code verification failed", twoFaHttpWebResponse.StatusCode);
                }
            }
        }

        /// <summary>
        /// A method which will extract the enctoken from the cookies and set it to the _encToken variable
        /// </summary>
        /// <param name="webResponse"></param>
        private void SetEncTokenIfReceived(WebResponse webResponse)
        {
            string encTokenCookie = ((HttpWebResponse)webResponse).Cookies.ToList().FirstOrDefault(x => x.Name.ToLower().Equals("enctoken"))?.Value ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(encTokenCookie)) this._encToken = encTokenCookie;
        }

        /// <summary>
        /// Adds the browser based extra custom headers needed for the requests
        /// </summary>
        /// <param name="Req"></param>
        public override void AddExtraHeaders(ref HttpWebRequest Req)
        {

            base.AddExtraHeaders(ref Req);

            Req.CookieContainer = new CookieContainer();

            #region Custom Headers

            Req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36";
            Req.Headers.Remove(HttpRequestHeader.Accept); Req.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            Req.Headers.Remove(HttpRequestHeader.AcceptLanguage); Req.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-GB,en-US;q=0.9,en;q=0.8");
            Req.Headers.Remove("X-Kite-Version"); Req.Headers.Add("X-Kite-Version", "3.0.6");
            Req.Headers.Remove("sec-fetch-site"); Req.Headers.Add("sec-fetch-site", "same-origin");
            Req.Headers.Remove("sec-fetch-mode"); Req.Headers.Add("sec-fetch-mode", "cors");
            Req.Headers.Remove("sec-fetch-dest"); Req.Headers.Add("sec-fetch-dest", "empty");
            Req.Headers.Remove(HttpRequestHeader.Referer); Req.Headers.Add(HttpRequestHeader.Referer, "https://kite.zerodha.com/dashboard");

            if (!string.IsNullOrWhiteSpace(this._userId))
            {
                Req.Headers.Remove("x-kite-userid"); Req.Headers.Add("x-kite-userid", _userId);
            }
            else
            {
                Req.Headers.Remove("x-kite-userid");
            }

            if (!string.IsNullOrWhiteSpace(this._encToken))
            {
                Req.Headers.Remove(HttpRequestHeader.Authorization); Req.Headers.Add(HttpRequestHeader.Authorization, $"enctoken {this._encToken}");
            }
            else
            {
                Req.Headers.Remove(HttpRequestHeader.Authorization);
            }

            #endregion
        }

        /// <summary>
        /// Overriden method of request where only the value of last received enctoken cookie is get and stored in the variable.
        /// </summary>
        /// <param name="Route"></param>
        /// <param name="Method"></param>
        /// <param name="Params"></param>
        /// <param name="QueryParams"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        /// <exception cref="GeneralException"></exception>
        /// <exception cref="TokenException"></exception>
        /// <exception cref="PermissionException"></exception>
        /// <exception cref="OrderException"></exception>
        /// <exception cref="InputException"></exception>
        /// <exception cref="KiteConnect.DataException"></exception>
        /// <exception cref="NetworkException"></exception>
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
                SetEncTokenIfReceived(webResponse);
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

        public async Task<System.Data.DataTable> GetInstrumentsCsv()
        {
            using HttpClient httpClient = new HttpClient();
            Stream stream = await httpClient.GetStreamAsync(Constants.KiteInstrumentsCsvUrl);
            using StreamReader reader = new StreamReader(stream);
            using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            // Do any configuration to `CsvReader` before creating CsvDataReader.
            using CsvDataReader dr = new CsvDataReader(csv);
            DataTable dt = new System.Data.DataTable();
            dt.Load(dr);
            return dt;
        }
    }
}