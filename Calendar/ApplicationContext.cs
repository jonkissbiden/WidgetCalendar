using System;
using System.Collections.Generic;
using System.Text;
using Calendar.Models;
using Microsoft.EntityFrameworkCore;

namespace Calendar
{
    public class ApplicationContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=calendar.db");
        public DbSet<Event> Events { get; set; }
        public DbSet<CheckList> CheckLists { get; set; }
        public DbSet<CheckListItem> CheckListsItems { get; set; }

        public DbSet<Note> Notes { get; set; }

        public DbSet<ClientInfo> Feedback { get; set; }
    }
}
