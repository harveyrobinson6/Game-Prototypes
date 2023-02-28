using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N_Grid;
using TMPro;

public class BattleSceneManager : MonoBehaviour
{
    public Transform TilePrefab;
    public G_Grid grid { get; private set; }

    public Transform unit;

    [SerializeField] private int w;
    [SerializeField] private int h;

    [SerializeField] Sprite ghostSprite;

    void Awake()
    {
        Transform[,] TilePrefabs = new Transform[w, h];

        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
                TilePrefabs[i,j] = (Instantiate(TilePrefab, new Vector3(0, 0, 0), Quaternion.identity));

        grid = new G_Grid(w, h, 5, TilePrefabs);

        for (int i = 0; i < grid.Tiles.GetLength(0); i++)
            for (int j = 0; j < grid.Tiles.GetLength(1); j++)
            {
                G_Tile tile = grid.Tiles[i, j];
                TilePrefabs[i, j].transform.position = tile.TileWorldPos;
                TilePrefabs[i, j].GetComponentInChildren<TextMeshProUGUI>().text = i + ", " + j;
                /*
                foreach ((int,int) item in tile.AdjacencyList)
                {
                    Debug.Log(i + " + " + j + " is adj to " + item.Item1 + "," + item.Item2);
                }
                */
            }

        grid.Tiles[0, 0].SetOccupant(unit);

        /*

        (int, int) source = (1, 3);
        (int, int) target = (4,4);

        var bfs = new BreadthFirstSearch(grid, source);

        Debug.Log($"source vertex {source} is has path to target vertex {target} via: {string.Join(" -> ", bfs.PathTo(target))}");

        var temp = bfs.PathTo(target);

        foreach (var item in temp)
        {
            Debug.Log($"H= {item.Item1} W= {item.Item2}");
        }

        */
    }

    public void DisplayGhostSprites(Transform cursor, Vector3 originalPos)
    {
        RemoveGhostSprites();

        if (cursor.position == originalPos)
            return;

        G_Tile tile = new G_Tile();

        if (grid.TileAtPos(originalPos, out tile))
        {
            
        }

        var bfs = new BreadthFirstSearch(grid, (tile.TileWIndex, tile.TileHIndex));

        if (grid.TileAtPos(cursor.position, out tile))
        {

        }

        var temp = bfs.PathTo((tile.TileWIndex, tile.TileHIndex));

        foreach (var item in temp)
        {
            //Debug.Log($"H= {item.Item1} W= {item.Item2}");
            grid.Tiles[item.Item1, item.Item2].SetGhostSprite(ghostSprite);
        }
    }

    public void RemoveGhostSprites()
    {
        foreach (G_Tile tile in grid.Tiles)
        {
            tile.ClearGhostSprite();
        }
    }
}