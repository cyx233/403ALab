﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Tsinghua.HCI.IoTVRP
{
    [System.Serializable]
    public struct Gesture
    {
        public string name;
        public List<Vector3> fingerDatas;
        public UnityEvent onRecognized;
    }

    public class GestureDetector : MonoBehaviour
    {
        public float threshold;
        public OVRSkeleton skeleton;
        public List<Gesture> gestures;
        public bool debugMode = true;
        public int bonesSize;
        public List<OVRBone> fingerBones;
        public Gesture previousGesture;
        public Gesture currentGesture;

        // Start is called before the first frame update
        void Start()
        {
            fingerBones = new List<OVRBone>(skeleton.Bones);
            previousGesture = new Gesture();
            previousGesture.name = "None";
            currentGesture = new Gesture();
            currentGesture.name = "None";
        }

        // Update is called once per frame
        void Update()
        {
            if(debugMode && Input.GetKeyDown(KeyCode.Space)){
                Save();
            }
            previousGesture = currentGesture;
            currentGesture = Recognize();
            //bool hasRecognized = !currentGesture.name.Equals("None");
            // Debug.Log("hasRecognized" + currentGesture.name);
            // if(hasRecognized && !currentGesture.Equals(previousGesture)){
            //if(hasRecognized){
            //    Debug.Log("New gesture found: " + currentGesture.name);
            //}
        }

        void Save()
        {
            Gesture g = new Gesture();
            g.name = "New Gesture";
            List<Vector3> data = new List<Vector3>();
            fingerBones = new List<OVRBone>(skeleton.Bones);
            foreach (var bone in fingerBones)
            {
                data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
            }
            g.fingerDatas = data;
            gestures.Add(g);
            bonesSize = fingerBones.Count;
        }

        Gesture Recognize(){
            Gesture currentgesture = new Gesture();
            float currentMin = Mathf.Infinity;
            fingerBones = new List<OVRBone>(skeleton.Bones);
            currentgesture.name = "None";

            foreach(var gesture in gestures){
                float sumDistance = 0;
                bool isDiscarded = false;
                double curth = threshold;
                if (gesture.name.Equals("ToggleOnOff"))
                {
                    curth = 0.4 * curth;
                }
                
                for(int i = 0; i < fingerBones.Count; i++){
                    Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                    float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);
                    if(distance > curth){
                        isDiscarded = true;
                        break;
                    }
                    sumDistance += distance;
                }
                if(!isDiscarded && sumDistance < currentMin){
                    currentMin = sumDistance;
                    currentgesture = gesture;
                }
            }
            return currentgesture;
        }
    }
}