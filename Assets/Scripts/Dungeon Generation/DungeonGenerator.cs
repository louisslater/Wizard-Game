using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] rooms;
    public List<GameObject> roomClones = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateDungeon();
    }

    GameObject CreateRoom(Vector3 position, Quaternion rotation, GameObject roomTemplate)
    {
        //var roomScript = roomTemplate.GetComponent<RoomBehaviour>();
        var newRoom = Instantiate(roomTemplate, new Vector3(position.x, position.y, position.z), rotation, transform).GetComponent<RoomBehaviour>();
        newRoom.UpdateRoom();
        return newRoom.gameObject;
    }


    void CreateRoomTest()
    {

    }

    void GenerateDungeon()
    {
        GenerateFirstRoom();

        var roomIndex = 0;//Random.Range(0, roomClones.Length);
        var entranceIndex = 1;// Random.Range(0, rooms[0].entrances.Length);
        // return room1.entrances[0].GetComponent<Entrance>().roomlink;

        GenerateRoom(roomIndex, 1);
        GenerateRoom(roomIndex, 2);
        GenerateRoom(roomIndex, 3);
    }



    void GenerateFirstRoom()
    {
        //room1
        var pos = new Vector3(20, 0, 20);
        var room = CreateRoom(pos, new Quaternion(0, 0, 0, 0), rooms[0]);
        roomClones.Add(room);        
    }


    GameObject GenerateRoom(int roomIndex, int entranceIndex)
    {
        var sourceRoom = roomClones[roomIndex];
        var sourceEntrance = sourceRoom.GetComponent<RoomBehaviour>().entrances[entranceIndex];
        var sourceTransform = sourceEntrance.transform;

        Vector3 pos = new Vector3(sourceTransform.position.x, sourceTransform.position.y, sourceTransform.position.z);

        var targetRoom = CreateRoom(pos, new Quaternion(0,0,0,0), sourceRoom);
        var targetTransform = targetRoom.GetComponent<RoomBehaviour>().roomContents.transform;
        targetTransform.localPosition = GetLocalPosition(sourceTransform);
        return targetRoom;
    }

    public Vector3 GetLocalPosition(Transform transform)
    {
        var localPosition = transform.localPosition;
        switch (transform.eulerAngles.y)
        {
            case (0):
                //transform.Rotate(new Vector3(0,180,0));
                return new Vector3(-localPosition.x, localPosition.y, -localPosition.z);
            
            case (90):
                return new Vector3(-localPosition.z, localPosition.y, localPosition.x);

            case (180):
                //transform.Rotate(new Vector3(0, 180, 0));
                return new Vector3(localPosition.x, localPosition.y, localPosition.z);

            case (270):
                return new Vector3(localPosition.z, localPosition.y, -localPosition.x);

        }
        throw new UnityException("Angle not found");//todo
    }

    /*
  Transform oldGenerateRoomFor2(Transform previousEntranceTransform)
  {
      //room2
      Vector3 pos2 = new Vector3(previousEntranceTransform.position.x, previousEntranceTransform.position.y, previousEntranceTransform.position.z);
      var room2 = CreateRoom(pos2, previousEntranceTransform.rotation, rooms[1]);

      //var randomExit = room2.entrances[Random.Range(0, room2.entrances.Length)];
      var randomExit = room2.entrances[0];

      Transform entranceTransform2 = room2.entrances[2].transform;

      //move room contents object so that coords are the same as the entrance position this should move the whole room to the correct position
      room2.roomContents.transform.localPosition = new Vector3(-entranceTransform2.localPosition.x, previousEntranceTransform.localPosition.y, -entranceTransform2.localPosition.z);

      return randomExit.transform;
  }


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
