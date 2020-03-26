//------------------------------------------------------------------------------------------------
// Edy's Vehicle Physics
// (c) Angel Garcia "Edy" - Oviedo, Spain
// http://www.edy.es
//------------------------------------------------------------------------------------------------

using UnityEngine;
using System;



[Serializable]
public class CameraOrbitSettings
	{
	public float distance = 10.0f;
	[Space(5)]
	public float horizontalSpeed = 5f;
	public float verticalSpeed = 2.5f;
	public float distanceSpeed = 10.0f;
	[Space(5)]
	public float minVerticalAngle = -20.0f;
	public float maxVerticalAngle = 80.0f;
	public float minDistance = 5.0f;
	public float maxDistance = 50.0f;
	[Space(5)]
	public float orbitDamping = 4.0f;
	public float distanceDamping = 4.0f;
	}

[Serializable]
public class CameraSmoothFollowSettings
	{
	public float distance = 10.0f;
	public float height = 5.0f;
	public float viewHeightRatio = 0.5f;      // Look above the target (height * this ratio)
	[Space(5)]
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	[Space(5)]
	public bool followVelocity = true;
	public float velocityDamping = 5.0f;
	}


public class VehicleCameraControl : MonoBehaviour
	{
	public enum Mode { Fixed, SmoothFollow, MouseOrbit };
	public Mode mode = Mode.SmoothFollow;

	public Transform target;
	public Transform targetFixedPosition;

	public KeyCode changeCameraKey = KeyCode.C;

	public CameraOrbitSettings orbitSettings = new CameraOrbitSettings();
	public CameraSmoothFollowSettings smoothFollowSettings = new CameraSmoothFollowSettings();


	Transform m_transform;
	Mode m_prevMode = Mode.SmoothFollow;


	void OnEnable ()
		{
		m_transform = GetComponent<Transform>();

		// Initialize each mode's settings and reset the camera

		InitializeMouseOrbit();
		ResetCamera();
		}


	void LateUpdate ()
		{
		if (Input.GetKeyDown(changeCameraKey))
			{
			if (mode == Mode.MouseOrbit)
				mode = Mode.Fixed;
			else
				mode++;
			}

		if (mode != m_prevMode)
			{
			ResetCamera();
			m_prevMode = mode;
			}

		switch (mode)
			{
			case Mode.Fixed:
				DoFixedCamera();
				break;

			case Mode.SmoothFollow:
				DoSmoothFollow();
				break;

			case Mode.MouseOrbit:
				DoMouseOrbit();
				break;
			}
		}


	public void ResetCamera ()
		{
		// Reset each camera mode

		ResetSmoothFollow();
		}


	//----------------------------------------------------------------------------------------------


	void DoFixedCamera ()
		{
		Transform fixedTarget = targetFixedPosition != null? targetFixedPosition : target;
		if (fixedTarget == null) return;

		m_transform.position = fixedTarget.position;
		m_transform.rotation = fixedTarget.rotation;
		}


	//----------------------------------------------------------------------------------------------


	private Vector3 m_smoothLastPos = Vector3.zero;
	private Vector3 m_smoothVelocity = Vector3.zero;
	private float m_smoothTargetAngle = 0.0f;


	void ResetSmoothFollow ()
		{
		if (target == null) return;

		m_smoothLastPos = target.position;
		m_smoothVelocity = target.forward * 2.0f;
		m_smoothTargetAngle = target.eulerAngles.y;
		}


	void DoSmoothFollow ()
		{
		if (target == null) return;

		Vector3 updatedVelocity = (target.position - m_smoothLastPos) / Time.deltaTime;
		m_smoothLastPos = target.position;

		updatedVelocity.y = 0.0f;

		if (updatedVelocity.magnitude > 1.0f)
			{
			m_smoothVelocity = Vector3.Lerp(m_smoothVelocity, updatedVelocity, smoothFollowSettings.velocityDamping * Time.deltaTime);
			m_smoothTargetAngle = Mathf.Atan2(m_smoothVelocity.x, m_smoothVelocity.z) * Mathf.Rad2Deg;
			}

		if (!smoothFollowSettings.followVelocity)
			m_smoothTargetAngle = target.eulerAngles.y;

		float wantedHeight = target.position.y + smoothFollowSettings.height;
		float currentRotationAngle = m_transform.eulerAngles.y;
		float currentHeight = m_transform.position.y;

		currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, m_smoothTargetAngle, smoothFollowSettings.rotationDamping * Time.deltaTime);
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, smoothFollowSettings.heightDamping * Time.deltaTime);

		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);

		m_transform.position = target.position;
		m_transform.position -= currentRotation * Vector3.forward * smoothFollowSettings.distance;

		Vector3 t = m_transform.position;
		t.y = currentHeight;
		m_transform.position = t;

		m_transform.LookAt (target.position + Vector3.up * smoothFollowSettings.height * smoothFollowSettings.viewHeightRatio);
		}


	//----------------------------------------------------------------------------------------------


	private float m_orbitX = 0.0f;
	private float m_orbitY = 0.0f;
	private float m_orbitDistance;


	void InitializeMouseOrbit ()
		{
		m_orbitDistance = orbitSettings.distance;

		Vector3 angles = m_transform.eulerAngles;
		m_orbitX = angles.y;
		m_orbitY = angles.x;
		}


	void DoMouseOrbit ()
		{
		if (target == null) return;

		m_orbitX += Input.GetAxis("Mouse X") * orbitSettings.horizontalSpeed;
		m_orbitY -= Input.GetAxis("Mouse Y") * orbitSettings.verticalSpeed;
		orbitSettings.distance -= Input.GetAxis("Mouse ScrollWheel") * orbitSettings.distanceSpeed;

		m_orbitY = Mathf.Clamp(m_orbitY, orbitSettings.minVerticalAngle, orbitSettings.maxVerticalAngle);
		orbitSettings.distance = Mathf.Clamp(orbitSettings.distance, orbitSettings.minDistance, orbitSettings.maxDistance);

		m_orbitDistance = Mathf.Lerp(m_orbitDistance, orbitSettings.distance, orbitSettings.distanceDamping * Time.deltaTime);
		m_transform.rotation = Quaternion.Slerp(m_transform.rotation, Quaternion.Euler(m_orbitY, m_orbitX, 0), Time.deltaTime * orbitSettings.orbitDamping);
		m_transform.position = target.position + m_transform.rotation * new Vector3(0.0f, 0.0f, -m_orbitDistance);
		}
	}
