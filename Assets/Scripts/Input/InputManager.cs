using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

enum InputState
{
    ACCEPTING_INPUT,
    PICKUP_UP_UNIT,
    UNIT_MOVING,
    NO_INPUT
}

public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class InputManager : MonoBehaviour
{
    GridMovement GridMovement;
    InputState InputState;
    [SerializeField] Cursor cursor;
    [SerializeField] BattleSceneManager BSM;
    Transform SelectedUnit = null;

    private void Awake()
    {
        GridMovement = new GridMovement();
        InputState = InputState.ACCEPTING_INPUT;

        GridMovement.CursorMovement.Up.performed += ctx => Cardinals(Direction.UP);
        GridMovement.CursorMovement.Down.performed += ctx => Cardinals(Direction.DOWN);
        GridMovement.CursorMovement.Left.performed += ctx => Cardinals(Direction.LEFT);
        GridMovement.CursorMovement.Right.performed += ctx => Cardinals(Direction.RIGHT);
        GridMovement.CursorMovement.Select.performed += ctx => Select();
        GridMovement.CursorMovement.Back.performed += ctx => Back();
    }

    private void OnEnable()
    {
        GridMovement.Enable();
    }

    private void OnDisable()
    {
        GridMovement.Disable();
    }

    void Cardinals(Direction dir)
    {
        switch (InputState)
        {
            case InputState.ACCEPTING_INPUT:
                switch (dir)
                {
                    case Direction.UP:
                        cursor.CardinalDirections(Direction.UP);
                        break;
                    case Direction.DOWN:
                        cursor.CardinalDirections(Direction.DOWN);
                        break;
                    case Direction.LEFT:
                        cursor.CardinalDirections(Direction.LEFT);
                        break;
                    case Direction.RIGHT:
                        cursor.CardinalDirections(Direction.RIGHT);
                        break;
                }
                break;

            case InputState.PICKUP_UP_UNIT:
                switch (dir)
                {
                    case Direction.UP:
                        cursor.CardinalDirections(Direction.UP);
                        BSM.DisplayGhostSprites(cursor.transform, SelectedUnit.transform.position);
                        break;
                    case Direction.DOWN:
                        cursor.CardinalDirections(Direction.DOWN);
                        BSM.DisplayGhostSprites(cursor.transform, SelectedUnit.transform.position);
                        break;
                    case Direction.LEFT:
                        cursor.CardinalDirections(Direction.LEFT);
                        BSM.DisplayGhostSprites(cursor.transform, SelectedUnit.transform.position);
                        break;
                    case Direction.RIGHT:
                        cursor.CardinalDirections(Direction.RIGHT);
                        BSM.DisplayGhostSprites(cursor.transform, SelectedUnit.transform.position);
                        break;
                }

                break;

            case InputState.UNIT_MOVING:

                break;

            case InputState.NO_INPUT:

                break;
        }
    }

    void Select()
    {
        switch (InputState)
        {
            case InputState.ACCEPTING_INPUT:
                UnitPickup(cursor.transform);
                break;

            case InputState.PICKUP_UP_UNIT:
                break;

            case InputState.UNIT_MOVING:

                break;

            case InputState.NO_INPUT:
                break;
        }
    }

    void Back()
    {

    }

    void UnitPickup(Transform cursor)
    {
        //Transform unit;

        if (BSM.grid.UnitAtPos(cursor.position, out SelectedUnit))  //MODIFY THIS LATER TO GET ENEMY INFO TOO
        {
            InputState = InputState.PICKUP_UP_UNIT;
        }

        Debug.Log("empty tile");
    }

}
