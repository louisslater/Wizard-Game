using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntranceMap
{

    private List<int> roomEntrances;

    private List<int> roomBounds;


    public RoomEntranceMap()
    {
        roomEntrances = new List<int>();

        roomBounds = new List<int>();

    }

    public void AddRoom(int roomIndex, int entranceCount)
    {
        for(int i = 0; i < entranceCount; i ++)
        {
            roomEntrances.Add(new RoomEntrance(roomIndex, i).Key);
        }
    }

    public void RemoveEntrance(int roomIndex ,int entranceIndex)
    {
        roomEntrances.Remove(new RoomEntrance(roomIndex, entranceIndex).Key);
    }

    public RoomEntrance PullRandomRoomEntrance()
    {
        int randomIndex = Random.Range(0, roomEntrances.Count);
        int key = roomEntrances[randomIndex];
        roomEntrances.Remove(key);
        return new RoomEntrance(key);
    }
}
