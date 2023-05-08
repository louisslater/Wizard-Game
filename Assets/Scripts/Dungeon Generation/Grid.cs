using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2 gridSize;
    Cell[,] grid;

    public GameObject[] rooms;

    void Start()
    {
        grid = new Cell[Mathf.FloorToInt(gridSize.x), Mathf.FloorToInt(gridSize.y)];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Cell cell = new Cell();
                cell.occupied = false;
                grid[x, y] = cell;
            }
        }
        GenerateDungeon();
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        for (int y = 0; y < Mathf.FloorToInt(gridSize.y); y++)
        {
            for (int x = 0; x < Mathf.FloorToInt(gridSize.x); x++)
            {
                Cell cell = grid[x, y];
                if (cell.occupied)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.white;
                Vector3 pos = new Vector3(x, 0, y);
                Gizmos.DrawWireCube(pos, Vector3.one);

            }
        }
    }
    RoomBehaviour CreateRoom(Vector2 position, GameObject room)
    {
        var roomScript = room.GetComponent<RoomBehaviour>();
        for (int y = 0; y < Mathf.FloorToInt(roomScript.roomSize.y+1); y++)
        {
            for (int x = 0; x < Mathf.FloorToInt(roomScript.roomSize.x+1); x++)
            {
                grid[Mathf.FloorToInt(position.x+x), Mathf.FloorToInt(position.y+y)].occupied = true;
            }
        }
        var newRoom =Instantiate(room, new Vector3(position.x + (roomScript.roomSize.x / 2), 0, position.y + (roomScript.roomSize.y / 2)), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
        newRoom.UpdateRoom();
        return newRoom;
    }

    void GenerateDungeon()
    {
        var pos = new Vector2(20, 20);
        var room1 = CreateRoom(pos, rooms[0]);

        Vector2 pos2 = new Vector2(room1.entrances[0].transform.position.x + (room1.roomSize.x / 2), room1.entrances[0].transform.position.z + (room1.roomSize.y / 2));

        Debug.Log(pos2);

        CreateRoom(pos2, rooms[1]);
    }
}
