using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] rooms;
    public DungeonModel dungeonModel = new DungeonModel();

    public int seed = 1;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSeed();
        GenerateDungeon();
    }

    void GenerateSeed()
    {
        Random.InitState(seed);
    }

    GameObject CreateRoom(Vector3 position, Quaternion rotation, GameObject roomTemplate)
    {
        //var roomScript = roomTemplate.GetComponent<RoomBehaviour>();
        var newRoom = Instantiate(roomTemplate, new Vector3(position.x, position.y, position.z), rotation, transform);
        newRoom.GetComponent<RoomBehaviour>().UpdateRoom();
        return newRoom;
    }

    void GenerateDungeon()
    {
        GenerateFirstRoom();

        StartCoroutine(dungeonGeneratorCoroutine());
        /*
        for(int i = 0; i < 30; i ++)
        {

            //source room
            
            var roomIndex = Random.Range(0, roomClones.Count);
            var room = roomClones[roomIndex];
            var roomBehaviour = room.GetComponent<RoomBehaviour>();
            var entranceIndex = Random.Range(1, roomBehaviour.entrances.Length);// Random.Range(0, rooms[0].entrances.Length);

            var roomEntrance = dungeonModel.PullRandomRoomEntrance();
            var roomTemplateIndex = Random.Range(0, rooms.Length);

            // return room1.entrances[0].GetComponent<Entrance>().roomlink;


            var newRoom = GenerateRoom(roomEntrance, roomTemplateIndex);

            CreateRoomTest();

            if (dungeonModel.IsIntersecting(newRoom))
            {
                //GameObject.Destroy(newRoom);

                continue;
            }

                Debug.Log(roomEntrance.Key);
            dungeonModel.AddRoom(newRoom);

            
        }
        */
    }

    IEnumerator dungeonGeneratorCoroutine()
    {
        for (int i = 0; i < 150;)
        {

            //source room
            /*
            var roomIndex = Random.Range(0, roomClones.Count);
            var room = roomClones[roomIndex];
            var roomBehaviour = room.GetComponent<RoomBehaviour>();
            var entranceIndex = Random.Range(1, roomBehaviour.entrances.Length);// Random.Range(0, rooms[0].entrances.Length);
            */
            var roomEntrance = dungeonModel.GetRandomRoomEntrance();
            var roomTemplateIndex = Random.Range(0, rooms.Length);

            // return room1.entrances[0].GetComponent<Entrance>().roomlink;


            var newRoom = GenerateRoom(roomEntrance, roomTemplateIndex);

            if (dungeonModel.IsIntersecting(newRoom))
            {
                Destroy(newRoom);
                Debug.Log("destroyed room!");
                yield return new WaitForSeconds(0.01f);
                continue;
            }
            dungeonModel.AddRoom(newRoom);
            dungeonModel.RemoveEntranceIndex(roomEntrance.Key);

            i++;
            yield return new WaitForSeconds(0.01f);
        }
    }



    void GenerateFirstRoom()
    {
        //room1
        var pos = new Vector3(20, 0, 20);
        var room = CreateRoom(pos, new Quaternion(0, 0, 0, 0), rooms[0]);
        dungeonModel.AddRoom(room);
    }

    GameObject GenerateRoom(RoomEntrance roomEntrance, int roomTemplateIndex)
    {
        var sourceRoom = dungeonModel.GetSourceRoom(roomEntrance);
        var sourceEntrance = sourceRoom.GetComponent<RoomBehaviour>().entrances[roomEntrance.entranceIndex];
        var sourceTransform = sourceEntrance.transform;

        var templateRoom = rooms[roomTemplateIndex];
        var templateEntrance = templateRoom.GetComponent<RoomBehaviour>().entrances[0];
        var templateTransform = templateEntrance.transform;

        Vector3 pos = new Vector3(sourceTransform.position.x, sourceTransform.position.y, sourceTransform.position.z);

        var targetRoom = CreateRoom(pos, sourceTransform.rotation, templateRoom);
        var targetTransform = targetRoom.GetComponent<RoomBehaviour>().roomContents.transform;
        targetTransform.localPosition = GetLocalPosition(templateTransform);

        return targetRoom;
    }

    public Vector3 GetLocalPosition(Transform transform)
    {
        var localPosition = transform.localPosition;
        switch (transform.eulerAngles.y)
        {
            case (0):
                return new Vector3(localPosition.x, localPosition.y, localPosition.z);

            case (90):
                return new Vector3(-localPosition.z, localPosition.y, localPosition.x);

            case (180):
                return new Vector3(-localPosition.x, localPosition.y, -localPosition.z);

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
