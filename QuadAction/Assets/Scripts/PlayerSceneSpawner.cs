using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSceneSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        SpawnPlayerPosition(GlobalPlayerPosition.nextSpawnPoint);  
    }

    void SpawnPlayerPosition(Vector3 position)
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
        {
            Player.transform.position = position;
        }
        else
        {
            Debug.LogError("Player object not found in the scene.");
        }
    }
}
