using UnityEngine;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

// TODO: refactor code to get; set 
namespace Tsinghua.HCI.IoTVRP
{
    public class GestureRecognizerItem : MonoBehaviour
    {
        private float EPS = 0.005f;

        [SerializeField]
        [Tooltip("Action performed after sensor is triggered")]
        private GestureEvent m_OnSensorTriggered;
        [SerializeField]
        [Tooltip("Action performed after sensor is triggered")]
        private GestureEvent m_OnSensorUntriggered;

        bool _isSensorTriggered = false;

        // Start is called before the first frame update
        void Start()
        {
            if (m_OnSensorTriggered == null)
                m_OnSensorTriggered = new GestureEvent();
            if (m_OnSensorUntriggered == null)
                m_OnSensorUntriggered = new GestureEvent();
        }


        void SensorTriggered(GestureType type)
        {
            _isSensorTriggered = true;
             m_OnSensorTriggered?.Invoke(new GestureEventData(type));
        }

        void SensorUntriggered(GestureType type)
        {
            _isSensorTriggered = false;
            m_OnSensorUntriggered?.Invoke(new GestureEventData(type));
        }


        //Calculate value here
        public void SensorTrigger(GestureType type)
        {
            if (!_isSensorTriggered) 
                SensorTriggered(type);
        }
        public void SensorUntrigger(GestureType type)
        {
            if (_isSensorTriggered) 
                SensorUntriggered(type);
        }


        // Update is called once per frame
        void Update()
        {
            Vector3 RightIndexTipPosition = Vector3.zero;
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose poseRightIndexTip))
            {
                RightIndexTipPosition = poseRightIndexTip.Position;
            }

            Vector3 LeftIndexTipPosition = Vector3.zero;
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out MixedRealityPose poseLeftIndexTip))
            {
                LeftIndexTipPosition = poseLeftIndexTip.Position;
            }
            if (Vector3.Distance(LeftIndexTipPosition, RightIndexTipPosition) < EPS)
            {
                SensorTrigger(GestureType.TurnOn);
            }
            else
            {
                SensorUntrigger(GestureType.TurnOff);
            }
        }
    }
}
