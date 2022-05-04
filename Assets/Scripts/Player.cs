using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [HideInInspector] public System.Collections.Generic.LinkedListNode<SpaceSlot> myCurrentSlot;
    [SerializeField] Camera playerCamera;
    void Awake()
    {
        Services.Player = this;
    }

    public void Init()
    {
        // SetPlayerPosOnTrailStart();
    }


    public void SetPlayerPosOnTrailStart()
    {
        myCurrentSlot = Services.TrailGenerator.trail.First;
        transform.position = myCurrentSlot.Value.transform.position;
    }

    public void DisablePlayerControls()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
    }

    public void EnablePlayerControls()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        Services.CanvasDisplay.map.MapUpdate();

        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            if (_moving)
                return;
            MovementStep();
        }

    }

    //Also makes all the kids move forward one
    public void MovementStep()
    {
        if (myCurrentSlot.Next == null)
        {
            //void EndLevel()
            return;
        }
        StartCoroutine(MoveTransition(myCurrentSlot.Value, myCurrentSlot.Next.Value));
        myCurrentSlot = myCurrentSlot.Next;
    }

    [SerializeField] private float _moveTransitionSeconds;
    private bool _moving = false;
    IEnumerator MoveTransition(SpaceSlot oldSlot, SpaceSlot newSlot)
    {
        _moving = true;
        transform.position = oldSlot.transform.position;
        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / _moveTransitionSeconds;
            transform.position = Vector3.Lerp(oldSlot.transform.position, newSlot.transform.position, t);
            yield return null;
        }
        transform.position = newSlot.transform.position;
        _moving = false;
        yield break;
    }

}
