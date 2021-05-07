using Calendar.Models;
using Calendar.Views;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Xaml.Data;
using Calendar;
using Notifications.Wpf;
using System.Windows.Threading;
using System.Threading;

namespace Calendar.ViewModels
{
    class CheckListViewModel : INotifyPropertyChanged
    {
        private ApplicationContext _db;
        private CheckList _selectedCheckList = new CheckList();
        private CheckListItem _checkListItem;
        private RelayCommand _addCheckListsCommand;
        private RelayCommand _deleteCheckListsCommand;
        private RelayCommand _addCheckListItem;
        private DelegateCommand<object> _checkedChangedCommand;
        private DateTime selectedDate = DateTime.Now;
        private IEnumerable<CheckList> _checkLists;
        private IEnumerable<CheckListItem> _checkListItems;
        private string _checkListItemText;
        private ICommand deleteCheckListItem;
        public ICommand DeleteCheckListItem => deleteCheckListItem ??= new RelayCommand(PerformDeleteCheckListItem);
        public DelegateCommand<object> CheckedChangeCommand => (_checkedChangedCommand = new DelegateCommand<object>(CheckChanged));



        public CheckListViewModel()
        {
            _db = new ApplicationContext();
            CheckLists = _db.CheckLists.OrderByDescending(x => x.Id).ToList();
            CheckListItems = _db.CheckListsItems.ToList();
            foreach (var chklst in CheckLists)
            {
                chklst.Items = CheckListItems.Where(x => x.CheckListId == chklst.Id).ToList();
            }
        }

        public IEnumerable<CheckListItem> CheckListItems
        {
            get 
            { 
                return _checkListItems;
            }
            set
            {
                _checkListItems = value;
                OnPropertyChanged(nameof(CheckListItems));
            }
        }

        public string CheckListItemText
        {
            get
            {
                return _checkListItemText;
            }
            set
            {
                _checkListItemText = value;
                OnPropertyChanged("CheckListItemText");
            }
        }
        
        public CheckList SelectedCheckList
        {
            get
            {
                return _selectedCheckList;
            }
            set
            {
                _selectedCheckList = value;
                OnPropertyChanged("SelectedCheckList");
            }

        }

        public CheckListItem CheckListItem
        {
            get
            {
                return _checkListItem;
            }
            set
            {
                _checkListItem = value;
                OnPropertyChanged("CheckListItem");
            }
        }

        public IEnumerable<CheckList> CheckLists
        {
            get { return _checkLists.Where(x => x.CreationDate.Date == selectedDate.Date); }
            set
            {
                _checkLists = value;
                OnPropertyChanged("CheckLists");
            }
        }
        
        private void CheckChanged(object obj)
        {
            _db.SaveChangesAsync();
        }


        public RelayCommand AddCheckListsCommand
        {
            get
            {
                
                return _addCheckListsCommand ??
                (_addCheckListsCommand = new RelayCommand((obj) =>
                {
                    CheckListView checkListWindow = new CheckListView(selectedDate);
                    if (checkListWindow.ShowDialog() == true)
                    {
                        CheckList newCheckList = checkListWindow.CheckList;
                        _db.CheckLists.Add(newCheckList);
                        _db.SaveChangesAsync();
                        CheckLists = _db.CheckLists.OrderByDescending(x => x.Id).ToList();
                    }
                }));
            }
        }
        public RelayCommand DeleteCheckListsCommand
        {
            get
            {
                return _deleteCheckListsCommand ??
                    (_deleteCheckListsCommand = new RelayCommand((obj) =>
                    {
                        CheckListView checkListWindow = new CheckListView(selectedDate);
                        CheckList deleteCheckList = checkListWindow.CheckList;
                        _db.CheckLists.Remove(SelectedCheckList);
                        _db.SaveChangesAsync();
                        CheckLists = _db.CheckLists.OrderByDescending(x => x.Id).ToList();
                    }));
            }
        }

        public RelayCommand AddCheckListItem
        {
            get
            {
                return _addCheckListItem ??
                  (_addCheckListItem = new RelayCommand(obj =>
                  {
                      if (!String.IsNullOrEmpty((string)obj))
                      {
                          CheckListItem item = new CheckListItem() { Checked = false, Text = (string)obj, CheckList = SelectedCheckList, CheckListId = SelectedCheckList.Id };
                          
                          _db.CheckListsItems.Add(item);
                          _db.SaveChangesAsync();
                          SelectedCheckList.Items = _db.CheckListsItems.Where(x => x.CheckListId == SelectedCheckList.Id).ToList();
                          CheckListItemText = null;
                      }
                      else
                      {
                          App.notificationManager.Show(new NotificationContent
                          {
                              Title = "MyOrganizer",
                              Message = "Text field is empty",
                              Type = NotificationType.Warning
                          });
                      }

                  }));
            }
        }

        
        private void PerformDeleteCheckListItem(object commandParameter)
        {
            if ((CheckListItem)commandParameter != null)
            {
                _db.CheckListsItems.Remove((CheckListItem)commandParameter);
                _db.SaveChangesAsync();
                SelectedCheckList.Items = _db.CheckListsItems.Where(x => x.CheckListId == SelectedCheckList.Id).ToList();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}