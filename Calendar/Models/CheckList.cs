using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calendar.Models
{
    public class CheckList : INotifyPropertyChanged
    {

        private string _checkListText;
        private DateTime _creationDate;
        private DateTime _updatedDate;
        private IEnumerable<CheckListItem> _items;

        public int Id { get; set; }

        public string CheckListText
        {
            get { return _checkListText; }
            set
            {
                _checkListText = value;
                OnPropertyChanged("CheckListText");
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

        public DateTime UpdatedDate
        {
            get { return _updatedDate; }
            set
            {
                _updatedDate = value;
                OnPropertyChanged("Updated");
            }
        }

        public IEnumerable<CheckListItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }
        public CheckList()
        {
            _items = new List<CheckListItem>();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
