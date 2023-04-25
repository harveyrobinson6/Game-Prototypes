using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using N_Grid;
using N_Entity;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] InputManager InputManager;
    [SerializeField] CameraFollow CameraFollow;

    Tween currentSelected;
    Tween UnitFlashBar;
    Tween EnemyFlashBar;

    int weaponCycle = 0;

    Unit currentUnit;
    Enemy currentEnemy;

    int contextMenuSelectedItem = 0;
    List<Animator> animators;
    //bool[] elementEnabled = { true, true, true };
    string[] elementType = { "Attack", "Inventory", "Stay" };
    Queue<Transform> UIPrefabs;
    [SerializeField] Transform[] uiprefabs;
    [SerializeField] Sprite[] sprites;
    List<Transform> UIElements;
    List<string> ElementType;
    Transform canvas;
    Vector3[] ContextMenuPositions = { new Vector3(0f, 2.5f, 0f),
                                       new Vector3(1.7f, 1.7f, 0f),
                                       new Vector3(2.5f, 0f, 0f) };

    [SerializeField] BattleSceneManager BSM;
    [SerializeField] TextMeshProUGUI TerrainName;
    [SerializeField] TextMeshProUGUI TerrainDesc;
    [SerializeField] TextMeshProUGUI MovementModifier;
    [SerializeField] TextMeshProUGUI EvasionModifier;
    [SerializeField] Transform CursorFeedbackObject;

    [SerializeField] Transform CursorFeedbackElement;
    [SerializeField] CanvasGroup CursorFeedbackCG;
    [SerializeField] Transform MovementBox;
    [SerializeField] Transform EvasionBox;

    [SerializeField] Transform UnitOverview1;
    [SerializeField] CanvasGroup UnitOverview1CG;

    [SerializeField] TextMeshProUGUI EntityName;
    [SerializeField] TextMeshProUGUI ClassName;
    [SerializeField] TextMeshProUGUI Health;
    [SerializeField] TextMeshProUGUI Movement;

    [SerializeField] TextMeshProUGUI Attack;
    [SerializeField] TextMeshProUGUI Defence;
    [SerializeField] TextMeshProUGUI Dexterity;
    [SerializeField] TextMeshProUGUI Aether;
    [SerializeField] TextMeshProUGUI Faith;
    [SerializeField] TextMeshProUGUI Forfeit;

    [SerializeField] Transform BattleForecastObj;
    [SerializeField] CanvasGroup BattleForecastCG;

    [SerializeField] TextMeshProUGUI UnitHealthNum;
    [SerializeField] TextMeshProUGUI EnemyHealthNum;
    [SerializeField] TextMeshProUGUI UnitPower;
    [SerializeField] TextMeshProUGUI EnemyPower;
    [SerializeField] TextMeshProUGUI UnitAccuracy;
    [SerializeField] TextMeshProUGUI EnemyAccuracy;
    [SerializeField] TextMeshProUGUI UnitCrit;
    [SerializeField] TextMeshProUGUI EnemyCrit;

    [SerializeField] Transform CycleWeapon1Trans;
    [SerializeField] Transform CycleWeapon2Trans;
    [SerializeField] TextMeshProUGUI CycleWeapon1;
    [SerializeField] TextMeshProUGUI CycleWeapon2;
    [SerializeField] TextMeshProUGUI EnemyWeapon;

    [SerializeField] Image unitBarMask;
    [SerializeField] Image enemyBarMask;
    [SerializeField] Image unitBarMaskPotential;
    [SerializeField] Image enemyBarMaskPotential;

    [SerializeField] CanvasGroup UnitBarFlash;
    [SerializeField] CanvasGroup EnemyBarFlash;

    [SerializeField] Transform IntroTitleCard;
    [SerializeField] CanvasGroup IntroTitleCardCG;
    [SerializeField] Transform IntroTopHalf;
    [SerializeField] Transform IntroBottomHalf;

    [SerializeField] Transform PlayerTurnTitleCard;
    [SerializeField] Transform PlayerText;
    [SerializeField] Transform TurnText;
    [SerializeField] CanvasGroup PlayerTurnCG;
    [SerializeField] Transform EnemyTurnTitleCard;
    [SerializeField] Transform EnemyText;
    [SerializeField] Transform TurnText2;
    [SerializeField] CanvasGroup EnemyTurnCG;

    private void Awake()
    {
        
    }

    public void GameIntro()
    {
        MovementBox.gameObject.SetActive(false);
        EvasionBox.gameObject.SetActive(false);

        UnitOverview1.gameObject.SetActive(false);
        BattleForecastObj.gameObject.SetActive(false);

        IntroTitleCard.gameObject.SetActive(false);
        IntroTopHalf.gameObject.SetActive(false);
        IntroBottomHalf.gameObject.SetActive(false);
        PlayerTurnTitleCard.gameObject.SetActive(false);
        PlayerText.gameObject.SetActive(false);
        TurnText.gameObject.SetActive(false);
        EnemyTurnTitleCard.gameObject.SetActive(false);
        EnemyText.gameObject.SetActive(false);
        TurnText2.gameObject.SetActive(false);

        IntroTitleCard.gameObject.SetActive(true);
        IntroTopHalf.gameObject.SetActive(true);
        IntroBottomHalf.gameObject.SetActive(true);

        IntroTopHalf.transform.localPosition = new Vector3(-1309, 100, 0);
        IntroBottomHalf.transform.localPosition = new Vector3(1309, -100, 0);

        IntroTopHalf.DOLocalMoveX(0, 2f);

        IntroBottomHalf.DOLocalMoveX(0, 2f).OnComplete(() =>
        {
            IntroTitleCardCG.DOFade(0, 2f).SetDelay(2f).OnComplete(() =>
            {
                IntroTitleCard.gameObject.SetActive(false);
                IntroTopHalf.gameObject.SetActive(false);
                IntroBottomHalf.gameObject.SetActive(false);

                BSM.PlayerTurnStart();
            });
        });
    }

    public void PlayerTurnIntro()
    {
        PlayerTurnTitleCard.gameObject.SetActive(true);
        PlayerText.gameObject.SetActive(true);
        TurnText.gameObject.SetActive(true);

        PlayerTurnCG.alpha = 1;

        PlayerText.localPosition = new Vector3(-1228, 0, 0);
        TurnText.localPosition = new Vector3(1142, 0, 0);

        PlayerText.DOLocalMoveX(-200, 1f);
        TurnText.DOLocalMoveX(250, 1f).OnComplete(() =>
        {
            PlayerTurnCG.DOFade(0, 1f).SetDelay(2f).OnComplete(() =>
            {
                PlayerTurnTitleCard.gameObject.SetActive(false);
                PlayerText.gameObject.SetActive(false);
                TurnText.gameObject.SetActive(false);

                OpenCursorFeedback();
                BSM.PlayerTurn();
            });
        });
    }

    public void EnemyTurnIntro()
    {
        CloseCursorFeedback();

        EnemyTurnTitleCard.gameObject.SetActive(true);
        EnemyText.gameObject.SetActive(true);
        TurnText2.gameObject.SetActive(true);

        EnemyTurnCG.alpha = 1;

        EnemyText.localPosition = new Vector3(-1228, 0, 0);
        TurnText2.localPosition = new Vector3(1142, 0, 0);

        EnemyText.DOLocalMoveX(-200, 1f);
        TurnText2.DOLocalMoveX(250, 1f).OnComplete(() =>
        {
            EnemyTurnCG.DOFade(0, 1f).SetDelay(2f).OnComplete(() =>
            {
                EnemyTurnTitleCard.gameObject.SetActive(false);
                EnemyText.gameObject.SetActive(false);
                TurnText2.gameObject.SetActive(false);

                BSM.EnemyTurn();
            });
        });
    }

    #region CONTEXTMENU

    public void OpenContextMenu(Transform unit, bool[] elementEnabled)
    {
        UIPrefabs = new Queue<Transform>();
        UIElements = new List<Transform>();
        ElementType = new List<string>();
        animators = new List<Animator>();
        canvas = unit.Find("Canvas");

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

        for (int i = 0; i < UIElements.Count; i++)
        {
            if (i == 0)
                UIElements[i].DOLocalMove(ContextMenuPositions[i], 0.5f).onComplete = ContextMenuOpened;
            else
                UIElements[i].DOLocalMove(ContextMenuPositions[i], 0.5f);

            UIElements[i].DOScale(new Vector3(1,1,1), 0.5f);
        }
    }

    void ContextMenuOpened()
    {
        //tell input manager to change enum
        InputManager.ContextMenuOpened();
        contextMenuSelectedItem = 0;
        currentSelected = UIElements[contextMenuSelectedItem].DOScale(new Vector3(1.5f, 1.5f, 0f), 0.5f).SetLoops(-1, LoopType.Yoyo);
        //selected icon
    }

    public void CloseContextMenu()
    {
        //set animator triggers
        //delay
        //yield return new WaitForSeconds(0.4f);
        //destroy transforms
        currentSelected.Kill();
        for (int i = 0; i < UIElements.Count; i++)
        {
            if (i == 0)
                UIElements[i].DOLocalMove(new Vector3(0, 0, 0), 0.5f).onComplete = ContextMenuClosed;
            else
                UIElements[i].DOLocalMove(new Vector3(0, 0, 0), 0.5f);

            UIElements[i].DOScale(new Vector3(0, 0, 0), 0.5f);
        }
    }

    void ContextMenuClosed()
    {
        DestroyContextMenuElements();
        contextMenuSelectedItem = 0;
        InputManager.ContextMenuClosed();
    }

    public void ContextMenuInput(Direction dir)
    {
        int prevIndex = contextMenuSelectedItem;

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

        currentSelected.Kill();
        UIElements[prevIndex].DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.25f);
        currentSelected = UIElements[contextMenuSelectedItem].DOScale(new Vector3(1.5f, 1.5f, 0f), 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void DestroyContextMenuElements()
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

    public void ContextMenuIconSelected()
    {
        string type = ElementType[contextMenuSelectedItem];
        Debug.Log(type);

        switch (type)
        {
            case "Attack":

                contextMenuSelectedItem = 0;

                InputManager.EnemyBattleHover.gameObject.SetActive(true);
                Vector3 newPos = InputManager.EnemyCycle[InputManager.selectedEnemy].position;
                Debug.Log(newPos);
                newPos.y = 3.5f;
                InputManager.EnemyBattleHover.position = newPos;
                InputManager.EnemyCycleMethod();

                break;

            case "Inventory":

                break;

            case "Stay":

                //move unit and anchor to new post

                contextMenuSelectedItem = 0;

                CloseContextMenu();
                //DestroyContextMenuElements();
                InputManager.ContextMenuStay();
                
                break;
        }
    }

    #endregion

    #region CURSORFEEDBACK

    public void CursorFeedback(G_Tile tile)
    {
        MovementBox.gameObject.SetActive(false);
        EvasionBox.gameObject.SetActive(false);

        foreach (var item in BSM.TerrainTypes)
        {
            if (item.TerrainName == tile.TileType.ToString())
            {
                TerrainType terrainType = item;
                TerrainName.text = terrainType.TerrainName;
                TerrainDesc.text = terrainType.TerrainDesc;

                if (terrainType.TerrainHinderance != 0 || terrainType.TerrainEvasion != 0)
                {
                    MovementBox.gameObject.SetActive(true);
                    EvasionBox.gameObject.SetActive(true);

                    MovementModifier.text = terrainType.TerrainHinderance.ToString();
                    EvasionModifier.text = terrainType.TerrainEvasion.ToString() + "%";
                }

                break;
            }
        }
    }

    public void OpenCursorFeedback()
    {
        CursorFeedbackElement.DOLocalMoveX(-730f, 0.5f);
        CursorFeedbackCG.DOFade(1f, 0.5f);
    }

    public void CloseCursorFeedback()
    {
        CursorFeedbackElement.DOLocalMoveX(-900f, 0.5f);
        CursorFeedbackCG.DOFade(0f, 0.5f);
    }


    #endregion

    #region UNITOVERVIEW

    public void UnitOverviewOne(Unit unit)
    {
        UnitOverview1.gameObject.SetActive(true);
        //StartCoroutine(CursorFeedbackExit());
        //populate menu

        EntityName.text = unit.EntityName;
        ClassName.text = unit.EntityClass.ToString();
        Debug.Log(unit.EntityStats.CurrentHealth);
        Health.text = unit.EntityStats.CurrentHealth.ToString() + "/" + unit.EntityStats.MaxHealth.ToString();
        Movement.text = unit.MaxMove.ToString();

        Attack.text = unit.EntityStats.Attack.ToString();
        Defence.text = unit.EntityStats.Defence.ToString();
        Dexterity.text = unit.EntityStats.Dexterity.ToString();
        Aether.text = unit.EntityStats.Aether.ToString();
        Faith.text = unit.EntityStats.Faith.ToString();
        Forfeit.text = unit.EntityStats.Forfeit.ToString();

        CameraFollow.MoveToUnitOverview(unit.EntityTransform);

        UnitOverview1.DOLocalMoveX(575f, 0.5f).onComplete = UnitOverviewOpened;
        UnitOverview1CG.DOFade(1f, 0.5f);
        CloseCursorFeedback();
    }

    void UnitOverviewOpened()
    {
        CameraFollow.CameraState = CameraState.UNIT_OVERVIEW;
        InputManager.UnitOverviewOpen();
    }

    public void UnitOverviewTwo(Unit unit)
    {

    }

    public void UnitOverviewExit()
    {
        CameraFollow.CameraState = CameraState.DESELECTING_UNIT_OVERVIEW;

        CursorFeedbackElement.gameObject.SetActive(true);

        //UnitOverview1Animator.SetTrigger("MenuLeave");

        UnitOverview1.DOLocalMoveX(850f, 0.5f).onComplete = UnitOverviewExited;
        UnitOverview1CG.DOFade(0f, 0.5f);
        OpenCursorFeedback();
    }

    void UnitOverviewExited()
    {
        UnitOverview1.gameObject.SetActive(false);
        CameraFollow.MoveToGrid();
        InputManager.UnitOverviewClosed();
    }

    #endregion

    #region BATTLEFORECAST

    public void BattleForecast(int unitID, int enemyID)
    {
        currentUnit = BSM.units[unitID];
        currentEnemy = BSM.enemies[enemyID];

        weaponCycle = currentUnit.SelectedWeapon;

        //set active
        BattleForecastObj.gameObject.SetActive(true);
        //set texts
        //check whether the player is using a physcial or magic weapon
        SetBattleForecastData();

        CycleWeapon1.text = currentUnit.Weapons[weaponCycle].Name;
        CycleWeapon2.text = "";
        EnemyWeapon.text = currentEnemy.Weapons[currentEnemy.SelectedWeapon].Name;

        UnitBarFlash.alpha = 1;
        EnemyBarFlash.alpha = 1;

        //tween

        BattleForecastObj.DOLocalMoveX(400f, 0.5f).onComplete = BattleForecastOpened;
        BattleForecastCG.DOFade(1f, 0.5f);

        //tell cam to do sum

        CloseCursorFeedback();
    }

    public void AcceptBattle()
    {
        //close thing
        BSM.units[currentUnit.ID].NewWeaponSelected(weaponCycle);
        BattleForecastExit(true);
    }

    void BattleForecastOpened()
    {
        InputManager.BattleForecast();
    }

    public void BattleForecastExit(bool battle)
    {
        UnitFlashBar.Kill();
        EnemyFlashBar.Kill();

        if (battle)
            BattleForecastObj.DOLocalMoveX(600f, 0.1f).onComplete = BattleForecastClosedBattle;
        else
            BattleForecastObj.DOLocalMoveX(600f, 0.1f).onComplete = BattleForecastClosedReturn;

        BattleForecastCG.DOFade(0f, 0.1f);
    }

    void BattleForecastClosedBattle()
    {
        BattleForecastObj.gameObject.SetActive(false);
        BSM.Startbattle(currentUnit, currentEnemy, InitalAttacker.UNIT);
    }

    void BattleForecastClosedReturn()
    {
        BattleForecastObj.gameObject.SetActive(false);
        InputManager.BattleForecastClosed();
    }

    public void BattleFirecastWeaponScroll(BumperDirection dir, int unitID)
    {
        InputManager.IgnoreInput();
        Unit unit = BSM.units[unitID];

        //set second textmesh to old weapon name, set position to middle

        CycleWeapon2.text = CycleWeapon1.text;
        CycleWeapon2Trans.localPosition = new Vector3(0, 0, 0);

        switch (dir)
        {
            case BumperDirection.LEFT:
                //set first textmesh to right
                CycleWeapon1Trans.localPosition = new Vector3(375, 0, 0);
                //text move from right to center

                if (weaponCycle == unit.Weapons.Count - 1)
                    weaponCycle = 0;
                else
                    weaponCycle++;

                CycleWeapon1.text = unit.Weapons[weaponCycle].Name;
                CycleWeapon2Trans.DOLocalMoveX(-375, 0.1f);
                CycleWeapon1Trans.DOLocalMoveX(0, 0.1f).onComplete = BattleFirecastWeaponCallback;

                break;
            case BumperDirection.RIGHT:
                //set first textmesh to left
                CycleWeapon1Trans.localPosition = new Vector3(-375, 0, 0);
                //text move from left to center

                if (weaponCycle == 0)
                    weaponCycle = unit.Weapons.Count - 1;
                else
                    weaponCycle--;

                CycleWeapon1.text = unit.Weapons[weaponCycle].Name;
                CycleWeapon2Trans.DOLocalMoveX(375, 0.1f);
                CycleWeapon1Trans.DOLocalMoveX(0, 0.1f).onComplete = BattleFirecastWeaponCallback;

                break;
        }

        //-move hideen text to right set text to next weapon, move both text
        //-move hideen text to left set text to next weapon, move both text

        //tween
    }

    void SetBattleForecastData()
    {
        Weapon unitWeapon = currentUnit.Weapons[weaponCycle];
        Weapon enemyWeapon = currentEnemy.Weapons[currentEnemy.SelectedWeapon];

        int unitPowerVal = 0;
        int enemyPowerVal = 0;

        switch (unitWeapon.AttackType)
        {
            case AttackType.MELEE:

                unitPowerVal = ((currentUnit.EntityStats.Attack + unitWeapon.Power) / 2) - currentEnemy.EntityStats.Defence;

                break;
            case AttackType.MAGIC:

                unitPowerVal = ((currentUnit.EntityStats.Aether + unitWeapon.Power) / 2) - currentEnemy.EntityStats.Faith;

                break;
        }

        switch (enemyWeapon.AttackType)
        {
            case AttackType.MELEE:

                enemyPowerVal = ((currentEnemy.EntityStats.Attack + enemyWeapon.Power) / 2) - currentUnit.EntityStats.Defence;

                break;
            case AttackType.MAGIC:

                enemyPowerVal = ((currentEnemy.EntityStats.Aether + enemyWeapon.Power) / 2) - currentUnit.EntityStats.Faith;

                break;
        }

        
        int unitAccuracyVal = (currentUnit.EntityStats.Dexterity + unitWeapon.Accuracy);
        unitAccuracyVal = Mathf.Clamp(unitAccuracyVal, 0, 100);
        int unitCriticalVal = ((currentUnit.EntityStats.Dexterity - currentUnit.EntityStats.Forfeit * 2) + unitWeapon.Crit);
        unitCriticalVal = Mathf.Clamp(unitCriticalVal, 0, 100);

        int enemyAccuracyVal = (currentEnemy.EntityStats.Dexterity + enemyWeapon.Accuracy);
        enemyAccuracyVal = Mathf.Clamp(enemyAccuracyVal, 0, 100);
        int enemyCriticalVal = ((currentEnemy.EntityStats.Dexterity - currentEnemy.EntityStats.Forfeit * 2) + enemyWeapon.Crit);
        enemyCriticalVal = Mathf.Clamp(enemyCriticalVal, 0, 100);

        UnitPower.text = unitPowerVal.ToString();
        UnitAccuracy.text = unitAccuracyVal.ToString();
        UnitCrit.text = unitCriticalVal.ToString();

        EnemyPower.text = enemyPowerVal.ToString();
        EnemyAccuracy.text = enemyAccuracyVal.ToString();
        EnemyCrit.text = enemyCriticalVal.ToString();

        int unitpotentialHealth = currentUnit.EntityStats.CurrentHealth - enemyPowerVal;
        unitpotentialHealth = Mathf.Clamp(unitpotentialHealth, 0, currentUnit.EntityStats.MaxHealth);

        int enemypotentialHealth = currentEnemy.EntityStats.CurrentHealth - unitPowerVal;
        enemypotentialHealth = Mathf.Clamp(enemypotentialHealth, 0, currentEnemy.EntityStats.MaxHealth);

        UnitHealthNum.text = unitpotentialHealth.ToString();
        EnemyHealthNum.text = enemypotentialHealth.ToString();

        GetCurrentFills(unitpotentialHealth, enemypotentialHealth);
    }

    void BattleFirecastWeaponCallback()
    {
        //reset battle forecast values for new weapon matchup
        SetBattleForecastData();
        InputManager.BattleForecast();
    }

    void GetCurrentFills(int unitPotentialHealth, int enemyPotentialHealth)
    {
        {
            float fillAmount = (float)currentUnit.EntityStats.CurrentHealth / (float)currentUnit.EntityStats.MaxHealth;
            unitBarMaskPotential.fillAmount = fillAmount;

            float fillAmountCurrentHealth = (float)unitPotentialHealth / (float)currentUnit.EntityStats.MaxHealth;
            unitBarMask.fillAmount = fillAmountCurrentHealth;

            float speed = CalcFlashSpeed(1, fillAmountCurrentHealth);
            UnitFlashBar.Kill();
            UnitBarFlash.alpha = 1;
            UnitFlashBar = UnitBarFlash.DOFade(0.5f, speed).SetLoops(-1, LoopType.Yoyo);
        }
        {
            float fillAmount = (float)currentEnemy.EntityStats.CurrentHealth / (float)currentEnemy.EntityStats.MaxHealth;
            enemyBarMaskPotential.fillAmount = fillAmount;

            float fillAmountCurrentHealth = (float)enemyPotentialHealth / (float)currentEnemy.EntityStats.MaxHealth;
            enemyBarMask.fillAmount = fillAmountCurrentHealth;

            float speed = CalcFlashSpeed(1, fillAmountCurrentHealth);
            EnemyFlashBar.Kill();
            EnemyBarFlash.alpha = 1;
            EnemyFlashBar = EnemyBarFlash.DOFade(0.5f, speed).SetLoops(-1, LoopType.Yoyo);
        }
    }

    float CalcFlashSpeed(int baseSpeed, float fill)
    {
        if (fill == 1)
            return 0f;

        if (fill == 0)
            return 0.4f;

        float speed = Mathf.Clamp(baseSpeed - (1-fill), 0.4f, 1f);

        return speed;
    }

    #endregion

    public CameraState GetCamState()
    {
        return CameraFollow.CameraState;
    }

}
