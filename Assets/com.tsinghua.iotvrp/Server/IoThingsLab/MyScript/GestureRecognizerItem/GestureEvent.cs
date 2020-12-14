using UnityEngine.Events;

namespace Tsinghua.HCI.IoTVRP
{
    /// <summary>
    /// A UnityEvent callback containing a SliderEventData payload.
    /// </summary>
    [System.Serializable]
    public class GestureEvent : UnityEvent<GestureEventData> { }

}