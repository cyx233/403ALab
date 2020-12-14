﻿using UnityEngine;

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
        public GestureDetector leftGestureDetector;
        public GestureDetector rightGestureDetector;
        public GestureEventData leftPreviousGesture = new GestureEventData(GestureType.None);
        public GestureEventData rightPreviousGesture = new GestureEventData(GestureType.None);

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
            GestureEventData temp = new GestureEventData(type);
            Debug.Log("Data@Re:" + temp.GestureType);
             m_OnSensorTriggered?.Invoke(temp);
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
                Debug.Log("AtReco:" + type);
            }
        }
        public void SensorUntrigger(GestureType type)
        {
            if (_isSensorTriggered)
            {
                SensorUntriggered(type);
                Debug.Log("AtRecoUn:" + type);
            }
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
            if(leftGestureDetector.currentGesture.name.Equals("LeftTurnUp")) {
                Debug.Log("Gesture: LeftTurnUp");
                SensorTrigger(GestureType.LeftTurnUp);
                leftPreviousGesture = new GestureEventData(GestureType.LeftTurnUp);
            }
            if(leftGestureDetector.currentGesture.name.Equals("LeftTurnDown"))
            {
                Debug.Log("Gesture: LeftTurnDown");
                SensorTrigger(GestureType.LeftTurnDown);
                leftPreviousGesture = new GestureEventData(GestureType.LeftTurnDown);
            }
            if(leftGestureDetector.currentGesture.name.Equals("ToggleOnOff"))
            {
                Debug.Log("Gesture: toggle");
                SensorTrigger(GestureType.ToggleOnOff);
                leftPreviousGesture = new GestureEventData(GestureType.ToggleOnOff);
            }
            if(leftGestureDetector.currentGesture.name.Equals("None")){
                SensorUntrigger(leftPreviousGesture.GestureType);
            }
            if(rightGestureDetector.currentGesture.name.Equals("RightTurnUp")) {
                Debug.Log("Gesture: RightTurnUp");
                SensorTrigger(GestureType.RightTurnUp);
                rightPreviousGesture = new GestureEventData(GestureType.RightTurnUp);
            }
            if(rightGestureDetector.currentGesture.name.Equals("RightTurnDown"))
            {
                Debug.Log("Gesture: RightTurnDown");
                SensorTrigger(GestureType.RightTurnDown);
                rightPreviousGesture = new GestureEventData(GestureType.RightTurnDown);
            }
            if(rightGestureDetector.currentGesture.name.Equals("ToggleOnOff"))
            {
                Debug.Log("Gesture: toggle");
                SensorTrigger(GestureType.ToggleOnOff);
                rightPreviousGesture = new GestureEventData(GestureType.ToggleOnOff);
            }
            if(rightGestureDetector.currentGesture.name.Equals("None")){
                SensorUntrigger(rightPreviousGesture.GestureType);
            }
        }
    }
}
