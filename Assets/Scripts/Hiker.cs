using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public enum HikerState { dorment, active, finished }
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

    public void Init(string firstName, string lastName)
    {
        this.firstName = firstName;
        this.lastName = lastName;
        this.firstNameTMP.text = firstName;
        this.lastNameTMP.text = lastName;
        this.moveSeconds = Random.value;
    }

    public void ActivateHiker(System.Collections.Generic.LinkedListNode<SpaceSlot> startSlot)
    {
        myState = HikerState.active;
        if (startSlot.Equals(Services.TrailGenerator.trail.First.Value)) { myDirection = HikerDirection.goingUp; }
        if (startSlot.Equals(Services.TrailGenerator.trail.Last.Value)) { myDirection = HikerDirection.goingDown; }
        currentSpaceSlot = startSlot;
        MoveToCurrentTrailSlot();
    }

    IEnumerator HikeRoutine()
    {
        while (!currentSpaceSlot.Next.Equals(null))
        {
            if (moving)
            {

            }
            else
            {

            }
        }
        yield break;
    }

    public void FinishHike()
    {
        myState = HikerState.finished;
    }

    IEnumerator MoveUpSlot()
    {
        moving = true;

        float t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(currentSpaceSlot.Value.SecondPointInSpace, currentSpaceSlot.Next.Value.SecondPointInSpace, t);
            t += Time.deltaTime / moveSeconds;
            yield return null;
        }
        currentSpaceSlot = currentSpaceSlot.Next;
        MoveToCurrentTrailSlot();

        if (currentSpaceSlot.Equals(Services.TrailGenerator.trail.Last))
        {
            FinishHike();
        }
        moving = false;
        yield break;
    }

    IEnumerator MoveDownSlot()
    {
        moving = true;
        float t = 0f;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(currentSpaceSlot.Value.SecondPointInSpace, currentSpaceSlot.Previous.Value.SecondPointInSpace, t);
            t += Time.deltaTime / moveSeconds;
            yield return null;
        }
        currentSpaceSlot = currentSpaceSlot.Previous;
        MoveToCurrentTrailSlot();
        if (currentSpaceSlot.Equals(Services.TrailGenerator.trail.First))
        {
            FinishHike();
        }
        moving = false;
        yield break;
    }

    void MoveToCurrentTrailSlot()
    {
        transform.position = currentSpaceSlot.Value.SecondPointInSpace;
    }

}
