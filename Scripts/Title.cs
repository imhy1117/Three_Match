using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Title : MonoBehaviour {

    public UILabel labTitle;
    public GameObject pfPiece;
    public PiecePool pool_piece;
    public EffectPool pool_effect;

    private List<Piece> m_listPiece = new List<Piece>();
    private const int PANEL_WIDTH = 300;
    private const int PANEL_HEIGHT = 500;
    private const int PIECE_NUM = 20;
    public const string PF_EFFECT = "Effect_piece";
    // Use this for initialization
    void Start ()
    {
        pool_piece.Generate(PIECE_NUM);
        pool_effect.Generate(PF_EFFECT, PIECE_NUM);

        StartCoroutine(BlinkTitle());
        StartCoroutine(PopPiece());
    }
    IEnumerator BlinkTitle()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            labTitle.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            labTitle.gameObject.SetActive(true);
        }
    }
    void GenPiece()
    {
        int num= (int)UnityEngine.Random.Range(1, 3);
        for(int i=0;i<num;i++)
        {
            Piece piece = pool_piece.GetPiece();
            int index = (int)UnityEngine.Random.Range(1.0f, 5.9f);

            piece.Activate();

            piece.transform.localPosition = new Vector3(UnityEngine.Random.Range(-PANEL_WIDTH, PANEL_WIDTH),
                UnityEngine.Random.Range(-PANEL_HEIGHT, PANEL_HEIGHT), 0);
            piece.Position = piece.transform.localPosition;
            piece.SetSprite(index);

            TweenScale tween = TweenScale.Begin(piece.gameObject, 0.2f, Vector3.one);
            tween.from = Vector3.zero;
            tween.style = UITweener.Style.Once;

            m_listPiece.Add(piece);
        } 
    }
    IEnumerator PopPiece()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            GenPiece();
            yield return new WaitForSeconds(0.5f);
            for(int i=0;i<m_listPiece.Count;i++)
            {
                foreach (var v in m_listPiece)
                {
                    if (v.gameObject.activeSelf)
                    {
                        float trigger = UnityEngine.Random.Range(-1.0f, 1.0f);
                        if (trigger < 0)
                        {
                            v.Explosion(pool_effect.GetEffect(PF_EFFECT));
                            m_listPiece.Remove(v);
                            break;
                        }
                    }
                }
            } 
        }
    }
    // Update is called once per frame
    void Update ()
    {
		
	}
    public void Click()
    {
        StopCoroutine(BlinkTitle());
        StopCoroutine(PopPiece());
        m_listPiece.Clear();
        SceneManager.LoadScene("Game");
    }

}
