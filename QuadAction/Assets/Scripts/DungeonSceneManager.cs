using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonSceneManager : MonoBehaviour
{
    private string[] Themes = { "Dungeon_Forrest"/*, "Dungeon_Sea", "Dungeon_Desert"*/ };
    private HashSet<GameObject> playersInPortal = new HashSet<GameObject>();

    // 던전 배열 생성 변수들
    public enum RoomType { Entrance, Trap, Benefit, Boss, Monster }
    public int width = 4;
    public int height = 4;

    private RoomType[,] dungeonMap;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersInPortal.Add(other.gameObject);

            CheckAllPlayersInPortal();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersInPortal.Remove(other.gameObject);
        }
    }

    private void CheckAllPlayersInPortal()
    {
        int totalPlayers = 1;

        if (playersInPortal.Count == totalPlayers)
        {
            EnterDungeon();
        }
    }

    public void EnterDungeon()
    {
        int index = Random.Range(0, Themes.Length);
        string selectedTheme = Themes[index];

        // 씬 전환시 지정 위치로 소환함
        Vector3 entrancePosition = new Vector3(0, 0.7f, 0);
        GlobalPlayerPosition.nextSpawnPoint = entrancePosition;

        // 던전 배열 생성
        GenerateDungeon();

        SceneManager.LoadScene(selectedTheme);
    }

    void GenerateDungeon()
    {
        dungeonMap = new RoomType[width, height];

        // 모든 방을 몬스터 방으로 초기화
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                dungeonMap[x, y] = RoomType.Monster;
            }
        }

        // 특별 방의 위치를 저장할 리스트 생성
        List<Vector2Int> specialRoomPositions = new List<Vector2Int>();

        // 특별 방 랜덤 배치 및 중복 방지
        PlaceSpecialRoom(RoomType.Entrance, specialRoomPositions);
        PlaceSpecialRoom(RoomType.Trap, specialRoomPositions);
        PlaceSpecialRoom(RoomType.Benefit, specialRoomPositions);
        PlaceSpecialRoom(RoomType.Boss, specialRoomPositions);

        // 방 출력 및 검사 (디버깅)
        for (int y = 0; y < height; y++)
        {
            string line = "";
            for (int x = 0; x < width; x++)
            {
                line += dungeonMap[x, y].ToString().Substring(0, 2) + " ";
            }
            Debug.Log(line);
        }
    }

    void PlaceSpecialRoom(RoomType roomType, List<Vector2Int> occupiedPositions)
    {
        Vector2Int position;
        do
        {
            position = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        }
        while (occupiedPositions.Contains(position));

        dungeonMap[position.x, position.y] = roomType;
        occupiedPositions.Add(position);
    }
}