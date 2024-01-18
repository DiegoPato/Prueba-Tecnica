using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BridgeAnimation : MonoBehaviour
{
    private bool isActivated = false;

    public void interact()
    {
        if (!isActivated)
            transform.DOLocalMoveY(-4f, 2);
        else
            transform.DOLocalMoveY(-6f, 2);
        isActivated = !isActivated;
    }
}
