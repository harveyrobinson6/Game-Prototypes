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
    [SerializeField] Sprite blockSprite;

    ReturnNode[] prevPath;

    int playerMaxMove = 3;

    [SerializeField] Sprite[] ghostSprites;

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
        //grid.Tiles[2, 3].DEBUG_OCCUPY();
        //grid.Tiles[2, 3].SetGhostSprite(blockSprite);

        foreach (var item in grid.Tiles[1, 1].AdjacencyList)
        {
            Debug.Log(item);
        }

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

        G_Tile OGPosTile = new G_Tile();

        if (grid.TileAtPos(originalPos, out OGPosTile))
        {
            
        }

        var bfs = new BreadthFirstSearch(grid, (OGPosTile.TileWIndex, OGPosTile.TileHIndex));

        G_Tile cursorPosTile = new G_Tile();

        if (grid.TileAtPos(cursor.position, out cursorPosTile))
        {

        }

        var path = bfs.PathTo((cursorPosTile.TileWIndex, cursorPosTile.TileHIndex));

        if (path.Length > 0)
        {
            Debug.Log("testing");
            foreach (var item in path)
            {
                grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
            }
        }
        /*
        else
        {
            foreach (var item in prevPath)
            {
                grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
            }
        }
        */
        prevPath = path;
    }

    public void RemoveGhostSprites()
    {
        foreach (G_Tile tile in grid.Tiles)
        {
            tile.ClearGhostSprite();
        }
    }
}