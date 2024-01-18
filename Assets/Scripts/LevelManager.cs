using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; } // Singleton
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private Respawn PreviousCheckpoint; 
    [SerializeField] private PlayerMovement player;
    public PlayerMovement Player => player;

    private void turnOffPrevCheckpoint(){
        PreviousCheckpoint.setFire(false);
    }

    public void setPreviousCheckpoint(Respawn newCheckpoint)
    {
        if (PreviousCheckpoint == null)
            PreviousCheckpoint = newCheckpoint;
        else if (PreviousCheckpoint != newCheckpoint)
        {
            turnOffPrevCheckpoint();
            PreviousCheckpoint = newCheckpoint;
        }
        PreviousCheckpoint.setFire(true);
    }
}
