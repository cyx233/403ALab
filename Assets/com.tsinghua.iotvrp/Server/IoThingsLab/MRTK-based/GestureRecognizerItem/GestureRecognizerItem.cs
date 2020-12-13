using UnityEngine;

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

// TODO: refactor code to get; set 
namespace Tsinghua.HCI.IoTVRP
{
    public class GestureRecognizerItem : MonoBehaviour
    {
        private float EPS = 0.005f;
        public OVRHand LeftHand;
        public OVRHand RightHand;

        [SerializeField]
        [Tooltip("Action performed after sensor is triggered")]
        private GestureEvent m_OnSensorTriggered;
        [SerializeField]
        [Tooltip("Action performed after sensor is triggered")]
        private GestureEvent m_OnSensorUntriggered;
        public GestureDetector gestureDetector;

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
            {
                SensorTriggered(type);
                Debug.Log("Successful");
            }
            Debug.Log("UnSuccessful");

        }
        public void SensorUntrigger(GestureType type)
        {
            if (_isSensorTriggered) 
                SensorUntriggered(type);
        }


        // Update is called once per frame
        void Update()
        {
            // Vector3 RightIndexTipPosition = Vector3.zero;
            // if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Right, out MixedRealityPose poseRightIndexTip))
            // {
            //     RightIndexTipPosition = poseRightIndexTip.Position;
            // }

            // Vector3 LeftIndexTipPosition = Vector3.zero;
            // if (HandJointUtils.TryGetJointPose(TrackedHandJoint.IndexTip, Handedness.Left, out MixedRealityPose poseLeftIndexTip))
            // {
            //     LeftIndexTipPosition = poseLeftIndexTip.Position;
            // }
            if(gestureDetector.currentGesture.name.Equals("Left fuck"))
            {
                Debug.Log(gestureDetector.currentGesture.name);
                SensorTrigger(GestureType.TurnOn);
            }
            else if(gestureDetector.currentGesture.name.Equals("Left thumb up"))
            {
                Debug.Log(gestureDetector.currentGesture.name);
                SensorUntrigger(GestureType.TurnOff);
            }
            else{
                SensorUntrigger(GestureType.None);
            }
        }
    }
}
