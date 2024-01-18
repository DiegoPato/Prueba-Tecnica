using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private GameObject fire;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.setPreviousCheckpoint(this);
            LevelManager.Instance.Player.setRespawnPosition(this.transform.position);
        }
    }

    public void setFire(bool isOn)
    {
        fire.SetActive(isOn);
    }
}
