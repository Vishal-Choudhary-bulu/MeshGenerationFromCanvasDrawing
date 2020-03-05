using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint 
{
    //position of the point
    public Vector3 Position { get; set; }

    public PathPoint(Vector3 pos)
    {
        Position = pos;
    }
}
