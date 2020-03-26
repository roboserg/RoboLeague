using UnityEngine;
using System.Collections;

public class Rotation_TIRES : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.Rotate(Vector3.left * Time.deltaTime * 500);
	
	}
}
