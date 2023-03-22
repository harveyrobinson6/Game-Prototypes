using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    NONE,
    FOLLOW_CURSOR,
    SELECTING_UNIT_OVERVIEW,
    DESELECTING_UNIT_OVERVIEW,
    UNIT_OVERVIEW
}

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform cursor;
    [SerializeField] Transform cam;

    [SerializeField] float smoothSpeed = 10f;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 UnitOverviewPos;

    public CameraState CameraState;

    private void Start()
    {
        CameraState = CameraState.FOLLOW_CURSOR;
    }

    private void FixedUpdate()
    {
        switch (CameraState)
        {
            case CameraState.NONE:
                break;

            case CameraState.FOLLOW_CURSOR:

                {
                    Vector3 desriedPos = cursor.position + offset;
                    Vector3 smoothedPos = Vector3.Lerp(transform.position, desriedPos, smoothSpeed * Time.deltaTime);
                    transform.position = smoothedPos;
                }

                break;

            case CameraState.SELECTING_UNIT_OVERVIEW:

                {
                    Vector3 desiredPos = cursor.position + UnitOverviewPos;
                    Vector3 smoothedPos = Vector3.Lerp(cam.position, desiredPos, smoothSpeed * Time.deltaTime);
                    cam.position = smoothedPos;
                }

                break;

            case CameraState.DESELECTING_UNIT_OVERVIEW:

                {
                    Vector3 smoothedPos = Vector3.Lerp(cam.position, transform.position, smoothSpeed * Time.deltaTime);
                    cam.position = smoothedPos;
                }

                break;

            case CameraState.UNIT_OVERVIEW:
                break;
        }
    }
}
