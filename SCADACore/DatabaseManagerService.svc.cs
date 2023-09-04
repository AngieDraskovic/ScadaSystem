using SCADACore.Authentication;
using SCADACore.Database.Context;
using SCADACore.DatabaseManagerInterfaces;
using SCADACore.DBModel;
using SCADACore.Tags;
using SCADACore.Tags.InputTags;
using SCADACore.TransferObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADACore
{
    public class DatabaseManagerService : IAuthentication, ITagWork
    {
        public string AddTag(Tag tag, string token)
        {
            return TagProcessing.AddTag(tag, token);
        }

        public bool ChangeOutputTagValue(string tagName, string type, string token, double newValue)
        {
            return TagProcessing.ChangeOutputTagValue(tagName, type, token, newValue);
        }

        public bool DeleteTag(string tagName, string type, string token)
        {
            return TagProcessing.DeleteTag(tagName, type, token);
        }

        public List<Tag> GetTags(string token, string type)
        {
            return TagProcessing.GetTags(token, type);
        }

        public List<TagTransfer> GetOutputTagValues(string token)
        {
            return TagProcessing.GetOutputTagValues(token);
        }

        public bool IsUserAdmin(string token)
        {
            return UserProcessing.IsUserAdmin(token);
        }

        public string Login(string username, string password)
        {
            return UserProcessing.Login(username, password);
        }

        public void Logout(string token)
        {
            UserProcessing.Logout(token);
        }

        public bool RegisterUser(string username, string password, string token)
        {
            return UserProcessing.RegisterUser(username, password, token);
        }

        public bool UsersEmpty()
        {
            return UserProcessing.UsersEmpty();
        }

        public bool ChangeTagScan(string token, string tagName, bool scanOn)
        {
            return TagProcessing.ChangeTagScan(token, tagName, scanOn);
        }

        public bool AddAlarm(string tagName, Alarm alarm, string token)
        {
            return TagProcessing.AddAlarm(tagName, alarm, token);
        }

        public bool DeleteAlarm(string tagName, int alarmId, string token)
        {
            return TagProcessing.DeleteAlarm(tagName, alarmId, token);
        }
    }
}
