using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Map : MonoBehaviour
{
    [SerializeField] Camera mapCamera;

    Hiker highlightedHiker;
    float panSpeed = 3f;
    bool mapEnabled = false;

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

        if (!mapEnabled)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {

            if (hit.transform.Equals(transform))
            {
                ray = mapCamera.ViewportPointToRay(hit.textureCoord);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.GetComponentInParent<Hiker>() != null)
                        hit.transform.gameObject.GetComponentInParent<Hiker>().HighlightHikerOnMap();
                }
                else
                {
                    foreach (Hiker h in Services.HikerGenerator.activeHikers)
                    {
                        h.UnHighlightHikerOnMap();
                    }
                }
            }


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
        Time.timeScale = 0f;
        mapCamera.gameObject.SetActive(true);
        gameObject.SetActive(true);
        mapEnabled = true;
        Services.Player.DisablePlayerControls();
    }

    void DisableMap()
    {
        Time.timeScale = 1f;
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
            if (mouseY > 0) { return; }
        }
        if (mapCamera.transform.position.z <= Services.TrailGenerator.trail.First.Value.transform.position.z)
        {
            if (mouseY < 0) { return; }
        }
        mapCamera.transform.position += new Vector3(0, 0, 1f) * mouseY;


        float mouseX = -Input.GetAxis("Mouse X");
        if (mapCamera.transform.localPosition.x >= 8f)
        {
            if (mouseX > 0) { return; }
        }
        if (mapCamera.transform.localPosition.x <= -8f)
        {
            if (mouseX < 0) { return; }
        }
        mapCamera.transform.localPosition += new Vector3(1f, 0, 0) * mouseX;
    }

}
