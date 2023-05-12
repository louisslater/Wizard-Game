using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntrance
{
    private const int ROOM_MULTIPLIER = 100;

    public int roomIndex { get; private set; }
    public int entranceIndex { get; private set; }

    public int Key 
    { 
        get
        {
            return ROOM_MULTIPLIER * roomIndex + entranceIndex;
        } 
    }

    public RoomEntrance(int roomIndex, int entranceIndex)
    {
        this.roomIndex = roomIndex;
        this.entranceIndex = entranceIndex;
    }

    public RoomEntrance(int key)
    {
        this.roomIndex = key / ROOM_MULTIPLIER;
        this.entranceIndex = key % ROOM_MULTIPLIER;
    }
}
