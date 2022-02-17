using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MetaWork.Data.Provider;

namespace MetaWork.WorkTime.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        //[HttpGet]
        //public ActionResult Create()
        //{
        //    string calendarId = string.Empty;
        //    using (var context = new UsersContext())
        //    {
        //        calendarId = context.UserProfiles.FirstOrDefault(c => c.UserName == User.Identity.Name).Email;
        //    }

        //    var model = new CalendarEvent()
        //    {
        //        CalendarId = calendarId,
        //        Title = "Stand up meeting",
        //        Location = "Starbucks",
        //        StartDate = DateTime.Today,
        //        EndDate = DateTime.Today.AddMinutes(60),
        //        Description = "Let's start this day with a great cup of coffee"
        //    };
        //    var colorList = Enum.GetValues(typeof(GoogleEventColors)).Cast<GoogleEventColors>()
        //                        .Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() });

        //    ViewBag.Colors = new SelectList(colorList, "Value", "Text");
        //    return View(model);
        //}

        ////
        //// POST: /CalendarEvent/Create

        //[HttpPost]
        //public ActionResult Create(CalendarEvent calendarEvent)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var authenticator = GetAuthenticator();
        //        var service = new GoogleCalendarServiceProxy(authenticator);

        //        var a = service.CreateEvent(calendarEvent);
        //        var b = "";
        //    }

        //    return RedirectToAction("Index", "Home");
        //}

        ////
        //// GET: /CalendarEvent/Edit?calendarId={calendarId}&eventId={eventId}

        //public ActionResult Edit(string calendarId, string eventId)
        //{
        //    var authenticator = GetAuthenticator();
        //    var service = new GoogleCalendarServiceProxy(authenticator);
        //    var model = service.GetEvent(calendarId, eventId);
        //    var colorList = Enum.GetValues(typeof(GoogleEventColors)).Cast<GoogleEventColors>()
        //                        .Select(v => new SelectListItem { Text = v.ToString(), Value = ((int)v).ToString() });

        //    ViewBag.Colors = new SelectList(colorList, "Value", "Text");
        //    return View(model);
        //}

        ////
        //// POST: /CalendarEvent/Edit

        //[HttpPost]
        //public ActionResult Edit(CalendarEvent calendarEvent)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var authenticator = GetAuthenticator();
        //        var service = new GoogleCalendarServiceProxy(authenticator);

        //        service.UpdateEvent(calendarEvent);
        //    }

        //    return RedirectToAction("Index", "Home");
        //}

        ////
        //// GET: /CalendarEvent/Delete?calendarId={calendarId}&eventId={eventId}

        //public ActionResult Delete(string calendarId, string eventId)
        //{
        //    var authenticator = GetAuthenticator();
        //    var service = new GoogleCalendarServiceProxy(authenticator);
        //    service.DeleteEvent(calendarId, eventId);

        //    return RedirectToAction("Index", "Home");
        //}

        //private GoogleAuthenticator GetAuthenticator()
        //{
        //    var authenticator = (GoogleAuthenticator)Session["authenticator"];

        //    if (authenticator == null || !authenticator.IsValid)
        //    {
        //        // Get a new Authenticator using the Refresh Token
        //        var refreshToken = new UsersContext().GoogleRefreshTokens.FirstOrDefault(c => c.UserName == User.Identity.Name).RefreshToken;
        //        authenticator = GoogleAuthorizationHelper.RefreshAuthenticator(refreshToken);
        //        Session["authenticator"] = authenticator;
        //    }

        //    return authenticator;
        //}
    }
}