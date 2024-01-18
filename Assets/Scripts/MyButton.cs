using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MyButton : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject redButton;
    private bool isNear = false; 
    private bool isActivated = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSource2;

    [SerializeField] private BridgeAnimation[] bridgesArray;

    public void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            isNear = true;
            player.addInteractable(this);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement player))
        {
            isNear = false;
            player.removeInteractable(this);
        }
    }

    public void interact()
    {
        if (isNear && !isActivated)
        {
            isActivated = true;
            StartCoroutine(animation());
            foreach (var bridge in bridgesArray)
            {
                bridge.interact();
            }
        }
    }

    private new IEnumerator animation()
    {
        audioSource.Play();
        redButton.transform.DOLocalMoveY(0.4f, 0.5f);
        yield return new WaitForSeconds(1);     
        audioSource2.Play();
        redButton.transform.DOLocalMoveY(1.1f, 0.5f);
        yield return new WaitForSeconds(0.5f);   
        isActivated = false;
    }
}
