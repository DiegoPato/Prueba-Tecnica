using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public delegate void coinCollected();
    public coinCollected onCoinCollected;

    private AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Coin"))
        {
            onCoinCollected?.Invoke();
            SetObjectInvisibleAndWithoutCollisions(other);
            audioSource = other.GetComponent<AudioSource>();

            if(audioSource != null)
            {
                audioSource.Play();
                StartCoroutine(PlaySound(other));
            }
        }
    }

    private void SetObjectInvisibleAndWithoutCollisions(Collider other)
    {
        // Disable renderer to make the object invisible
        MeshRenderer renderer = other.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        // Disable collider to make the object without collisions
        MeshCollider collider = other.GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
    }

    private IEnumerator PlaySound(Collider other)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(other.gameObject);
    }
}
