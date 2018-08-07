using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMove : MonoBehaviour
{
    public Camera cam;
    public float orthoZoomSpeed = .5f;
    public Vector2 zoomMinMax;
    public float moveSpeed = .5f;

    bool centered;
    float distance;

    CharacterController controller;

    // Mouse Controlls
    Vector3 prevMousePos;

    // Touch Controlls
    bool touch0;
    bool touch1;
    Vector2 prevTouchPos0;
    Vector2 prevTouchPos1;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()
            || EventSystem.current.IsPointerOverGameObject(0)
            || EventSystem.current.currentSelectedGameObject != null)
            return;

        #region Mouse
        // benutzen wir nicht
        #region Move Mouse
        if (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                prevMousePos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                centered = false;
                Vector3 pos = cam.ScreenToViewportPoint(Input.mousePosition - prevMousePos);
                Vector3 move = new Vector3(pos.x, 0, pos.y * 2);

                transform.Translate(-move * moveSpeed * cam.orthographicSize * Time.deltaTime * 15f, Space.Self);

                prevMousePos = Input.mousePosition;
            }
        }
        #endregion

        #region Zoom Mouse 
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * cam.orthographicSize * orthoZoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, zoomMinMax.x, zoomMinMax.y);
        }
        #endregion
        #endregion

        #region check touches
        if (Input.touchCount < 2)
        {
            touch0 = false;
            touch1 = false;
            return;
        }
        #endregion

        #region Zoom
        if (!touch1)
        {
            prevTouchPos0 = Input.GetTouch(0).position;
            prevTouchPos1 = Input.GetTouch(1).position;
            touch1 = true;
        }

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = prevTouchPos0;
        Vector2 touchOnePrevPos = prevTouchPos1;

        float prevTouchDeltaMag = Vector2.Distance(touchZeroPrevPos, touchOnePrevPos);
        float touchDeltaMag = Vector2.Distance(touchZero.position, touchOne.position);

        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed * cam.orthographicSize * .001f;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, zoomMinMax.x, zoomMinMax.y);

        prevTouchPos0 = Input.GetTouch(0).position;
        prevTouchPos1 = Input.GetTouch(1).position;
        #endregion

        // benutzen wir nicht
        #region Move
        if (false)
        {
            if (Input.touchCount == 1)
            {
                if (!touch0)
                {
                    prevTouchPos0 = Input.GetTouch(0).position;
                    touch0 = true;
                }
                Touch touch = Input.GetTouch(0);

                Vector3 direction = touch.position - prevTouchPos0;
                direction = new Vector3(direction.x, 0, direction.y);
                if (direction.magnitude > 1)
                {
                    centered = false;

                    transform.Translate(-direction * moveSpeed * cam.orthographicSize * Time.deltaTime * .03f);
                }

                prevTouchPos0 = Input.GetTouch(0).position;
            }
        }
        #endregion
    }
}
