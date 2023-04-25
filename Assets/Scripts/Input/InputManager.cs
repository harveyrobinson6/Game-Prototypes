using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using N_Grid;
using N_Entity;
using DG.Tweening;
using static UnityEngine.InputSystem.InputAction;

enum InputState
{
    ACCEPTING_INPUT,
    ONLY_MOVEMENT,
    UNIT_CONTEXT_MENU,
    CONEXT_MENU_ENEMY_SELECT,
    UNIT_OVERVIEW_MENU,
    PICKUP_UP_UNIT,
    UNIT_MOVING,
    CUSOR_MOVING,
    ATTACK_PROMPT,
    BATTLE_FORECAST,
    NO_INPUT
}

public enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public enum BumperDirection
{
    LEFT,
    RIGHT
}

public enum StickDirection
{
    UP,
    DOWN
}

public enum RightStickState
{
    NONE,
    UP,
    DOWN
}

public class InputManager : MonoBehaviour
{
    GridMovement GridMovement;
    InputState InputState;
    [SerializeField] BattleSceneManager BSM;
    [SerializeField] UIManager UIManager;
    Transform SelectedUnit = null;
    [SerializeField] Transform Cursor;

    [SerializeField] CameraFollow CameraFollow;
    float camVal = 0.5f;

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

    int currentBattleUnitID;
    int currentBattleEnemyID;

    RightStickState RightStickState = RightStickState.NONE;

    bool UnitCanMove = false;

