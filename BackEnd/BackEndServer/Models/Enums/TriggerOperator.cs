using System;
using System.ComponentModel;

namespace BackEndServer.Models.Enums
{
    public enum TriggerOperator
    {
        More,
        Less
    }
    
    public static class TriggerOperatorExtension
    {
        public static String GetSqlForm(this TriggerOperator triggerOperator)
        {
            switch (triggerOperator)
            {
                case TriggerOperator.More:
                    return ">";
                case TriggerOperator.Less:
                    return "<";
                default:
                    throw new InvalidEnumArgumentException("TriggerOperator.GetSqlForm() called with invalid operator");
            }
        }
    }

}