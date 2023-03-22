using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using N_Grid;
using N_Entity;

enum InputState
{
    ACCEPTING_INPUT,
    ONLY_MOVEMENT,
    UNIT_CONTEXT_MENU,
    UNIT_OVERVIEW_MENU,
    PICKUP_UP_UNIT,
    UNIT_MOVING,
    CUSOR_MOVING,
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
    [SerializeField] BattleSceneManager BSM;
    [SerializeField] UIManager UIManager;
    Transform SelectedUnit = null;
    [SerializeField] Transform Cursor;

    int contextMenuSelectedItem = 0;

    List<Animator> animators;

    bool[] elementEnabled = { true, true, true };
    string[] elementType = { "Attack", "Inventory", "Stay" };
    Queue<Transform> UIPrefabs;
    [SerializeField] Transform[] uiprefabs;
    [SerializeField] Sprite[] sprites;
    List<Transform> UIElements;
    List<string> ElementType;
    Transform canvas;

    Vector3 oldCursorPos;
    Vector3 newCursorPos;
    float elapsedTime;
    float desiredDuration = 0.1f;
    bool pickup = false;
    [SerializeField] Transform cursorAnchor;

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
        GridMovement.CursorMovement.Menu.performed += ctx => MenuKey();
    }

    private void Start()
    {
        UIManager.CursorFeedback(BSM.grid.Tiles[0,0]);  //will need to chnage when the cursor no longer starts at 0,0
    }

    private void OnEnable()
    {
        GridMovement.Enable();
    }

    private void OnDisable()
    {
        GridMovement.Disable();
    }

    public void GhostMoveFinished()
    {
        InputState = InputState.UNIT_CONTEXT_MENU;
        UIManager.OpenContextMenu(SelectedUnit);
    }

    void Cardinals(Direction dir)
    {
        switch (InputState)
        {
            case InputState.ACCEPTING_INPUT:
                switch (dir)
                {
                    case Direction.UP:
                        CardinalDirections(Direction.UP);
                        break;
                    case Direction.DOWN:
                        CardinalDirections(Direction.DOWN);
                        break;
                    case Direction.LEFT:
                        CardinalDirections(Direction.LEFT);
                        break;
                    case Direction.RIGHT:
                        CardinalDirections(Direction.RIGHT);
                        break;
                }
                break;

            case InputState.ONLY_MOVEMENT:
                switch (dir)
                {
                    case Direction.UP:
                        CardinalDirections(Direction.UP);
                        break;
                    case Direction.DOWN:
                        CardinalDirections(Direction.DOWN);
                        break;
                    case Direction.LEFT:
                        CardinalDirections(Direction.LEFT);
                        break;
                    case Direction.RIGHT:
                        CardinalDirections(Direction.RIGHT);
                        break;
                }
                break;

            case InputState.UNIT_CONTEXT_MENU:
                UIManager.ContextMenuInput(dir);
                break;

            case InputState.PICKUP_UP_UNIT:
                switch (dir)
                {
                    case Direction.UP:
                        CardinalDirections(Direction.UP);
                        BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                    case Direction.DOWN:
                        CardinalDirections(Direction.DOWN);
                        BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                    case Direction.LEFT:
                        CardinalDirections(Direction.LEFT);
                        BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                    case Direction.RIGHT:
                        CardinalDirections(Direction.RIGHT);
                        BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                }

                break;

            case InputState.UNIT_MOVING:

                break;

            case InputState.CUSOR_MOVING:

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
                UnitPickup(cursorAnchor);
                break;

            case InputState.ONLY_MOVEMENT:
                break;

            case InputState.UNIT_CONTEXT_MENU:
                InputState = InputState.NO_INPUT;
                UIManager.ContextMenuIconSelected();
                break;

            case InputState.PICKUP_UP_UNIT:

                if (SelectedUnit.position == Cursor.position)
                {
                    //unit hasnt moved, open menu

                    InputState = InputState.NO_INPUT;
                    UIManager.OpenContextMenu(SelectedUnit);
                }
                else
                {
                    //unit has moved
                    //move unit first, then open menu
                    InputState = InputState.NO_INPUT;
                    BSM.GhostMoveUnit(SelectedUnit);
                }

                break;

            case InputState.UNIT_MOVING:

                break;

            case InputState.CUSOR_MOVING:

                break;

            case InputState.NO_INPUT:
                break;
        }
    }

    void Back()
    {
        switch (InputState)
        {
            case InputState.ACCEPTING_INPUT:
                InputState = InputState.ACCEPTING_INPUT;
                break;

            case InputState.ONLY_MOVEMENT:
                break;

            case InputState.UNIT_CONTEXT_MENU:

                InputState = InputState.NO_INPUT;
                UIManager.CloseContextMenu();
                SelectedUnit = null;
                pickup = false;
                BSM.UnitDropped();
                BSM.CancelGhostMove();

                break;

            case InputState.UNIT_OVERVIEW_MENU:
                
                if (UIManager.GetCamState() == CameraState.UNIT_OVERVIEW)
                {
                    UIManager.UnitOverviewExit();
                }
                
                break;

            case InputState.PICKUP_UP_UNIT:
                SelectedUnit = null;
                pickup = false;
                BSM.UnitDropped();
                InputState = InputState.ACCEPTING_INPUT;
                break;

            case InputState.UNIT_MOVING:

                break;

            case InputState.CUSOR_MOVING:

                break;

            case InputState.NO_INPUT:
                break;
        }
    }

    void MenuKey()
    {
        if (InputState != InputState.ACCEPTING_INPUT)
            return;

        if (BSM.grid.UnitAtPos(cursorAnchor.position, out SelectedUnit))  //MODIFY THIS LATER TO GET ENEMY INFO TOO
        {
            //set enum to menu open (only accept back button and l r triggers)
            InputState = InputState.NO_INPUT;
            //spawn unitoverview (use animator)

            Unit unit = BSM.UnitFromTransform(SelectedUnit);
            UIManager.UnitOverviewOne(unit);
        }
        else
            return;

    }

    void UnitPickup(Transform cursorAnchor)
    {
        if (BSM.grid.UnitAtPos(cursorAnchor.position, out SelectedUnit))  //MODIFY THIS LATER TO GET ENEMY INFO TOO
        {
            InputState = InputState.PICKUP_UP_UNIT;
            pickup = true;
        }
        else
            Debug.Log("empty tile");
    }

    public void CardinalDirections(Direction dir)
    {
        switch (dir)
        {
            case Direction.UP:
                MoveCursor(new Vector3(0, 0, 5));
                break;
            case Direction.DOWN:
                MoveCursor(new Vector3(0, 0, -5));
                break;
            case Direction.LEFT:
                MoveCursor(new Vector3(-5, 0, 0));
                break;
            case Direction.RIGHT:
                MoveCursor(new Vector3(5, 0, 0));
                break;
        }
    }
    public void MoveCursor(Vector3 dir)
    {
        Vector3 potentialPos = Cursor.position + dir;
        G_Tile tile = new G_Tile();

        if (BSM.grid.TileAtPos(potentialPos, out tile))
        {
            oldCursorPos = Cursor.position;
            newCursorPos = potentialPos;
            cursorAnchor.position = potentialPos;

            InputState = InputState.CUSOR_MOVING;
            //Cursor.position = potentialPos;
            UIManager.CursorFeedback(tile);
        }
        else
            return;
    }

    private void FixedUpdate()
    {
        if (InputState == InputState.CUSOR_MOVING)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / desiredDuration;

            Cursor.position = Vector3.Lerp(oldCursorPos, newCursorPos, percentageComplete);

            if (Cursor.position == newCursorPos)
            {
                elapsedTime = 0;

                if (pickup)
                    InputState = InputState.PICKUP_UP_UNIT;
                else
                    InputState = InputState.ACCEPTING_INPUT;
            }
        }
    }

    public void ContextMenuOpened()
    {
        InputState = InputState.UNIT_CONTEXT_MENU;
    }

    public void ContextMenuClosed()
    {
        InputState = InputState.ACCEPTING_INPUT;
    }

    public void IgnoreInput()
    {
        InputState = InputState.NO_INPUT;
    }

    public void UnitOverviewOpen()
    {
        InputState = InputState.UNIT_OVERVIEW_MENU;
    }

    public void UnitOverviewClosed()
    {
        InputState = InputState.ACCEPTING_INPUT;
    }

    public void ContextMenuStay()
    {
        BSM.UnitCommitMove();
        BSM.RemoveGhostSprites();
        pickup = false;
        InputState = InputState.ACCEPTING_INPUT;
    }
}
