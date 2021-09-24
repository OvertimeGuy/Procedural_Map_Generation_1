using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MST_Line : MonoBehaviour
{
    public static List<MST_Line> mst_Lines = new List<MST_Line>();
    public static List<MST_Line> deselected_Lines = new List<MST_Line>();
    public static List<MST_Line> selected_Lines = new List<MST_Line>();

    public Vector3Int startPoint;
    public Vector3Int endPoint;
    public MST_Room room1;
    public MST_Room room2;
    public float distance;
    public void CreateLine(Vector3Int startPoint, Vector3Int endPoint)
    {
        
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        distance = Vector3.Distance(startPoint, endPoint);
        room1 = MST_Room.posDic[startPoint];
        room2 = MST_Room.posDic[endPoint];
        mst_Lines.Add(this);
    }
    public static bool AlreadyConatins(Vector3Int startPoint, Vector3Int endPoint)
    {
        foreach(MST_Line l in mst_Lines)
        {
            if (l.startPoint == startPoint && l.endPoint == endPoint) return true;
            if (l.startPoint == endPoint && l.endPoint == startPoint) return true;
        }
        return false;
    } 

}
