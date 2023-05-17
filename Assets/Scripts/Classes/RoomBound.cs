using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBound
{

    public Vector3 bottomBound { get; private set; }
    public Vector3 topBound { get; private set; }

    public RoomBound(Vector3 bottomBound, Vector3 topBound)
    {
        this.bottomBound = bottomBound;
        this.topBound = topBound;
    }

    public bool IsIntersecting(RoomBound room)
    {
        return IsIntersecting(bottomBound, topBound, room.bottomBound) ||
            IsIntersecting(bottomBound, topBound, room.topBound) ||
            IsIntersecting(room.bottomBound, room.topBound, bottomBound) ||
            IsIntersecting(room.bottomBound, room.topBound, topBound);
;
    }

    private bool IsIntersecting(Vector3 bottomBound, Vector3 topBound, Vector3 point)
    {
        return IsIntersecting(bottomBound.x, topBound.x, point.x) &&
            IsIntersecting(bottomBound.y, topBound.y, point.y) &&
            IsIntersecting(bottomBound.z, topBound.z, point.z);
    }

    private bool IsIntersecting(float point1, float point2, float point3)
    {
        if (point1 < point2)
            return point3 >= point1 && point3 <= point2;
        return point3 <= point1 && point3 >= point2;
    }
}
