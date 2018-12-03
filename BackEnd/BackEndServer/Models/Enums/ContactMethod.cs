using System.ComponentModel;

namespace BackEndServer.Models.Enums
{
    public enum ContactMethod
    {
        [Description("Notify me by email and inside this application")]
        Email,
        [Description("Notify me inside this application")]
        Notification
    }
}