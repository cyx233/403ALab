using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastDetector : MonoBehaviour
{
    private Ray camerRay;
    public static int POINT_NUM=1000;
    [SerializeField]
    private new Camera camera;
    static public Queue<Vector3>HitPoints;
    // Start is called before the first frame update
    void Start()
    {
        HitPoints = new Queue<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 begin;
        Vector3 end;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward*100);
        if (Physics.Raycast(ray, out hit))
        {
            begin = ray.origin;
            end = hit.point;
            HitPoints.Enqueue(hit.point);
            if (HitPoints.Count>POINT_NUM)
            {
                HitPoints.Dequeue();
            }
        }
        else
        {
            begin = ray.origin;
            end = ray.origin + camera.transform.forward * 100;
        }
        Debug.DrawLine(begin, end, Color.red, 2, false);
    }
}
