using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CameraState
{
    NONE,
    FOLLOW_CURSOR,
    SELECTING_UNIT_OVERVIEW,
    DESELECTING_UNIT_OVERVIEW,
    UNIT_OVERVIEW,
    //BATTLE_FORECAST,
    BATTLE
}

public class CameraFollow : MonoBehaviour
{
    //[SerializeField] Transform cursor;
    //[SerializeField] Transform cam;

    [SerializeField] CinemachineFreeLook MainCamera;
    [SerializeField] CinemachineVirtualCamera UnitOverviewCamera;
    [SerializeField] CinemachineVirtualCamera BattleCamera;
    //[SerializeField] CinemachineVirtualCamera BattleForecastCamera;

    [SerializeField] Vector3 unitOverviewOffset;

    public CameraState CameraState;

    private void Start()
    {
        CameraState = CameraState.FOLLOW_CURSOR;
    }
    
    public void MoveToUnitOverview(Transform lookAt)
    {
        UnitOverviewCamera.LookAt = lookAt;
        Vector3 newPos = lookAt.position;
        newPos = newPos + unitOverviewOffset;
        UnitOverviewCamera.transform.position = newPos;

        CameraState = CameraState.UNIT_OVERVIEW;
        MainCamera.Priority = 0;
        UnitOverviewCamera.Priority = 1;
        BattleCamera.Priority = 0;
        //BattleForecastCamera.Priority = 0;
    }

    public void MoveToBattle()
    {
        CameraState = CameraState.BATTLE;
        MainCamera.Priority = 0;
        UnitOverviewCamera.Priority = 0;
        BattleCamera.Priority = 1;
        //BattleForecastCamera.Priority = 0;
    }
    /*
    public void MoveToBattleForecast()
    {
        CameraState = CameraState.BATTLE_FORECAST;
        MainCamera.Priority = 0;
        UnitOverviewCamera.Priority = 0;
        BattleCamera.Priority = 0;
        BattleForecastCamera.Priority = 1;
    }
    */
    public void MoveToGrid()
    {
        CameraState = CameraState.FOLLOW_CURSOR;
        MainCamera.Priority = 1;
        UnitOverviewCamera.Priority = 0;
        BattleCamera.Priority = 0;
        //BattleForecastCamera.Priority = 0;
    }

    public void NewCameraVal(float val)
    {
        MainCamera.m_YAxis.Value = val;
    }
}
