using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OrientedPoint
{
    public Vector3 position;
    public Quaternion orientation;

    public OrientedPoint(Vector3 pos, Quaternion rot)
    {
        this.position = pos;
        this.orientation = rot;
    }

    public OrientedPoint(Vector3 pos, Vector3 rot)
    {
        this.position = pos;
        this.orientation = Quaternion.LookRotation(rot);
    }

    public Vector3 LocalToWorldPos(Vector3 localSpacePos)
    {
        return position + orientation * localSpacePos;
    }

    public Vector3 LocalToWorldVect(Vector3 localSpacePos)
    {
        return orientation * localSpacePos;
    }
}
