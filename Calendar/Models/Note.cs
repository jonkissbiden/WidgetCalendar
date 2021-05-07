using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calendar.Models
{
    public class Note : INotifyPropertyChanged
    {
        private string _noteTitle;
        private string _noteText;
        private DateTime _creationDate;

        public int Id { get; set; }
        public string NoteTitle
        {
            get { return _noteTitle; }
            set
            {
                _noteTitle = value;
                OnPropertyChanged("NoteTitle");
            }
        }

        public string NoteText
        {
            get { return _noteText; }
            set
            {
                _noteText = value;
                OnPropertyChanged("NoteText");
            }
        }

        public DateTime CreationDate
        {
            get { return _creationDate; }
            set
            {
                _creationDate = value;
                OnPropertyChanged("CreationDate");
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
