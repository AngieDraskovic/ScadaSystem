using SCADACore.Authentication;
using SCADACore.Database.Context;
using SCADACore.Database.Model;
using SCADACore.DBModel;
using SCADACore.Interfaces.AlarmDisplayInterface;
using SCADACore.Tags;
using SCADACore.Tags.InputTags;
using SCADACore.Tags.OutputTags;
using SCADACore.TransferObjects;
using SCADACore.TrendingConsoleAppInterface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Xml.Serialization;

namespace SCADACore
{
    public static class TagProcessing
    {
        private static ConcurrentDictionary<string, Tag> _tags = new ConcurrentDictionary<string, Tag>();
        private static ConcurrentDictionary<string, double> _outputTagValues = new ConcurrentDictionary<string, double>();
        private static ConcurrentDictionary<string, Thread> _inputTagThreads = new ConcurrentDictionary<string, Thread>();
        private static ConcurrentDictionary<string, bool> _stopInputTagThreads = new ConcurrentDictionary<string, bool>();

        private delegate void TagValueChangedDelegate(double value, Tag tag);
        private static event TagValueChangedDelegate onTagValueChanged = null;
        private static readonly object locker = new object();
        
        private delegate void AlarmHappenedDelegate(AlarmTime alarmTime);
        private static event AlarmHappenedDelegate onAlarmHappened = null;

        private static ConcurrentDictionary<string, string> _rtuPaths = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, string> _rtuAddresses = new ConcurrentDictionary<string, string>();
        private static ConcurrentDictionary<string, double> _rtuValues = new ConcurrentDictionary<string, double>();

        private static readonly string CONFIGURATION_FILE = "/configFile.xml";
        private static readonly string ALARM_FILE = "/alarmsLog.txt";

        public static void PeriodicSaveConfiguration()
        {
            while (true)
            {
                int seconds = 10;
                while (seconds > 0)
                {
                    seconds--;
                    Thread.Sleep(1000);
                }

                SaveConfiguration();
            }
        }

        public static void WriteAlarmTimeToFile(AlarmTime alarmTime)
        {
            lock (locker)
            {
                using (var file = File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory + ALARM_FILE))
                {
                    file.WriteLine($"Vreme : {alarmTime.TimeStamp} , Vrednost : {alarmTime.Value} , Limit : {alarmTime.Limit} , " +
                        $"Tip alarma : {alarmTime.AlarmType} , Prioritet : {alarmTime.Priority} , Ime taga : {alarmTime.TagName}");
                }
            }
        }

        public static void WriteAlarmToDatabase(Alarm alarm, double value, double limit)
        {
            AlarmTime alarmTime = new AlarmTime(alarm, value, limit);

            using (var db = new SCADAContext())
            {
                db.AlarmTimes.Add(alarmTime);
                db.SaveChanges();
            }

            WriteAlarmTimeToFile(alarmTime);
            
            lock (locker)
            {
                onAlarmHappened?.Invoke(alarmTime);
            }
        }

        public static void SaveTagValue(string tagName, double value, string type)
        {
            using (var db = new SCADAContext())
            {
                db.TagValues.Add(new TagValue(tagName, value, type));
                db.SaveChanges();
            }
        }

        public static void LoadConfiguration()
        {
            List<Tag> tags;
            XmlSerializer serialier = new XmlSerializer(typeof(List<Tag>));

            try
            {
                using (var stream = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_FILE, FileMode.Open))
                {
                    tags = (List<Tag>)serialier.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                tags = new List<Tag>();
            }

            foreach (Tag tag in tags)
            {
                AddTag(tag, "", true);
            }
        }

