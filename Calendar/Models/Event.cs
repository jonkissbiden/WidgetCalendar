using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calendar.Models
{
    public class Event: INotifyPropertyChanged, IEquatable<Event>
    {
        private string title;
        private string date;
        private string endDate;
        private string updated;
        private string description;
        private string googleId;

        public string GoogleId
        {
            get { return googleId; }
            set
            {
                googleId = value;
                OnPropertyChanged(nameof(GoogleId));
            }
        }
        public int Id { get; set; }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public DateTime Date
        {
            get { return DateTime.Parse(date); }
            set
            {
                date = value.ToString();
                OnPropertyChanged("Date");
            }
        }

        public DateTime EndDate
        {
            get { return DateTime.Parse(endDate); }
            set
            {
                endDate = value.ToString();
                OnPropertyChanged("EndDate");
            }
        }

        public DateTime Updated
        {
            get { return DateTime.Parse(updated); }
            set
            {
                updated = value.ToString();
                OnPropertyChanged("Updated");
            }
        }

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public bool Equals([AllowNull] Event other)
        {
            if (this.Id == other.Id) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
