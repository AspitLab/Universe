/********************************************************************************//**
\file      SerialConnection.cs
\brief     Serial COMM connection
\copyright Copyright 2016 Khora VR, LLC All Rights reserved.
************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using System;

public class SerialConnection : MonoBehaviour {

    //==============================================================================
    // Fields
    //==============================================================================

 

    private SerialPort m_Serial;

    [SerializeField]
    HeadActiveArea animationEventController;

	[SerializeField]
	private string m_PortName = "COM8";//"COM4";

    [SerializeField]
    private int m_BaudRate = 115200;  // 9600, 19200, 38400, 57600, 14400, 28800, 56000, 115200

    [SerializeField]
    private Parity m_Parity = Parity.None;

    [SerializeField]
    private int m_DataBits = 8;

    [SerializeField]
    private StopBits m_StopBits = StopBits.One;

    //[SerializeField]
    private float secondsToWaitBetweenSerialPolling = 0.1f;//100ms
    private float lastTimePolled;

    private string _latestResponse = "";
    private string latestResponse { get { return _latestResponse; } set { _latestResponse = value; } }
    float handChannel1 = 0;
    float handChannel2 = 0;
    private int hand1ResponseCounter = 0;
    private int hand2ResponseCounter = 0;
    private int totalDebounce = 4;

    //==============================================================================
    // MonoBehaviour
    //==============================================================================

    //==============================================================================
    private void Awake () {
        m_Serial = new SerialPort();

        m_Serial.PortName   = m_PortName;   // COM4
        m_Serial.Parity     = m_Parity;     // Parity.None
        m_Serial.BaudRate   = m_BaudRate;   // 115200
        m_Serial.DataBits   = m_DataBits;   // 8
        m_Serial.StopBits   = m_StopBits;   // StopBits.One

        // debug portnames
        string[] portNames = SerialPort.GetPortNames();

        for (int i = 0; i < portNames.Length; i++) {
            Debug.Log("Available PortNames: " + portNames[i]);
        }
    }

    private void Start()
    {
        try
        {
            m_Serial.Open();
            m_Serial.NewLine = "\r\n";
        }
        catch (Exception e)
        {
            Debug.LogWarning("Could not open serial port " + m_PortName + ". Access is denied. Maybe the device al;ready has an open connection and needs to be unplugged and plugged in again.");
        }

        startBoardCommand();
    }

    void OnApplicationQuit()
    {
        stopBoardCommand();
    }

    //==============================================================================
    // RELx.ON
    // RELx.OFF
    // RELx.TOGGLE
    // RELx.GET
    // 
    // RELS.ON
    // RELS.OFF
    // RELS.GET
    // 
    // CHx.ON
    // CHx.OFF
    // CHx.TOGGLE
    // CHx.GET
    // CHx.SETMODE
    // CHx.GETANALOG
    // CHx.GETTEMP
    // 
    // CHS.ON
    // CHS.OFF
    // CHS.GET
    // CHS.SETTEMPRES
    // 
    // SETID
    // ABOUT
    //==============================================================================

    //this function is called by Unity every frame (90FPS on a vive)
    private void Update()
    {
        // I turned off the serial button based input. using head collider instead
        /*
        if (Time.time - lastTimePolled > secondsToWaitBetweenSerialPolling)
        {

            PrepareAndSendGetAllChannelsCommand();

            // note: the latestResponse value is asynchronously populated. IE not necessarily populated right away from the function call above
            // Debug.Log("val " + latestResponse);
            processCHSResponseForHands();

            lastTimePolled = Time.time;
        }
        */
    }

    public void processCHSResponseForHands()
    {
        //Debug.Log(latestResponse + "; len: " + latestResponse.Length);
        // The response we're looking for is like this: 0 0 0 0 0 0. Length 11

        float channel1 = -1;
        float channel2 = -1;

        if (latestResponse.Length == 11)
        {
            char[] chars = latestResponse.ToCharArray();
            
            float.TryParse(chars[0].ToString(), out channel1);
            float.TryParse(chars[2].ToString(), out channel2);

            if(channel1 > -1 )//&& channel2 > -1)
            {
                handChannel1 += channel1;
                hand1ResponseCounter++;
                handChannel2 += channel2;
                hand2ResponseCounter++;
            }

            if (hand1ResponseCounter >= totalDebounce )//&& hand2ResponseCounter >= totalDebounce)
            {
                channel1 = handChannel1 / totalDebounce;
                channel2 = handChannel2 / totalDebounce;

                handChannel1 = 0;
                hand1ResponseCounter = 0;
                handChannel2 = 0;
                hand2ResponseCounter = 0;
                
            }

        }

        if (channel1 == 1 )//&& channel2 == 1)
        {
            // start or resume animation
            animationEventController.SetUserTableTouchState(true);
        }
        else if (channel1 == 0 )//&& channel2 == 0)
        {
            // pause animation and queue auto stop in 15s
            animationEventController.SetUserTableTouchState(false);
        }

        // Force auto run (ignore hand sensors)
        //animationEventController.SetUserTableTouchState(true);
    }

    public void startBoardCommand()
    {
        string s = "\r\n";
        StartCoroutine(SendCommand(s));

        PrepareAndSendRelCommand("OFF", 1);
        PrepareAndSendRelCommand("OFF", 2);
        PrepareAndSendRelCommand("OFF", 3);
        PrepareAndSendRelCommand("OFF", 4);

        // CH1.SETMODE(2)
        // set these channels to input, to read if the hands are on the table sensor
        PrepareAndSendChannelCommand("SETMODE(2)", 1);
        PrepareAndSendChannelCommand("SETMODE(2)", 2);
    }

    public void stopBoardCommand()
    {   
        PrepareAndSendRelCommand("OFF",1);
        PrepareAndSendRelCommand("OFF",2);
        PrepareAndSendRelCommand("OFF",3);
        PrepareAndSendRelCommand("OFF",4);
        m_Serial.Close();
    }

    public void PrepareAndSendRelCommand(string command, int channel){
        string s = "REL" + channel.ToString() + "." + command + "\r\n"; 
        
        StartCoroutine(SendCommand(s));
    }

    public void PrepareAndSendChannelCommand(string command, int channel)
    {
        string s = "CH" + channel.ToString() + "." + command + "\r\n";

        StartCoroutine(SendCommand(s));
    }

    public void PrepareAndSendGetAllChannelsCommand()
    {
        string s = "CHS.GET" + "\r\n";

        StartCoroutine(SendCommandWithResponse(s));
    }

    //==============================================================================
    private IEnumerator SendCommand(string command)
    {

        //Debug.Log("Trying to send:" + command + " : | PortName: " + m_Serial.PortName + " | Parity: " + m_Serial.Parity + " | BaudRate: " + m_Serial.BaudRate + " | DataBits: " + m_Serial.DataBits + " | StopBits: " + m_Serial.StopBits);

        // wait until serial connection is open
        while (!m_Serial.IsOpen) {
            yield return null;
        }

        Debug.Log(command + " : Has Been Send | PortName: " + m_Serial.PortName + " | Parity: " + m_Serial.Parity + " | BaudRate: " + m_Serial.BaudRate + " | DataBits: " + m_Serial.DataBits + " | StopBits: " + m_Serial.StopBits);
        m_Serial.Write(command);    // REL1.ON<CR><LF> or REL1.OFF<CR><LF>
    }

    private IEnumerator SendCommandWithResponse(string command)
    {

        //Debug.Log("Trying to send:" + command + " : | PortName: " + m_Serial.PortName + " | Parity: " + m_Serial.Parity + " | BaudRate: " + m_Serial.BaudRate + " | DataBits: " + m_Serial.DataBits + " | StopBits: " + m_Serial.StopBits);

        // wait until serial connection is open
        while (!m_Serial.IsOpen)
        {
            yield return null;
        }

        //Debug.Log(command + " : Has Been Send | PortName: " + m_Serial.PortName + " | Parity: " + m_Serial.Parity + " | BaudRate: " + m_Serial.BaudRate + " | DataBits: " + m_Serial.DataBits + " | StopBits: " + m_Serial.StopBits);
        m_Serial.Write(command);    // REL1.ON<CR><LF> or REL1.OFF<CR><LF>
        latestResponse = m_Serial.ReadLine();
        //Debug.Log("val "+latestResponse);

    }
}