        public static void SaveConfiguration()
        {
            List<Tag> tags = new List<Tag>(_tags.Values.Where(tag => tag is InputTag));
            XmlSerializer serialier = new XmlSerializer(typeof(List<Tag>));

            using (var writer = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + CONFIGURATION_FILE))
            {
                serialier.Serialize(writer, tags);
            }
        }

        public static bool DeleteAlarm(string tagName, int alarmId, string token)
        {
            if (!UserProcessing.IsUserAuthenticated(token, Role.USER))
            {
                return false;
            }

            bool ok = _tags.TryGetValue(tagName, out Tag tag);
            if (!ok)
            {
                return false;
            }

            if (!(tag is AnalogInputTag))
            {
                return false;
            }

            lock (locker)
            {
                AnalogInputTag inTag = tag as AnalogInputTag;
                Alarm foundAlarm = inTag.Alarms.Find(alarm => alarm.AlarmId == alarmId);
                return inTag.Alarms.Remove(foundAlarm);
            }
        }

        public static bool AddAlarm(string tagName, Alarm alarm, string token)
        {
            if (!UserProcessing.IsUserAuthenticated(token, Role.USER) || alarm.TagName != tagName)
            {
                return false;
            }

            bool ok = _tags.TryGetValue(tagName, out Tag tag);
            if (!ok)
            {
                return false;
            }

            if (!(tag is AnalogInputTag))
            {
                return false;
            }

            lock (locker)
            {
                AnalogInputTag inTag = tag as AnalogInputTag;
                alarm.AlarmId = inTag.Alarms.Count;
                inTag.Alarms.Add(alarm);
            }

            return true;
        }
        public static bool InitRealTimeUnit(string rtuId, string path, string rtuAddress)
        {
            if (_rtuPaths.ContainsKey(rtuId))
            {
                return false;
            }

            if (_rtuValues.ContainsKey(rtuAddress))
            {
                return false;
            }

            return _rtuPaths.TryAdd(rtuId, path) && _rtuAddresses.TryAdd(rtuId, rtuAddress);
        }

        public static void WriteValue(string rtuId, string value, string rtuAddress, byte[] signature)
        {
            if (_rtuPaths.ContainsKey(rtuId))
            {
                _rtuPaths.TryGetValue(rtuId, out string path);
                Validation.ImportPublicKey(path);
                if (Validation.VerifySignedMessage(value, signature))
                {
                    _rtuAddresses.TryGetValue(rtuId, out string address);
                    if (address != rtuAddress)
                    {
                        return;
                    }

                    try
                    {
                        double newValue = double.Parse(value);
                        _rtuValues.AddOrUpdate(rtuAddress, newValue, (key, oldValue) => newValue);
                    }
                    catch (Exception) { }
                }
            }   
        }

        public static void TrendingConsoleAppInit()
        {
            lock (locker)
            {
                onTagValueChanged += OperationContext.Current.GetCallbackChannel<ITrendingConsoleAppCallback>().TagValueChanged;
            }
        }

        public static void AlarmDisplayInit()
        {
            lock (locker)
            {
                onAlarmHappened += OperationContext.Current.GetCallbackChannel<IAlarmDisplayCallback>().AlarmHappened;
            }
        }

        public static bool ChangeTagScan(string token, string tagName, bool scanOn)
        {
            if (!UserProcessing.IsUserAuthenticated(token, Role.USER))
            {
                return false;
            }

            bool ok = _tags.TryGetValue(tagName, out Tag tag);
            if (!ok)
            {
                return false;
            }

            lock (locker)
            {
                InputTag inTag = tag as InputTag;
                inTag.ScanOn = scanOn;
            }

            return true;

        }

        public static string AddTag(Tag tag, string token, bool loading = false)
        {
            if (!loading && !UserProcessing.IsUserAuthenticated(token, Role.USER))
            {
                return "nemate pristup ovoj funkciji!";
            }

            if (_tags.ContainsKey(tag.TagName))
            {
                return "tag sa unetim imenom već postoji!";
            }

            try
            {
                if (tag is OutputTag)
                {
                    UpdateOutputValues(tag as OutputTag);
                }

                _tags.TryAdd(tag.TagName, tag);

                if (tag is InputTag)
                {
                    _stopInputTagThreads.TryRemove(tag.TagName, out bool vl);
                    StartThreadForInputTag(tag.TagName);
                }


                return "";
            }

            catch (Exception)
            {
                return "na IO adresi se nalazi output drugog tipa!";
            }
        }

        private static void StartThreadForInputTag(string tagName)
        {
            bool ok = _tags.TryGetValue(tagName, out Tag tag);

            if (ok)
            {
                Thread t = new Thread(CalculateInputTagValue);

                _inputTagThreads.TryAdd(tagName, t);

                t.Start(tag);
            }
        }

        private static void CalculateInputTagValue(object obj) 
        {
            InputTag getTag = obj as InputTag;
            string tagName = getTag.TagName;
            double value = 0;
            double newValue = 0;
            bool first = true;
            int seconds = getTag.ScanTime;

            while (true)
            {
                int wait = seconds;

                while (wait > 0)
                {
                    Thread.Sleep(1000);
                    wait--;
                }

                if (_stopInputTagThreads.ContainsKey(tagName))
                {
                    _stopInputTagThreads.TryRemove(tagName, out bool vl);
                    break;
                }

                bool ok = _tags.TryGetValue(tagName, out Tag tag);
                if (!ok)
                {
                    _stopInputTagThreads.TryRemove(tagName, out bool vl);
                    break;
                }

                InputTag inTag = tag as InputTag;
                bool contains;


                if (inTag is DigitalInputTag)
                {
                    contains = GetValueForDigitalInputTag(inTag as DigitalInputTag, ref newValue);
                }
                else
                {
                    contains = GetValueForAnalogInputTag(inTag as AnalogInputTag, ref newValue);
                }

                if (!contains)
                {
                    continue;
                }

                // TODO
                if (first)
                {
                    first = false;
                    value = newValue - 1;
                }

                if (newValue != value) 
                {
                    string type;

                    if (inTag is DigitalInputTag)
                    {
                        type = typeof(DigitalInputTag).Name;
                    }
                    else
                    {
                        type = typeof(AnalogInputTag).Name;
                    }

                    value = newValue;
                    SaveTagValue(tagName, value, type);
                    if (inTag.ScanOn)
                    {
                        lock (locker)
                        {
                            onTagValueChanged?.Invoke(value, inTag);
                        }
                    }
                }
            }
        }

        private static bool GetValueForDigitalInputTag(DigitalInputTag inTag, ref double newValue)
        {
            double border;
            if (inTag.Driver == DriverType.SIMULATION_DRIVER)
            {
                newValue = SimulationDriver.SimulationDriver.ReturnValue(inTag.IoAddress);
                border = (inTag.IoAddress == "S" || inTag.IoAddress == "R") ? 50 : 0;
            }
            else
            {
                bool contains = _rtuValues.TryGetValue(inTag.IoAddress, out double findVal);
                if (!contains)
                {
                    return false;
                }

                newValue = findVal;
                border = 0.5;
            }
            newValue = newValue < border ? 0 : 1;

            return true;
        }

        private static bool GetValueForAnalogInputTag(AnalogInputTag inTag, ref double newValue)
        {
            if (inTag.Driver == DriverType.SIMULATION_DRIVER)
            {
                newValue = SimulationDriver.SimulationDriver.ReturnValue(inTag.IoAddress);
            }
            else
            {
                bool contains = _rtuValues.TryGetValue(inTag.IoAddress, out double findVal);
                if (!contains)
                {
                    return false;
                }

                newValue = findVal;
            }

            if (inTag.ScanOn)
            {
                foreach (Alarm alarm in inTag.Alarms)
                {
                    if (alarm.AlarmType == AlarmType.HIGH && newValue > inTag.HighLimit)
                    {
                        WriteAlarmToDatabase(alarm, newValue, inTag.HighLimit);
                    }
                    else if (alarm.AlarmType == AlarmType.LOW && newValue < inTag.LowLimit)
                    {
                        WriteAlarmToDatabase(alarm, newValue, inTag.LowLimit);
                    }
                }
            }

            if (newValue < inTag.LowLimit)
            {
                newValue = inTag.LowLimit;
            }
            else if (newValue > inTag.HighLimit)
            {
                newValue = inTag.HighLimit;
            }

            return true;
        }

        public static bool DeleteTag(string tagName, string type, string token)
        {
            if (!UserProcessing.IsUserAuthenticated(token, Role.USER) || !TagExistsForNameAndType(tagName, type))
            {
                return false;
            }

            _tags.TryGetValue(tagName, out Tag value);

            if (value is InputTag)
            {
                _stopInputTagThreads.TryAdd(value.TagName, true);
            }


            return _tags.TryRemove(tagName, out Tag vl);
        }

        public static bool ChangeOutputTagValue(string tagName, string type, string token, double newValue)
        {
            if (!UserProcessing.IsUserAuthenticated(token, Role.USER) || !TagExistsForNameAndType(tagName, type))
            {
                return false;
            }

            try
            {
                Tag changeTag = _tags[tagName];
                if (!(changeTag is DigitalOutputTag) && !(changeTag is AnalogOutputTag))
                {
                    return false;
                }

                if (changeTag is DigitalOutputTag && newValue != 1 && newValue != 0)
                {
                    return false;
                } 
                else if (changeTag is AnalogOutputTag)
                {
                    AnalogOutputTag anTag = changeTag as AnalogOutputTag;
                    if (newValue < anTag.LowLimit || newValue > anTag.HighLimit)
                    {
                        return false;
                    }
                }

                _outputTagValues.AddOrUpdate(changeTag.IoAddress, newValue, (key, oldValue) => newValue);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TagExistsForNameAndType(string tagName, string type)
        {
            try
            {
                Tag tag = _tags.Where(t => t.Value.TagName == tagName && t.Value.GetType().Name == type).First().Value;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static List<Tag> GetTags(string token, string type)
        {
            if (!UserProcessing.IsUserAuthenticated(token, Role.USER))
            {
                return null;
            }

            return _tags.Values.Where(tag => tag.GetType().Name == type).ToList();
        }

        public static List<TagTransfer> GetOutputTagValues(string token)
        {
            if (!UserProcessing.IsUserAuthenticated(token, Role.USER))
            {
                return null;
            }

            List<TagTransfer> tagTransfers = new List<TagTransfer>();
            tagTransfers.AddRange(
                _tags
                .Where(elem => elem.Value is OutputTag)
                .Select(elem => new TagTransfer
                {
                    Tag = elem.Value,
                    Value = _outputTagValues[elem.Value.IoAddress]
                })
            );

            return tagTransfers;
        }

        private static void UpdateOutputValues(OutputTag outTag)
        {
            bool error = false;

            try { 
                Tag tag = _tags.Values.Where(t => t is OutputTag && t.IoAddress == outTag.IoAddress).First();

                if (outTag is DigitalOutputTag && !(tag is DigitalOutputTag))
                {
                    error = true;
                }
                else if (outTag is AnalogOutputTag && !(tag is AnalogOutputTag))
                {
                    error = true;
                }
            }
            catch (Exception) { }

            if (error)
            {
                throw new Exception();
            }

            _outputTagValues.AddOrUpdate(outTag.IoAddress, outTag.InitialValue, (key, oldValue) => outTag.InitialValue);
        }

        public static List<AlarmTime> GetAllAlarmsForTime(DateTime start, DateTime end, string sortType, bool asc)
        {
            using (var db = new SCADAContext())
            {
                var unorderedResult = db.AlarmTimes.Where(alarmTime => alarmTime.TimeStamp >= start && alarmTime.TimeStamp <= end);
                if (sortType == "prioritet")
                {
                    return asc ? unorderedResult.OrderBy(alarmTime => alarmTime.Priority).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList() :
                                 unorderedResult.OrderByDescending(alarmTime => alarmTime.Priority).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList();
                }
                else
                {
                    return asc ? unorderedResult.OrderBy(alarmTime => alarmTime.TimeStamp).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList() :
                                 unorderedResult.OrderByDescending(alarmTime => alarmTime.TimeStamp).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList();
                }


            }
        }

        public static List<AlarmTime> GetAllAlarmsForPriority(int priority, bool asc)
        {
            using (var db = new SCADAContext())
            {
                var unorderedResult = db.AlarmTimes.Where(alarmTime => alarmTime.Priority == priority);
                return asc ? unorderedResult.OrderBy(alarmTime => alarmTime.TimeStamp).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList() :
                                unorderedResult.OrderByDescending(alarmTime => alarmTime.TimeStamp).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList();
            }
        }

        public static List<TagValue> GetAllTagValuesForTime(DateTime start, DateTime end, bool asc)
        {
            using (var db = new SCADAContext())
            {
                var unorderedResult = db.TagValues.Where(tagValue => tagValue.TimeStamp >= start && tagValue.TimeStamp <= end);
                return asc ? unorderedResult.OrderBy(tagValue => tagValue.TimeStamp).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList() :
                                unorderedResult.OrderByDescending(tagValue => tagValue.TimeStamp).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList();
            }
        }

        public static List<TagValue> GetAllTagValuesForType(string type, bool asc)
        {
            using (var db = new SCADAContext())
            {
                var unorderedResult = db.TagValues.Where(tagValue => tagValue.Type == type);
                return asc ? unorderedResult.OrderBy(tagValue => tagValue.TimeStamp).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList() :
                                unorderedResult.OrderByDescending(tagValue => tagValue.TimeStamp).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList();
            }
        }

        public static List<TagValue> GetAllTagValuesForName(string tagName, bool asc)
        {
            using (var db = new SCADAContext())
            {
                var unorderedResult = db.TagValues.Where(tagValue => tagValue.TagName == tagName);
                return asc ? unorderedResult.OrderBy(tagValue => tagValue.Value).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList() :
                                unorderedResult.OrderByDescending(tagValue => tagValue.Value).Skip(Math.Max(0, unorderedResult.Count() - 100)).ToList();
            }
        }
    }
}