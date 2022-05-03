using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class TrailGenerator : MonoBehaviour
{

    public List<Level> levels = new List<Level>();

    [SerializeField] private int trailDistance;
    [SerializeField] private int trailWidth;
    [SerializeField] private GameObject _spaceSlotPrefab;
    [SerializeField] private float _spaceSlotPadding;


    [Range(0f, 1f)][SerializeField] private float _bendyness;
    [Range(0f, 1f)][SerializeField] private float _bendySeparation;


    [Range(0, 29)][SerializeField] private int startX;

    [Range(1, 29)][SerializeField] private int endX;

    public SpaceSlot[,] grid;
    List<SpaceSlot> gridList = new List<SpaceSlot>();

    float _slotHeightSpaceModifier = .5f;
    List<SpaceSlot> notTrailSpots = new List<SpaceSlot>();

    private List<SpaceSlot> trailList = new List<SpaceSlot>();
    private List<SpaceSlot> gridBorderList = new List<SpaceSlot>();
    [HideInInspector] public LinkedList<SpaceSlot> trail = new LinkedList<SpaceSlot>();
    [HideInInspector] public LinkedList<SpaceSlot> trailOther = new LinkedList<SpaceSlot>();

    [HideInInspector] public List<GameObject> treeBatch = new List<GameObject>();
    [HideInInspector] public List<GameObject> rockBatch = new List<GameObject>();
    [HideInInspector] public List<GameObject> nonTrail_slotBatch = new List<GameObject>();
    [HideInInspector] public List<GameObject> plantBatch = new List<GameObject>();

    int firstTrailCount, entireTrailCount;
    public void Awake()
    {
        Services.TrailGenerator = this;
    }


    public void Init()
    {
        Services.Game.currentLevel = levels[0];
        GenerateTrail(Services.Game.currentLevel);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Redo();
        }
    }

    void Redo()
    {
        foreach (SpaceSlot s in grid)
        {
            Destroy(s.gameObject);
            Destroy(s);
        }
        Services.Game.currentLevel = levels[0];
        gridList.Clear();
        notTrailSpots.Clear();
        trailList.Clear();
        GenerateTrail(Services.Game.currentLevel);

    }



    //Trail Generation//
    void GenerateTrail(Level level)
    {

        //Grid Generation//
        grid = new SpaceSlot[trailDistance, trailWidth];
        float startAltitude = level.startTrailSpot.altitutde;
        float endAltitude = level.endTrailSpot.altitutde;
        float totalAlt = endAltitude - startAltitude;
        float altInc = totalAlt / trailDistance * 10f;


        Vector2 startMapPosition = level.startTrailSpot.mapPosition;
        Vector2 endMapPosition = level.endTrailSpot.mapPosition;

        int counter = 0;
        float altitudeTrack = 0f;
        float slope = 0;
        for (int i = -1; i <= trailDistance; i++)
        {
            if (i != -1 && i != trailDistance)
            {
                //ideally at some point this would be set through a derivative... but idk how to code that.
                slope =
                (Services.Game.currentLevel.altitudeCurve.Evaluate(ExtensionMethods.Remap(i + 1, 0, (float)trailDistance, 0, 1f))
                - Services.Game.currentLevel.altitudeCurve.Evaluate(ExtensionMethods.Remap(i - 1, 0, (float)trailDistance, 0, 1f))) / ((i + 1) - (i - 1));

                altitudeTrack = Services.Game.currentLevel.altitudeCurve.Evaluate(ExtensionMethods.Remap(i, 0, (float)trailDistance, 0, 1f)) * _slotHeightSpaceModifier * altInc;
                // Debug.Log(level.altitudeCurve.Evaluate(ExtensionMethods.Remap(i, 0, (float)trailDistance, 0, 1f)));
            }
            else //This is the code for the grid border//
            {
                if (i == -1)
                    altitudeTrack = Services.Game.currentLevel.altitudeCurve.Evaluate(ExtensionMethods.Remap(0, 0, (float)trailDistance, 0, 1f)) * _slotHeightSpaceModifier * altInc;
                else if (i == trailDistance)
                    altitudeTrack = Services.Game.currentLevel.altitudeCurve.Evaluate(ExtensionMethods.Remap(trailDistance - 1, 0, (float)trailDistance, 0, 1f)) * _slotHeightSpaceModifier * altInc;
            }


            for (int j = -1; j <= trailWidth; j++)
            {
                SpaceSlot spaceSlot = null;
                if (j != -1 && j != trailWidth && i != -1 && i != trailDistance)
                {
                    spaceSlot = Instantiate(_spaceSlotPrefab, new Vector3((j * _spaceSlotPadding) + gameObject.transform.position.x, gameObject.transform.position.y + altitudeTrack, (i * _spaceSlotPadding) + gameObject.transform.position.z), Quaternion.identity).GetComponent<SpaceSlot>();
                    spaceSlot.Init(slope);
                    spaceSlot.name = "node (" + counter + ")";
                    grid[i, j] = spaceSlot;
                    spaceSlot.point = new Vector2Int(i, j);
                    gridList.Add(spaceSlot);
                    counter++;
                    notTrailSpots.Add(spaceSlot);
                    continue;
                }

                else if (i == -1 || i == trailDistance || j == -1 || j == trailWidth)
                {
                    spaceSlot = Instantiate(_spaceSlotPrefab, new Vector3((j * _spaceSlotPadding) + gameObject.transform.position.x, gameObject.transform.position.y + altitudeTrack, (i * _spaceSlotPadding) + gameObject.transform.position.z), Quaternion.identity).GetComponent<SpaceSlot>();
                    spaceSlot.point = new Vector2Int(i, j);
                    gridBorderList.Add(spaceSlot);
                    continue;
                }
            }
        }
        //Grid done being generated//


        //Make first trail
        trailList = GetPathDFS(grid, gridList[startX], gridList[gridList.Count - endX]);

        int trailCounter = 0;
        foreach (SpaceSlot t in trailList)
        {
            t.SetSlotToPath();
            t.isTrail = true;
            notTrailSpots.Remove(t);
            t.trailInt = trailCounter;
            trailCounter++;
            trail.AddLast(t);
            firstTrailCount = trailCounter;
        }


        foreach (SpaceSlot s in notTrailSpots)
        {

            if (s.plantChance > 1)
            {
                s.SetSlotToRock();
                continue;
            }
            if (s.plantChance >= 1)
            {

                s.SetSlotToPlant(UnityEngine.Random.Range(.4f, .9f), UnityEngine.Random.Range(0f, 360f));
                continue;
            }
            else
            {
                if (UnityEngine.Random.value < .5)
                {
                    s.SetSlotToTree();
                    continue;
                }

            }
            s.SetSlotToPlant(1f, UnityEngine.Random.Range(0f, 360f));

        }

        foreach (SpaceSlot s in gridBorderList)
        {
            s.SetSlotToTree();
        }

        Services.Player.SetPlayerPosOnTrailStart();
    }



    List<SpaceSlot> GetPathDFS(SpaceSlot[,] indexGrid, SpaceSlot startPoint, SpaceSlot endPoint)
    {

        List<SearchVertex> openSet = new List<SearchVertex>();
        List<SearchVertex> closedSet = new List<SearchVertex>();
        // List<Vector2Int> path = new List<Vector2Int>();
        if (IsPointNavigable(indexGrid, startPoint.point))
        {
            SearchVertex startVertex = new SearchVertex();
            startVertex.Point = startPoint.point;
            startVertex.Parent = null;
            startVertex.Altitude = indexGrid[startPoint.point.x, startPoint.point.y].FirstPointInSpace.z;
            openSet.Add(startVertex);
        }

        while (openSet.Count > 0)
        {
            int index = openSet.Count - 1;
            SearchVertex currentVertex = openSet[index];
            openSet.RemoveAt(index);

            if (currentVertex.Point == endPoint.point)
            {
                List<SpaceSlot> retVal = new List<SpaceSlot>();

                while (currentVertex != null)
                {
                    retVal.Add(indexGrid[currentVertex.Point.x, currentVertex.Point.y]);
                    currentVertex = currentVertex.Parent;
                }

                retVal.Reverse();

                return retVal;
            }


            List<SearchVertex> tempSet = new List<SearchVertex>();
            //up neighbor
            Vector2Int rightNeighbor = new Vector2Int(currentVertex.Point.x, currentVertex.Point.y + 1);
            // openSet.Exists(x=>x.Point==upNeighbor); //=> "for each value of x, treat it as x.Point and do this comparison
            if (DoesListContainPoint(openSet, rightNeighbor) == false && DoesListContainPoint(closedSet, rightNeighbor) == false)
            {
                if (IsPointNavigable(indexGrid, rightNeighbor))
                {
                    SpaceSlot relativeSlot = grid[rightNeighbor.x, rightNeighbor.y];
                    SearchVertex upNeighborVertex = new SearchVertex(rightNeighbor, currentVertex, PointMemoryConfiguration.right, relativeSlot.FirstPointInSpace.z, relativeSlot.Slope);
                    tempSet.Add(upNeighborVertex);
                    relativeSlot.plantChance += 1f;
                }
            }

            //down neighbor
            Vector2Int leftNeighbor = new Vector2Int(currentVertex.Point.x, currentVertex.Point.y - 1);

            if (DoesListContainPoint(openSet, leftNeighbor) == false && DoesListContainPoint(closedSet, leftNeighbor) == false)
            {
                if (IsPointNavigable(indexGrid, leftNeighbor))
                {
                    SpaceSlot relativeSlot = grid[leftNeighbor.x, leftNeighbor.y];
                    SearchVertex downNeighborVertex = new SearchVertex(leftNeighbor, currentVertex, PointMemoryConfiguration.left, relativeSlot.FirstPointInSpace.z, relativeSlot.Slope);
                    tempSet.Add(downNeighborVertex);
                    relativeSlot.plantChance += 1f;
                }
            }

            //left neighbor
            Vector2Int downNeighbor = new Vector2Int(currentVertex.Point.x - 1, currentVertex.Point.y);

            if (DoesListContainPoint(openSet, downNeighbor) == false && DoesListContainPoint(closedSet, downNeighbor) == false)
            {
                if (IsPointNavigable(indexGrid, downNeighbor))
                {
                    SpaceSlot relativeSlot = grid[downNeighbor.x, downNeighbor.y];
                    SearchVertex leftNeighborVertex = new SearchVertex(downNeighbor, currentVertex, PointMemoryConfiguration.down, relativeSlot.FirstPointInSpace.z, relativeSlot.Slope);
                    tempSet.Add(leftNeighborVertex);
                    relativeSlot.plantChance += 1f;
                }
            }

            //right neighbor

            Vector2Int upNeighbor = new Vector2Int(currentVertex.Point.x + 1, currentVertex.Point.y);

            if (DoesListContainPoint(openSet, upNeighbor) == false && DoesListContainPoint(closedSet, upNeighbor) == false)
            {
                if (IsPointNavigable(indexGrid, upNeighbor))
                {
                    SpaceSlot relativeSlot = grid[upNeighbor.x, upNeighbor.y];
                    SearchVertex rightNeighborVertex = new SearchVertex(upNeighbor, currentVertex, PointMemoryConfiguration.up, relativeSlot.FirstPointInSpace.z, relativeSlot.Slope);
                    tempSet.Add(rightNeighborVertex);
                    relativeSlot.plantChance += 1f;
                }
            }
            //tempset.shuffle
            tempSet = ListShuffleInPlace(tempSet);
            float tempBendyness = _bendyness; //if bendyness is over .8 then not enough tiles spawn
            float tempBendySeparation = _bendySeparation;


            foreach (SearchVertex n in tempSet)
            {
                tempBendySeparation = _bendySeparation + (n.Slope) * 100;

                float totalBend = tempBendyness + ExtensionMethods.Remap(tempBendySeparation, 0, 1, 0, .4f);

                totalBend = ExtensionMethods.Remap(totalBend, 0, 2, 0, .8f);
                // Debug.Log((_bendySeparation + n.Slope * 100));
                if (n.Configuration.Equals(PointMemoryConfiguration.down))
                {
                    continue;
                }

                // float r = UnityEngine.Random.Range(0f, 1f);
                if (n.Configuration.Equals(PointMemoryConfiguration.up))
                {
                    if (UnityEngine.Random.value < totalBend)
                        continue;
                }

                if (n.Configuration.Equals(PointMemoryConfiguration.left))
                {
                    if (UnityEngine.Random.value * .1f >= totalBend)
                        continue;
                }
                if (n.Configuration.Equals(PointMemoryConfiguration.right))
                {
                    if (UnityEngine.Random.value * .1f >= totalBend)
                        continue;
                }
                // dydx[i] = Differentiate(x[i]);
                openSet.Add(n);
            }


            //foreach in tempset add to open set --> a purely random wandering. if we want to trend towards up, can assign different weights to shuffling
            //so its more likely so that up is first or last, or leave out a neighbor like the back neighbor, so that it never goes back
            closedSet.Add(currentVertex);
        }


        return new List<SpaceSlot>();
    }

    public List<T> ListShuffleInPlace<T>(List<T> l)
    {
        for (int i = l.Count - 1; i >= 1; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            T card = l[randomIndex];
            l[randomIndex] = l[i];
            l[i] = card;
        }
        return l;
    }




    bool IsPointNavigable(SpaceSlot[,] indexGrid, Vector2Int point)
    {
        if (IsPointInGrid(point) == false)
            return false;
        if (indexGrid[point.x, point.y].navigableVal == 1)
            return false;
        else
            return true;
    }
    bool IsPointInGrid(Vector2Int point)
    {
        if (point.x < 0 || point.x >= trailDistance)
            return false;
        if (point.y < 0 || point.y >= trailWidth)
            return false;
        return true;
    }
    bool DoesListContainPoint(List<SearchVertex> list, Vector2Int point)
    {
        foreach (SearchVertex searchVertex in list)
        {
            if (searchVertex.Point == point)
                return true;
        }
        return false;
    }

    ///<BATCH><RELATED>///

    bool[,,] _wallGrid;
    bool[,,] _nextWallGrid;
    Vector3[,,] _positions;
    Vector3[,,] _scales;
    public int batchIndexMax = 1023;

    List<List<ObjDataGen>> _batches = new List<List<ObjDataGen>>();
    public Mesh objMesh;
    public Material objMat;
    void AddObj(List<ObjDataGen> currBatch, int i, int j, int k)
    {
        currBatch.Add(new ObjDataGen(_positions[i, j, k], Vector3.one, Quaternion.identity));
    }
    List<ObjDataGen> BuildNewBatch()
    {
        return new List<ObjDataGen>();
    }




    public class SearchVertex : IComparable<SearchVertex>
    {
        public SearchVertex()
        {

        }
        public SearchVertex(Vector2Int point, SearchVertex parent, PointMemoryConfiguration configuration, float altitude, float slope)
        {
            Point = point;
            Parent = parent;
            Configuration = configuration;
            Altitude = altitude;
            Slope = slope;
        }
        public SearchVertex(Vector2Int point, SearchVertex parent, PointMemoryConfiguration configuration)
        {
            Point = point;
            Parent = parent;
            Configuration = configuration;
        }
        public float Altitude;
        public Vector2Int Point;
        public SearchVertex Parent;
        public float Slope;
        public PointMemoryConfiguration Configuration;
        int Cost;

        public int CompareTo(SearchVertex obj)
        {
            if (Point.x < obj.Point.x)
                return -1;
            else if (Point.x > obj.Point.x)
                return 1;
            else
                return 0;
        }
    }


    /// <summary>
    /// This class holds the data for each object that will be batched
    /// </summary>
    public class ObjDataGen
    {
        //location data
        public Vector3 pos;
        public Vector3 scale;
        public Quaternion rot;

        //matrix conversion
        public Matrix4x4 matrix
        {
            get
            {
                return Matrix4x4.TRS(pos, rot, scale);
            }
        }

        //constructor
        public ObjDataGen(Vector3 pos, Vector3 scale, Quaternion rot)
        {
            this.pos = pos;
            this.scale = scale;
            this.rot = rot;
        }
    }


}
public enum PointMemoryConfiguration { up, down, right, left, count };