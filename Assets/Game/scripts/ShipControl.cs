using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(ShipMotor))]
public class ShipControl : MonoBehaviour {

    [SerializeField]
    private bool useClassicControls = false;

    private TiltJoystick tiltJoystick;

    private ShipMotor shipMotor;

    void Awake()
    {
        shipMotor = GetComponent<ShipMotor>();

        tiltJoystick = GetComponent<TiltJoystick>();
    }

    void FixedUpdate()
    {
        float lateral, vertical;

        shipMotor.setThrust(0.5f);

        if(useClassicControls || tiltJoystick == null || !tiltJoystick.enabled)
        {
            vertical = Input.GetAxis("Vertical");
            lateral = Input.GetAxis("Horizontal");
        } else
        {
            vertical = tiltJoystick.getPitch();
            lateral = tiltJoystick.getRoll();
        }

        shipMotor.setPitchAngle(vertical);
        shipMotor.setTurnAmount(lateral);
    }
}
