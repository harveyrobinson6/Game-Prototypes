using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundScroll : MonoBehaviour
{
    Vector3 newStart;
    Vector3 bg2Start;

    float cloudSpeed = 0.3f;

    [SerializeField] Transform bg1;
    [SerializeField] Transform bg2;
    [SerializeField] Transform bg3;

    private void Start()
    {
        newStart = bg1.position;
        bg2Start = bg2.position;
    }

    private void FixedUpdate()
    {
        Vector3 temp =  bg1.position;
        temp.x += cloudSpeed;
        bg1.position = temp;

        temp = bg2.position;
        temp.x += cloudSpeed;
        bg2.position = temp;

        if (bg1.position.x >= Mathf.Abs(bg2Start.x))
        {
            bg1.position = bg2Start;
        }
        if (bg2.position.x >= Mathf.Abs(bg2Start.x))
        {
            bg2.position = bg2Start;
        }
    }
}
