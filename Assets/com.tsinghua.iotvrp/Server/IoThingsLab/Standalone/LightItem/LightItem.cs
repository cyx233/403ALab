using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: refactor code to get; set 
namespace Tsinghua.HCI.IoTVRP
{
    public enum MappingMode
    {
        Direct, // adjust all the lights at the same pace
        InvDistance, // speed = direct_speed / dist
        ExpDistance, // speed = direct_speed * (1 + 4 * e^{-x})
    }

    public class LightItem : MonoBehaviour
    {
        class MappingFunctions
        {
            public static float Direct(float direct_speed, float dist)
            {
                return direct_speed;
            }

            public static float InvDistance(float direct_speed, float dist)
            {
                return direct_speed / dist * 7;
            }

            public static float ExpDistance(float ds, float dist)
            {
                return ds * (1 + 4 * (float)Math.Exp(-dist));
            }
        }


        static public MappingMode mode;
        static private int mode_len;

        delegate float MappingFunc(float direct_speed, float dist);
        static Dictionary<MappingMode, MappingFunc> mappingFuncs = new Dictionary<MappingMode, MappingFunc>()
        {
            {MappingMode.Direct, MappingFunctions.Direct},
            {MappingMode.InvDistance, MappingFunctions.InvDistance},
            {MappingMode.ExpDistance, MappingFunctions.ExpDistance},
        };

        // it takes `speed_in_frames` number of frames to change light intensity from min to max
        static int speed_in_frames = 100000;
        private float direct_speed; // = (max - min) / speed_in_frames

        static Dictionary<string, float> defaultIntensity = new Dictionary<string, float>()
        {
            { "DeskLightItem", 4 },
            { "MainLightItem", 4 },
            { "ReadingLightItem", 3 },
            { "NightLightItem", 5 },
        };
        static Dictionary<string, float> minIntensity = new Dictionary<string, float>()
        {
            { "DeskLightItem", 1 },
            { "MainLightItem", 1 },
            { "ReadingLightItem", 1 },
            { "NightLightItem", 5 },
        };
        static Dictionary<string, float> maxIntensity = new Dictionary<string, float>()
        {
            { "DeskLightItem", 10 },
            { "MainLightItem", 10 },
            { "ReadingLightItem", 10 },
            { "NightLightItem", 5 },
        };

        public static void ToggleMode()
        {
            /*
            int int_mode = System.Convert.ToInt32(mode);
            int_mode = (int_mode + 1) % mode_len;
            mode = (MappingMode)Enum.ToObject(typeof(MappingMode), int_mode);
            */
            if (mode == MappingMode.Direct)
                mode = MappingMode.ExpDistance;
            else if (mode == MappingMode.ExpDistance)
                mode = MappingMode.InvDistance;
            else if (mode == MappingMode.InvDistance)
                mode = MappingMode.Direct;
        }

        public static string GetMode()
        {
            return Enum.GetName(typeof(MappingMode), mode);
        }


        private Light _light;
        private GestureType gestureType;
        private string lightname;
        private OVRPlayerController player;

        // Start is called before the first frame update
        void Start()
        {
            _light = GetComponent<Light>();
            player = GameObject.FindObjectOfType<OVRPlayerController>();
            gestureType = GestureType.None;
            lightname = gameObject.name;
            direct_speed = (maxIntensity[lightname] - minIntensity[lightname]) / speed_in_frames;
            mode_len = Enum.GetNames(typeof(MappingMode)).Length;
        }

        // Update is called once per frame
        void Update()
        {
            switch (gestureType)
            {
                case GestureType.TurnUp:
                    if (_light.intensity < maxIntensity[lightname])
                        _light.intensity += mappingFuncs[mode](direct_speed, GetDistanceFromPlayer());
                    break;
                case GestureType.TurnDown:
                    if (_light.intensity > minIntensity[lightname])
                        _light.intensity -= mappingFuncs[mode](direct_speed, GetDistanceFromPlayer());
                    break;
            }
        }

        float GetDistanceFromPlayer()
        {
            float dx = player.transform.position.x - _light.transform.position.x;
            float dy = player.transform.position.y - _light.transform.position.y;
            float dz = player.transform.position.z - _light.transform.position.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public void SetColor(Color newColor)
        {
            _light.color = newColor;
        }

        public void Toggle()
        {
            if (_light.enabled)
                TurnOff();
            else
                TurnOn();
        }

        public void TurnOff()
        {
            _light.enabled = false;
        }

        public void TurnOn()
        {
            _light.intensity = defaultIntensity[lightname];
            _light.enabled = true;
        }

        public void SetRange(float newRange)
        {
            _light.range = newRange;
        }

        public void SetIntensity(float newIntensity)
        {
            _light.intensity = newIntensity;
        }

        public void SetType(LightType lightType)
        {
            _light.type = lightType;
        }

        public Light GetLight()
        {
            return _light;
        }
        public void IncreaseIntensity(float num)
        {
            if (_light.intensity < maxIntensity[lightname])
                _light.intensity += num;
        }
        public void DecreaseIntensity(float num)
        {
            if (_light.intensity > minIntensity[lightname])
                _light.intensity -= num;
        }

        public void GestureControl(GestureEventData gestureEventData)
        {
            switch (gestureEventData.GestureType)
            {
                case GestureType.TurnOn:
                    gestureType = GestureType.TurnOn;
                    TurnOn();
                    break;
                case GestureType.TurnOff:
                    gestureType = GestureType.TurnOff;
                    TurnOff();
                    break;
                case GestureType.TurnUp:
                    gestureType = GestureType.TurnUp;
                    break;
                case GestureType.TurnDown:
                    gestureType = GestureType.TurnDown;
                    break;
                default:
                    gestureType = GestureType.None;
                    break;
            }
        }

        public void UnsetGestureControl()
        {
            gestureType = GestureType.None;
        }
    }

}
