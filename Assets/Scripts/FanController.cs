using UnityEngine;
using System.Collections;
/********************************************************************************//**
\file      FanController.cs
\brief     Serial COMM connection
\copyright Copyright 2017 Khora VR, LLC All Rights reserved.
************************************************************************************/

public class FanController : MonoBehaviour {

    //==============================================================================
    // Fields
    //==============================================================================

    private bool m_FanSwith = false;

    [SerializeField]
    private SerialConnection m_SerialConnection;

    [SerializeField]
    private AudioSource m_SwitchSound;

    [SerializeField]
    private float m_StartSpeed = 4;

    private float m_Speed;

    private AudioSource m_AudioSource;

    [SerializeField]
    private Transform windZone;

    //==============================================================================
    // MonoBehaviour
    //==============================================================================

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_FanSwith = !m_FanSwith;

            // is the fan on
            /*if (m_SerialConnection != null) { 
                if (m_FanSwith){
                    m_SerialConnection.PrepareAndSendCommand("ON", 1);
                }
                else {
                    m_SerialConnection.PrepareAndSendCommand("OFF", 1);
                }
            }*/
        }

        if (m_FanSwith)
        {
            m_Speed = m_StartSpeed;
        }
        else {
            m_Speed = 0;
        }

        Rotate();
    }

    public void FanSwitch(bool b ) {
        m_FanSwith = b;
        if (b == true)
        {
            m_SwitchSound.Play();
            m_AudioSource.Play();
            windZone.gameObject.SetActive(true);
        }
        else { 
            m_SwitchSound.Play();
            m_AudioSource.Stop();
            windZone.gameObject.SetActive(false);
        }
    }
    

    public void Rotate()
    {
        transform.Rotate(0, m_Speed * Time.deltaTime, 0);
    }
}
