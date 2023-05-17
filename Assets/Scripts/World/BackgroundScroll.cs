using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BackgroundScroll : MonoBehaviour
{
    Vector3 newStart;
    Vector3 bg2Start;

    float cloudSpeed = 40;

    [SerializeField] Transform bg1;
    [SerializeField] Transform bg2;
    [SerializeField] Transform bg3;

    private void Start()
    {
        B1Move();
        B2Move();
        B3Move();
    }

    void B1Move()
    {
        bg1.DOMoveX(0, cloudSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {
            bg1.DOMoveX(1250, cloudSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                bg1.DOMoveX(2500, cloudSpeed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    bg1.position = new Vector3(-1250, 132, 1200);
                    B1Move();
                });
            });
        });
    }

    void B2Move()
    {
        bg2.DOMoveX(1250, cloudSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {
            bg2.DOMoveX(2500, cloudSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                bg2.position = new Vector3(-1250, 132, 1200);

                bg2.DOMoveX(0, cloudSpeed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    B2Move();
                });
            });
        });
    }

    void B3Move()
    {
        bg3.DOMoveX(2500, cloudSpeed).SetEase(Ease.Linear).OnComplete(() =>
        {
            bg3.position = new Vector3(-1250, 132, 1200);

            bg3.DOMoveX(0, cloudSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                bg3.DOMoveX(1250, cloudSpeed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    B3Move();
                });
            });
        });
    }
}
