/********************************************************************************//**
\file      CameraPositionCalibrate.cs
\brief     Move and store the parent of the camera (used to align to realworld)
\copyright Copyright 2017 Khora VR, LLC All Rights reserved.
************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionCalibrate : MonoBehaviour {

    //==============================================================================
    // Fields
    //==============================================================================

    private float m_x;
    private float m_y;
    private float m_z;

    private KeyCode x_inc = KeyCode.W;
    private KeyCode x_dec = KeyCode.S;

    private KeyCode z_inc = KeyCode.A;
    private KeyCode z_dec = KeyCode.D;

    private float m_IncAmount = 0.05f;

    //==============================================================================
    // MonoBehaviour
    //==============================================================================

    //==============================================================================
    private void Start () {
        m_x = PlayerPrefs.GetFloat("x");
        m_y = PlayerPrefs.GetFloat("y");
        m_z = PlayerPrefs.GetFloat("z");
        GetStoredPosition();

    }

    //==============================================================================
    private void Update () {
        if (!Input.anyKeyDown)
            return;

        Vector3 newPosition = this.transform.position;

        if (Input.GetKeyDown(x_inc)){
            newPosition.x += m_IncAmount;
        }

        if (Input.GetKeyDown(x_dec))
        {
            newPosition.x -= m_IncAmount;
        }

        if (Input.GetKeyDown(z_inc))
        {
            newPosition.z += m_IncAmount;
        }

        if (Input.GetKeyDown(z_dec))
        {
            newPosition.z -= m_IncAmount;
        }

        SetPosition(newPosition);
        StoreCurrentPosition();

    }

    //==============================================================================
    // Private
    //==============================================================================

    //==============================================================================
    private void SetPosition(Vector3 position) {
        this.transform.position = position;
    }

    //==============================================================================
    private void StoreCurrentPosition() {
        m_x = this.transform.position.x;
        m_z = this.transform.position.z;
    }

    //==============================================================================
    private void GetStoredPosition() {
        this.transform.position = new Vector3(m_x, m_y, m_z);
        }

    //==============================================================================
    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("x", m_x);
        PlayerPrefs.SetFloat("y", m_y);
        PlayerPrefs.SetFloat("z", m_z);
    }
}