    public List<Transform> EnemyCycle { get; private set; }
    public int selectedEnemy { get; private set; }
    [SerializeField] public Transform EnemyBattleHover;

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
        GridMovement.CursorMovement.BumperLeft.performed += ctx => Bumpers(BumperDirection.LEFT);
        GridMovement.CursorMovement.BumperRight.performed += ctx => Bumpers(BumperDirection.RIGHT);
        GridMovement.CursorMovement.CameraZoomUp.performed += ctx => RightStick(ctx, StickDirection.UP);
        GridMovement.CursorMovement.CameraZoomUp.canceled += ctx => RightStick(ctx, StickDirection.UP);
        GridMovement.CursorMovement.CameraZoomDown.performed += ctx => RightStick(ctx, StickDirection.DOWN);
        GridMovement.CursorMovement.CameraZoomDown.canceled += ctx => RightStick(ctx, StickDirection.DOWN);
        GridMovement.CursorMovement.Toggle.performed += ctx => ToggleButton();
    }

    private void Start()
    {
        UIManager.CursorFeedback(BSM.grid.Tiles[0,0]);  //will need to chnage when the cursor no longer starts at 0,0

        EnemyBattleHover.gameObject.SetActive(false);
        selectedEnemy = 0;
    }

    private void Update()
    {
        switch (RightStickState)
        {
            case RightStickState.NONE:

                //Debug.Log("NONE");

                break;
            case RightStickState.UP:

                //Debug.Log("UP");
                camVal = Mathf.Clamp(camVal + 0.01f, 0, 1);
                CameraFollow.NewCameraVal(camVal);

                break;
            case RightStickState.DOWN:

                //Debug.Log("DOWN");
                camVal = Mathf.Clamp(camVal - 0.01f, 0, 1);
                CameraFollow.NewCameraVal(camVal);

                break;
        }
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
        ContextMenuPrep();
    }

    void ContextMenuPrep()
    {
        bool[] elementEnabled = new bool[] { false, true, true };

        elementEnabled[0] = EnemyNearUnit(SelectedUnit);

        UIManager.OpenContextMenu(SelectedUnit, elementEnabled);
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

            case InputState.CONEXT_MENU_ENEMY_SELECT:

                //InputState = InputState.NO_INPUT;

                if (EnemyCycle.Count <= 1)
                    return;

                switch (dir)
                {
                    case Direction.UP:

                        if (selectedEnemy == 0)
                            selectedEnemy = EnemyCycle.Count - 1;
                        else
                            selectedEnemy--;

                        break;

                    case Direction.DOWN:

                        if (selectedEnemy == EnemyCycle.Count - 1)
                            selectedEnemy = 0;
                        else
                            selectedEnemy++;

                        break;
                }

                EnemyBattleHover.gameObject.SetActive(true);
                Vector3 newPos = EnemyCycle[selectedEnemy].position;
                Debug.Log(newPos);
                newPos.y = 3.5f;
                EnemyBattleHover.position = newPos;

                break;

            case InputState.PICKUP_UP_UNIT:

                switch (dir)
                {
                    case Direction.UP:
                        CardinalDirections(Direction.UP);
                        //BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                    case Direction.DOWN:
                        CardinalDirections(Direction.DOWN);
                        //BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                    case Direction.LEFT:
                        CardinalDirections(Direction.LEFT);
                        //BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                    case Direction.RIGHT:
                        CardinalDirections(Direction.RIGHT);
                        //BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                }

                break;

            case InputState.UNIT_MOVING:

                break;

            case InputState.CUSOR_MOVING:

                break;

            case InputState.ATTACK_PROMPT:
                Debug.Log("attackie");
                InputState = InputState.PICKUP_UP_UNIT;
                switch (dir)
                {
                    case Direction.UP:
                        CardinalDirections(Direction.UP);
                        //BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                    case Direction.DOWN:
                        CardinalDirections(Direction.DOWN);
                        //BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                    case Direction.LEFT:
                        CardinalDirections(Direction.LEFT);
                        //BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                    case Direction.RIGHT:
                        CardinalDirections(Direction.RIGHT);
                        //BSM.CalculatePath(cursorAnchor, SelectedUnit);
                        break;
                }

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

            case InputState.CONEXT_MENU_ENEMY_SELECT:

                InputState = InputState.NO_INPUT;

                Unit unit = BSM.UnitFromTransform(SelectedUnit);
                Enemy enemy = BSM.EnemyFromTransform(EnemyCycle[selectedEnemy]);

                UIManager.BattleForecast(unit.ID, enemy.ID);
                //pull up battle forecast

                break;

            case InputState.PICKUP_UP_UNIT:

                if (SelectedUnit.position == Cursor.position)
                {
                    //unit hasnt moved, open menu

                    InputState = InputState.NO_INPUT;

                    ContextMenuPrep();
                }
                else
                {
                    if (UnitCanMove)
                    {
                        //unit has moved
                        //move unit first, then open menu
                        InputState = InputState.NO_INPUT;
                        BSM.GhostMoveUnit(SelectedUnit);
                    }
                }

                break;

            case InputState.UNIT_MOVING:

                break;

            case InputState.CUSOR_MOVING:

                break;

            case InputState.ATTACK_PROMPT:

                InputState = InputState.NO_INPUT;
                UIManager.BattleForecast(currentBattleUnitID, currentBattleEnemyID);
                
                Debug.Log("grrr big nasty attack");

                break;

            case InputState.BATTLE_FORECAST:

                InputState = InputState.NO_INPUT;
                UIManager.AcceptBattle();

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

            case InputState.CONEXT_MENU_ENEMY_SELECT:

                EnemyBattleHover.gameObject.SetActive(false);
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

            case InputState.ATTACK_PROMPT:

                BattleForecastClosed();

                break;

            case InputState.BATTLE_FORECAST:

                EnemyBattleHover.gameObject.SetActive(false);
                UIManager.CloseContextMenu();
                SelectedUnit = null;
                pickup = false;
                BSM.UnitDropped();
                BSM.CancelGhostMove();

                UIManager.BattleForecastExit(false);

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

    void Bumpers(BumperDirection dir)
    {
        switch (InputState)
        {
            case InputState.ACCEPTING_INPUT:
                //LIKE FIRE EMBLEM
                //CYCLE THROUGH UNITS








                //YE
                break;

            

            case InputState.BATTLE_FORECAST:

                InputState = InputState.NO_INPUT;
                UIManager.BattleFirecastWeaponScroll(dir, currentBattleUnitID);

                break;

            case InputState.NO_INPUT:
                break;
        }
    }

    void RightStick(CallbackContext ctx, StickDirection dir)
    {
        if (InputState == InputState.ACCEPTING_INPUT)
        {
            if (ctx.performed)
            {
                switch (dir)
                {
                    case StickDirection.UP:

                        RightStickState = RightStickState.UP;

                        break;
                    case StickDirection.DOWN:

                        RightStickState = RightStickState.DOWN;

                        break;
                }
            }
            if (ctx.canceled)
            {
                RightStickState = RightStickState.NONE;
            }
        }
    }

    void ToggleButton()
    {
        switch (InputState)
        {
            case InputState.ACCEPTING_INPUT:

                BSM.grid.ToggleSprites();

                break;
        }
    }

    void UnitPickup(Transform cursorAnchor)
    {
        if (BSM.grid.UnitAtPos(cursorAnchor.position, out SelectedUnit))  //MODIFY THIS LATER TO GET ENEMY INFO TOO
        {
            Unit unit = BSM.UnitFromTransform(SelectedUnit);

            if (!unit.ActionUsed)
            {
                InputState = InputState.PICKUP_UP_UNIT;
                pickup = true;
            }
        }
        else
            Debug.Log("no yoo nit");
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
        //if (pickup)
            //BSM.CalculatePath(cursorAnchor, SelectedUnit);

        InputState temp = InputState;
        Debug.Log(temp);
        Vector3 potentialPos = Cursor.position + dir;
        G_Tile tile = new G_Tile();

        if (BSM.grid.TileAtPos(potentialPos, out tile))
        {
            InputState = InputState.CUSOR_MOVING;
            oldCursorPos = Cursor.position;
            cursorAnchor.position = potentialPos;

            if (pickup)
            {
                InputState = temp;
                UnitCanMove = BSM.CalculatePath(cursorAnchor, SelectedUnit);
            }
            else
                InputState = InputState.ACCEPTING_INPUT;


            Debug.Log(InputState);

            Cursor.DOMove(potentialPos,0.25f).OnComplete(() =>
            {
                /*
                if (pickup)
                {
                    InputState = temp;
                    BSM.CalculatePath(cursorAnchor, SelectedUnit);
                }
                else
                    InputState = InputState.ACCEPTING_INPUT;
                */
                UIManager.CursorFeedback(tile);
            });

            
            //Cursor.position = potentialPos;
            
        }
        else
            return;
    }

    bool EnemyNearUnit(Transform unitTrans)
    {
        bool rtnBool = false;
        int num = 1 * BSM.indexSizeMultiplier;

        (int, int)[] tilesPositions = new (int, int)[] {
            ((int)unitTrans.position.x + num, (int)unitTrans.position.z),  //+1,0
            ((int)unitTrans.position.x - num, (int)unitTrans.position.z),  //-1,0
            ((int)unitTrans.position.x, (int)unitTrans.position.z + num),  //0,+1
            ((int)unitTrans.position.x, (int)unitTrans.position.z - num)   //0,-1
        };

        List<Transform> temp = new List<Transform>();

        for (int i = 0; i < tilesPositions.Length; i++)
        {
            Transform enemy;
            Vector3 testPos = new Vector3(tilesPositions[i].Item1, -1.4f, tilesPositions[i].Item2);
            Debug.Log(testPos);

            if (BSM.grid.EnemyAtPos(testPos, out enemy))
            {
                temp.Add(enemy);
                rtnBool = true;
                Debug.Log("Enemy Found");
            }
        }

        EnemyCycle = temp;
        return rtnBool;
    }

    public void PlayerTurn()
    {
        InputState = InputState.ACCEPTING_INPUT;
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

    public void BattleForecastClosed()
    {
        SelectedUnit = null;
        pickup = false;
        BSM.CancelGhostMove();
        BSM.UnitDropped();
        InputState = InputState.ACCEPTING_INPUT;
    }

    public void BattleOver()
    {
        SelectedUnit = null;
        pickup = false;
        InputState = InputState.ACCEPTING_INPUT;
    }

    public void AttackPrompt(int unitID, int enemyID)
    {
        InputState = InputState.ATTACK_PROMPT;
        currentBattleUnitID = unitID;
        currentBattleEnemyID = enemyID;
    }
    public void BattleForecast()
    {
        InputState = InputState.BATTLE_FORECAST;
    }

    public void EnemyCycleMethod()
    {
        InputState = InputState.CONEXT_MENU_ENEMY_SELECT;
    }

    public void ContextMenuStay()
    {
        BSM.UnitCommitMove();
        BSM.RemoveGhostSprites();
        pickup = false;
        InputState = InputState.ACCEPTING_INPUT;
    }
}
