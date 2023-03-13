using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using N_Grid;

enum InputState
{
    ACCEPTING_INPUT,
    UNIT_CONTEXT_MENU,
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
        OpenContextMenu();
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

            case InputState.UNIT_CONTEXT_MENU:
                ContextMenuInput(dir);
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

            case InputState.UNIT_CONTEXT_MENU:
                ContextMenuIconSelected();
                break;

            case InputState.PICKUP_UP_UNIT:

                if (SelectedUnit.position == Cursor.position)
                {
                    //unit hasnt moved, open menu

                    InputState = InputState.UNIT_CONTEXT_MENU;
                    OpenContextMenu();
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

            case InputState.UNIT_CONTEXT_MENU:

                CloseContextMenu();
                SelectedUnit = null;
                pickup = false;
                BSM.UnitDropped();
                BSM.CancelGhostMove();
                InputState = InputState.ACCEPTING_INPUT;

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

    #region CONTEXT_MENU

    void OpenContextMenu()
    {
        UIPrefabs = new Queue<Transform>();
        UIElements = new List<Transform>();
        ElementType = new List<string>();
        animators = new List<Animator>();
        canvas = SelectedUnit.Find("Canvas");

        foreach (var item in uiprefabs)
        {
            UIPrefabs.Enqueue(item);
        }

        for (int i = 0; i < elementEnabled.Length; i++)
        {
            if (!elementEnabled[i])
                continue;

            Transform t = Instantiate(UIPrefabs.Dequeue(), canvas);
            t.GetComponent<SpriteRenderer>().sprite = sprites[i];
            animators.Add(t.GetComponent<Animator>());
            UIElements.Add(t);
            ElementType.Add(elementType[i]);
        }

        ContextMenuPreSelect();
        ContextMenuPostSelect();
    }

    void CloseContextMenu()
    {
        List<Transform> list = new List<Transform>();

        foreach (Transform child in UIElements)
        {
            list.Add(child);
        }

        while (list.Count > 0)
        {
            var temp = list[0];
            list.RemoveAt(0);
            Destroy(temp.gameObject);
        }
    }

    void ContextMenuInput(Direction dir)
    {
        ContextMenuPreSelect();

        switch (dir)
        {
            case Direction.UP:

                if (contextMenuSelectedItem == 0)
                    contextMenuSelectedItem = UIElements.Count - 1;
                else
                    contextMenuSelectedItem--;

                break;

            case Direction.DOWN:

                if (contextMenuSelectedItem == UIElements.Count - 1)
                    contextMenuSelectedItem = 0;
                else
                    contextMenuSelectedItem++;

                break;
        }

        ContextMenuPostSelect();
    }

    void ContextMenuPreSelect()
    {
        animators[contextMenuSelectedItem].ResetTrigger("Selected");
        animators[contextMenuSelectedItem].SetTrigger("Deselected");
    }

    void ContextMenuPostSelect()
    {
        animators[contextMenuSelectedItem].ResetTrigger("Deselected");
        animators[contextMenuSelectedItem].SetTrigger("Selected");
    }

    void ContextMenuIconSelected()
    {
        string type = ElementType[contextMenuSelectedItem];
        Debug.Log(type);

        switch (type)
        {
            case "Attack":

                break;

            case "Inventory":

                break;

            case "Stay":

                //move unit and anchor to new post
                BSM.UnitCommitMove();
                BSM.RemoveGhostSprites();
                CloseContextMenu();
                pickup = false;
                contextMenuSelectedItem = 0;
                InputState = InputState.ACCEPTING_INPUT;
                break;
        }
    }

    #endregion
}
