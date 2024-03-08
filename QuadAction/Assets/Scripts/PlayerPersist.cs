using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPersist : MonoBehaviour
{
    private static bool playerExists = false;
    private static bool playerStateExists = false;
    private static bool dungeonSceneManagerExists = false;

    public enum ObjectType
    {
        Player,
        PlayerState,
        DungeonSceneManager
    }

    public ObjectType objectType;

    void Awake()
    {
        switch (objectType)
        {
            case ObjectType.Player:
                if (!playerExists)
                {
                    playerExists = true;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
                break;
            case ObjectType.PlayerState:
                if (!playerStateExists)
                {
                    playerStateExists = true;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
                break;
            case ObjectType.DungeonSceneManager:
                if (!dungeonSceneManagerExists)
                {
                    dungeonSceneManagerExists = true;
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
                break;
        }
    }
}