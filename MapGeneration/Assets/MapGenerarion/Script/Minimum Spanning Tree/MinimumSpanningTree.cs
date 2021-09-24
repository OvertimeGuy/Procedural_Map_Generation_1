using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MinimumSpanningTree : MonoBehaviour
{
    public static List<MST_Line> Create_MST(List<MST_Line> lines,List<MST_Room> rooms)
    {
        List<MST_Room> beginRoom = new List<MST_Room>();
        List<MST_Room> finishedRoom = new List<MST_Room>();
        List<MST_Line> beginLine = new List<MST_Line>();
        List<MST_Line> finishedLine = new List<MST_Line>();

        var _line_dist_ = from line in lines
                          orderby line.distance ascending
                          select line;
        foreach (MST_Room r in rooms)
        {
            beginRoom.Add(r);
        }
        foreach (MST_Line l in _line_dist_)
        {
            beginLine.Add(l);
        }

        int warningCount = 0;
        BEGIN:

        for(int i=0; i<beginLine.Count;i++)
        {

            if (warningCount > 999999)
            {
                Debug.LogError("WARNING!!");
                break;
            }
            warningCount++;
            MST_Line l = beginLine[i];

            if (l.room1.parentIndex != 0 && l.room2.parentIndex != 0) continue;
            else if (l.room1.parentIndex > l.room2.parentIndex)
            {
                l.room1.parentIndex = l.room2.parentIndex;
                finishedLine.Add(l);

                beginLine.RemoveAt(i);
                if (beginLine.Count > 0) goto BEGIN;
            }
            else if (l.room2.parentIndex > l.room1.parentIndex)
            {
                l.room2.parentIndex = l.room1.parentIndex;
                finishedLine.Add(l);

                beginLine.RemoveAt(i);
                if (beginLine.Count > 0) goto BEGIN;
            }
            else
            {
                beginLine.RemoveAt(i);
                if (beginLine.Count > 0) goto BEGIN;
            }
            
            
        }

        return finishedLine;

    }
}
