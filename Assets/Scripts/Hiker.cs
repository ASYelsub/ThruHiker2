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
        MoveToCurrentTrailSlot();
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
                    yield return MoveUpSlot();
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

    void MoveToCurrentTrailSlot()
    {
        transform.position = currentSpaceSlot.Value.SecondPointInSpace;
    }

}
