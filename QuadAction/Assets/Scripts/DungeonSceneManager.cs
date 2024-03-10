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

    // ���� �迭 ���� ������
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
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // �ߺ� �ν��Ͻ� ����
        }
    }

    public void EnterDungeon()
    {
        int index = Random.Range(0, Themes.Length);
        string selectedTheme = Themes[index];

        // ���� �迭 ����
        GenerateDungeon();

        SceneManager.LoadScene(selectedTheme);
    }

    void GenerateDungeon()
    {
        dungeonMap = new RoomType[width, height];
        connections = new RoomConnection[width, height];

        // ��� ���� ���� ������ �ʱ�ȭ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                dungeonMap[x, y] = RoomType.Monster;
                connections[x, y] = new RoomConnection();
            }
        }

        // Ư�� ���� ��ġ�� ������ ����Ʈ ����
        List<Vector2Int> specialRoomPositions = new List<Vector2Int>();

        // Ư�� �� ���� ��ġ �� �ߺ� ����
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

        // �� ��� �� �˻� (�����)
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

        path = new List<Vector2Int>(); // Ŭ���� ������ path ����Ʈ�� �ʱ�ȭ
        path.Add(currentPos); // ����� ��ġ�� ��ο� �߰�

        while (currentPos != bossPos)
        {
            // ������ ��� ���� ���� (���⼭�� �����¿� �� �������θ� �̵�)
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

            path.Add(currentPos); // ���� ��ġ�� ��ο� �߰�
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

        // ���� ���ϴ� Ÿ���� ���� ã�� ���� ���, ������ ��ȯ�ϰų� ��ȿ���� ���� ��ġ�� ��ȯ
        Debug.LogError($"Room of type {targetRoomType} not found.");
        return new Vector2Int(-1, -1); // ��ȿ���� ���� ��ġ�� ��Ÿ���� ��
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

        // ������ ȭ��ǥ�� �����մϴ�.
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

                // �濡 �ƹ� ���ᵵ ���� ���
                if (!connection.top && !connection.right && !connection.bottom && !connection.left)
                {
                    sb.Append("None");
                }

                Debug.Log(sb.ToString());
            }
        }
    }
}