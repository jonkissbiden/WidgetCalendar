using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calendar.Models
{
    public class CheckListItem : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _text;
        private bool _checked;
        private int _checkListId;
        private CheckList _checkList; 

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public bool Checked
        {
            get { return _checked; }
            set
            {
                _checked = value;
                OnPropertyChanged("Checked");
            }
        }

        public int CheckListId
        {
            get { return _checkListId; }
            set
            {
                _checkListId = value;
                OnPropertyChanged("CheckListId");
            }
        }

        public CheckList CheckList
        {
            get { return _checkList; }
            set
            {
                _checkList = value;
                OnPropertyChanged("CheckList");
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
