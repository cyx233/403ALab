namespace Tsinghua.HCI.IoTVRP
{
    public enum GestureType
    {
        None,
        ToggleOnOff,
        LeftTurnUp,
        RightTurnUp,
        LeftTurnDown,
        RightTurnDown,
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
