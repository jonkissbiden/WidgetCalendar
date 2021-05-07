using Calendar.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Calendar
{
    class GoogleEvents
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "MyOrganizer";
        private static async Task<CalendarService> GetService()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromMinutes(1));
                CancellationToken ct = cts.Token;
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    ct,
                    new FileDataStore(credPath, true));
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }

        public static async Task<Events> Get()
        {
            var service = await GetService();
            if (service == null) return null;
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 1000;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            try
            {
                Events events = request.Execute();
                return events;
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static async void AddUpdate(Models.Event newEvent)
        {
            try
            {
                CalendarService service = await GetService();
                if (service == null) return;
                var ev = new Google.Apis.Calendar.v3.Data.Event();
                EventDateTime start = new EventDateTime();
                start.DateTime = newEvent.Date;
                EventDateTime end = new EventDateTime();
                end.DateTime = newEvent.EndDate;
                ev.Start = start;
                ev.End = end;
                ev.Summary = newEvent.Title;
                ev.Description = newEvent.Description;
                ev.Id = newEvent.GoogleId;
                ev.Updated = DateTime.Now;
                if (newEvent.GoogleId == null)
                {
                    ev.Source = new Google.Apis.Calendar.v3.Data.Event.SourceData() { Title = "MyOrganizer", Url = "https://myorganaizer.com/" };
                }

                var match = service.Events.Get("primary", newEvent.GoogleId);
                if (ev.Id != null && match != null)
                {
                    try
                    {
                        Google.Apis.Calendar.v3.Data.Event result = service.Events.Patch(ev, "primary", ev.Id).Execute();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    try
                    {
                        Google.Apis.Calendar.v3.Data.Event result = service.Events.Insert(ev, "primary").Execute();
                        newEvent.GoogleId = result.Id;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public static async void Delete(Models.Event calendarEvent)
        {
            CalendarService service = await GetService();
            if (service == null) return;
            var ev = new Google.Apis.Calendar.v3.Data.Event();
            EventDateTime start = new EventDateTime();
            start.DateTime = calendarEvent.Date;
            EventDateTime end = new EventDateTime();
            end.DateTime = calendarEvent.EndDate;
            ev.Start = start;
            ev.End = end;
            ev.Summary = calendarEvent.Title;
            ev.Description = calendarEvent.Description;
            ev.Id = calendarEvent.GoogleId;

            if (service != null && ev.Id != null && service.Events.Get("primary", calendarEvent.GoogleId) != null)
            {
                try
                {
                    service.Events.Delete("primary", ev.Id).Execute();
                }
                catch (System.Net.Http.HttpRequestException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Google.GoogleApiException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}