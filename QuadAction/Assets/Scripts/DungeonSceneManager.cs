using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonSceneManager : MonoBehaviour
{
    public static DungeonSceneManager Instance { get; private set; }

    private string[] Themes = { "Dungeon_Forrest"/*, "Dungeon_Sea", "Dungeon_Desert"*/ };
    private HashSet<GameObject> playersInPortal = new HashSet<GameObject>();

    // 던전 배열 생성 변수들
    public enum RoomType { Entrance, Trap, Benefit, Shop, Boss, Monster }
    public int width = 4;
    public int height = 4;

    public RoomType[,] dungeonMap;
    public Vector3 entrancePosition;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }


    public void EnterDungeon()
    {
        int index = Random.Range(0, Themes.Length);
        string selectedTheme = Themes[index];

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
        PlaceSpecialRoom(RoomType.Shop, specialRoomPositions);
        PlaceSpecialRoom(RoomType.Boss, specialRoomPositions);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (dungeonMap[x, y] == RoomType.Entrance)
                {
                    // Entrance 위치 계산, 방의 실제 월드 위치를 entrancePosition에 저장
                    entrancePosition = new Vector3(x * 500, 0, y * 500); // 간격을 500으로 설정했다고 가정
                    break;
                }
            }
        }

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