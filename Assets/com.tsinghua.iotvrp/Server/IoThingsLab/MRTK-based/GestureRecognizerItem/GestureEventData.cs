﻿namespace Tsinghua.HCI.IoTVRP
{
    public enum GestureType
    {
        None,
        ToggleOnOff,
        TurnUp,
        TurnDown,
    }

    public class GestureEventData 
    {
        public GestureType GestureType { get; private set; }
        public GestureEventData(GestureType type)
        {
            GestureType = type;
        }
        /// <summary>
        /// The previous value of the slider
        /// </summary>
    }
}
