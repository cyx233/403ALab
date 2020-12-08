namespace Tsinghua.HCI.IoTVRP
{
    public enum GestureType
    {
        TurnOn,
        TurnOff
    }

    public class GestureEventData 
    {
        public GestureEventData(GestureType type)
        {
            GestureType GestureType = type;
        }
        /// <summary>
        /// The previous value of the slider
        /// </summary>
        public GestureType GestureType { get; private set; }
    }
}
