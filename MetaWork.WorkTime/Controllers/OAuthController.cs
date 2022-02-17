using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MetaWork.WorkTime.Controllers
{
    public class OAuthController : Controller
    {
        // GET: OAuth
        public void Callback(string code, string error, string state)
        {
            if (string.IsNullOrEmpty(error))
            {
                GetTokends(code);
            }
        }

        public ActionResult GetTokends(string code)
        {
            var tokenFile = AppDomain.CurrentDomain.BaseDirectory + "Files\\tokens.json";
            var credentialsFile = AppDomain.CurrentDomain.BaseDirectory + "Files\\credentials.json";
            JObject credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));
            RestClient restClient = new RestClient();
            RestRequest request = new RestRequest();
            request.AddQueryParameter("client_id", credentials["client_id"].ToString());
            request.AddQueryParameter("client_secret", credentials["client_secret"].ToString());
            request.AddQueryParameter("code", code);
            request.AddQueryParameter("grant_type", "authorization_code");
            request.AddQueryParameter("redirect_uri", "http://beta.tecotec.vn/oauth/callback");
            //request.AddQueryParameter("redirect_uri", "https://localhost:44303/oauth/callback");
            restClient.BaseUrl = new System.Uri("https://oauth2.googleapis.com/token");
            var response = restClient.Post(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.File.WriteAllText(tokenFile, response.Content);
                return RedirectToAction("Index", "Metawork");
            }
            return View("Error");           
        }
        public string RefreshToken()
        {
            var tokenFile = AppDomain.CurrentDomain.BaseDirectory + "Files\\tokens.json";
            var credentialsFile = AppDomain.CurrentDomain.BaseDirectory + "Files\\credentials.json";
            JObject credentials = JObject.Parse(System.IO.File.ReadAllText(credentialsFile));
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));
            RestClient restClient = new RestClient();
            RestRequest request = new RestRequest();
            request.AddQueryParameter("client_id", credentials["client_id"].ToString());
            request.AddQueryParameter("client_secret", credentials["client_secret"].ToString());           
            request.AddQueryParameter("grant_type", "refresh_token");
            request.AddQueryParameter("refresh_token", tokens["refresh_token"].ToString());
            restClient.BaseUrl = new System.Uri("https://oauth2.googleapis.com/token");
            var response = restClient.Post(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject newtokens = JObject.Parse(response.Content);
                newtokens["refresh_token"] = tokens["refresh_token"].ToString();
                System.IO.File.WriteAllText(tokenFile, newtokens.ToString());
                return "succes";
            }
            return "error";
            
        }

        public string RevokeToken()
        {
            var tokenFile = AppDomain.CurrentDomain.BaseDirectory + "Files\\tokens.json";           
            JObject tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));
            RestClient restClient = new RestClient();
            RestRequest request = new RestRequest();
            request.AddQueryParameter("token", tokens["access_token"].ToString());
           ;
            restClient.BaseUrl = new System.Uri("https://oauth2.googleapis.com/revoke");
            var response = restClient.Post(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {               
                return "succes";
            }
            return "error";

        }

        public ActionResult Index()
        {
            return View();
        }
    }
}