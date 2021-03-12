using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Camera myCamera;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float zoomSpeed;

    [SerializeField] private float maxX;
    [SerializeField] private float maxY;
    [SerializeField] private float minX;
    [SerializeField] private float minY;

    private bool moveLock;
    private bool zoomLock;

    // Start is called before the first frame update
    public void Init()
    {
        myCamera = GetComponent<Camera>();
        moveLock = true;
        zoomLock = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!moveLock)
        {
            if (Input.GetKey("w") && transform.position.y <= maxY)
            {
                transform.Translate(Vector3.up * cameraSpeed);
            }

            if (Input.GetKey("d") && transform.position.x <= maxX)
            {
                transform.Translate(Vector3.right * cameraSpeed);
            }

            if (Input.GetKey("s") && transform.position.y >= minY)
            {
                transform.Translate(Vector3.down * cameraSpeed);
            }

            if (Input.GetKey("a") && transform.position.x >= minX)
            {
                transform.Translate(Vector3.left * cameraSpeed);
            }
        }
        
        if(!zoomLock)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                myCamera.orthographicSize -= zoomSpeed;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                myCamera.orthographicSize += zoomSpeed;
            }
        }
        
    }

    public void SetCameraPos(float x, float y)
    {
        transform.position = new Vector3(x, y, -10);
    }

    public void SetCameraSize(float size)
    {
        myCamera.orthographicSize = size;
    }

    public void SetCameraBounds(float maxX_val, float minX_val, float maxY_val, float minY_val)
    {
        maxX = maxX_val;
        minX = minX_val;
        maxY = maxY_val;
        minY = minY_val;
    }

    public void MoveCameraTo(float x, float y) // move the camera to (camera actual moves, not jump) NOT DONE
    {
        // use coroutine and disable all other input?
    }

    public void SetMoveLock(bool val)
    {
        moveLock = val;
    }

    public void SetZoomLock(bool val)
    {
        zoomLock = val;
    }

}
