using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    public RoomConnection[,] connections;

    List<Vector2Int> path;

    public class RoomConnection
    {
        public bool top, right, bottom, left;

        public RoomConnection()
        {
            top = right = bottom = left = false;
        }
    }

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
        connections = new RoomConnection[width, height];

        // 모든 방을 몬스터 방으로 초기화
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                dungeonMap[x, y] = RoomType.Monster;
                connections[x, y] = new RoomConnection();
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

        CreateGuaranteedPath();

        PrintPath();

        Vector2Int entrancePositionIndex = FindRoomPosition(RoomType.Entrance);
        entrancePosition = new Vector3(entrancePositionIndex.x * 500, 0, entrancePositionIndex.y * 500);

        PrintAllPortalConnections();

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
    void CreateGuaranteedPath()
    {
        Vector2Int entrancePos = FindRoomPosition(RoomType.Entrance);
        Vector2Int bossPos = FindRoomPosition(RoomType.Boss);

        Vector2Int currentPos = entrancePos;

        path = new List<Vector2Int>(); // 클래스 레벨의 path 리스트를 초기화
        path.Add(currentPos); // 입장방 위치를 경로에 추가

        while (currentPos != bossPos)
        {
            // 간단한 경로 생성 로직 (여기서는 상하좌우 한 방향으로만 이동)
            if (currentPos.x < bossPos.x)
            {
                connections[currentPos.x, currentPos.y].right = true;
                currentPos.x++;
            }
            else if (currentPos.x > bossPos.x)
            {
                connections[currentPos.x, currentPos.y].left = true;
                currentPos.x--;
            }
            else if (currentPos.y < bossPos.y)
            {
                connections[currentPos.x, currentPos.y].top = true;
                currentPos.y++;
            }
            else if (currentPos.y > bossPos.y)
            {
                connections[currentPos.x, currentPos.y].bottom = true;
                currentPos.y--;
            }

            path.Add(currentPos); // 현재 위치를 경로에 추가
        }
    }

    Vector2Int FindRoomPosition(RoomType targetRoomType)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (dungeonMap[x, y] == targetRoomType)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        // 만약 원하는 타입의 방을 찾지 못한 경우, 에러를 반환하거나 유효하지 않은 위치를 반환
        Debug.LogError($"Room of type {targetRoomType} not found.");
        return new Vector2Int(-1, -1); // 유효하지 않은 위치를 나타내는 값
    }


    void PrintPath()
    {
        if (path == null || path.Count == 0)
        {
            Debug.Log("No path data available to print.");
            return;
        }

        StringBuilder pathString = new StringBuilder("Path from Entrance to Boss: ");
        foreach (Vector2Int pos in path)
        {
            pathString.Append($"({pos.x}, {pos.y}) -> ");
        }

        // 마지막 화살표를 제거합니다.
        pathString.Remove(pathString.Length - 4, 4);

        Debug.Log(pathString.ToString());
    }

    void PrintAllPortalConnections()
    {
        StringBuilder sb = new StringBuilder();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                sb.Clear();
                RoomConnection connection = connections[x, y];
                sb.AppendFormat("Room at ({0},{1}) connections: ", x, y);

                if (connection.top) sb.Append("Top ");
                if (connection.right) sb.Append("Right ");
                if (connection.bottom) sb.Append("Bottom ");
                if (connection.left) sb.Append("Left ");

                // 방에 아무 연결도 없는 경우
                if (!connection.top && !connection.right && !connection.bottom && !connection.left)
                {
                    sb.Append("None");
                }

                Debug.Log(sb.ToString());
            }
        }
    }
}