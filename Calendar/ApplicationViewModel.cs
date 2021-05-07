using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using Calendar.Models;
using Notifications.Wpf;

namespace Calendar
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        private Dictionary<Event, Timer> notificationTimers = new Dictionary<Event, Timer>();
        private DateTime selectedDate = DateTime.Now;
        private Event selectedEvent;
        string weatherString;
        public string WeatherString
        {
            get { return weatherString; }
            set
            {
                weatherString = value;
                OnPropertyChanged("WeatherString");
            }
        }
        public bool IsGoogleSyncEnabled
        {
            get { return Properties.Settings.Default.isGoogleSyncEnabled; }
            set
            {
                Properties.Settings.Default.isGoogleSyncEnabled = value;
                Properties.Settings.Default.Save();
            }
        }
        public Event SelectedEvent
        {
            get { return selectedEvent; }
            set { selectedEvent = value; }
        }
        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                selectedDate = value;
                Events = events.Where(x => x.Date.Date == selectedDate.Date);
                OnPropertyChanged("selectedDate");
            }
        }
        RelayCommand googleSyncCommand;
        RelayCommand addEventCommand;
        RelayCommand editEventCommand;
        RelayCommand deleteEventCommand;
        IEnumerable<Event> events;
        public IEnumerable<Event> Events
        {
            get { return events.Where(x => x.Date.Date == selectedDate.Date); }
            set
            {
                events = value;
                OnPropertyChanged("Events");
            }
        }
        public ApplicationViewModel()
        {
            var db = new ApplicationContext();
            Events = db.Events.OrderBy(x => x.Date).ToList();
            Thread googleSync = new Thread(GoogleAutoSync);
            googleSync.IsBackground = true;
            googleSync.Start();
            Thread weatherUpdate = new Thread(WeatherUpdate);
            weatherUpdate.IsBackground = true;
            weatherUpdate.Start();
            Thread notificationTimersUpdate = new Thread(NotificationTimersUpdate);
            notificationTimersUpdate.IsBackground = true;
            notificationTimersUpdate.Start();
            
        }
        public RelayCommand GoogleSyncCommand
        {
            get
            {
                return googleSyncCommand ??
                    (googleSyncCommand = new RelayCommand(async (obj) =>
                    {
                        var db = new ApplicationContext();
                        try
                        {
                            Google.Apis.Calendar.v3.Data.Events googleEvents = await GoogleEvents.Get();
                            if (googleEvents != null)
                            {
                                var googleEventsList = googleEvents.Items;
                                foreach (Event item in db.Events)
                                {
                                    if (googleEventsList.Where(x => x.Id == item.GoogleId && x.Updated < item.Updated).Count() != 0)
                                    {
                                        GoogleEvents.AddUpdate(item);
                                    }
                                    else
                                    {
                                        notificationTimers.Remove(item);
                                        db.Events.Remove(item);
                                    }
                                }
                                await db.SaveChangesAsync();
                                Events = db.Events.OrderBy(x => x.Date).ToList();
                                foreach (Google.Apis.Calendar.v3.Data.Event item in googleEventsList)
                                {
                                    DateTime dateTime = (item.Start.DateTime != null) ? (DateTime)item.Start.DateTime : DateTime.Parse(item.Start.Date);
                                    DateTime endDateTime = (item.End.DateTime != null) ? (DateTime)item.End.DateTime : DateTime.Parse(item.End.Date);
                                    int count = db.Events.Where(x => x.GoogleId == item.Id).Count();
                                    if (count == 0 && dateTime != null)
                                    {
                                        Event evnt = new Event { GoogleId = item.Id, Title = item.Summary, Date = dateTime, Description = item.Description, EndDate = endDateTime, Updated = DateTime.Now };
                                        db.Events.Add(evnt);
                                        await db.SaveChangesAsync();
                                        SetUpTimer(evnt);
                                    }
                                    else if(count > 0)
                                    {
                                        try
                                        {
                                            Event match = Events.First(x => x.GoogleId == item.Id);
                                            if (match.Updated < item.Updated)
                                            {
                                                match.Date = dateTime;
                                                match.EndDate = endDateTime;
                                                match.Updated = DateTime.Now;
                                                match.Description = item.Description;
                                                match.Title = item.Summary;
                                                db.Events.Update(match);
                                                await db.SaveChangesAsync();
                                                SetUpTimer(match);
                                            }
                                        }
                                        catch(System.InvalidOperationException e)
                                        {
                                            Console.WriteLine(e.Message);
                                        }
                                    }
                                }
                                Events = db.Events.OrderBy(x => x.Date).ToList();
                            }
                        }
                        catch (Exception e)
                        {
                            if(!IsGoogleSyncEnabled)
                            {
                                App.notificationManager.Show(new NotificationContent
                                {
                                    Title = "MyOrganizer",
                                    Message = Properties.Resources.GoogleTimedOutMassage,
                                    Type = NotificationType.Warning
                                });
                            }
                        }
                    }));
            }
        }
        public RelayCommand AddEventCommand
        {
            get
            {
                return addEventCommand ??
                    (addEventCommand = new RelayCommand(async (obj) =>
                        {
                            var db = new ApplicationContext();
                            EventCreateWindow eventWindow = new EventCreateWindow(selectedDate);
                            if (eventWindow.ShowDialog() == true)
                            {
                                Event newevent = eventWindow.Event;
                                try
                                {
                                    GoogleEvents.AddUpdate(newevent);
                                }
                                catch (Exception e)
                                {
                                    App.notificationManager.Show(new NotificationContent
                                    {
                                        Title = "MyOrganizer",
                                        Message = Properties.Resources.GoogleTimedOutMassage,
                                        Type = NotificationType.Warning
                                    });
                                }
                                db.Events.Add(newevent);
                                await db.SaveChangesAsync();
                                SetUpTimer(newevent);
                                Events = db.Events.OrderBy(x => x.Date).ToList();
                            }
                        }));
            }
        }
        public RelayCommand EditEventCommand
        {
            get
            {
                return editEventCommand ??
                    (editEventCommand = new RelayCommand(async (obj) =>
                    {
                        var db = new ApplicationContext();
                        if (SelectedEvent != null)
                        {
                            EventEditWindow eventWindow = new EventEditWindow(SelectedEvent);
                            if (eventWindow.ShowDialog() == true)
                            {
                                Event newevent = eventWindow.UpdatedEvent;
                                try
                                {
                                    GoogleEvents.AddUpdate(newevent);
                                }
                                catch (Exception e)
                                {
                                    App.notificationManager.Show(new NotificationContent
                                    {
                                        Title = "MyOrganizer",
                                        Message = Properties.Resources.GoogleTimedOutMassage,
                                        Type = NotificationType.Warning
                                    });
                                }
                                db.Events.Update(newevent);
                                await db.SaveChangesAsync();
                                SetUpTimer(newevent);
                                Events = db.Events.OrderBy(x => x.Date).ToList();
                            }
                        }
                        else
                        {
                            App.notificationManager.Show(new NotificationContent
                            {
                                Title = "MyOrganizer",
                                Message = Properties.Resources.EventNotSelectedMassage,
                                Type = NotificationType.Warning
                            });
                        }
                    }));
            }
        }
        public RelayCommand DeleteEventCommand
        {
            get
            {
                return deleteEventCommand ??
                    (deleteEventCommand = new RelayCommand(async (obj) =>
                    {
                        var db = new ApplicationContext();
                        if (SelectedEvent != null)
                        {
                            try
                            {
                                GoogleEvents.Delete(SelectedEvent);
                            }
                            catch (Exception e)
                            {
                                App.notificationManager.Show(new NotificationContent
                                {
                                    Title = "MyOrganizer",
                                    Message = Properties.Resources.GoogleTimedOutMassage,
                                    Type = NotificationType.Warning
                                });
                            }
                            notificationTimers.Remove(SelectedEvent);
                            db.Events.Remove(SelectedEvent);
                            await db.SaveChangesAsync();
                            Events = db.Events.OrderBy(x => x.Date).ToList();
                        }
                        else
                        {
                            App.notificationManager.Show(new NotificationContent
                            {
                                Title = "MyOrganizer",
                                Message = Properties.Resources.EventNotSelectedMassage,
                                Type = NotificationType.Warning
                            });
                        }
                    }));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void SetUpTimer(Event evnt)
        {
            DateTime alertTime = evnt.Date;
            if (alertTime < DateTime.Now || alertTime.Date != DateTime.Now.Date)
            {
                return;//time already passed
            }
            if(notificationTimers.ContainsKey(evnt))
            {
                Timer timer;
                notificationTimers.TryGetValue(evnt, out timer);
                timer.Change(alertTime - DateTime.Now, Timeout.InfiniteTimeSpan);
            }
            else
            {
                notificationTimers.Add(evnt, new System.Threading.Timer(x =>
                {
                    EventNotify(evnt.Title);
                }, null, alertTime - DateTime.Now, Timeout.InfiniteTimeSpan));
            }
        }
        private void EventNotify(string message)
        {
            App.notificationManager.Show(new NotificationContent
            {
                Title = "MyOrganizer",
                Message = message,
                Type = NotificationType.Success
            }, expirationTime: TimeSpan.FromMinutes(1));
        }

        private void GoogleAutoSync()
        {
            while (true)
            {
                if (IsGoogleSyncEnabled && File.Exists("token.json/Google.Apis.Auth.OAuth2.Responses.TokenResponse-user"))
                {
                    var o = new object();
                    GoogleSyncCommand.Execute(o);
                }
                Thread.Sleep((int)10 * 1000);
            }
        }

        private void WeatherUpdate()
        {
            while (true)
            {
                WeatherString = Weather.Get();
                Thread.Sleep((int)10 * 60 * 1000);
            }
        }

        private void NotificationTimersUpdate()
        {
            var db = new ApplicationContext();
            while (true)
            {
                foreach (Event evnt in db.Events.Where(x => x.Date.Date == DateTime.Now.Date))
                {
                    SetUpTimer(evnt);
                }
                Thread.Sleep((int)24 * 60 * 60 * 1000);
            }
        }
    }
}
