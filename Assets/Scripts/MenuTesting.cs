using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTesting : MonoBehaviour
{
    int selectedItem = 0;

    List<Animator> animators;

    bool[] elementEnabled = { true, true, true };
    Queue<Transform> UIPrefabs;
    [SerializeField] Transform[] uiprefabs;
    [SerializeField] Sprite[] sprites;
    List<Transform> UIElements;
    [SerializeField] Transform canvas;
    GridMovement GM;

    private void Start()
    {
        //StartCoroutine(SetTrig());
        GM = new GridMovement();

        GM.CursorMovement.Up.performed += ctx => Input("up");
        GM.CursorMovement.Down.performed += ctx => Input("down");

        UIPrefabs = new Queue<Transform>();
        UIElements = new List<Transform>();
        animators = new List<Animator>();

        foreach (var item in uiprefabs)
        {
            UIPrefabs.Enqueue(item);
        }

        //elementEnabled.GetEnumerator().MoveNext();
        for (int i = 0; i < elementEnabled.Length; i++)
        {
            if (!elementEnabled[i])
                continue;

            Transform t = Instantiate(UIPrefabs.Dequeue(), canvas);
            t.GetComponent<SpriteRenderer>().sprite = sprites[i];
            animators.Add(t.GetComponent<Animator>());
            UIElements.Add(t);
        }

        GM.Enable();
        PreSelect();
        PostSelect();
    }

    void Input(string dir)
    {
        //Debug.Log(selectedItem);

        PreSelect();

        if (dir == "down")
        {
            if (selectedItem == UIElements.Count - 1)
                selectedItem = 0;
            else
                selectedItem++;
        }
        else if (dir == "up")
        {
            if (selectedItem == 0)
                selectedItem = UIElements.Count - 1;
            else
                selectedItem--;
        }

        PostSelect();
    }

    void PreSelect()
    {
        animators[selectedItem].ResetTrigger("Selected");
        animators[selectedItem].SetTrigger("Deselected");
    }

    void PostSelect()
    {
        animators[selectedItem].ResetTrigger("Deselected");
        animators[selectedItem].SetTrigger("Selected");
    }
}
