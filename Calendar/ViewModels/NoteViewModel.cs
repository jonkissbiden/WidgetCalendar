using Calendar.Models;
using Calendar.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Calendar.ViewModels
{
    class NoteViewModel : INotifyPropertyChanged
    {
        private Note _note;
        private ApplicationContext _db;
        private RelayCommand _addNoteCommand;
        private RelayCommand _deleteNoteCommand;
        private RelayCommand _editNoteCommand;
        private DateTime selectedDate = DateTime.Now;
        IEnumerable<Note> _notes;
        
        public Note SelectedNote
        {
            get { return _note; }
            set { _note = value; }
        }
        
        public NoteViewModel()
        {
            _db = new ApplicationContext();
            Notes = _db.Notes.OrderBy(x => x.CreationDate);
            _db.SaveChanges();
        }

        public IEnumerable<Note> Notes
        {
            get { return _notes.Where(x => x.CreationDate.Date == selectedDate.Date); }
            set
            {
                _notes = value;
                OnPropertyChanged("Notes");
            }
        }


        public RelayCommand AddNoteCommand
        {
            get
            {
                return _addNoteCommand ??
                    (_addNoteCommand = new RelayCommand((obj) =>
                    {
                        NoteView noteWindow = new NoteView(selectedDate);
                        if (noteWindow.ShowDialog() == true)
                        {
                            Note newNote = noteWindow.Note;
                            _db.Notes.Add(newNote);
                            _db.SaveChanges();
                            Notes = _db.Notes.OrderBy(x => x.CreationDate).ToList();
                        }
                    }));
            }
        }
        public RelayCommand DeleteNoteCommand
        {
            get
            {
                return _deleteNoteCommand ??
                    (_deleteNoteCommand = new RelayCommand((obj) =>
                    {
                        if (SelectedNote != null)
                        {
                            _db.Notes.Remove(SelectedNote);
                            _db.SaveChanges();
                            Notes = _db.Notes.OrderBy(x => x.CreationDate).ToList();
                        }
                    }));
            }
        }

        public RelayCommand EditNoteCommand
        {
            get
            {
                return _editNoteCommand ??
                    (_editNoteCommand = new RelayCommand((obj) =>
                    {
                        NoteEditView noteWindow = new NoteEditView(SelectedNote);
                        if (noteWindow.ShowDialog() == true)
                        {
                            Note newNote = noteWindow.NoteCopy;
                            _db.Notes.Update(newNote);
                            _db.SaveChanges();
                            Notes = _db.Notes.OrderBy(x => x.CreationDate).ToList();
                        }
                    }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
