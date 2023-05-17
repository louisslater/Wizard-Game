using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBounds
{
    private const int BOUND_MULTIPLIER = 1000;

    public int roomBottomBound { get; private set; }
    public int roomTopBound { get; private set; }

    public int Key 
    { 
        get
        {
            return BOUND_MULTIPLIER * roomBottomBound + roomTopBound;
        } 
    }

    public RoomBounds(int roomBottomBound, int roomTopBound)
    {
        this.roomBottomBound = roomBottomBound;
        this.roomTopBound = roomTopBound;
    }

    public RoomBounds(int key)
    {
        this.roomBottomBound = key / BOUND_MULTIPLIER;
        this.roomTopBound = key % BOUND_MULTIPLIER;
    }
}
