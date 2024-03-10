using System.Collections;
using System.Collections.Generic;
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

        // ��� ���� ���� ������ �ʱ�ȭ
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                dungeonMap[x, y] = RoomType.Monster;
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

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (dungeonMap[x, y] == RoomType.Entrance)
                {
                    // Entrance ��ġ ���, ���� ���� ���� ��ġ�� entrancePosition�� ����
                    entrancePosition = new Vector3(x * 500, 0, y * 500); // ������ 500���� �����ߴٰ� ����
                    break;
                }
            }
        }

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
}