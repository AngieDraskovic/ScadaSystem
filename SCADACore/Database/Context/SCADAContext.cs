using SCADACore.Database.Model;
using SCADACore.DBModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SCADACore.Database.Context
{
    public class SCADAContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TagValue> TagValues { get; set; }
        public DbSet<AlarmTime> AlarmTimes { get; set; }
    }
}