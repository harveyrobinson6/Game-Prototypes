using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N_Grid;
using N_Entity;
using TMPro;

public class BattleSceneManager : MonoBehaviour
{
    public Transform TilePrefab;
    public Transform EntityPrefab;
    public Transform EntityAnchorPrefab;
    public Transform GhostEntityPrefab;
    [SerializeField] InputManager InputManager;

    Transform GhostEntity;

    public G_Grid grid { get; private set; }

    [SerializeField] private int w;
    [SerializeField] private int h;

    [SerializeField] Sprite ghostSprite;
    [SerializeField] Sprite blockSprite;

    ReturnNode[] prevPath;

    int playerMaxMove = 3;

    [SerializeField] Sprite[] ghostSprites;

    Unit[] units;
    Transform[] entityAnchors;

    int indexSizeMultiplier = 5;

    float elapsedTime;
    float desiredDuration = 0.3f;
    int unitID;
    bool unitMoving = false;
    Vector3 oldPos;
    Vector3 newPos;
    int idInc = 1;

    G_Tile GhostMovePrevTile;
    G_Tile GhostMoveCurrentTile;

    void Awake()
    {
        Transform[,] TilePrefabs = new Transform[w, h];

        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
                TilePrefabs[i,j] = (Instantiate(TilePrefab, new Vector3(0, 0, 0), Quaternion.identity));

        grid = new G_Grid(w, h, indexSizeMultiplier, TilePrefabs);

        for (int i = 0; i < grid.Tiles.GetLength(0); i++)
            for (int j = 0; j < grid.Tiles.GetLength(1); j++)
            {
                G_Tile tile = grid.Tiles[i, j];
                TilePrefabs[i, j].transform.position = tile.TileWorldPos;
                TilePrefabs[i, j].GetComponentInChildren<TextMeshProUGUI>().text = i + ", " + j;
            }

        //grid.Tiles[2, 3].DEBUG_OCCUPY();
        //grid.Tiles[2, 3].SetGhostSprite(blockSprite);
        /*
        foreach (var item in grid.Tiles[1, 1].AdjacencyList)
        {
            Debug.Log(item);
        }
        */

        //UNITS AND ENEMIES

        //make gameObjects

        units = new Unit[6];
        entityAnchors = new Transform[6];
        (int, int)[] indexes = {(5,5), (2,8), (8,8), (2,2), (8,2), (8,5)};
        //Vector3[] positions = {new Vector3(0,0,0), new Vector3(25, 0, 25), new Vector3(10, 0, 35)};

        for (int i = 0; i < units.Length; i++)
        {
            Vector3 pos = new Vector3(indexes[i].Item1 * indexSizeMultiplier, 0, indexes[i].Item2 * indexSizeMultiplier);
            Transform entityObject = (Instantiate(EntityPrefab, pos, Quaternion.identity));
            entityAnchors[i] = Instantiate(EntityAnchorPrefab, pos, Quaternion.identity);
            Stats stats = new Stats(5, 5, 5, 5, 5, 5, 5);
            units[i] = new Unit(i, stats, entityObject.Find("EntitySprite").GetComponentInChildren<SpriteRenderer>(), entityObject);
            grid.Tiles[indexes[i].Item1, indexes[i].Item2].SetOccupant(units[i].EntityTransform);
        }

        //make classes, put in list
        GhostEntity = Instantiate(GhostEntityPrefab, new Vector3(0,0,0), Quaternion.identity);
        GhostEntity.gameObject.SetActive(false);
    }

    public void CalculatePath(Transform cursor, Transform unit)
    {
        RemoveGhostSprites();

        int ID = UnitIDFromTransform(unit);
        Transform unitAnchor = entityAnchors[ID];

        if (cursor.position == unitAnchor.position)
            return;

        G_Tile OGPosTile = new G_Tile();

        if (grid.TileAtPos(unitAnchor.position, out OGPosTile))
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
            //Debug.Log("testing");
            foreach (var item in path)
            {
                grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
            }

            //GhostEntity.gameObject.SetActive(true);
            //GhostEntity.position = cursor.position;
        }/*
        else
        {
            foreach (var item in prevPath)
            {
                grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
                grid.Tiles[item.Self.Item1, item.Self.Item2].GhostSpriteRend.color = new Color(1f, 0f, 0f, 0.4f);
            }
        }*/

