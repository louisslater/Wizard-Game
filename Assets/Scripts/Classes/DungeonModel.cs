using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonModel
{

    private List<int> roomEntranceIndexes;

    private List<GameObject> roomClones = new List<GameObject>();

    private List<RoomBound> roomBounds;


    public DungeonModel()
    {
        roomEntranceIndexes = new List<int>();
        roomBounds = new List<RoomBound>();

    }

    public void AddRoom(GameObject room)
    {
        var roomBehaviour = room.GetComponent<RoomBehaviour>();

        roomClones.Add(room);

        roomBounds.Add(GetRoomBound(room));

        for (int i = 0; i < roomBehaviour.entrances.Length; i++)
        {
            roomEntranceIndexes.Add(new RoomEntrance(roomClones.Count - 1, i).Key);
        }
        RemoveEntranceIndex(roomClones.Count - 1, 0);
    }

    public RoomBound GetRoomBound(GameObject room)
    {
        var roomBehaviour = room.GetComponent<RoomBehaviour>();
        return new RoomBound(roomBehaviour.roomBottomCorner, roomBehaviour.roomTopCorner);
    }

    public bool IsIntersecting(GameObject room)
    {
        var roomBoundTarget = GetRoomBound(room);
        foreach(var roomBound in roomBounds)
        {
            if (roomBound.IsIntersecting(roomBoundTarget))
                return true;
        }
        return false;
    }

    public GameObject GetSourceRoom(RoomEntrance roomEntrance)
    {
        return roomClones[roomEntrance.roomIndex];
    }

    public void RemoveEntranceIndex(int roomIndex ,int entranceIndex)
    {
        roomEntranceIndexes.Remove(new RoomEntrance(roomIndex, entranceIndex).Key);
    }

    public void RemoveEntranceIndex(int key)
    {
        roomEntranceIndexes.Remove(key);
    }

    public RoomEntrance GetNextRoomEntrance()
    {
        int key = roomEntranceIndexes[0];
        //roomEntranceIndexes.Remove(key);
        Debug.Log($"count:{roomEntranceIndexes.Count}");
        return new RoomEntrance(key);
    }

    public RoomEntrance GetRandomRoomEntrance()
    {
        int randomIndex = Random.Range(0, roomEntranceIndexes.Count - 1);
        int key = roomEntranceIndexes[randomIndex];
        //roomEntranceIndexes.Remove(key);
        Debug.Log($"randomIndex:{randomIndex} count:{roomEntranceIndexes.Count}");
        return new RoomEntrance(key);
    }
}
