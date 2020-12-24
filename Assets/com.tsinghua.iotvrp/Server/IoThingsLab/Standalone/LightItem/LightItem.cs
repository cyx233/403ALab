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
        SightExpDistance,
    }

    public class LightItem : MonoBehaviour
    {
        class MappingFunctions
        {
            static float CalcDist(Vector3 v1, Vector3 v2)
            {
                float dx = v1.x - v2.x;
                float dy = v1.y - v2.y;
                float dz = v1.z - v2.z;
                return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }

            public static float Direct(float direct_speed, Vector3 lightPos, Vector3 targetPos)
            {
                return direct_speed;
            }

            public static float InvDistance(float direct_speed, Vector3 lightPos, Vector3 targetPos)
            {
                float dist = CalcDist(lightPos, targetPos);
                return direct_speed / (1.4f + dist) * 7;
            }

            public static float ExpDistance(float ds, Vector3 lightPos, Vector3 targetPos)
            {
                float dist = CalcDist(lightPos, targetPos);
                return ds * (5 * (float)Math.Exp(-dist));
            }

            static float[] discount_factors = new float[RayCastDetector.POINT_NUM];
            public static void init_discount_factors()
            {
                float val = 1f;
                float sum = 0f;
                for (int i = discount_factors.Length - 1; i >= 0; i--)
                {
                    discount_factors[i] = val;
                    sum += val;
                    val *= (float)Math.Exp(-3 * (discount_factors.Length - i) / (float)discount_factors.Length);
                }
                for (int i = 0; i < discount_factors.Length; i++)
                {
                    discount_factors[i] /= sum;
                }
            }

            public static float SightExpDistance(float ds, Vector3 lightPos, Vector3 targetPos)
            {
                float ns = 0f;
                Vector3[] hitpoints = RayCastDetector.HitPoints.ToArray();
                for (int i = 0; i < hitpoints.Length; i++)
                {
                    ns += discount_factors[i] * ExpDistance(ds, lightPos, hitpoints[i]);
                }
                return ns;
            }
        }


        static public MappingMode mode = MappingMode.Direct;
        static private int mode_len = Enum.GetNames(typeof(MappingMode)).Length;

        delegate float MappingFunc(float direct_speed, Vector3 lightPos, Vector3 targetPos);
        static Dictionary<MappingMode, MappingFunc> mappingFuncs = new Dictionary<MappingMode, MappingFunc>()
        {
            {MappingMode.Direct, MappingFunctions.Direct},
            {MappingMode.InvDistance, MappingFunctions.InvDistance},
            {MappingMode.ExpDistance, MappingFunctions.ExpDistance},
            {MappingMode.SightExpDistance, MappingFunctions.SightExpDistance},
        };

        // it takes `speed_in_frames` number of frames to change light intensity from min to max
        static int speed_in_frames = 500;
        private float direct_speed; // = (max - min) / speed_in_frames

        static Dictionary<string, float> defaultIntensity = new Dictionary<string, float>()
        {
            { "DeskLightItem", 0.5f },
            { "MainLightItem", 1 },
            { "ReadingLightItem", 0.5f },
            { "WallLightItem1", 0 },
            { "WallLightItem2", 0 },
            { "WallLightItem3", 0 },
            { "WallLightItem4", 0 },
            { "WallLightItem5", 0 },
            { "WallLightItem6", 0 },
        };
        static Dictionary<string, float> minIntensity = new Dictionary<string, float>()
        {
            { "DeskLightItem", 0f },
            { "MainLightItem", 0f },
            { "ReadingLightItem", 0f },
            { "WallLightItem1", 0f },
            { "WallLightItem2", 0f },
            { "WallLightItem3", 0f },
            { "WallLightItem4", 0f },
            { "WallLightItem5", 0f },
            { "WallLightItem6", 0f },
        };
        static Dictionary<string, float> maxIntensity = new Dictionary<string, float>()
        {
            { "DeskLightItem", 8 },
            { "MainLightItem", 14 },
            { "ReadingLightItem", 8 },
            { "WallLightItem1", 8 },
            { "WallLightItem2", 8 },
            { "WallLightItem3", 8 },
            { "WallLightItem4", 8 },
            { "WallLightItem5", 8 },
            { "WallLightItem6", 8 },
        };

        public static void ToggleMode()
        {
            LightItem[] lights = GameObject.FindObjectsOfType<LightItem>();
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].SetIntensity(defaultIntensity[lights[i].GetLightName()]);
            }
            if (mode == MappingMode.Direct)
                mode = MappingMode.ExpDistance;
            else if (mode == MappingMode.ExpDistance)
                mode = MappingMode.InvDistance;
            else if (mode == MappingMode.InvDistance)
                mode = MappingMode.SightExpDistance;
            else if (mode == MappingMode.SightExpDistance)
                mode = MappingMode.Direct;
        }

        public static string GetMode()
        {
            switch (mode)
            {
                case MappingMode.Direct:
                    return "Direct";
                case MappingMode.InvDistance:
                    return "Inverse Distance";
                case MappingMode.ExpDistance:
                    return "Exponential Distance";
                case MappingMode.SightExpDistance:
                    return "Sight Exponential Distance";
                default:
                    return "";
            }
        }


        private Light _light;
        private string lightname;
        private OVRPlayerController player;
        private long THRESH = 60;
        long triggered_frame;

        // Start is called before the first frame update
        void Start()
        {
            _light = GetComponent<Light>();
            player = GameObject.FindObjectOfType<OVRPlayerController>();
            lightname = gameObject.name;
            SetIntensity(defaultIntensity[lightname]);
            _light.enabled = false;
            direct_speed = (maxIntensity[lightname] - minIntensity[lightname]) / speed_in_frames;
            triggered_frame = THRESH;
            MappingFunctions.init_discount_factors();
        }

        // Update is called once per frame
        void Update()
        {
            triggered_frame += 1;
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

        public string GetLightName()
        {
            return lightname;
        }

        public void IncreaseIntensity(float num)
        {
            if (_light.intensity < maxIntensity[lightname] && _light.enabled)
                _light.intensity += num;
        }
        public void DecreaseIntensity(float num)
        {
            if (_light.intensity > minIntensity[lightname] && _light.enabled)
                _light.intensity -= num;
        }

        public void GestureControl(GestureEventData gestureEventData)
        {
            Debug.Log("light:"+gestureEventData.GestureType);
            switch (gestureEventData.GestureType)
            {
                case GestureType.ToggleOnOff:
                    if (triggered_frame > THRESH)
                    {
                        Debug.Log(triggered_frame);
                        Toggle();
                        triggered_frame = 0;
                    }
                    break;
                case GestureType.LeftTurnUp:
                case GestureType.RightTurnUp:
                    IncreaseIntensity(mappingFuncs[mode](direct_speed, _light.transform.position, player.transform.position));
                    break;
                case GestureType.LeftTurnDown:
                case GestureType.RightTurnDown:
                    DecreaseIntensity(mappingFuncs[mode](direct_speed, _light.transform.position, player.transform.position));
                    break;
            }
        }

        public void UnsetGestureControl(GestureEventData gestureEventData) { }
    }

}
