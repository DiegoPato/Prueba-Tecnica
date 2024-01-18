using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void OnTriggerEnter(Collider other);

    void OnTriggerExit(Collider other);

    void interact();
}
