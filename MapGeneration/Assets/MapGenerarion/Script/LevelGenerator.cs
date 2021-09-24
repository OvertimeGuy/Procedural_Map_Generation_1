using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drawing;
using System.Linq;
public class LevelGenerator : MonoBehaviourGizmos
{

    private Mesh delaunayMesh = null;
    public int roomCount =20;
    public Sprite roomSprite;
    public List<Level_Room> roomPresets = new List<Level_Room>();
    private bool spreadFinished = false;
    public void ClearAll()
    {
        MST_Room.posDic.Clear();
        MST_Line.mst_Lines.Clear();
        MST_Line.deselected_Lines.Clear();
        MST_Line.selected_Lines.Clear();
        MST_Room.mst_Rooms.Clear();
        if (delaunayMesh != null)
        {
            DestroyImmediate(delaunayMesh);
            delaunayMesh = null;
        }
        ulong count = 0;
        while (transform.childCount > 0)
        {
            foreach (Transform t in transform)
            {
                DestroyImmediate(t.gameObject);
            }
        }
    }
    
    //���߿� position,posdic ����������.
    public void AddRoom()
    {
        Vector3Int createPos = Vector3Int.zero;
        int createRange = 5;
        ClearAll();
        for (int i = 0; i < roomCount; i++)
        {
            int ii = Random.Range(0, i % (roomPresets.Count+1));
            GameObject room_obj = new GameObject("Room " + i.ToString());
            MST_Room room = room_obj.AddComponent(typeof(MST_Room)) as MST_Room;
            
            //room.parentIndex = i;//**�߿�!!**//
            Vector2 v2 = Random.insideUnitCircle * createRange;
            room.transform.position = createPos+new Vector3(v2.x,v2.y,0);
            room.transform.parent = transform;
            MST_Room.mst_Rooms.Add(room);
            GameObject sprite_Renderer = new GameObject("DebugSpriteRenderer");
            sprite_Renderer.transform.parent = room.transform;
            (sprite_Renderer.gameObject.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer).sprite = roomSprite;
            room.r2= sprite_Renderer.gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
            sprite_Renderer.gameObject.transform.localPosition = Vector3.zero;

            room.width = roomPresets[ii].width;
            room.height = roomPresets[ii].height;
            room.padding = roomPresets[ii].padding;
            sprite_Renderer.gameObject.transform.localScale = new Vector3(room.width + room.padding, room.height + room.padding, 0);
            room.r2.gravityScale = 0;
            room.r2.constraints = RigidbodyConstraints2D.FreezeRotation;
            room.r2.drag = 10;
            sprite_Renderer.gameObject.AddComponent(typeof(BoxCollider2D));
            
        }
    }

