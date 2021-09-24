using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MST_Room : MonoBehaviour
{
    public static List<MST_Room> mst_Rooms = new List<MST_Room>();
    public static Dictionary<Vector3Int, MST_Room> posDic = new Dictionary<Vector3Int, MST_Room>();
    public int parentIndex = 0;

    public Rigidbody2D r2 = null;
    public Vector3 lastPos;
    public Vector3Int? roomPos = null;
    public float padding = 0;
    [Range(1, 20)]
    public int width = 10;
    [Range(1, 20)]
    public int height = 10;
    public void Set_RoomPos()
    {
        roomPos = new Vector3Int(Mathf.RoundToInt(transform.GetChild(0).position.x), Mathf.RoundToInt(transform.GetChild(0).position.y), 0);
        posDic.Add(roomPos.Value, this);
    }
    public static bool CheckCollide(MST_Room room1, MST_Room room2)
    {
        float r1_Left = room1.transform.GetChild(0).position.x - room1.width * 0.5f-room1.padding;
        float r1_Right = room1.transform.GetChild(0).position.x + room1.width * 0.5f+ room1.padding;
        float r1_Up = room1.transform.GetChild(0).position.y + room1.height * 0.5f+ room1.padding;
        float r1_Down = room1.transform.GetChild(0).position.y - room1.height * 0.5f- room1.padding;
        float r2_Left = room2.transform.GetChild(0).position.x - room2.width * 0.5f- room2.padding;
        float r2_Right = room2.transform.GetChild(0).position.x + room2.width * 0.5f+ room2.padding;
        float r2_Up = room2.transform.GetChild(0).position.y + room2.height * 0.5f+ room2.padding;
        float r2_Down = room2.transform.GetChild(0).position.y - room2.height * 0.5f- room2.padding;

        if (r1_Left < r2_Left && r2_Left < r1_Right)
        {
            if (r1_Down < r2_Down && r2_Down < r1_Up)
            {
                return true;
            }
            else if (r1_Down < r2_Up && r2_Up < r1_Up)
            {
                return true;
            }
        }
        else if (r1_Left < r2_Right && r2_Right < r1_Right)
        {
            if (r1_Down < r2_Down && r2_Down < r1_Up)
            {
                return true;
            }
            else if (r1_Down < r2_Up && r2_Up < r1_Up)
            {
                return true;
            }
        }
        return false;
    }
}
