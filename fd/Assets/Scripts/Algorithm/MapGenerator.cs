using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room
{
    public bool isConnected = false;
    public bool isItemRoom;
    public Vector2Int coord; //grid coord;
    //사이즈의 기준은 왼쪽 위 부터 오른쪽 아래로
    public Vector2Int size;//벽 포함
    public Vector2Int baseCoord;
    public Vector2Int center;
}
public class MapGenerator : MonoBehaviour
{
    public Transform player;
    public Tile floorTile;
    public Tile wallTile;
    public Tile bushTile;
    public Tile lockedDoorTile;
    public Tile stairTile;
    public Tilemap floor;
    public Tilemap wall;
    public GameObject stair;
    public GameObject mobSpawner;
    public GameObject mobSpawners;
    public Astar astar;
    public GameManager gm;
    public TilemapCollider2D wallcol;
    void Start()
    {
        //시작 시 gm으로부터 
        //gm astar player 객체를 받음
    }

    public void DebugForceRegen()
    {
        GenerateFloor();
    }
    public void GenerateFloor()
    {
        var nowFloor = gm.nowFloor > 20 ? 20 : gm.nowFloor;
        nowFloor = gm.nowFloor;
        ClearFloor();
        int mapCount = 3 + nowFloor;
        int increaseRoomSize = nowFloor / 4;
        increaseRoomSize = 0;
        var minGridSize = 2;
        //그리드에 모든 방이 들어가도록 만듬 (그리드 = 방수 X 더 크게해서 빈 여백 만들어야 함)
        while (minGridSize * minGridSize <= mapCount) minGridSize++;

        Debug.Log(mapCount.ToString());
        bool[,] grid = new bool[minGridSize, minGridSize];
        var rooms = new List<Room>();

        //그리드 채우기
        for (int i = 0; i < minGridSize; i++)
        {
            for (int j = 0; j < minGridSize; j++)
            {
                var room = new Room();
                room.coord = new Vector2Int(i, j);
                room.size = new Vector2Int(Random.Range(4, 6 + 1 + increaseRoomSize), Random.Range(4, 6 + 1 + increaseRoomSize));
                rooms.Add(room);
            }
        }

        //그리드에서 방 개수에 맞을 때 까지 방 줄이기
        while (rooms.Count > mapCount) rooms.RemoveAt(Random.Range(0, rooms.Count));

        //방 그리기
        for (int i = 0; i < rooms.Count; i++)
        {
            var xbase = rooms[i].coord.x * 8 + increaseRoomSize + (8 + increaseRoomSize - rooms[i].size.x) / 2;
            var ybase = rooms[i].coord.y * 8 + increaseRoomSize + (8 + increaseRoomSize - rooms[i].size.y) / 2;
            rooms[i].baseCoord = new Vector2Int(xbase, ybase);
            rooms[i].center = new Vector2Int(xbase + ((rooms[i].size.x) / 2),
                                            ybase + ((rooms[i].size.y) / 2));
            for (int x = xbase; x < xbase + rooms[i].size.x; x++)
            {
                for (int y = ybase; y < ybase + rooms[i].size.y; y++)
                {
                    floor.SetTile(new Vector3Int(x, y, 0), floorTile);
                }
            }
        }

        var startRoomIndex = Random.Range(0, rooms.Count);
        //방 연결하기
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            ConnectRoom(rooms[i].center, rooms[i + 1].center);
        }

        //20%의 방에 길 더 만들어주기
        for (int i = 0; i < (rooms.Count - 1 / 5); i++)
        {
            ConnectRoom(rooms[Random.Range(0, rooms.Count)].center, rooms[Random.Range(0, rooms.Count)].center);
        }

        //벽 생성하기
        wallcol.enabled = false;
        CreateWall();
        wallcol.enabled = true;

        //플레이어 스폰하기
        player.transform.position = new Vector3(rooms[startRoomIndex].center.x, rooms[startRoomIndex].center.y, 0);

