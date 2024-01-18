using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShaderTurnOff : MonoBehaviour
{
    public GameObject camera;
    public GameObject target;
    public LayerMask myLayerMask;

    void Update()
    {
        RaycastHit hit;
        // Does the ray intersects with the sphere?
        if (Physics.Raycast(camera.transform.position, (target.transform.position - camera.transform.position).normalized, out hit, Mathf.Infinity))
        {
            // if it collides with the sphere, scale it to 4 with Dotween
            if (hit.collider.gameObject.tag == "MaskSurface") 
            {
                transform.DOScale(4, 1);
            }
            else // if it does not collide, scale it to 0
            {
                transform.DOScale(0, 0.2f);
            }
        }
    }
}

