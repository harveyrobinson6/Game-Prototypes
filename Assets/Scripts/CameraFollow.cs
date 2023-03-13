using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform cursor;

    [SerializeField] float smoothSpeed = 10f;
    [SerializeField] Vector3 offset;

    private void FixedUpdate()
    {
        Vector3 desriedPos = cursor.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desriedPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;

        //transform.LookAt(cursor);
    }
}
