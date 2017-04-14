using UnityEngine;

public class CameraControl : MonoBehaviour {
  
	// Update is called once per frame
	void Update () {
    var m = Input.GetAxis("Mouse ScrollWheel");
    transform.position = new Vector3(0,0, transform.position.z * (1 - m));
  }
}
