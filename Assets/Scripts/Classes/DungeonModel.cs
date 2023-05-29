using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonModel
{

    private List<int> roomEntranceIndexes;

    private List<GameObject> roomClones = new List<GameObject>();

    private List<Collider> roomColliders;


    public DungeonModel()
    {
        roomEntranceIndexes = new List<int>();
        roomColliders = new List<Collider>();

    }

    public void AddRoom(GameObject room)
    {
        var roomBehaviour = room.GetComponent<RoomBehaviour>();

        roomClones.Add(room);

        roomColliders.Add(roomBehaviour.roomCollider);

        for (int i = 0; i < roomBehaviour.entrances.Length; i++)
        {
            roomEntranceIndexes.Add(new RoomEntrance(roomClones.Count - 1, i).Key);
        }
        RemoveEntranceIndex(roomClones.Count - 1, 0);

        Debug.Log(roomClones.Count);
    }

    public Collider GetRoomCollider(GameObject room)
    {
        var roomBehaviour = room.GetComponent<RoomBehaviour>();
        return roomBehaviour.roomCollider;
    }

    public bool IsIntersecting(GameObject targetRoom)
    {
        var roomColliderTarget = GetRoomCollider(targetRoom);

        Collider[] collidingRooms = Physics.OverlapBox(roomColliderTarget.gameObject.transform.position + roomColliderTarget.bounds.center , roomColliderTarget.bounds.extents, Quaternion.identity, LayerMask.GetMask("Room"));
        Debug.Log("colliding rooms: " + collidingRooms.Length);


        if(collidingRooms.Length > 1)
        {
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
        int randomIndex = Random.Range(0, roomEntranceIndexes.Count);
        int key = roomEntranceIndexes[randomIndex];
        //roomEntranceIndexes.Remove(key);
        Debug.Log($"randomIndex:{randomIndex} count:{roomEntranceIndexes.Count}");
        return new RoomEntrance(key);
    }
}
