using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum HikerState { dorment, active, waiting, finished }
public enum HikerDirection { goingUp, goingDown }
public class Hiker : MonoBehaviour
{
    [SerializeField] string firstName;
    [SerializeField] string lastName;
    [SerializeField] TextMeshPro firstNameTMP;
    [SerializeField] TextMeshPro lastNameTMP;
    HikerState myState = HikerState.dorment;
    HikerDirection myDirection;
    System.Collections.Generic.LinkedListNode<SpaceSlot> currentSpaceSlot;
    float moveSeconds;
    bool moving = false;
    Transform camTransform;
    Quaternion originalRotation;

    void FixedUpdate()
    {
        transform.rotation = camTransform.rotation * originalRotation;
    }

    public void Init(string firstName, string lastName)
    {
        this.firstName = firstName;
        this.lastName = lastName;
        this.firstNameTMP.text = firstName;
        this.lastNameTMP.text = lastName;
        this.moveSeconds = UnityEngine.Random.Range(.3f, 1f);
        this.originalRotation = transform.rotation;
        this.camTransform = Camera.main.GetComponent<Transform>();
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
            while (!currentSpaceSlot.Next.Equals(null))
            {
                if (!moving)
                {
                    yield return MoveUpSlotRoutine();
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

    public void FinishHike()
    {
        myState = HikerState.finished;
    }

    IEnumerator MoveUpSlotRoutine()
    {
        moving = true;

        //Check that there is space to move into.
        if (!currentSpaceSlot.Next.Value.HasSpace())
        {
            myState = HikerState.waiting;
            while (!currentSpaceSlot.Next.Value.HasSpace())
            {
                yield return null;
            }
            myState = HikerState.active;
        }

        Vector3 startPos = transform.position;
        Vector3 endPos = currentSpaceSlot.Next.Value.PutHikerInPoint(this);

        //Check again.
        if (endPos.Equals(Vector3.zero))
        {
            myState = HikerState.waiting;
            while (endPos.Equals(Vector3.zero))
            {
                endPos = currentSpaceSlot.Next.Value.PutHikerInPoint(this);
                yield return null;
            }
            myState = HikerState.active;
        }

        currentSpaceSlot.Value.ResetSpaceSlotFill(this);

        float t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime / moveSeconds;
            yield return null;
        }

        currentSpaceSlot = currentSpaceSlot.Next;
        MoveToCurrentTrailSlot(endPos);

        if (currentSpaceSlot.Equals(Services.TrailGenerator.trail.Last))
        {
            FinishHike();
        }
        moving = false;
        yield break;
    }

    void MoveToCurrentTrailSlot(Vector3 pos)
    {
        transform.position = pos;
    }

}