        prevPath = path;
        SetPrevAndCurrent();
    }

    void SetPrevAndCurrent()
    {
        //SCOPED SO I DONT HAVE TO GIVE VARIABLES SILLY NAMES :)
        {
            (int, int) index = (0, 0);
            index.Item1 = prevPath[0].Self.Item1 * indexSizeMultiplier;
            index.Item2 = prevPath[0].Self.Item2 * indexSizeMultiplier;
            Vector3 vec3 = new Vector3(index.Item1, 0, index.Item2);

            if (grid.TileAtPos(vec3, out GhostMovePrevTile))
            {

            }
        }
        {
            (int, int) index = (0, 0);
            index.Item1 = prevPath[prevPath.Length - 1].Self.Item1 * indexSizeMultiplier;
            index.Item2 = prevPath[prevPath.Length - 1].Self.Item2 * indexSizeMultiplier;
            Vector3 vec3 = new Vector3(index.Item1, 0, index.Item2);

            if (grid.TileAtPos(vec3, out GhostMoveCurrentTile))
            {

            }
        }
    }

    public void GhostMoveUnit(Transform unit)
    {
        idInc = 1;
        unitID = UnitIDFromTransform(unit);
        oldPos = entityAnchors[unitID].position;
        newPos = new Vector3(prevPath[idInc].Self.Item1 * indexSizeMultiplier, 0, prevPath[idInc].Self.Item2 * indexSizeMultiplier);
        Debug.Log(newPos);
        unitMoving = true;
    }

    public void GhostUnitFinishedMove()
    {
        //free previous tile
        //occupy new tile

        //prevpath[0] to go back if user cancels


        //change enum in input manager    DONE
        InputManager.GhostMoveFinished();


        //store prev tile before move   DONE
        //go back to prev tile if the user cancels when at new tile
        //do ghost sprite
    }

    public void CancelGhostMove()
    {
        //move unit back to original pos
        units[unitID].EntityTransform.position = entityAnchors[unitID].transform.position;
    }

    private void Update()
    {
        if (unitMoving)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / desiredDuration;

            units[unitID].EntityTransform.position = Vector3.Lerp(oldPos, newPos, percentageComplete);

            if (units[unitID].EntityTransform.position == newPos)
            {
                if ((idInc + 1) < prevPath.Length)
                {
                    oldPos = newPos;
                    idInc++;
                    newPos = new Vector3(prevPath[idInc].Self.Item1 * indexSizeMultiplier, 0, prevPath[idInc].Self.Item2 * indexSizeMultiplier);
                    elapsedTime = 0;
                }
                else
                {
                    unitMoving = false;
                    GhostUnitFinishedMove();
                }
            }
        }
    }

    public void UnitCommitMove()
    {
        (int, int) tileIndex = (((int)units[unitID].EntityTransform.position.x / indexSizeMultiplier), ((int)units[unitID].EntityTransform.position.z / indexSizeMultiplier));
        grid.Tiles[tileIndex.Item1, tileIndex.Item2].SetOccupant(units[unitID].EntityTransform);
        Debug.Log(tileIndex);

        (int, int) tileIndexAnchor = (((int)entityAnchors[unitID].position.x / indexSizeMultiplier), ((int)entityAnchors[unitID].position.z / indexSizeMultiplier));
        grid.Tiles[tileIndexAnchor.Item1, tileIndexAnchor.Item2].FreeTile();
        entityAnchors[unitID].position = units[unitID].EntityTransform.position;
    }

    public void RemoveGhostSprites()
    {
        foreach (G_Tile tile in grid.Tiles)
        {
            tile.ClearGhostSprite();
            tile.GhostSpriteRend.color = new Color(1f, 1f, 1f, 0.4f);
        }
    }

    public void UnitDropped()
    {
        RemoveGhostSprites();
        GhostEntity.gameObject.SetActive(false);
        GhostEntity.position = new Vector3(0, 0, 0);
    }

    int UnitIDFromTransform(Transform transform)
    {
        int rtnInt = 0;

        foreach (Unit unit in units)
        {
            if (unit.EntityTransform == transform)
            {
                rtnInt = unit.ID;
                return rtnInt;
            }
        }

        return rtnInt;
    }
}