    public void Triangulation()
    {
        if (MST_Room.mst_Rooms.Count == 0) AddRoom();
        List<Vector3> points = new List<Vector3>();
        foreach(MST_Room r in MST_Room.mst_Rooms)
        {
            points.Add((Vector3)r.roomPos);
        }
        delaunayMesh =TriangulationTest.Triangulation(points);
    }
    public void Create_MST()
    {
        if (MST_Room.mst_Rooms.Count == 0||MST_Line.mst_Lines.Count>0) Triangulation();
        #region MST_Line����
        if (delaunayMesh != null)
        {
            for (int i = 0; i < delaunayMesh.triangles.Length; i++)
            {
                if (i % 3 == 1)
                {
                    AddLine(i, i+1); AddLine(i+1, i-1); AddLine(i-1, i);
                }
            }
        }
        Vector3Int Vec3_Vec3Int(Vector3 target)
        {
            return new Vector3Int(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), Mathf.RoundToInt(target.z));
        }
        void AddLine(int index1,int index2)
        {
            Vector3Int startPoint = Vec3_Vec3Int(delaunayMesh.vertices[delaunayMesh.triangles[index1]]);
            Vector3Int endPoint = Vec3_Vec3Int(delaunayMesh.vertices[delaunayMesh.triangles[index2]]);
            if (!MST_Line.AlreadyConatins(startPoint, endPoint))
            {
                GameObject obj = new GameObject("Line");
                obj.transform.parent = transform;
                MST_Line line=obj.AddComponent(typeof(MST_Line)) as MST_Line;
                line.CreateLine(startPoint, endPoint);
            }
        }
        #endregion
        #region MST����
        List<MST_Line> mst_Finished_Line = MinimumSpanningTree.Create_MST(MST_Line.mst_Lines, MST_Room.mst_Rooms);
        foreach (MST_Line line in MST_Line.mst_Lines)
        {
            if (mst_Finished_Line.Contains(line)) MST_Line.selected_Lines.Add(line);
            else MST_Line.deselected_Lines.Add(line);
        }
        #endregion
    }
    
    public void Generate()
    {
        StartCoroutine("Generating");
    }
    public override void DrawGizmos()
    {
        if (MST_Room.mst_Rooms.Count == 0) return;
        //room���� ���� �׸���
        foreach (MST_Room room in MST_Room.mst_Rooms)
        {
            //Draw.CircleXZ(room.transform.position, 1.0f, Color.red);
            //Draw.Label2D(room.transform.position, room.parentIndex.ToString(), 24, LabelAlignment.Center, Color.white);
            using (Draw.WithLineWidth(1.25f))
            {
                Draw.WireRectangle(room.transform.GetChild(0).position + Vector3.forward * 0.1f
                    , Quaternion.Euler(90, 0, 0), new Vector2(room.width + room.padding, room.height + room.padding), Color.black);
            }
            if (room.roomPos.HasValue)
            {
                using (Draw.WithLineWidth(1.75f))
                {
                    Draw.WireRectangle((Vector3)(room.roomPos)+Vector3.forward*0.1f, Quaternion.Euler(90, 0, 0), new Vector2(room.width, room.height), Color.red);
                }
            }
        }

        foreach (MST_Line line in MST_Line.selected_Lines)
        {
            using (Draw.WithLineWidth(1.2f)) { Draw.Line(line.startPoint, line.endPoint, Color.black); }
        }
        foreach (MST_Line line in MST_Line.deselected_Lines)
        {
            using (Draw.WithLineWidth(0.75f)) { Draw.Line(line.startPoint + Vector3.down * 0.01f, line.endPoint + Vector3.down * 0.01f, Color.gray); }
        }

    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }
        if(MST_Room.mst_Rooms.Count==0) return;
        spreadFinished = true;
        foreach(MST_Room room in MST_Room.mst_Rooms)
        {
          
            if(room.r2.transform.position != room.lastPos)
            {
                room.lastPos = room.r2.transform.position;
                spreadFinished = false;
            }
            
        }
        if (spreadFinished)
        {
            print("STOPPED!");
        }
        else
        {
            print("Moving!");
        }
    }
    public IEnumerator Generating()
    {
        //print("�� �߰�");
        AddRoom();
        yield return null;
        //print("�� �۶߸�");
        Time.timeScale = 20;
        int count = 0;//�Ϸ� �� ���� ������
        float spreadFinishedTime = 0;//���� �ð�
        while (!spreadFinished||Time.time-spreadFinishedTime<2||count<100)
        {
            if (spreadFinished)
            {
                if (count == 0) spreadFinishedTime = Time.time;
                count++;
            }
            else
            {
                count = 0;
            }
            yield return null;
        }
        Time.timeScale = 1;
        //print("�� ����");
        List<MST_Room> deleteTarget = new List<MST_Room>();
        int deleteLength = 10;//�󸶳� ������ ����.
        var _deleteRooms = from room in MST_Room.mst_Rooms
                           orderby room.width * room.height ascending
                           select room;
        int deleteCount = 0;
        foreach (var room in _deleteRooms)
        {

            if (deleteCount >= deleteLength) break;
            deleteCount++;
            MST_Room.mst_Rooms.Remove(room);
            deleteTarget.Add(room);
            room.gameObject.SetActive(false);
        }
        for (int i = 0; i < MST_Room.mst_Rooms.Count; i++)
        {
            MST_Room.mst_Rooms[i].parentIndex = i;
        }
        //print("�� ��¥ ��ġ ����");
        foreach (MST_Room room in MST_Room.mst_Rooms)
        {
            room.Set_RoomPos();
        }
        //print("��γ� �ﰢ����");
        Triangulation();
        yield return null;
        //print("�ּ� ���д�Ʈ��");
        Create_MST();
        print("�Ϸ�");
        bool checker = true;
        foreach(MST_Line line in MST_Line.selected_Lines)
        {
            if (line.room1 == null || line.room2 == null) checker = false;
        }
        print(checker);
    }
}