        var itemAndMobCount = Mathf.RoundToInt((float)mapCount * 2 / 3);
        //몹 스포너 설치하기 플레이어 스폰지점 제외
        //6층까지는 한방에 몬스터 하나 이후로는 랜덤으로 두마리
        var roomIndexList = new List<int>();
        for (var i = 0; i < rooms.Count; i++)
        {
            roomIndexList.Add(i);
        }
        roomIndexList.RemoveAt(startRoomIndex);
        //shuffle
        for (var i = 0; i < roomIndexList.Count; i++)
        {
            var temp = roomIndexList[i];
            var rand1 = Random.Range(0, roomIndexList.Count);
            roomIndexList[i] = roomIndexList[rand1];
            roomIndexList[rand1] = temp;
        }
        //set
        for (int i = 0; i < roomIndexList.Count; i++)
        {
            var nowRoomsCenter = rooms[roomIndexList[i]].center;
            if (i + 1 < roomIndexList.Count && nowFloor > 6 && Random.Range(0, 2) == 1)
            {
                roomIndexList.RemoveAt(roomIndexList.Count - 1);
                var spawner2 = Instantiate(mobSpawner, RandomMove1Block(new Vector3Int(nowRoomsCenter.x, nowRoomsCenter.y, 0)), Quaternion.identity);
                spawner2.transform.SetParent(mobSpawners.transform);
            }
            var spawner = Instantiate(mobSpawner, RandomMove1Block(new Vector3Int(nowRoomsCenter.x, nowRoomsCenter.y, 0)), Quaternion.identity);
            spawner.transform.SetParent(mobSpawners.transform);
        }

        //필수 음식 대 생성
        Item item = new Item(5, 1);
        gm.SpawnItem(rooms[Random.Range(0, rooms.Count)].center, item);
        itemAndMobCount--;
        //4층 마다 필수적으로 무기를 하나 생성한다.
        if (nowFloor % 4 == 0)
        {
            gm.SpawnEquipItemWithFloor(RandomMove1Block(rooms[Random.Range(0, rooms.Count)].center));
            itemAndMobCount--;
        }
        for (int i = 0; i < itemAndMobCount - 1; i++)
        {
            gm.SpawnItemWithFloor(RandomMove1Block(rooms[Random.Range(0, rooms.Count)].center));
        }
        //계단 설치하기 플레이어 스폰지점 제외
        var rand = Random.Range(0, rooms.Count);
        while (startRoomIndex == rand) rand = Random.Range(0, rooms.Count);
        var nowRoomsCenter2 = rooms[rand].center;
        Instantiate(stair, (Vector3)RandomMove1Block(new Vector3Int(nowRoomsCenter2.x, nowRoomsCenter2.y, 0)), Quaternion.identity);

