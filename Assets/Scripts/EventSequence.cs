using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSequence : MonoBehaviour {

    public float pauseTimeout = 15; 

    public delegate void SequenceEndedAction();
    public static event SequenceEndedAction OnSequenceEnded;

    public delegate void SpeechEndedAction();
    public static event SpeechEndedAction OnSpeechEnded;

    [SerializeField]
    private SerialConnection m_SerialConnection;

    [SerializeField]
    private FanController m_FanController;

    [SerializeField]
    private Renderer m_Indicator; 

    private Animator m_Animator;
    public Animator m_DeconstructionAnimator;

    [SerializeField]
    private AudioClip m_StartBeep;

    [SerializeField]
    private AudioSource m_ZenMusicAudioSource;

    [SerializeField]
    private ParticleSystem[] m_ParticleSystems;

    [SerializeField]
    private AudioSource m_WelcomeSpeech;

    private AudioSource m_AudioSource;

    [SerializeField]
    private AudioSource m_RoomBreakAudioSource;

    [SerializeField]
    private CloudController m_CloudController;

    [SerializeField]
    private bool m_IsActive = false;

    public bool IsActive { get { return m_IsActive; } }



    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Indicator.material.color = Color.red;
    }

    public void StartSequence() {
        Debug.Log("Start Sequence, m_IsActive: "+ m_IsActive);
        if (m_IsActive == false)
        {

            m_IsActive = true;
            m_Indicator.material.color = Color.green;
            m_Animator.speed = 1;
            m_Animator.SetTrigger("Start");

            // play beep sound
            m_AudioSource.clip = m_StartBeep;
            m_AudioSource.Play();
        }
    }

	public void ResumeSequence()
	{
		if (m_IsActive == false)
			return;
			
		m_Indicator.material.color = Color.green;
		m_Animator.speed = 1;
		m_AudioSource.UnPause();

        m_ZenMusicAudioSource.UnPause();
        m_WelcomeSpeech.UnPause();

        m_DeconstructionAnimator.speed = 1;

    }

    public void PauseSequence()
    {
        m_Indicator.material.color = Color.yellow;
        m_Animator.speed = 0;
		m_AudioSource.Pause();
        m_ZenMusicAudioSource.Pause();
        m_WelcomeSpeech.Pause();

        m_DeconstructionAnimator.speed = 0;
    }

    public void PlayZenMusic()
    {
        m_ZenMusicAudioSource.Play();
        Debug.Log("m_ZenMusicAudioSource");
    }

    public void PlayWelcomeSpeech()
    {
        m_WelcomeSpeech.Play();
        
    }

    public void ResetSequence()
    {
        FanOff();
        SetInactive();
        LightOff();
        m_Animator.speed = 0;
        m_Animator.StopPlayback();
        m_Animator.CrossFade("New State",0);
        
    }

    public void LightsOn(){
        m_CloudController.StartFadeOut();
        // send message to IOboard
        m_SerialConnection.PrepareAndSendRelCommand("ON", 4);
        Debug.Log("Lights On");
    }

    public void LightOff() {
        m_CloudController.StartFadeIn();
        // send message to IOboard
        m_SerialConnection.PrepareAndSendRelCommand("Off", 4);
        Debug.Log("Lights Off");
    }

    public void StartRoomDeconstructionSkybox()
    {
        m_CloudController.ChangeToDeconstructionlSkybox();
        
    }

    public void StartRoomDeconstructionAnimation()
    {
        m_RoomBreakAudioSource.Play();
        m_DeconstructionAnimator.SetTrigger("Start");

        for (int i = 0; i < m_ParticleSystems.Length; i++)
        {
            m_ParticleSystems[i].Play();
        }
    }

    public void StopParticleSystems() {
        for (int i = 0; i < m_ParticleSystems.Length; i++)
        {
            m_ParticleSystems[i].Stop();
        }
    }



    public void FanOn() {
        // send message to fan animation
        m_FanController.FanSwitch(true);
        // send message to IOboard
        m_SerialConnection.PrepareAndSendRelCommand("ON", 3);
        Debug.Log("Fan On");
    }

    public void FanOff() {
        // send message to fan animation
        m_FanController.FanSwitch(false);
        // send message to IOboard
        m_SerialConnection.PrepareAndSendRelCommand("Off", 3);
        Debug.Log("Fan Off");
    }

    private void SetInactive() {
        Debug.Log("SetInactive, isActive == false");
        m_IsActive = false;
        m_Indicator.material.color = Color.red;
        Debug.Log("EventSequence: OnSequenceEnded");
        OnSequenceEnded();
    }

    private void resetSky()
    {
        m_CloudController.ResetSky();
    }

    public void SetSpeechEnded()
    {
        OnSpeechEnded();
    }
}
