using UnityEngine;
using System.Collections;

public class RayCameraController : MonoBehaviour {
	[SerializeField] Transform target;
	Vector3 camDesiredTarget;
	float maxDistance = 15f;

	void LateUpdate()
	{
		transform.position = target.position;
		Quaternion targetRotation = Quaternion.Euler(0,target.rotation.eulerAngles.y,0);
		transform.rotation = targetRotation;
		transform.Translate(new Vector3(0,6,-maxDistance));

		RaycastHit hit;
		var camVector = transform.position-target.position;
		Ray ray = new Ray(target.position,camVector);
		if (Physics.Raycast(ray,out hit,maxDistance+0.5f))
		{
			transform.position = hit.point + hit.normal;
		}

		var rot = transform.rotation.eulerAngles;
		rot.x = Vector3.Angle(target.position - transform.position, transform.forward);
		transform.rotation = Quaternion.Euler(rot);
		transform.Translate(Vector3.forward*0.5f);
	}
}
