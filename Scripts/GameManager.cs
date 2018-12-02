using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class GameManager : MonoBehaviour {

    public Timer m_timer;

    public UISprite m_spriteAlert;
    public UISprite m_spritePauseButton;
    public UISprite m_spritePauseWindow;
    public UISprite m_spriteResultWindow;

    public UILabel m_labResult;
    public UILabel m_labResultScore;
    public UILabel[] m_labRank = new UILabel[3];
    public UILabel m_labScore;

  

    private const int PIECE_NUM = 49;
    private const int ELEMENT_IN_LINE = 7;
    private const int PIECE_MOVERANGE = 30;
    public const int PIECE_SIZE = 100;
    public const int PANEL_SIZE = 300;
    private const int DEFAULT_GAINSCORE = 100;
    private const int INIT = 0;
    private const int IDLE = 1;
    private const int CHECK = 2;
    private const int DESTROY = 3;
    private const int GENERATE = 4;
    private const int PAUSE = 5;
    private const int GAMEOVER = 6;
    public const string PF_EFFECT = "Effect_piece";
    //private List<Piece> m_listPiece = new List<Piece>();
    private Piece m_pPiece = null;
    private Piece[] m_pieces = new Piece[PIECE_NUM];
    
   // private List<ParticleSystem> m_listEffect=new List<ParticleSystem>();

    public PiecePool poolPiece;
    public EffectPool poolEffect;

    public float fLimitTime = 45;
    public float fPopDelay = 0.2f;
    public float fDelay = 0.5f;

    private float m_fTime = 0;
    private float m_fDelayTimer = 0;

    private int[] m_iRank=new int[3];
    private int m_iScore=0;
    private int m_iChain = 1;
    private int m_iCombo = 0;
    private int m_iProcess= INIT;
    private bool m_bClick = false;
    private bool m_bMove = true;
    private bool m_bRun = false;

    private string m_strScore;
    private Vector3 m_vecMouse;
    
    public int Process
    {
        get { return m_iProcess; }
    }
    public bool Move
    {
        set { m_bMove = value; }
        get { return m_bMove; }
    }
    public bool Click
    {
        set { m_bClick = value; }
        get { return m_bClick; }
    }
    public Piece GetPiece(int element)
    {
        return m_pieces[element];
    }
    public void SetPiece(int element,Piece piece)
    {
        m_pieces[element] = piece;
    }
    public Vector3 MousePt
    {
        set { m_vecMouse = value; }
        get { return m_vecMouse; }
    }
   
	// Use this for initialization
	void Start ()
    {
        for (int i = 0; i < 3; i++)
        {
            string key;
            key = System.String.Format("HiScore{0}", i);
            m_iRank[i]=PlayerPrefs.GetInt(key,0);
        }
        m_fTime = fLimitTime;
        m_iProcess = INIT;

        CreateObjects();

        SetPieces();

        m_spritePauseWindow.gameObject.SetActive(false);
        m_spritePauseButton.gameObject.SetActive(false);
        m_spriteResultWindow.gameObject.SetActive(false);
        StartCoroutine(StartGame());
    }

    private void SetRank()
    {
        int rank = 4;
        string alert = "순위갱신 실패...";
        for(int i=2;i>=0;i--)
        {
            if(m_iRank[i]<m_iScore)
            {
                if (i <2)
                    m_iRank[i +1] = m_iRank[i];  
                m_iRank[i] = m_iScore;
                rank = i + 1;
            }
        }

        if(rank<4)
           alert= System.String.Format("{0}위 달성!", rank);
        m_labResult.text = alert;

        for (int i=0;i<3;i++)
        {
            string key;
            key = System.String.Format("HiScore{0}", i);
            PlayerPrefs.SetInt(key,m_iRank[i]);
            key = System.String.Format("{0}",m_iRank[i]);
            m_labRank[i].text = key;
        }
    }
    public void Pause()
    {
        m_bRun = false;
        m_spritePauseButton.gameObject.SetActive(false);

        for (int i = 0; i < PIECE_NUM; i++)
        {
            m_pieces[i].gameObject.SetActive(false);
        }

        m_spritePauseWindow.gameObject.SetActive(true);
        m_timer.SetEffect(false);

        PopUp(m_spritePauseWindow.gameObject, 0, 1);
    }
    public void Continue()
    {
        for (int i = 0; i < PIECE_NUM; i++)
        {
            m_pieces[i].gameObject.SetActive(true);
        }
        m_spritePauseButton.gameObject.SetActive(true);
        m_spritePauseWindow.gameObject.SetActive(false);

        m_bRun = true;
        m_timer.SetEffect(true);
    }
    
    public void Restart()
    {
        m_spriteAlert.GetComponent<UISprite>().spriteName = "Ready";
        m_spritePauseWindow.gameObject.SetActive(false);
        m_spriteResultWindow.gameObject.SetActive(false);
        m_spriteAlert.gameObject.SetActive(true);

        m_fTime = fLimitTime;
        m_iScore = 0;
        m_iChain = 1;
        m_iCombo = 0;
        m_fDelayTimer = 0;
        m_iProcess = INIT;

        ResetPieces();

        SetPieces();

        StartCoroutine(StartGame());
    }
    IEnumerator StartGame()
    {
        m_labScore.text = "0";

        yield return new WaitForSeconds(2.0f);

        m_spriteAlert.GetComponent<UISprite>().spriteName = "Go!";

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < PIECE_NUM; i++)
        {
            m_pieces[i].Activate();
        }

        m_iProcess = IDLE;
        m_bRun = true;
        m_spriteAlert.gameObject.SetActive(false);
        m_spritePauseButton.gameObject.SetActive(true);
        m_labScore.gameObject.SetActive(true);

        m_timer.SetTimer(1);
        m_timer.SetEffect(true);
    }
    public void TimeOver()
    {
        m_bRun = false;
        m_iProcess = GAMEOVER;
        m_timer.SetEffect(false);
        m_spriteAlert.gameObject.SetActive(true);
        m_spritePauseButton.gameObject.SetActive(false);
        m_spriteAlert.GetComponent<UISprite>().spriteName = "TimeOver";

        StartCoroutine(GameOver());
    }
    private void PopUp(GameObject obj,float from,float to)
    {
        Vector3 vecFrom = new Vector3(from, from, 1);
        Vector3 vecTo = new Vector3(to, to, 1);
        TweenScale tween=TweenScale.Begin(obj, fPopDelay, vecTo);
        tween.from = vecFrom;
        tween.style= UITweener.Style.Once;
    }
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(3.0f);

        SetRank();

        for (int i = 0; i < PIECE_NUM; i++)
        {
            if(m_pieces[i]!=null)
            m_pieces[i].gameObject.SetActive(false);
        }
        m_spriteAlert.gameObject.SetActive(false);
        m_spriteResultWindow.gameObject.SetActive(true);
        m_labResultScore.text = m_strScore;

        PopUp(m_spriteResultWindow.gameObject, 0, 1);
    }
    public void GoTitle()
    {
        SceneManager.LoadScene("Title");
    }
    private void CreateObjects()
    {
        poolPiece.Generate(PIECE_NUM * 2);
        poolEffect.Generate(PF_EFFECT,PIECE_NUM);
    }
    private void SetPieces()
    {
        for (int y = 0; y < ELEMENT_IN_LINE; y++)
        {
            for (int x = 0; x < ELEMENT_IN_LINE; x++)
            {
                Piece piece = poolPiece.GetPiece();

                int index = (int)UnityEngine.Random.Range(1.0f, 5.9f);

                piece.Activate();
                piece.SetSprite(index);
                piece.transform.localPosition = new Vector3(-PANEL_SIZE + (PIECE_SIZE * x),
                PANEL_SIZE - (PIECE_SIZE * y), 0);
                piece.Position = piece.transform.localPosition;
                piece.Num = (y * ELEMENT_IN_LINE) + x;
                m_pieces[(y * ELEMENT_IN_LINE) + x] = piece;
            }   
        }
        while (true)
        {
            if (!Check())
            {
                m_iCombo = 0;
                m_iChain = 1;
                m_iScore = 0;
                m_labScore.text = "0";
                for (int i = 0; i < PIECE_NUM; i++)
                {
                    m_pieces[i].gameObject.SetActive(false);
                }
                break;
            }
        }
    }

    private void ResetPieces()
    {
        poolEffect.SetAllActive(false);
        poolEffect.SetAllActive(false);

        for (int i = 0; i < PIECE_NUM; i++)
        {
            m_pieces[i] = null;
        }
    }
    public ParticleSystem GetEffect()
    {
        return poolEffect.GetEffect(PF_EFFECT);
    }
    public bool Check()
    {
        bool value = false;
        if (m_iProcess==IDLE|| m_iProcess ==INIT)
        {
            for (int i = 0; i < PIECE_NUM; i++)
            {
                if (i %ELEMENT_IN_LINE<(ELEMENT_IN_LINE-2))
                {
                    int valueRow = 1;
                    if (CheckRow(i + 1, m_pieces[i].Index, ref valueRow))
                    {
                        value = true;
                        for (int j = 0; j < valueRow; j++)
                        {
                            m_pieces[i + j].Check = true;
                        }
                    }
                }
                if (i/ELEMENT_IN_LINE< (ELEMENT_IN_LINE - 2))
                {
                    int valueCol = 1;
                    if (CheckCol(i + ELEMENT_IN_LINE, m_pieces[i].Index, ref valueCol))
                    {
                        value = true;
                        for (int j = 0; j < valueCol; j++)
                        {
                            m_pieces[i + (j * ELEMENT_IN_LINE)].Check = true;
                        }
                    }
                }
            }
            if(m_iProcess==INIT)
            {
                DestroyPiece();
                GenPiece();
            }
            else if(m_iProcess==IDLE)
            {
                if (value)
                    m_iProcess = CHECK;
            }
        }

        return value;
    }
    public void ClickPiece(Piece piece)
    {
        m_pPiece = piece;
        m_vecMouse = Input.mousePosition;
    }
    public void MovePiece()
    {
        if(m_pPiece!=null)
        {
            if (m_bMove && m_iProcess ==IDLE)
            {
                Vector3 vecDrag = Input.mousePosition-m_vecMouse;

                if (Math.Abs(vecDrag.x) > Math.Abs(vecDrag.y))
                {
                    vecDrag.y = 0;
                    if(vecDrag.x>PIECE_MOVERANGE&&m_pPiece.Num%ELEMENT_IN_LINE<6)
                    { 
                        ExchangePiece(m_pPiece, m_pieces[m_pPiece.Num + 1]);
                        m_bMove = false;
                        if (!Check())
                        {
                            m_iCombo = 0;
                            ExchangePiece(m_pPiece, m_pieces[m_pPiece.Num - 1]);
                            m_pPiece = null;
                        }
                        else
                            m_iCombo++;
                        return;
                    }
                    if (vecDrag.x < -PIECE_MOVERANGE && m_pPiece.Num % ELEMENT_IN_LINE > 0)
                    {
                        ExchangePiece(m_pPiece, m_pieces[m_pPiece.Num - 1]);
                        m_bMove = false;
                        if (!Check())
                        {
                            m_iCombo = 0;
                            ExchangePiece(m_pPiece, m_pieces[m_pPiece.Num + 1]);
                            m_pPiece = null;
                        }
                        else
                            m_iCombo++;
                        return;
                    }
                }

                else if (Math.Abs(vecDrag.x) < Math.Abs(vecDrag.y))
                {
                    vecDrag.x = 0;
                    if (vecDrag.y >PIECE_MOVERANGE && (m_pPiece.Num / ELEMENT_IN_LINE) > 0)
                    {
                        ExchangePiece(m_pPiece, m_pieces[m_pPiece.Num - ELEMENT_IN_LINE]);
                        m_bMove = false;
                        if (!Check())
                        {
                            m_iCombo = 0;
                            ExchangePiece(m_pPiece, m_pieces[m_pPiece.Num + ELEMENT_IN_LINE]);
                            m_pPiece = null;
                        }
                        else
                            m_iCombo++;
                        return;
                    }
                    if (vecDrag.y < -PIECE_MOVERANGE&& (m_pPiece.Num / ELEMENT_IN_LINE) < ELEMENT_IN_LINE-1)
                    {
                        ExchangePiece(m_pPiece, m_pieces[m_pPiece.Num + ELEMENT_IN_LINE]);
                        m_bMove = false;
                        if (!Check())
                        {
                            m_iCombo = 0;
                            ExchangePiece(m_pPiece, m_pieces[m_pPiece.Num - ELEMENT_IN_LINE]);
                            m_pPiece = null;
                        }
                        else
                            m_iCombo++;
                        return;
                    }
                }

                Vector3 vecPos = m_pPiece.Position + vecDrag;

                if (vecPos.x < -PANEL_SIZE)
                    vecPos.x = -PIECE_MOVERANGE;
                else if (vecPos.x > PANEL_SIZE)
                    vecPos.x = PANEL_SIZE ;
                if (vecPos.y < -PANEL_SIZE)
                    vecPos.y = -PANEL_SIZE;
                else if (vecPos.y > PANEL_SIZE)
                    vecPos.y = PANEL_SIZE;

                m_pPiece.gameObject.transform.localPosition = vecPos;
            }
        }
    }
    public void ExchangePiece(Piece piece1,Piece piece2)
    {
        m_pieces[piece1.Num] = piece2;
        m_pieces[piece2.Num] = piece1;

        int tmpNum = piece1.Num;
        piece1.Num = piece2.Num;
        piece2.Num = tmpNum;

        Vector3 tmpVec = piece1.Position;
        piece1.Position =piece2.Position;
        piece2.Position = tmpVec;

        piece1.TweenMove();
        piece2.TweenMove();
    }
    private void DestroyPiece()
    {
        int iDestroyCount=0;
        for (int i = 0; i < PIECE_NUM; i++)
        {
            if (m_pieces[i].Check)
            {
                if (m_iProcess == INIT)
                {
                    m_pieces[i].gameObject.SetActive(false);
                    m_pieces[i].Check = false;
                }
                else
                {
                    m_pieces[i].Explosion(GetEffect());
                    iDestroyCount++;
                }
                m_pieces[i] = null;  
            }
        }
        if (m_iProcess !=INIT)
        {
            int gainScore = iDestroyCount * m_iCombo * m_iChain * DEFAULT_GAINSCORE;
            Vector3Int info = new Vector3Int(gainScore, m_iCombo, m_iChain);
            Debug.Log(info);
            m_iScore += gainScore;
            m_strScore = System.String.Format("{0}", m_iScore);
            m_labScore.text = m_strScore;
        }
    }

    private void GenPiece()
    {
        for(int i=PIECE_NUM-1;i>=ELEMENT_IN_LINE;i--)
        {
            if(m_pieces[i]==null)
            {
                for(int y=1;i-(y*ELEMENT_IN_LINE)>=0;y++)
                {
                    if (m_pieces[i - (ELEMENT_IN_LINE * y)]!=null)
                    {
                        m_pieces[i] = m_pieces[i - (ELEMENT_IN_LINE * y)];
                        m_pieces[i].Num = i;
                        m_pieces[i - (ELEMENT_IN_LINE * y)] = null;
                        m_pieces[i].Position = new Vector3(-PANEL_SIZE + (PIECE_SIZE * (i % ELEMENT_IN_LINE)), 
                            PANEL_SIZE - (PIECE_SIZE * (i / ELEMENT_IN_LINE)), 0);
                        if (m_iProcess == INIT)
                        {
                            m_pieces[i].transform.localPosition = m_pieces[i].Position;
                        }
                        else
                            m_pieces[i].TweenMove();
                        break;
                    }
                }       
            }
        }
        for (int x = 6; x >= 0; x--)
        {
            int col = 1;
            for (int y = 6; y >= 0; y--)
            {
                if (m_pieces[x + (y * ELEMENT_IN_LINE)] == null)
                {
                    int index = (int)UnityEngine.Random.Range(1.0f, 5.9f);
                    Piece piece = poolPiece.GetPiece();
                    if (piece != null)
                    {
                        piece.Activate();
                        piece.SetSprite(index);
                        piece.Position = new Vector3(-PANEL_SIZE + (PIECE_SIZE * x),
                        PANEL_SIZE - (PIECE_SIZE * y), 0);
                        piece.Num = (y * ELEMENT_IN_LINE) + x;
                        m_pieces[(y * ELEMENT_IN_LINE) + x] = piece;
                        if (m_iProcess == INIT)
                        {
                            piece.transform.localPosition = piece.Position;
                        }
                        else
                        {
                            piece.transform.localPosition = new Vector3(-PANEL_SIZE + (PIECE_SIZE * (x)),
                                PANEL_SIZE + (PIECE_SIZE * col), 0);
                            piece.TweenMove();
                        }
                        piece.Num = (y * ELEMENT_IN_LINE) + x;
                        m_pieces[(y * ELEMENT_IN_LINE) + x] = piece;
                    }
                    col++;
                }    
            }
        }
    }
    private bool CheckRow(int pos,int index,ref int value)
    {
        if (m_pieces[pos].Index==index)
        {
            value++;
            if (pos % ELEMENT_IN_LINE< ELEMENT_IN_LINE-1 && pos < PIECE_NUM-1)
                return CheckRow(pos + 1, index, ref value);
            else
            {
                if (value < 3)
                    return false;
                else
                    return true;
            }
        }
        else
        {
            if (value < 3)
                return false;
            else
                return true;
        }
    }
    private bool CheckCol(int pos, int index, ref int value)
    {
        if (m_pieces[pos].Index == index)
        {
            value++;
            if (pos < 42)
                return CheckCol(pos + ELEMENT_IN_LINE, index, ref value);
            else
            {
                if (value < 3)
                    return false;
                else
                    return true;
            }
        }
        else
        {
            if (value < 3)
                return false;
            else
                return true;
        }
    }
   
		// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)&&!m_bClick)
        {
            m_bClick = true;
        }
        if(Input.GetKeyUp(KeyCode.Mouse0)&&m_bClick)
        {
            if (m_pPiece != null)
                m_pPiece.TweenMove();
            m_bMove = true;
            m_bClick = false;
            m_pPiece = null;
        }
        if(m_bRun)
        {
            MovePiece();
            m_fTime -= Time.deltaTime;
            m_timer.SetTimer(m_fTime / fLimitTime);
           
            if (m_fTime<=0)
            {
                TimeOver();
            }
            if (m_iProcess != INIT && m_iProcess !=IDLE)
            {
                m_fDelayTimer += Time.deltaTime;
                if (m_iProcess == CHECK)
                {
                    if (m_fDelayTimer >= fDelay)
                    {
                        DestroyPiece();
                        m_iProcess =DESTROY;
                        m_fDelayTimer = 0;
                    }
                }
                else if (m_iProcess == DESTROY)
                {
                    if (m_fDelayTimer >= fDelay)
                    {
                        GenPiece();
                        m_iProcess = GENERATE;
                        m_fDelayTimer = 0;
                    }
                }
                else if (m_iProcess == GENERATE)
                {
                    if (m_fDelayTimer >= fDelay)
                    {
                        m_iProcess = IDLE;
                        if (!Check())
                        {
                            m_iChain = 1;
                        }
                        else
                            m_iChain++;
                        m_fDelayTimer = 0;
                    }
                }
            }
        }
    }
}
