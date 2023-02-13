using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

enum InputState
{
    ACCEPTING_INPUT,
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

    private void Awake()
    {
        GridMovement = new GridMovement();
        InputState = InputState.NO_INPUT;

        GridMovement.CursorMovement.Up.performed += ctx => cursor.CardinalDirections(Direction.UP);
        GridMovement.CursorMovement.Down.performed += ctx => cursor.CardinalDirections(Direction.DOWN);
        GridMovement.CursorMovement.Left.performed += ctx => cursor.CardinalDirections(Direction.LEFT);
        GridMovement.CursorMovement.Right.performed += ctx => cursor.CardinalDirections(Direction.RIGHT);
    }

    private void OnEnable()
    {
        GridMovement.Enable();
    }

    private void OnDisable()
    {
        GridMovement.Disable();
    }
}
