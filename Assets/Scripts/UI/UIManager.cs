using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using N_Grid;
using N_Entity;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] InputManager InputManager;
    [SerializeField] CameraFollow CameraFollow;

    Tween currentSelected;

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

    private void Start()
    {
        MovementBox.gameObject.SetActive(false);
        EvasionBox.gameObject.SetActive(false);

        UnitOverview1.gameObject.SetActive(false);

        OpenCursorFeedback();
    }

    #region CONTEXTMENU

    public void OpenContextMenu(Transform unit)
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
        Health.text = unit.EntityStats.CurrentHealth.ToString() + "/" + unit.EntityStats.MaxHealth.ToString();
        Movement.text = unit.MaxMove.ToString();

        Attack.text = unit.EntityStats.Attack.ToString();
        Defence.text = unit.EntityStats.Defence.ToString();
        Dexterity.text = unit.EntityStats.Dexterity.ToString();
        Aether.text = unit.EntityStats.Aether.ToString();
        Faith.text = unit.EntityStats.Faith.ToString();
        Forfeit.text = unit.EntityStats.Forfeit.ToString();

        CameraFollow.CameraState = CameraState.SELECTING_UNIT_OVERVIEW;

        UnitOverview1.DOLocalMoveX(575f, 0.5f).onComplete = Test;
        UnitOverview1CG.DOFade(1f, 0.5f);
        CloseCursorFeedback();
    }

    void Test()
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

        UnitOverview1.DOLocalMoveX(850f, 0.5f).onComplete = Test2;
        UnitOverview1CG.DOFade(0f, 0.5f);
        OpenCursorFeedback();
    }

    void Test2()
    {
        UnitOverview1.gameObject.SetActive(false);
        CameraFollow.CameraState = CameraState.FOLLOW_CURSOR;
        InputManager.UnitOverviewClosed();
    }

    #endregion

    public CameraState GetCamState()
    {
        return CameraFollow.CameraState;
    }

}
