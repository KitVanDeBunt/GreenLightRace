using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldPath : MonoBehaviour
{
    //public Node[] path;
    [HideInInspector]
    [SerializeField]
    private List<Node> pathList = new List<Node>();

    [HideInInspector]
    [SerializeField]
    private Transform holder;

    [SerializeField]
    private Track[] trackParts;

    [SerializeField]
    private float bezierStepSize = 1;
    //private int pathLength;

    void Start()
    {
       // pathLength = pathList.count;
    }

    void AddPathPoint(int number)
    {
        Node newPathePoint = new GameObject("P" + number.ToString()).AddComponent<Node>();
        newPathePoint.transform.position = transform.position;
        pathList.Add(newPathePoint);
    }

    //returns true if build
    public bool Rebuild()
    {
        int i;
        int j;

        //create new
        List<Vector3> pointList = new List<Vector3>();
        List<Quaternion> rotationList = new List<Quaternion>();
        List<float> widthList = new List<float>();

        Vector3[] tempPositions = new Vector3[1] { new Vector3(0, 0, 0) };
        Quaternion[] tempRotatioins = new Quaternion[1] { new Quaternion(0, 0, 0, 0) };
        float[] tempWidth = new float[1] { 0 };

        //List<Node> newNodes = new List<Node>();

        Debug.Log("trackParts L:" + trackParts.Length);
        for (i = 0; i < trackParts.Length; i++)
        {
            float? bezierTimeStepSize = trackParts[i].BezierStepSize(bezierStepSize);
            bool createPointsSucses = trackParts[i].CreateBezieredPoints(ref tempPositions, ref tempRotatioins, ref tempWidth, false, bezierTimeStepSize.Value);
            if (createPointsSucses)
            {
                float pointCount = tempPositions.Length;
                for (j = 0; j < pointCount-1; j++) /// -1 SKIPS LAST POINT
                {
                    pointList.Add(tempPositions[j]);
                    rotationList.Add(tempRotatioins[j]);
                    widthList.Add(tempWidth[j]);
                }
            }
            else
            {
                return false;
            }
        }

        Debug.Log("pointList COUNT L:" + pointList.Count);
       
        //remove old
        for (i = pathList.Count - 1; i > -1; i--)
        {
            if (pathList[i] != null)
            {
                GameObject.DestroyImmediate(pathList[i].gameObject);
                //pathList.RemoveAt(i);
            }
            pathList.RemoveAt(i);
        }

        // if holder is null create holder
        if (holder == null)
        {
            holder = new GameObject("Path Holder").transform;
            holder.parent = transform;
        }

        //add nodes if needed
        for (i = 0; i < pointList.Count; i++)
        {
            Debug.Log("pointList i:" + i);
            if (i >= pathList.Count)
            {
                Debug.Log("add");
                AddPathPoint(i);
            }
            if (pathList[i] == null)
            {
                AddPathPoint(i);
            }

            for (j = 0; j < pathList.Count; j++)
            {
                if (i != j)
                {
                    if (pathList[i] == pathList[j])
                    {
                        AddPathPoint(i);
                    }
                }
            }
            pathList[i].transform.parent = holder.transform;
#if UNITY_EDITOR
            IconManager.SetIcon(pathList[i].gameObject, IconManager.LabelIcon.Purple);
#endif
        }

        //Debug.Log("pointList COUNT L:" + pointList.Count);
       
        //update
        for (i = 0; i < pointList.Count; i++)
        {
            pathList[i].transform.position = pointList[i];
            pathList[i].center.position = pointList[i];
            pathList[i].center.rotation = rotationList[i];

            pathList[i].maxRight.position = pointList[i];
            pathList[i].maxRight.rotation = rotationList[i];
            pathList[i].maxRight.Translate(new Vector3(-(widthList[i] / 2), 0, 0), Space.Self);

            pathList[i].maxLeft.position = pointList[i];
            pathList[i].maxLeft.rotation = rotationList[i];
            pathList[i].maxLeft.Translate(new Vector3((widthList[i]/2), 0, 0), Space.Self);

            pathList[i].maxSpeed = 120F;

           // pathList[i].maxLeft.position = widthList[i];
            
            //newNodes.pointList[i].
        }
        //


        return true;
    }

    public int getNextID(int currentID)
    {
        currentID++;
        if (currentID > pathList.Count - 1)
        {
            currentID = 0;
        }
        return currentID;
    }

    public Vector3 getNextPointByID(int id)
    {
        return pathList[id].center.position;
    }

    public Node getNodeByID(int id)
    {
        return pathList[id];
    }
}
