using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePool : ObjectPool {

    // Use this for initialization
    private const string PF_PIECE = "Piece";
    public override void InitObj(GameObject obj)
    {
        base.InitObj(obj);

        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = new Vector3(-GameManager.PANEL_SIZE * 2,
        GameManager.PIECE_SIZE * 2, 0);
        Piece piece = obj.GetComponent<Piece>();
        piece.Position = Vector3.zero;

    }
    public void Generate(int num)
    {
        base.Generate(PF_PIECE,num);
    }
    public Piece GetPiece()
    {
        Piece piece = GetObject(PF_PIECE).GetComponent<Piece>();
        return piece;
    }

}
