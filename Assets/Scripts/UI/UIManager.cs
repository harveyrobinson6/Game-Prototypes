using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using N_Grid;
using N_Entity;

public class UIManager : MonoBehaviour
{
    [SerializeField] InputManager InputManager;

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

    [SerializeField] BattleSceneManager BSM;
    [SerializeField] TextMeshProUGUI TerrainName;
    [SerializeField] TextMeshProUGUI TerrainDesc;
    [SerializeField] TextMeshProUGUI MovementModifier;
    [SerializeField] TextMeshProUGUI EvasionModifier;
    [SerializeField] Transform CursorFeedbackObject;

    [SerializeField] Transform CursorFeedbackElement;
    [SerializeField] Transform MovementBox;
    [SerializeField] Transform EvasionBox;

    [SerializeField] Animator CursorFeedbackAnimator;

    [SerializeField] Transform UnitOverview1;

    [SerializeField] Animator UnitOverview1Animator;

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

        ContextMenuPreSelect();
        ContextMenuPostSelect();

        StartCoroutine(ContextMenuOpening());
    }

    public void CloseContextMenu()
    {
        StartCoroutine(ContextMenuClosing());
    }

    IEnumerator ContextMenuOpening()
    {
        //delay
        yield return new WaitForSeconds(0.4f);
        //tell input manager to change enum
        InputManager.ContextMenuOpened();
    }

    IEnumerator ContextMenuClosing()
    {
        //set animator triggers
        foreach (var animator in animators)
        {
            animator.SetTrigger("Close");
        }
        //delay
        yield return new WaitForSeconds(0.4f);
        //destroy transforms
        DestroyContextMenuElements();
        InputManager.ContextMenuClosed();
    }

    public void ContextMenuPreSelect()
    {
        animators[contextMenuSelectedItem].ResetTrigger("Selected");
        animators[contextMenuSelectedItem].SetTrigger("Deselected");
    }

    public void ContextMenuPostSelect()
    {
        animators[contextMenuSelectedItem].ResetTrigger("Deselected");
        animators[contextMenuSelectedItem].SetTrigger("Selected");
    }

    public void ContextMenuInput(Direction dir)
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

    public IEnumerator ContextMenuIconSelected()
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

                foreach (var animator in animators)
                {
                    animator.SetTrigger("Close");
                }

                yield return new WaitForSeconds(0.4f);

                DestroyContextMenuElements();
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

    public IEnumerator CursorFeedbackEnter()
    {
        yield return new WaitForSeconds(0.0f);
    }

    public IEnumerator CursorFeedbackExit()
    {
        CursorFeedbackAnimator.SetTrigger("MenuLeave");

        yield return new WaitForSeconds(0.4f);

        CursorFeedbackElement.gameObject.SetActive(false);
    }

    #endregion

    #region UNITOVERVIEW

    public IEnumerator UnitOverviewOne(Unit unit)
    {
        UnitOverview1.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.4f);
    }

    public void UnitOverviewTwo(Unit unit)
    {

    }

    #endregion

}
