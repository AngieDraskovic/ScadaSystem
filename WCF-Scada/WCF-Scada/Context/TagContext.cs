﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WCF_Scada.models;

namespace WCF_Scada.Context
{
    public class TagContext : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
    }
}