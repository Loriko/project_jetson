using System;
using BackEndServer.Models.Enums;
using BackEndServer.Models.DBModels;
using Castle.Core.Internal;
using NUnit.Framework;

namespace BackEndServer.Models.ViewModels
{
    public class AlertDetails
    {
        public static readonly string TIME_FORMAT = "hh:mm tt";
        
        public int AlertId { get; set; }
        public string AlertName { get; set; }
        public int CameraId { get; set; }
        public int UserId { get; set; }
        public ContactMethod ContactMethod { get; set; }
        public TriggerOperator TriggerOperator { get; set; }
        public int TriggerNumber { get; set; }
        public bool AlwaysActive { get; set; }
        //DateTime is used but Date is ignored
        public DateTime StartTime { get; set; }

        public string StartTimeAsString()
        {
            return StartTime.ToString(TIME_FORMAT);
        }

        public DateTime EndTime { get; set; }

        public string EndTimeAsString()
        {
            return EndTime.ToString(TIME_FORMAT);
        }
        
        public string TriggerString()
        {
            //TODO: remove the lower after to string and instead have a display string for the enum values
            string periodString = AlwaysActive
                ? "at all times"
                : $"between {StartTimeAsString()} and {EndTimeAsString()}";
            string triggerString = $"Triggered when {TriggerOperator.ToString().ToLower()} than {TriggerNumber}" +
                                   $" people are spotted {periodString}";
            return triggerString;
        }

        public AlertDetails()
        {
        }

        public AlertDetails(DatabaseAlert dbAlert)
        {
            AlertId = dbAlert.AlertId;
            AlertName = dbAlert.AlertName;
            CameraId = dbAlert.CameraId;
            UserId = dbAlert.UserId;
//            if (dbAlert.ContactMethod != null)
//            {
//                ContactMethod = dbAlert.ContactMethod.Value;
//            }
//            else
//            {
//                throw new InvalidOperationException("Passed in DatabaseAlert argument has a null dbAlert");
//            }
            ContactMethod = (ContactMethod) Enum.Parse(typeof(ContactMethod), dbAlert.ContactMethod);
            TriggerOperator = (TriggerOperator) Enum.Parse(typeof(TriggerOperator), dbAlert.TriggerOperator);
            TriggerNumber = dbAlert.TriggerNumber;
            AlwaysActive = dbAlert.AlwaysActive;
            //Could throw format exception if times aren't correctly formatted
            if (!dbAlert.StartTime.IsNullOrEmpty())
            {
                StartTime = DateTime.Parse(dbAlert.StartTime);
            }
            if (!dbAlert.EndTime.IsNullOrEmpty())
            {
                EndTime = DateTime.Parse(dbAlert.EndTime);
            }
        }
    }
}