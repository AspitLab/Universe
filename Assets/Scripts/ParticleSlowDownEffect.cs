using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSlowDownEffect : MonoBehaviour {

    [Range(0,1f)]
    public float m_time;

    public GameObject particles;

    private ParticleSystem[] m_ParticleSystems;


    public float lightIntensity = 0;

    public Light blowLight;

	void Start () {
        m_ParticleSystems = particles.GetComponentsInChildren<ParticleSystem>();
        
    }
	
	void Update () {
        for (int i = 0; i < m_ParticleSystems.Length; i++)
        {
            ParticleSystem.MainModule m = m_ParticleSystems[i].main;
            m.simulationSpeed = m_time;
        }

        blowLight.intensity = lightIntensity;

    }
}
