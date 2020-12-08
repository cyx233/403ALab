using UnityEngine;
using System.Collections;

public class Scr_camera : MonoBehaviour {

    public float rotate_amount;
    

	// Use this for initialization
	void Start () {
	
	}

    public void UpdateRotateAmount(float _rotate_amount)
    {
        rotate_amount = _rotate_amount;
    }
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, (Mathf.Sin(Time.realtimeSinceStartup) * rotate_amount) + transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