        for (int i = 0; i < itemAndMobCount - 1; i++)
        {
            gm.SpawnItemWithFloor(RandomMove1Block(rooms[Random.Range(0, rooms.Count)].center));
        }
        //a스타 맵 갱신
        astar.LoadTile();
    }


    //해당 좌표를 랜덤으로 한칸 상하좌우중 아무대나 움직여서 반환하는 함수
    public Vector3Int RandomMove1Block(Vector3Int pos)
    {
        var xdelta = new int[4] { 0, -1, 0, +1 };
        var ydelta = new int[4] { -1, 0, +1, 0 };
        var rand = Random.Range(0, 4);
        return new Vector3Int(pos.x + xdelta[rand], pos.y + ydelta[rand], 0);
    }
    public Vector3Int RandomMove1Block(Vector2Int pos)
    {
        var xdelta = new int[4] { 0, -1, 0, +1 };
        var ydelta = new int[4] { -1, 0, +1, 0 };
        var rand = Random.Range(0, 4);
        return new Vector3Int(pos.x + xdelta[rand], pos.y + ydelta[rand], 0);
    }
    public void CreateWall()
    {
        var xdelta = new int[4] { 0, -1, 0, +1 };
        var ydelta = new int[4] { -1, 0, +1, 0 };
        for (int i = 0; i < 500; i++)
        {
            for (int j = 0; j < 500; j++)
            {
                var aroundTileCount = Count4Directions(floor, new Vector3Int(i, j, 0));
                if (floor.HasTile(new Vector3Int(i, j, 0)) == false
                    && (aroundTileCount >= 1))
                {
                    wall.SetTile(new Vector3Int(i, j, 0), wallTile);
                }
            }
        }
    }
    void ConnectRoom(Vector2Int center, Vector2Int center2)
    {
        bool firstxConflict = false;
        bool firstyConflict = false;
        //가로축
        int x = center.x;
        int y = center.y;
        for (; x != center2.x;)
        {
            if (x > center2.x)
                x--;
            else x++;
            //if (CheckYDirections(floor, new Vector3Int(x, y, 0)) == false)
            {
                floor.SetTile(new Vector3Int(x, y, 0), floorTile);
            }
        }
        //세로축
        for (; y != center2.y;)
        {
            if (y > center2.y)
                y--;
            else y++;
            //if (CheckXDirections(floor, new Vector3Int(x, y, 0)) == false)
            {
                floor.SetTile(new Vector3Int(x, y, 0), floorTile);
            }
        }
    }

    bool CheckYDirections(Tilemap tilemap, Vector3Int pos)
    {
        var ydelta = new int[4] { -1, 0, +1, 0 };
        for (int i = 0; i < 4; i++)
        {
            if (tilemap.HasTile(new Vector3Int(pos.x, pos.y + ydelta[i], pos.z)))
            {
                return true;
            }
        }
        return false;
    }
    bool CheckXDirections(Tilemap tilemap, Vector3Int pos)
    {
        var xdelta = new int[4] { 0, -1, 0, +1 };
        for (int i = 0; i < 4; i++)
        {
            if (tilemap.HasTile(new Vector3Int(pos.x + xdelta[i], pos.y, pos.z)))
            {
                return true;
            }
        }
        return false;
    }
    //그 타일의 상하좌우로 타일이 하나라도 존재하는지 체크 충돌방지
    // true 4방향에 하나라도 있음 false 4방향에 한개도 없음)
    bool Check4Directions(Tilemap tilemap, Vector3Int pos)
    {
        var xdelta = new int[4] { 0, -1, 0, +1 };
        var ydelta = new int[4] { -1, 0, +1, 0 };
        for (int i = 0; i < 4; i++)
        {
            if (tilemap.HasTile(new Vector3Int(pos.x + xdelta[i], pos.y + ydelta[i], pos.z)))
            {
                return true;
            }
        }
        return false;
    }

    //그 타일의 좌표 중심으로 상하좌우에 타일이 몇개 있는지 반환
    int Count4Directions(Tilemap tilemap, Vector3Int pos)
    {
        var xdelta = new int[4] { 0, -1, 0, +1 };
        var ydelta = new int[4] { -1, 0, +1, 0 };
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (tilemap.HasTile(new Vector3Int(pos.x + xdelta[i], pos.y + ydelta[i], pos.z)))
            {
                count++;
            }
        }
        return count;
    }

    void ClearFloor()
    {
        foreach (var e in GameObject.FindGameObjectsWithTag("Monster"))
            Destroy(e);
        foreach (var e in GameObject.FindGameObjectsWithTag("EnemyAttack"))
            Destroy(e);
        foreach (var e in GameObject.FindGameObjectsWithTag("PlayerAttack"))
            Destroy(e);
        foreach (var e in GameObject.FindGameObjectsWithTag("DroppedItem"))
            Destroy(e);
        foreach (var e in GameObject.FindGameObjectsWithTag("MobSpawner"))
            Destroy(e);
        foreach (var e in GameObject.FindGameObjectsWithTag("Stair")) ;
        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < 1000; j++)
            {
                floor.SetTile(new Vector3Int(i, j, 0), null);
                wall.SetTile(new Vector3Int(i, j, 0), null);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
