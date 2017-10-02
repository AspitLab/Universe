using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadActiveArea : MonoBehaviour {

    //private bool mustReEnter = false;
    private bool mustReset = false;
    [SerializeField]
    private bool isUserTouching = false;
    private bool isUserTouching_prev = false;

    [SerializeField]
    private Transform m_Trigger;

    [SerializeField]
    private EventSequence m_EventSequence;

    private Coroutine eventPauserCoroutine;


    void Start()
    {
        EventSequence.OnSequenceEnded += SequenceEnded;
    }


    private void Update()
    {

        if(isUserTouching_prev == false && isUserTouching == true)
        {
            OnTouchEnter();
        }
        else
        if (isUserTouching_prev == true && isUserTouching == false)
        {
            OnTouchExit();
        }
        else
        if (isUserTouching)
        {
            OnUserTouchingSurface();
        }

        isUserTouching_prev = isUserTouching;
    }

    public void SetUserTableTouchState(bool isUserTouching)
    {
        this.isUserTouching = isUserTouching;
    }

    private void OnTriggerStay(Collider other)
    {
        /*
        if (m_EventSequence.IsActive || mustReEnter)
            return;

        if (other.transform == m_Trigger.transform)
        {
            m_EventSequence.StartSequence();
        }
        */

        if (other.transform == m_Trigger.transform)
        {
            SetUserTableTouchState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == m_Trigger.transform)
        {
            SetUserTableTouchState(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.transform == m_Trigger.transform)
        {
            mustReEnter = false;
        }*/
    }


    private void OnTouchEnter()
    {
        if (eventPauserCoroutine != null)
        {
            StopCoroutine(eventPauserCoroutine);
            Debug.Log("Coroutine stopped!");
        }

       
        m_EventSequence.ResumeSequence();
        Debug.Log("OnTouchEnter");

    }

    private void OnTouchExit()
    {
        //if (mustReEnter == false)
        //{
            //pause animation
            m_EventSequence.PauseSequence();

            if (eventPauserCoroutine != null)
            {
                StopCoroutine(eventPauserCoroutine);
                Debug.Log("Coroutine stopped!");
            }
            eventPauserCoroutine = StartCoroutine(waitForReset(m_EventSequence.pauseTimeout));

            
        //}
        //else
        //    mustReEnter = false;
        Debug.Log("OnTouchExit");
    }

    private IEnumerator waitForReset(float time)
    {
        Debug.Log("Waiting for "+ time);
        yield return new WaitForSeconds(time);
        
        Debug.Log("Resetting Sequence");
        mustReset = false;
        m_EventSequence.ResetSequence();

       
    }

    private IEnumerator waitForStart(float time)
    {
        Debug.Log("Waiting for " + time);
        yield return new WaitForSeconds(time);

        Debug.Log("Restarting Sequence");
        mustReset = false;

    }

    private void OnUserTouchingSurface()
    {
        
        if (m_EventSequence.IsActive || mustReset)//|| mustReEnter
            return;

        if(eventPauserCoroutine != null)
        {
            StopCoroutine(eventPauserCoroutine);
            Debug.Log("Pauser Coroutine stopped.");
           
        }
           
        m_EventSequence.StartSequence();
        
    }

    private void SequenceEnded()
    {
        Debug.Log("OnSequenceEnded");
        //mustReEnter = true;
        mustReset = true;
        StartCoroutine(waitForStart(m_EventSequence.pauseTimeout));
    }

}
