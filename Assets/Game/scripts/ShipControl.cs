using UnityEngine;
using System.Collections;
using System;

public class ShipControl : MonoBehaviour {

	private Gyroscope gyro;

	Vector3 calibratedPos;
	int calibrationSamplesLeft = 10;
	int takenCalibrationSamples = 0;

	[SerializeField]
	Vector3 gyroCtrlRange = new Vector3(1,1,1);
	[SerializeField]
	Vector3 gyroLPFilter = new Vector3(1,1,1);

	Vector3 gyroSmothPosition = new Vector3(0, 0, 0);

	// Use this for initialization
	void Start () {
		if (SystemInfo.supportsGyroscope) {
			gyro = Input.gyro;
			gyro.enabled = true;
		}
	}

	void FixedUpdate() {
		if (calibrationSamplesLeft > 0) {
			calibrationSamplesLeft--;
			calibratedPos += gyro.attitude.eulerAngles;
			takenCalibrationSamples++;

			if (calibrationSamplesLeft == 0) {
				calibratedPos = calibratedPos / takenCalibrationSamples;
				Debug.Log ("Calibrated on " + calibratedPos);
			}
		} else {
			updateControls ();

		}
	}

	// Update is called once per frame
	void Update () {
		if (calibrationSamplesLeft == 0) {
			Debug.Log (String.Format ("{0}   <<<<<  {1}", gyroSmothPosition, gyro.attitude.eulerAngles));

			Vector3 dir = new Vector3 (getPitch (), getYaw(), 0);

			transform.rotation = Quaternion.Euler (dir);
		}
	}

	void updateControls() {
		Vector3 sample = gyro.attitude.eulerAngles - calibratedPos;
		Vector3 diff = sample - gyroSmothPosition;
		gyroSmothPosition.x += (diff.x) * gyroLPFilter.x;
		gyroSmothPosition.y += (diff.y) * gyroLPFilter.y;
		gyroSmothPosition.z += (diff.z) * gyroLPFilter.z;

		gyroSmothPosition.x = gyroSmothPosition.x / gyroCtrlRange.x;
		gyroSmothPosition.y = gyroSmothPosition.y / gyroCtrlRange.y;
		gyroSmothPosition.z = gyroSmothPosition.z / gyroCtrlRange.z;
	}

	float getPitch() {
		return gyroSmothPosition.y;
	}

	float getYaw() {
		return gyroSmothPosition.z;
	}
}
