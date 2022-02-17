using MetaWork.WorkTime.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MetaWork.WorkTime.Controllers
{
    public class CalendarEventController : Controller
    {
        [HttpPost]
        public string CreateEvent(Event calendarEvent,string to)
        {
            try
            {
                var tokenFile = AppDomain.CurrentDomain.BaseDirectory + "Files\\tokens.json";
                var token = JObject.Parse(System.IO.File.ReadAllText(tokenFile));
                RestClient restClient = new RestClient();
                RestRequest request = new RestRequest();
                calendarEvent.Start.DateTime = DateTime.ParseExact(calendarEvent.Start.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture).ToString("yyyy-MM-dd'T'HH:mm:sss.fffK");
                calendarEvent.End.DateTime = DateTime.ParseExact(calendarEvent.End.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture).ToString("yyyy-MM-dd'T'HH:mm:sss.fffK");

                var model = JsonConvert.SerializeObject(calendarEvent, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
                PhongChatModel pcm = new PhongChatModel();
                var pcInfo = pcm.GetCalendarInfoBy(int.Parse(to));
                var accesstoken = token["access_token"].ToString();
                var bear = "Bearer " + accesstoken;
                request.AddHeader("Authorization", "Bearer " + token["access_token"].ToString());
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", model, ParameterType.RequestBody);
                restClient.BaseUrl = new System.Uri("https://www.googleapis.com/calendar/v3/calendars/" + pcInfo.GhiChu + "/events?key=AIzaSyAyYQm3w_Rz3IDFkl--ffho6TNS3nXVeUA");
                var respone = restClient.Post(request);
                if (respone.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var contens = JObject.Parse(respone.Content);
                    var result = contens.Children().ToList();
                    var link = result[4].Values().ToList()[0].ToString();
                    return link;
                }
                return "error";
            }
            catch
            {
                return "error";
            }
            
        }
    }
}