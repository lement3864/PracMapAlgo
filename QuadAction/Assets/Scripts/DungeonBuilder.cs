using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DungeonSceneManager;

public class DungeonBuilder : MonoBehaviour
{
    public GameObject entrancePrefab;
    public GameObject bossPrefab;
    public GameObject monsterPrefab;
    public GameObject benefitPrefab;
    public GameObject shopPrefab;
    public GameObject trapPrefab;

    void Start()
    {
        if (DungeonSceneManager.Instance == null)
        {
            Debug.LogError("DungeonSceneManager.Instance is null.");
            return;
        }

        BuildDungeon();
        MovePlayerToEntrance();
    }

    void BuildDungeon()
    {
        RoomType[,] dungeonMap = DungeonSceneManager.Instance.dungeonMap;

        for (int x = 0; x < dungeonMap.GetLength(0); x++)
        {
            for (int y = 0; y < dungeonMap.GetLength(1); y++)
            {
                Vector3 position = new Vector3(x * 500, 0, y * 500); // 각 방의 위치
                RoomType roomType = dungeonMap[x, y];

                GameObject toInstantiate = null;

                switch (roomType)
                {
                    case RoomType.Entrance:
                        toInstantiate = entrancePrefab;
                        break;
                    case RoomType.Boss:
                        toInstantiate = bossPrefab;
                        break;
                    case RoomType.Monster:
                        toInstantiate = monsterPrefab;
                        break;
                    case RoomType.Benefit: 
                        toInstantiate = benefitPrefab;
                        break;
                    case RoomType.Shop: 
                        toInstantiate = shopPrefab;
                        break;
                    case RoomType.Trap:
                        toInstantiate = trapPrefab;
                        break;
                }

                if (toInstantiate != null)
                {
                    Instantiate(toInstantiate, position, Quaternion.identity);
                }
            }
        }
    }

    void MovePlayerToEntrance()
    {
        // 플레이어를 Entrance 위치로 이동
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = DungeonSceneManager.Instance.entrancePosition;
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }
}
