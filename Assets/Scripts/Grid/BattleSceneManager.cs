using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N_Grid;
using TMPro;

public class BattleSceneManager : MonoBehaviour
{
    public Transform TilePrefab;
    public G_Grid grid { get; private set; }

    [SerializeField] private int w;
    [SerializeField] private int h;

    void Awake()
    {
        grid = new G_Grid(w, h, 5);

        for (int i = 0; i < grid.Tiles.GetLength(0); i++)
            for (int j = 0; j < grid.Tiles.GetLength(1); j++)
            {
                G_Tile tile = grid.Tiles[i, j];
                Transform t = Instantiate(TilePrefab, new Vector3(tile.TileWorldPos.x, 0, tile.TileWorldPos.z), Quaternion.identity);
                t.GetComponentInChildren<TextMeshProUGUI>().text = i + ", " + j;
            }
    }
}