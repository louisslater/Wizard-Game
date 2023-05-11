using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] rooms;


    // Start is called before the first frame update
    void Start()
    {
        GenerateDungeon();
    }

    RoomBehaviour CreateRoom(Vector3 position, Quaternion rotation, GameObject room)
    {
        var roomScript = room.GetComponent<RoomBehaviour>();
        var newRoom = Instantiate(room, new Vector3(position.x, position.y, position.z), rotation, transform).GetComponent<RoomBehaviour>();
        newRoom.UpdateRoom();
        return newRoom;
    }


    void CreateRoomTest()
    {

    }

    void GenerateDungeon()
    {
        var pos = new Vector3(20, 0, 20);
        var room1 = CreateRoom(pos, new Quaternion(0, 0, 0, 0), rooms[0]);

        Transform entranceTransform = room1.entrances[0].transform;

        //place room at position of entrance

        Vector3 pos2 = new Vector3(entranceTransform.position.x, entranceTransform.position.y, entranceTransform.position.z);
        var room2 = CreateRoom(pos2, entranceTransform.rotation, rooms[1]);

        var randomExit = room2.entrances[Random.Range(0, room2.entrances.Length)];

        Transform entranceTransform2 = room2.entrances[2].transform;
        Transform exitTransform = randomExit.transform;

        //move room contents object so that coords are the same as the entrance position this should move the whole room to the correct position

        room2.roomContents.transform.localPosition = new Vector3(-entranceTransform2.localPosition.x, entranceTransform.localPosition.y, -entranceTransform2.localPosition.z);

        var randomRoom = rooms[Random.Range(0, rooms.Length)];

        Vector3 pos3 = new Vector3(exitTransform.position.x, exitTransform.position.y, exitTransform.position.z);

        var room3 = CreateRoom(pos3, entranceTransform.rotation, randomRoom);

        Transform entranceTransform3 = room3.entrances[2].transform;

        switch (exitTransform.eulerAngles.y)
        {
            case (0):
                room3.roomContents.transform.localPosition = new Vector3(-entranceTransform3.localPosition.x, entranceTransform3.localPosition.y, -entranceTransform3.localPosition.z);
                room3.roomContents.transform.Rotate(exitTransform.eulerAngles);
                Debug.Log("0");
                break;
            case (90):
                room3.roomContents.transform.localPosition = new Vector3(-entranceTransform3.localPosition.z, entranceTransform3.localPosition.y, entranceTransform3.localPosition.x);
                room3.roomContents.transform.Rotate(exitTransform.eulerAngles);
                Debug.Log("90");
                break;
            case (180):
                break;
                /*
                room3.roomContents.transform.localPosition = new Vector3(entranceTransform3.localPosition.x, entranceTransform3.localPosition.y, entranceTransform3.localPosition.z);
                room3.roomContents.transform.Rotate(exitTransform.eulerAngles);
                Debug.Log("180");
                break;
                */
            case (270):
                room3.roomContents.transform.localPosition = new Vector3(entranceTransform3.localPosition.z, entranceTransform3.localPosition.y, -entranceTransform3.localPosition.x);
                room3.roomContents.transform.Rotate(exitTransform.eulerAngles);
                Debug.Log("270");
                break;
        }



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
