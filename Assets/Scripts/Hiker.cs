using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum HikerState { dorment, active, waiting, finished }
public enum HikerDirection { goingUp, goingDown }
public class Hiker : MonoBehaviour
{
    [SerializeField] List<Material> potentialMaterials = new List<Material>();
    [SerializeField] MeshRenderer highlightRenderer;
    [SerializeField] MeshRenderer hikerBody;
    [SerializeField] TextMeshPro firstNameTMP, firstNameMapTMP;
    [SerializeField] TextMeshPro lastNameTMP, lastNameMapTMP;
    [SerializeField] string firstName;
    [SerializeField] string lastName;
    HikerState myState = HikerState.dorment;
    HikerDirection myDirection;
    LinkedListNode<SpaceSlot> currentSpaceSlot;
    float moveSeconds;
    bool moving = false;
    Transform camTransform;
    Quaternion originalRotation;

    public void Init(string firstName, string lastName)
    {
        this.firstName = firstName;
        this.lastName = lastName;
        this.firstNameTMP.text = firstName;
        this.firstNameMapTMP.text = firstName;
        this.lastNameTMP.text = lastName;
        this.lastNameMapTMP.text = lastName;
        this.moveSeconds = UnityEngine.Random.Range(.3f, 1f);
        this.originalRotation = transform.rotation;
        this.camTransform = Camera.main.GetComponent<Transform>();
        this.hikerBody.material = potentialMaterials[UnityEngine.Random.Range(0, potentialMaterials.Count)];
        float currentScale = hikerBody.transform.localScale.y;
        float randScale = UnityEngine.Random.Range(.3f, 1f) * currentScale;
        this.hikerBody.transform.localScale = new Vector3(randScale, currentScale, randScale);
    }

    void FixedUpdate()
    {
        firstNameTMP.transform.rotation = camTransform.rotation * originalRotation;
        lastNameTMP.transform.rotation = camTransform.rotation * originalRotation;
    }

    public void ActivateHiker(System.Collections.Generic.LinkedListNode<SpaceSlot> startSlot)
    {
        myState = HikerState.active;
        if (startSlot.Value.Equals(Services.TrailGenerator.trail.First.Value))
        {
            myDirection = HikerDirection.goingUp;
        }
        else if (startSlot.Value.Equals(Services.TrailGenerator.trail.Last.Value))
        {
            myDirection = HikerDirection.goingDown;
        }
        currentSpaceSlot = startSlot;
        MoveToCurrentTrailSlot(currentSpaceSlot.Value.FirstPointInSpace);
        StartCoroutine(HikeRoutine());
    }

    IEnumerator HikeRoutine()
    {
        if (myDirection.Equals(HikerDirection.goingUp))
        {
            while (currentSpaceSlot.Next != null)
            {
                if (!moving)
                {
                    yield return MoveSlotRoutine(currentSpaceSlot.Next.Value);
                    while (moving)
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(moveSeconds);
                    yield return null;
                }
            }
            yield break;
        }
        else if (myDirection.Equals(HikerDirection.goingDown))
        {
            while (currentSpaceSlot.Previous != null)
            {
                if (!moving)
                {
                    yield return MoveSlotRoutine(currentSpaceSlot.Previous.Value);
                    while (moving)
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(moveSeconds);
                    yield return null;
                }
            }
            yield break;
        }
    }

    void FinishHike()
    {
        myState = HikerState.finished;
    }

    IEnumerator MoveSlotRoutine(SpaceSlot targetSlot)
    {
        moving = true;

        //Check that there is space to move into.
        if (!targetSlot.HasSpace())
        {
            StartWait();
            while (!targetSlot.HasSpace())
            {
                yield return null;
            }
            EndWait();
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = targetSlot.PutHikerInPoint(this);

        //Check again.
        if (endPos.Equals(Vector3.zero))
        {
            StartWait();
            while (endPos.Equals(Vector3.zero))
            {
                endPos = targetSlot.PutHikerInPoint(this);
                yield return null;
            }
            EndWait();
        }

        //Clear for future hikers.
        currentSpaceSlot.Value.ResetSpaceSlotFill(this);

        //Movement.
        float t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime / moveSeconds;
            yield return null;
        }

        currentSpaceSlot = targetSlot.myLink;
        MoveToCurrentTrailSlot(endPos);


        //Check if at last tile.
        if (myDirection.Equals(HikerDirection.goingUp))
        {
            if (currentSpaceSlot.Equals(Services.TrailGenerator.trail.Last))
            {
                FinishHike();
            }
        }
        else if (myDirection.Equals(HikerDirection.goingDown))
        {
            if (currentSpaceSlot.Equals(Services.TrailGenerator.trail.First))
            {
                FinishHike();
            }
        }

        moving = false;
        yield break;
    }

    void MoveToCurrentTrailSlot(Vector3 pos)
    {
        transform.position = pos;
    }

    void StartWait()
    {
        myState = HikerState.waiting;
    }

    void EndWait()
    {
        myState = HikerState.active;
    }
    public void HighlightHikerOnMap()
    {
        firstNameMapTMP.gameObject.SetActive(true);
        lastNameMapTMP.gameObject.SetActive(true);
        highlightRenderer.gameObject.SetActive(true);
    }
    public void UnHighlightHikerOnMap()
    {
        firstNameMapTMP.gameObject.SetActive(false);
        lastNameMapTMP.gameObject.SetActive(false);
        highlightRenderer.gameObject.SetActive(false);
    }
}
