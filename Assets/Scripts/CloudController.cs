using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour {

    [SerializeField]
    private float m_FadeTime = 10;

    [SerializeField]
    private Material startSkybox;

    [SerializeField]
    private Material finalSkybox;

    private YieldInstruction m_WaitForEndOfFrameInstruction = new WaitForEndOfFrame();
    private Renderer m_Renderer;


    private void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        startSkybox = RenderSettings.skybox;

        //EventSequence.OnSequenceEnded += SequenceEnded;
    }
    // Update is called once per frame
    void Update () {
        Rotate();
	}

    public void Rotate()
    {
        transform.Rotate(0, 5 * Time.deltaTime, 0);
    }


    public void StartFadeIn() {
        StartCoroutine(FadeInClouds());

        
        


    }

    public void ChangeToDeconstructionlSkybox()
    {
        StartCoroutine(ChangeToFinalSkybox());
    }

    public void ResetSky()//SequenceEnded()
    {
        Debug.Log("resetting sky to init state");
        StartCoroutine(ChangeToStartSkybox());
    }

    private IEnumerator ChangeToStartSkybox()
    {
        StartCoroutine(FadeInClouds());
        yield return new WaitForSeconds(m_FadeTime);
        RenderSettings.skybox = startSkybox;
    }

    public IEnumerator ChangeToFinalSkybox()
    {
        yield return new WaitForSeconds(m_FadeTime);

        Debug.Log("Changing skybox to end skybox.");

        RenderSettings.skybox = finalSkybox;

        StartCoroutine(FadeOutClouds());
    }

    public void StartFadeOut() {
        StartCoroutine(FadeOutClouds());
    }

    public IEnumerator FadeOutClouds() {
        float elapsedTime = 0.0f;

        Color color = m_Renderer.material.color;
        color.a = 1;

        while (elapsedTime < m_FadeTime) {
            yield return m_WaitForEndOfFrameInstruction;
            elapsedTime += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsedTime / m_FadeTime);
            m_Renderer.material.color = color;
        }
        
    }

    public IEnumerator FadeInClouds()
    {
        float elapsedTime = 0.0f;

        Color color = m_Renderer.material.color;
        color.a = 0;

        while (elapsedTime < m_FadeTime)
        {
            yield return m_WaitForEndOfFrameInstruction;
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / m_FadeTime);
            m_Renderer.material.color = color;
        }
    }


}
