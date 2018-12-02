using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Piece : MonoBehaviour {
    public UIButton Button;
    public UISprite Sprite;
    private ParticleSystem m_particle=null;

    public float fTweenDelay=0.5f;
    public float fEffectZPos = 2;

    private bool m_bCheck = false;
    private const int INIT = 0;
    private const int IDLE = 1;
    private int m_iIndex = 0;
    private int m_iNum = 0;
    private Vector3 m_vecPos;
    private Vector3 m_vecDrag = new Vector3(0, 0, 0);
    private GameManager m_gameManager;

    void Start()
    {
        if(SceneManager.GetActiveScene().name=="Game")
        m_gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {

    }

    public int Num
    {
        set { m_iNum = value; }
        get { return m_iNum; }
    }

    public Vector3 Position
    {
        set { m_vecPos = value; }
        get { return m_vecPos; }
    }

    public bool Check
    {
        set { m_bCheck = value; }
        get { return m_bCheck; }
    }

    public int Index
    {
        set { m_iIndex = value; }
        get { return m_iIndex; }
    }
  
	
    public void Click()
    {
        if (m_gameManager.Move && m_gameManager.Process == IDLE)
        {
            m_gameManager.ClickPiece(this);
        }
    }
    
    public void TweenMove()
    {
        TweenPosition tween = TweenPosition.Begin(gameObject, fTweenDelay, m_vecPos);
        tween.style = UITweener.Style.Once;
    }

    public Vector3 GetDistance()
    {
        return m_vecDrag;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        Sprite.enabled = true;
    }
   
    public void Explosion(ParticleSystem effect)
    {
        if(this.gameObject.activeSelf)
        {
            m_bCheck = false;
            Sprite.enabled = false;
            Vector3 vecEffect = this.Position;
            vecEffect.z = fEffectZPos;
            m_particle = effect;
            if(m_particle!=null)
            {
                m_particle.transform.localPosition = vecEffect;
                m_particle.gameObject.SetActive(true);
                m_particle.Play();
            }
            StartCoroutine("DestroyPiece");
        }
    }

    IEnumerator DestroyPiece()
    {
        yield return new WaitForSeconds(0.3f);
        if (m_particle != null)
        {
            m_particle.gameObject.SetActive(false);
            m_particle = null;
        }
        gameObject.SetActive(false);
    }

    public void SetSprite(int index)
    {
        m_iIndex = index;
        switch (index)
        {
            case 1:
                Sprite.spriteName = "circle";
                Button.normalSprite = "circle";
                break;
            case 2:
               Sprite.spriteName = "heart";
                Button.normalSprite = "heart";
                break;
            case 3:
               Sprite.spriteName = "ice";
                Button.normalSprite = "ice";
                break;
            case 4:
               Sprite.spriteName = "moon";
                Button.normalSprite = "moon";
                break;
            case 5:
                Sprite.spriteName = "sun";
                Button.normalSprite = "sun";
                break;
        }
    }

    private void OnDestroy()
    {
        Destroy(m_particle);
    }
}
