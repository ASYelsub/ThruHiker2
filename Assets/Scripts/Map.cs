using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    bool mapEnabled = false;
    [SerializeField] Camera mapCamera;
    float panSpeed = 3f;

    void Start()
    {
        DisableMap();
    }

    public void MapUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMap();
        }
        if (!mapEnabled)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            ScrollMap();
        }
        if (Input.GetMouseButton(1))
        {
            ZoomMap();
        }
    }

    public void ToggleMap()
    {
        if (mapEnabled)
        {
            DisableMap();
            return;
        }
        if (!mapEnabled)
        {
            EnableMap();
            return;
        }
    }

    void EnableMap()
    {
        mapCamera.gameObject.SetActive(true);
        gameObject.SetActive(true);
        mapEnabled = true;
        Services.Player.DisablePlayerControls();
    }

    void DisableMap()
    {
        mapCamera.gameObject.SetActive(false);
        gameObject.SetActive(false);
        mapEnabled = false;
        Services.Player.EnablePlayerControls();
    }

    void ZoomMap()
    {
        float xIncrease = -Input.GetAxis("Mouse X") * panSpeed;
        float yIncrease = -Input.GetAxis("Mouse Y") * panSpeed;

        if (mapCamera.orthographicSize <= 7f && xIncrease + yIncrease < 0f)
        {
            return;
        }
        if (mapCamera.orthographicSize >= 30f && xIncrease + yIncrease > 0f)
        {
            return;
        }

        mapCamera.orthographicSize += xIncrease;
        mapCamera.orthographicSize += yIncrease;
    }

    public void ScrollMap()
    {
        float mouseY = -Input.GetAxis("Mouse Y");

        if (mapCamera.transform.position.z >= Services.TrailGenerator.trail.Last.Value.transform.position.z)
        {
            if (mouseY > 0)
            {
                return;
            }
        }

        if (mapCamera.transform.position.z <= Services.TrailGenerator.trail.First.Value.transform.position.z)
        {
            if (mouseY < 0)
            {
                return;
            }
        }

        mapCamera.transform.position += new Vector3(0, 0, 1f) * mouseY;
    }

}
