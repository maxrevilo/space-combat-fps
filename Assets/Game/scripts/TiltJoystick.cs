﻿using UnityEngine;
using System;

public class TiltJoystick : MonoBehaviour
{
    public bool isCalibrating() { return calibrationSamples > 0; }

    public float getPitch() { return Mathf.Clamp(output.y, -1, 1); }

    public float getRoll() { return Mathf.Clamp(output.z, -1, 1); }

    [SerializeField]
    private int calibrationSamples = 10;

    [SerializeField]
    private Vector3 gyroCtrlRange = new Vector3(180, 180, 180);

    [SerializeField]
    private Vector3 gyroLPFilter = new Vector3(1, 1, 1);


    Vector3 calibratedPos;
    int takenCalibrationSamples = 0;
    private Gyroscope gyro;

    Vector3 gyroSmothPosition = new Vector3(0, 0, 0);
    Vector3 output = new Vector3(0, 0, 0);

    void Awake()
    {
        if (SystemInfo.supportsGyroscope) {
            gyro = Input.gyro;
            gyro.enabled = true;
        } else
        {
            enabled = false;
            Debug.LogWarning("Gyroscope not supported.");
        }
    }

    void FixedUpdate()
    {
        if (isCalibrating()) calibrateStep();
        else updateControls();
    }

    private void calibrateStep()
    {
        calibrationSamples--;
        calibratedPos += gyro.attitude.eulerAngles;
        takenCalibrationSamples++;

        if (calibrationSamples == 0)
        {
            calibratedPos = calibratedPos / takenCalibrationSamples;
            Debug.Log("Gyro Calibrated on " + calibratedPos);
        }
    }

    void updateControls()
    {
        Vector3 sample = gyro.attitude.eulerAngles - calibratedPos;

        gyroSmothPosition.x = Mathf.LerpAngle(gyroSmothPosition.x, sample.x, gyroLPFilter.x);
        gyroSmothPosition.y = Mathf.LerpAngle(gyroSmothPosition.y, sample.y, gyroLPFilter.y);
        gyroSmothPosition.z = Mathf.LerpAngle(gyroSmothPosition.z, sample.z, gyroLPFilter.z);

        output = GeomHelper.div(gyroSmothPosition, gyroCtrlRange);
    }

    class GeomHelper
    {
        static public Vector3 AngleDifference(Vector3 aEuler, Vector3 bEuler)
        {
            return Abs(ToAbsoluteAngle(aEuler - bEuler));
        }

        // To angles between 0 and 360
        static public Vector3 ToAbsoluteAngle(Vector3 euler)
        {
            euler.x = euler.x % 360;
            euler.y = euler.y % 360;
            euler.z = euler.z % 360;

            if (euler.x < 0) euler.x = euler.x + 360;
            if (euler.y < 0) euler.y = euler.y + 360;
            if (euler.z < 0) euler.z = euler.z + 360;

            return euler;
        }

        // To angles between -180 and 180
        static public Vector3 toRelativeAngle(Vector3 euler)
        {
            euler.x = toRelativeAngle(euler.x);
            euler.y = toRelativeAngle(euler.y);
            euler.z = toRelativeAngle(euler.z);
            return euler;
        }

        // To angles between -180 and 180
        static public float toRelativeAngle(float angle)
        {
            angle = angle % 360;
            if (angle > 180) angle -= 360;
            else if (angle < -180) angle += 360;
            return angle;
        }

        static public Vector3 div(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.x / b.x,
                a.y / b.y,
                a.z / b.z
            );
        }

        static public Vector3 Abs(Vector3 v)
        {
            v.x = Mathf.Abs(v.x);
            v.y = Mathf.Abs(v.y);
            v.z = Mathf.Abs(v.z);
            return v;
        }
    }
}
