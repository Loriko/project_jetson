using System;
using BackEndServer.Models.Enums;
using BackEndServer.Models.ViewModels;
using MySql.Data.MySqlClient;

namespace BackEndServer.Models.DBModels
{
    public class DatabaseAlert
    {
        // Table Name
        public static readonly string TABLE_NAME = "alert";
        // Attributes of Alert table.
        public static readonly string ALERT_ID_LABEL = "id";
        public static readonly string ALERT_NAME_LABEL = "alert_name";
        public static readonly string CAMERA_ID_LABEL = "camera_id";
        public static readonly string USER_ID_LABEL = "user_id";
        public static readonly string CONTACT_METHOD_LABEL = "contact_method";
        public static readonly string TRIGGER_OPERATOR_LABEL = "trigger_operator";
        public static readonly string TRIGGER_NUMBER_LABEL = "trigger_number";
        public static readonly string ALWAYS_ACTIVE_LABEL = "always_active";
        public static readonly string START_TIME_LABEL = "start_time";
        public static readonly string END_TIME_LABEL = "end_time";
        public static readonly string DISABLED_UNTIL_LABEL = "disabled_until";
        public static readonly string SNOOZED_UNTIL_LABEL = "snoozed_until";
        
        public int AlertId { get; set; }
        public string AlertName { get; set; }
        public int CameraId { get; set; }
        public int UserId { get; set; }
        public string ContactMethod { get; set; }
        public string TriggerOperator { get; set; }
        public int TriggerNumber { get; set; }
        public bool AlwaysActive { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DateTime? DisabledUntil { get; set; }
        public DateTime? SnoozedUntil { get; set; }
        
        public DatabaseAlert()
        {
        }
        
        public DatabaseAlert(AlertDetails cameraDetails)
        {
            AlertId = cameraDetails.AlertId;
            AlertName = cameraDetails.AlertName;
            CameraId = cameraDetails.CameraId;
            UserId = cameraDetails.UserId;
            ContactMethod = Enum.GetName(typeof(ContactMethod), cameraDetails.ContactMethod);
            TriggerOperator = Enum.GetName(typeof(TriggerOperator), cameraDetails.TriggerOperator);
            TriggerNumber = cameraDetails.TriggerNumber;
            AlwaysActive = cameraDetails.AlwaysActive;
            StartTime = cameraDetails.StartTime.ToString("HH:mm");
            EndTime = cameraDetails.EndTime.ToString("HH:mm");
            if (cameraDetails.DisabledUntil != null)
            {
                DisabledUntil = cameraDetails.DisabledUntil;
            }
            if (cameraDetails.SnoozedUntil != null)
            {
                SnoozedUntil = cameraDetails.SnoozedUntil;
            }
        }
        
        public void EscapeStringFields()
        {
            if (AlertName != null)
                AlertName = MySqlHelper.EscapeString(AlertName);
            if (ContactMethod != null)
                ContactMethod = MySqlHelper.EscapeString(ContactMethod);
            if (TriggerOperator != null)
                TriggerOperator = MySqlHelper.EscapeString(TriggerOperator);
        }
    }
}