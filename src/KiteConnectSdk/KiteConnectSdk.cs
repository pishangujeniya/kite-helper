using System.Net;
using KiteConnect;

namespace KiteConnectSdk
{

    public class KiteConnectSdk : Kite
    {
        public KiteConnectSdk(string APIKey, string AccessToken = null, string Root = null, bool Debug = false, int Timeout = 7000, WebProxy Proxy = null, int Pool = 2) : base(APIKey, AccessToken, Root, Debug, Timeout, null, Pool)
        {
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