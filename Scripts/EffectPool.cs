using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : ObjectPool
{
    // Use this for initialization

    public override void InitObj(GameObject obj)
    {
        base.InitObj(obj);
        obj.transform.localPosition = new Vector3(-GameManager.PANEL_SIZE * 2,
            GameManager.PIECE_SIZE * 2, 0);
        obj.transform.localScale = new Vector3(0.2f, 0.2f, 1);
    }
    public ParticleSystem GetEffect(string strObject)
    {
        ParticleSystem effect= GetObject(strObject).GetComponent<ParticleSystem>();

        return effect;
    }
	// Update is called once per frame

}
