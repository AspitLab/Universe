/********************************************************************************//**
\file      WebCam.cs
\brief     WebCam Screens
\copyright Copyright 2017 Khora VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;
using System.Collections;

public class WebCam : MonoBehaviour {

    [SerializeField]
    private int m_webCamNumber;

    private Renderer m_Renderer;

    private Color m_MaterialStartColor;

    private bool m_Active = true;

    WebCamTexture texture;


    void Start () {
        

        WebCamDevice[] devices = WebCamTexture.devices;

		// for debugging purposes, prints available devices to the console
		for(int i = 0; i < devices.Length; i++)
		{
			print("Webcam available: " + devices[i].name);
		}

		m_Renderer = this.GetComponentInChildren<Renderer>();
        m_MaterialStartColor = m_Renderer.material.GetColor("_Color");

        // assuming the first available WebCam is desired
        if (devices.Length > m_webCamNumber)
        {
            texture = new WebCamTexture(devices[m_webCamNumber].name);
            m_Renderer.material.mainTexture = texture;
            texture.Play();

            EventSequence.OnSpeechEnded += SpeechEnded;
            EventSequence.OnSequenceEnded += SequenceEnded;
        }

	}

    private void SpeechEnded()
    {
        pauseWebcam();
    }

    private void SequenceEnded()
    {
        resumeWebcam();
    }

    private void pauseWebcam()
    {
        Debug.Log("pause webcam");
        texture.Pause();
    }

    private void resumeWebcam()
    {
        Debug.Log("resume webcam");

        texture.Play();
    }


    void Update () {
        

        if (m_Active)
        {
            m_Renderer.material.SetColor("_Color", m_MaterialStartColor);
        }
        else {
            m_Renderer.material.SetColor("_Color", Color.black);
        }
	}
}
