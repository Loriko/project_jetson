using System;

namespace JsonGenerator.Classes
{
    /// <summary>
    /// This class also acts as a SingleSecondTimeRequest class.
    /// </summary>
    public class SingleSecondTime
    {
        public long UnixTime { get; set; }

        public SingleSecondTime(long UnixTime)
        {
            this.UnixTime = UnixTime;
        }
    }
}