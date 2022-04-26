using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpaceSlot : MonoBehaviour
{
    //unsure what to do with this other than like it'll interact
    //w the visual part of Unity?
    public List<GameObject> childObjects = new List<GameObject>();
    [HideInInspector] public int navigableVal = 0;
    [HideInInspector] public Vector2Int point;
    private SlotTypes slotType;
    private Hiker hikerInFirstPoint;
    private Hiker hikerInSecondPoint;
    private Vector3 firstPointInSpace; //where character slots into, usually the kids, on "the left" of trail
    private Vector3 secondPointInSpace; //where a second character slots into... eithr a kid or another hiker
                                        //can be irritating for other hikers if kid slots into here and they cant pass
    private bool firstPointFilled; //if a character is currently at the location
    private bool secondPointFilled;
    private SlotTypes slotTypes;
    SlotTypes mySlotType = SlotTypes.Plants;
    private Vector3 v1;
    private Vector3 v2;

    private Material slotMat;
    private float slope;
    [HideInInspector] public bool isTrail = false;
    public Transform plantTransform, treeTransform, groundTransform, rockTransform;
    public float plantChance = 0;
    [HideInInspector] public int trailInt;
    public SpaceSlot(Vector3 firstPointInSpace, Vector3 secondPointInSpace, GameObject slotPrefab, GameObject trailHolder, Material slotMat, float slope)
    {
        this.firstPointInSpace = firstPointInSpace;
        this.secondPointInSpace = secondPointInSpace;
        this.firstPointFilled = false;
        this.slotMat = slotMat;
        this.slope = slope;

        this.v1 = firstPointInSpace;
        GameObject newSlot = Instantiate(slotPrefab, firstPointInSpace, Quaternion.identity, trailHolder.transform);
        newSlot.GetComponent<MeshRenderer>().material = slotMat;

    }

    public void Init(float slope)
    {
        this.slope = slope;
        this.firstPointInSpace = gameObject.transform.position;
    }

    public void SetSlotToTree()
    {
        mySlotType = SlotTypes.Tree;
        SetPlantOff();
        SetRockOff();
        SetTreeOn();
    }

    public void SetSlotToPlant()
    {
        mySlotType = SlotTypes.Plants;
        SetRockOff();
        SetTreeOff();
        SetPlantOn();
    }

    public void SetSlotToRock()
    {
        mySlotType = SlotTypes.Rock;
        SetPlantOff();
        SetTreeOff();
        SetRockOn();
    }

    public void SetSlotToCull()
    {
        Destroy(this);
    }
    public void SetSlotToPath()
    {
        mySlotType = SlotTypes.Path;
        groundTransform.gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
        SetPlantOff();
        SetRockOff();
        SetTreeOff();
    }
    public void PutHikerInFirstPoint(Hiker hiker)
    {
        if (hikerInFirstPoint == null)
        {
            this.hikerInFirstPoint = hiker;
        }
        else
        {
            // Console.WriteLine(this.hikerInFirstPoint.CodeName + "is already in this slot's first point.");
            PutHikerInSecondtPoint(hiker);
            Console.WriteLine("Attempting to put " + hiker + " in second point.");
        }

    }

    public void PutHikerInSecondtPoint(Hiker hiker)
    {
        if (HikerInSecondPoint == null)
        {
            this.HikerInSecondPoint = hiker;
        }
        else
        {
            // Console.WriteLine(this.hikerInSecondPoint.CodeName + "is already in this slot's second point.");
        }

    }


    //Getters and Setters
    public float Slope
    {
        get { return slope; }
        set { slope = value; }
    }
    public SlotTypes SlotType
    {
        get { return slotType; }
        set { slotType = value; }
    }
    public Hiker HikerInFirstPoint
    {
        get { return hikerInFirstPoint; }
        set { hikerInFirstPoint = value; }
    }
    public Hiker HikerInSecondPoint
    {
        get { return hikerInSecondPoint; }
        set { hikerInSecondPoint = value; }
    }
    public Vector3 FirstPointInSpace
    {
        get { return firstPointInSpace; }
        set { firstPointInSpace = value; }
    }
    public Vector3 SecondPointInSpace
    {
        get { return secondPointInSpace; }
        set { secondPointInSpace = value; }
    }
    public bool FirstPointFilled
    {
        get { return firstPointFilled; }
        set { firstPointFilled = value; }
    }
    public bool SecondPointFilled
    {
        get { return secondPointFilled; }
        set { secondPointFilled = value; }
    }


    private void SetTreeOn()
    {
        treeTransform.gameObject.SetActive(true);
    }
    private void SetTreeOff()
    {
        treeTransform.gameObject.SetActive(false);
    }
    private void SetRockOn()
    {
        rockTransform.gameObject.SetActive(true);
    }
    private void SetRockOff()
    {
        rockTransform.gameObject.SetActive(false);
    }
    private void SetPlantOn()
    {
        plantTransform.gameObject.SetActive(true);
    }
    private void SetPlantOff()
    {
        plantTransform.gameObject.SetActive(false);
    }
}

public enum SlotTypes
{
    //to add more types you also need to go into slotGenerator and edit:
    //slotTypeFromInt and the top range of intRandom
    Rock,
    Cairn,
    Tree,
    Plants,
    Stream,
    Flooded,
    Path,
    Cull
}
