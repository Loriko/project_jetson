using System;
using BackEndServer.Models.DBModels;
using BackEndServer.Models.Enums;

namespace BackEndServer.Models.ViewModels
{
    public class AlertSummary
    {
        public bool IsMoreThanTrigger { get; set; }
        public int TriggerNumber { get; set; }

        public AlertSummary()
        {
        }

        public AlertSummary(DatabaseAlert alert)
        {
            TriggerNumber = alert.TriggerNumber;
            IsMoreThanTrigger = (TriggerOperator) Enum.Parse(typeof(TriggerOperator), alert.TriggerOperator) == TriggerOperator.More;
        }
    }
}