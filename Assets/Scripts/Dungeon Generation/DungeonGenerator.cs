using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int startPos = 0;
    public List<GameObject> rooms;
    public Vector2 offset;

    public Vector2 roomPos;


    // Start is called before the first frame update
    void Start()
    {
        CreateRoom(roomPos);
    }

    void CreateRoom(Vector2 position)
    {
        var randomRoom = rooms[Random.Range(0, rooms.Count)];
        var roomScript = randomRoom.GetComponent<RoomBehaviour>();
        var randomEntrance = roomScript.doors[Random.Range(0, roomScript.doors.Length)];
        //todo change [0] to enum x
        var newRoom = Instantiate(randomRoom, new Vector3(randomEntrance.transform.position.x, 0, randomEntrance.transform.position.z), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
    }

    /*
    void GenerateDungeon()
    {
        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                Cell currentCell = grid[Mathf.FloorToInt(i + j * gridSize.x)];
                if (currentCell.visited)
                {
                    var randomRoom = rooms[Random.Range(0, rooms.Count)];
                    var roomScript = randomRoom.GetComponent<RoomBehaviour>();
                    var newRoom = Instantiate(randomRoom, new Vector3(i * roomScript.roomSize[0], 0, -j * roomScript.roomSize[1]), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);

                    newRoom.name += " " + i + "," + j;
                }
            }
        }
    }

    void MazeGenerator()
    {
        grid = new List<Cell>();

        int currentCell = startPos;
        Stack<int> path = new Stack<int>();

        int k = 0;
        while(k < 1000)
        {
            k++;

            grid[currentCell].visited = true;

            if(currentCell == grid.Count-1)
            {
                break;
            }

            List<int> neighbors = CheckNeighbors(currentCell);

            if (neighbors.Count == 0)
            {
                if (path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if(newCell > currentCell)
                {
                    //down or right
                    if(newCell - 1 == currentCell)
                    {
                        grid[currentCell].status[2] = true;
                        currentCell = newCell;
                        grid[currentCell].status[3] = true;
                    }
                    else
                    {
                        grid[currentCell].status[1] = true;
                        currentCell = newCell;
                        grid[currentCell].status[0] = true;
                    }
                }
                else
                {
                    //up or left
                    if (newCell + 1 == currentCell)
                    {
                        grid[currentCell].status[3] = true;
                        currentCell = newCell;
                        grid[currentCell].status[2] = true;
                    }
                    else
                    {
                        grid[currentCell].status[0] = true;
                        currentCell = newCell;
                        grid[currentCell].status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        //up
        if(cell - gridSize.x >= 0 && !grid[Mathf.FloorToInt(cell-gridSize.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - gridSize.x));
        }
        //down
        if (cell + gridSize.x < grid.Count && !grid[Mathf.FloorToInt(cell + gridSize.x)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + gridSize.x));
        }
        //right
        if ((cell+1) % gridSize.x != 0 && !grid[Mathf.FloorToInt(cell + 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        }
        //left
        if (cell % gridSize.x != 0 && !grid[Mathf.FloorToInt(cell - 1)].visited)
        {
            neighbors.Add(Mathf.FloorToInt(cell - 1));
        }
        return neighbors;
    }
    */
}
