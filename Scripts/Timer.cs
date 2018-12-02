using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    // Use this for initialization
    public UIProgressBar m_ProgressBar;
    public ParticleSystem pfSpark;

    private int m_iTimerWidth;
    private ParticleSystem m_particle;
    private Vector3 m_vecSparkPos;
   
    void Start ()
    { 
        m_iTimerWidth = m_ProgressBar.foregroundWidget.width;
        m_ProgressBar.value = 1;
        m_vecSparkPos = new Vector3(m_iTimerWidth, 0);
        m_particle = Instantiate<ParticleSystem>(pfSpark);
        m_particle.transform.SetParent(m_ProgressBar.transform);
        m_particle.transform.localScale = new Vector3(0.2f, 0.2f, 1);
        m_particle.transform.localPosition = m_vecSparkPos;
        m_particle.gameObject.SetActive(false);
    }
    public void SetTimer(float value)
    {
        m_ProgressBar.value = value;
        m_vecSparkPos.x = m_iTimerWidth * value;
        m_particle.transform.localPosition = m_vecSparkPos;
    }
    public void SetEffect(bool value)
    {
        if (value)
            m_particle.gameObject.SetActive(true);
        else
            m_particle.gameObject.SetActive(false);
    }
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        Destroy(m_particle);
    }
}
