using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using N_Grid;
using N_Entity;
using TMPro;
using DG.Tweening;

public struct TerrainType
{
    public string TerrainName { get; private set; }
    public string TerrainDesc { get; private set; }
    public int TerrainHinderance { get; private set; }
    public int TerrainEvasion { get; private set; }

    public TerrainType(string name, string desc, int hinderance, int evasion)
    {
        TerrainName = name;
        TerrainDesc = desc;
        TerrainHinderance = hinderance;
        TerrainEvasion = evasion;
    }
}

public enum GameState
{
    INTRODUCTION,
    PLAYER_TURN_START,
    PLAYER_TURN,
    ENEMY_TURN_START,
    ENEMY_TURN
}

public class BattleSceneManager : MonoBehaviour
{
    public Transform TilePrefab;
    public Transform EntityPrefab;
    public Transform EntityAnchorPrefab;
    public Transform GhostEntityPrefab;
    [SerializeField] InputManager InputManager;
    [SerializeField] Transform BattleObject;
    [SerializeField] CameraFollow CameraFollow;
    [SerializeField] UIManager UIManager;

    public GameState GameState { get; private set; }

    public List<TerrainType> TerrainTypes { get; private set; }

    Transform GhostEntity;
    Transform AttackPromptObject;

    public G_Grid grid { get; private set; }

    [SerializeField] private int w;
    [SerializeField] private int h;

    [SerializeField] Sprite ghostSprite;
    [SerializeField] Sprite blockSprite;
    [SerializeField] Transform AttackSpritePrompt;

    [SerializeField] Sprite[] WeaponSprites;

    List<ReturnNode> prevPath;

    int playerMaxMove = 3;

    [SerializeField] Sprite[] ghostSprites;

    public Unit[] units { get; private set; }
    public Enemy[] enemies { get; private set; }

    public int indexSizeMultiplier { get; private set;}

    float elapsedTime;
    float desiredDuration = 0.3f;
    int unitID;
    bool unitMoving = false;
    Vector3 oldPos;
    Vector3 newPos;
    int idInc = 1;

    G_Tile GhostMovePrevTile;
    G_Tile GhostMoveCurrentTile;

    Sprite[][] Sprites;
    [SerializeField] Sprite[] MageSprites;
    [SerializeField] Sprite[] KnightSprites;

    Sprite[][] LeftSprites;
    [SerializeField] Sprite[] MageSpritesLeft;
    [SerializeField] Sprite[] KnightSpritesLeft;

    Sprite[][] RightSprites;
    [SerializeField] Sprite[] MageSpritesRight;
    [SerializeField] Sprite[] KnightSpritesRight;



    void Awake()
    {
        indexSizeMultiplier = 5;
        Sprites = new Sprite[2][];

        Sprites[0] = MageSprites;
        Sprites[1] = KnightSprites;

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
                //TilePrefabs[i, j].GetComponentInChildren<TextMeshProUGUI>().text = i + ", " + j;
                TilePrefabs[i, j].GetComponentInChildren<TextMeshProUGUI>().text = "";
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

        units = new Unit[2];
        (int, int)[] unitIndexes = {(5,5), (2,8), (8,8), (0,0), (8,2), (8,5)};
        int[] unitClasses = new int[] { 1, 1, 0, 0, 1, 0 };
        //Vector3[] positions = {new Vector3(0,0,0), new Vector3(25, 0, 25), new Vector3(10, 0, 35)};

        for (int i = 0; i < units.Length; i++)
        {
            Vector3 pos = new Vector3(unitIndexes[i].Item1 * indexSizeMultiplier, -1.4f, unitIndexes[i].Item2 * indexSizeMultiplier);
            Transform entityObject = (Instantiate(EntityPrefab, pos, Quaternion.identity));
            Transform unitAnchor = Instantiate(EntityAnchorPrefab, pos, Quaternion.identity);
            Stats stats = new Stats(20, 13, 8, 11, 3, 7, 2);
            List<Weapon> weapons = new List<Weapon>();
            weapons.Add(new Weapon(4, 120, 5, "Sword", AttackType.MELEE, WeaponSprites[0]));
            weapons.Add(new Weapon(4, 120, 0, "Fire Staff", AttackType.MAGIC, WeaponSprites[1]));

            EntityClass entityClass = (EntityClass)unitClasses[i];

            var obj = entityObject.Find("Parent");

            SpriteRenderer[] spriteRenderers = new SpriteRenderer[6];
            spriteRenderers[0] = obj.Find("HeadSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[0].sprite = Sprites[unitClasses[i]][0];
            spriteRenderers[1] = obj.Find("ShoulderSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[1].sprite = Sprites[unitClasses[i]][1];
            spriteRenderers[2] = obj.Find("BodySprite").GetComponent<SpriteRenderer>();
            spriteRenderers[2].sprite = Sprites[unitClasses[i]][2];
            spriteRenderers[3] = obj.Find("TorsoSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[3].sprite = Sprites[unitClasses[i]][3];
            spriteRenderers[4] = obj.Find("LegsSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[4].sprite = Sprites[unitClasses[i]][4];
            spriteRenderers[5] = obj.Find("FeetSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[5].sprite = Sprites[unitClasses[i]][5];

            units[i] = new Unit(i, stats, entityObject, unitAnchor, weapons, spriteRenderers);
            grid.Tiles[unitIndexes[i].Item1, unitIndexes[i].Item2].SetUnitOccupant(units[i].EntityTransform);
        }

        enemies = new Enemy[2];
        (int, int)[] enemyIndexes = { (2, 2), (1, 1)};
        int[] enemyClasses = new int[] { 1, 0 };



        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 enemyPos = new Vector3(enemyIndexes[i].Item1 * indexSizeMultiplier, -1.4f, enemyIndexes[i].Item2 * indexSizeMultiplier);
            Transform enemyObject = (Instantiate(EntityPrefab, enemyPos, Quaternion.identity));
            Transform enemyAnchor = Instantiate(EntityAnchorPrefab, enemyPos, Quaternion.identity);
            Stats enemyStats = new Stats(20 + i, 13, 8, 11, 3, 7, 2);
            List<Weapon> weaponsEnemy = new List<Weapon>();
            weaponsEnemy.Add(new Weapon(14, 120, 1, "Frying Pan", AttackType.MELEE, WeaponSprites[2]));

            EntityClass entityClass = (EntityClass)enemyClasses[i];

            var obj = enemyObject.Find("Parent");

            SpriteRenderer[] spriteRenderers = new SpriteRenderer[6];
            spriteRenderers[0] = obj.Find("HeadSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[0].sprite = Sprites[enemyClasses[i]][0];
            spriteRenderers[1] = obj.Find("ShoulderSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[1].sprite = Sprites[enemyClasses[i]][1];
            spriteRenderers[2] = obj.Find("BodySprite").GetComponent<SpriteRenderer>();
            spriteRenderers[2].sprite = Sprites[enemyClasses[i]][2];
            spriteRenderers[3] = obj.Find("TorsoSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[3].sprite = Sprites[enemyClasses[i]][3];
            spriteRenderers[4] = obj.Find("LegsSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[4].sprite = Sprites[enemyClasses[i]][4];
            spriteRenderers[5] = obj.Find("FeetSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[5].sprite = Sprites[enemyClasses[i]][5];

            enemies[i] = new Enemy(i, enemyStats, enemyObject, enemyAnchor, weaponsEnemy, spriteRenderers);
            grid.Tiles[enemyIndexes[i].Item1, enemyIndexes[i].Item2].SetEnemyOccupant(enemies[i].EntityTransform);
        }

        //make classes, put in list
        GhostEntity = Instantiate(GhostEntityPrefab, new Vector3(0,0,0), Quaternion.identity);
        GhostEntity.gameObject.SetActive(false);
        AttackPromptObject = Instantiate(AttackSpritePrompt, new Vector3(0, 0, 0), Quaternion.identity);
        AttackPromptObject.gameObject.SetActive(false);

        TerrainTypes = new List<TerrainType>();

        TerrainTypes.Add(new TerrainType(
            "Meadows",
            "Flat open plains, prime battlefield",
            0,
            0
            ));

        TerrainTypes.Add(new TerrainType(
            "Swamp",
            "A horrible boggy mess",
             1,
            +10
            ));

        TerrainTypes.Add(new TerrainType(
            "Forest",
            "The trees offer their cover",
             2,
            -20
            ));

        TerrainTypes.Add(new TerrainType(
            "Graveyard",
            "The spirits amplify your magic",
            0,
            0
            ));

        TerrainTypes.Add(new TerrainType(
            "Bridge",
            "Rickety and old... be careful",
            0,
            0
            ));

        TerrainTypes.Add(new TerrainType(
            "River",
            "Untraversable on foot",
            0,
            0
            ));

        CameraFollow.MoveToGrid();
        GameIntroduction();
    }

    public bool CalculatePath(Transform cursor, Transform unit)
    {
        RemoveGhostSprites();
        GhostEntity.gameObject.SetActive(false);
        AttackPromptObject.gameObject.SetActive(false);

        int unitID = UnitIDFromTransform(unit);
        Transform unitAnchor = units[unitID].EntityAnchorTransform;

        if (cursor.position == unitAnchor.position)
            return false;

        G_Tile OGPosTile = new G_Tile();

        if (grid.TileAtPos(unitAnchor.position, out OGPosTile))
        {
            
        }

        G_Tile cursorPosTile = new G_Tile();

        if (grid.TileAtPos(cursor.position, out cursorPosTile))
        {

        }

        (int, int)? index = (0,0);

        if (grid.IDFromTile(cursorPosTile, out index))
        {
            
        }

        OccupationStatus occupationStatus = grid.Tiles[index.Value.Item1, index.Value.Item2].OccupationState;
        grid.Tiles[index.Value.Item1, index.Value.Item2].FreeTile();

        //set the last tile to open (change it after)
        var bfs = new BreadthFirstSearch(grid, (OGPosTile.TileWIndex, OGPosTile.TileHIndex));

        if (bfs.HasPathTo((cursorPosTile.TileWIndex, cursorPosTile.TileHIndex)))
        {
            var path = bfs.PathTo((cursorPosTile.TileWIndex, cursorPosTile.TileHIndex));

            int finalMoveDist = units[unitID].MaxMove;

            foreach (var item in path)
            {
                TerrainType terrainType = TerrainTypes[(int)grid.Tiles[item.Self.Item1, item.Self.Item2].TileType];
                finalMoveDist -= terrainType.TerrainHinderance;
            }

            if (path.Count > finalMoveDist)
            {
                Debug.Log("NO GO STINKY");
            }
            else if (path.Count > 0)
            {
                switch (occupationStatus)
                {
                    case OccupationStatus.OPEN:

                        foreach (var item in path)
                        {
                            grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
                        }

                        grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);
                        prevPath = path;
                        SetPrevAndCurrent();
                        return true;

                    case OccupationStatus.UNITOCCUPIED:

                        foreach (var item in path)
                        {
                            grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
                            grid.Tiles[item.Self.Item1, item.Self.Item2].GhostSpriteRend.color = new Color(1f, 0f, 0f, 0.4f);
                        }

                        grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);
                        prevPath = path;
                        SetPrevAndCurrent();
                        return false;

                    case OccupationStatus.ENEMYOCCUPIED:

                        foreach (var item in path)
                        {
                            grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
                            grid.Tiles[item.Self.Item1, item.Self.Item2].GhostSpriteRend.color = new Color(1f, 0f, 0f, 0.4f);
                        }

                        GhostEntity.gameObject.SetActive(true);
                        (int, int) intTup = (path.Last().PrevNode.Item1, path.Last().PrevNode.Item2);
                        Vector3 newPos = new Vector3(intTup.Item1 * indexSizeMultiplier, -1.4f,
                                                     intTup.Item2 * indexSizeMultiplier);
                        GhostEntity.transform.position = newPos;
                        GhostEntity.GetComponentInChildren<SpriteRenderer>().sprite = units[unitID].EntityTransform.GetComponentInChildren<SpriteRenderer>().sprite;

                        Enemy enemy;
                        AttackPrompt(unitID, index, out enemy);
                        InputManager.AttackPrompt(unitID, enemy.ID);

                        grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);
                        prevPath = path;
                        SetPrevAndCurrent();
                        return false;

                    case OccupationStatus.EDIFICE:

                        grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);
                        prevPath = path;
                        SetPrevAndCurrent();
                        return false;

                    case OccupationStatus.OUTOFBOUNDS:

                        grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);
                        prevPath = path;
                        SetPrevAndCurrent();
                        return false;

                    default:
                        break;
                }

                //set ghostunit pos to path.last
                //GhostEntity.gameObject.SetActive(true);
                //(int, int) intTup = (path.Last().Self.Item1, path.Last().Self.Item2);
                //Vector3 newPos = new Vector3(intTup.Item1 * indexSizeMultiplier, 0,
                                            // intTup.Item2 * indexSizeMultiplier);
               // GhostEntity.transform.position = newPos;
                //GhostEntity.GetComponentInChildren<SpriteRenderer>().sprite = unit.GetComponentInChildren<SpriteRenderer>().sprite;

                
            }
        }
        else
        {
            foreach (var item in prevPath)
            {
                grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
                grid.Tiles[item.Self.Item1, item.Self.Item2].GhostSpriteRend.color = new Color(1f, 0f, 0f, 0.4f);
            }

            return false;
        }

        return false;
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
            index.Item1 = prevPath[prevPath.Count - 1].Self.Item1 * indexSizeMultiplier;
            index.Item2 = prevPath[prevPath.Count - 1].Self.Item2 * indexSizeMultiplier;
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
        //oldPos = entityAnchors[unitID].position;
        //newPos = new Vector3(prevPath[idInc].Self.Item1 * indexSizeMultiplier, 0, prevPath[idInc].Self.Item2 * indexSizeMultiplier);
        //Debug.Log(newPos);
        //unitMoving = true;

        List<ReturnNode>.Enumerator temp = prevPath.GetEnumerator();
        temp.MoveNext();
        UnitMove(temp);
    }

    void UnitMove(IEnumerator<ReturnNode> em)
    {
        if (em.MoveNext())
        {
            Vector3 newPosition = new Vector3(em.Current.Self.Item1 * indexSizeMultiplier,
                                      -1.4f,
                                      em.Current.Self.Item2 * indexSizeMultiplier);

            units[unitID].EntityTransform.DOMove(newPosition, 0.25f).SetEase(Ease.Linear).OnComplete(() => UnitMove(em));
        }
        else
        {
            GhostUnitFinishedMove();
        }
    }

    public void GhostMoveUnitAttack(Transform unitTrans, Unit unit, Enemy enemy, InitalAttacker initalAttacker)
    {
        idInc = 1;
        unitID = UnitIDFromTransform(unitTrans);
        //oldPos = entityAnchors[unitID].position;
        //newPos = new Vector3(prevPath[idInc].Self.Item1 * indexSizeMultiplier, 0, prevPath[idInc].Self.Item2 * indexSizeMultiplier);
        //Debug.Log(newPos);
        //unitMoving = true;

        List<ReturnNode>.Enumerator temp = prevPath.GetEnumerator();
        temp.MoveNext();
        UnitMoveAttack(temp, unit, enemy, initalAttacker);
    }

    void UnitMoveAttack(IEnumerator<ReturnNode> em, Unit unit, Enemy enemy, InitalAttacker initalAttacker)
    {
        if (em.MoveNext())
        {
            Vector3 newPosition = new Vector3(em.Current.PrevNode.Item1 * indexSizeMultiplier,
                                      -1.4f,
                                      em.Current.PrevNode.Item2 * indexSizeMultiplier);

            units[unitID].EntityTransform.DOMove(newPosition, 0.25f).SetEase(Ease.Linear).OnComplete(() => UnitMoveAttack(em, unit, enemy, initalAttacker));
        }
        else
        {
            StartBattleCallback(unit, enemy, initalAttacker);
        }
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
        units[unitID].EntityTransform.position = units[unitID].EntityAnchorTransform.position;
    }

    public void AttackPrompt(int ID, (int,int)? pos, out Enemy enemy)
    {
        Vector3 enemyPos = new Vector3(pos.Value.Item1 * indexSizeMultiplier, -1.4f,
                                       pos.Value.Item2 * indexSizeMultiplier);

        enemy = EnemyFromPos(enemyPos);

        Debug.Log(enemyPos);

        G_Tile tile;

        if (grid.TileAtPos(enemyPos, out tile))
        {
            Debug.Log("true");
        }
        
        Vector3 midpoint = (GhostEntity.position +
                            tile.TileWorldPos) / 2;

        AttackPromptObject.gameObject.SetActive(true);
        AttackPromptObject.position = midpoint;
    }

    public void GameIntroduction()
    {
        GameState = GameState.INTRODUCTION;
        //set no input on input manager
        InputManager.IgnoreInput();
        //have ui manager show title card
        UIManager.GameIntro();
    }

    public void PlayerTurnStart()
    {
        GameState = GameState.PLAYER_TURN_START;
        //set no input on input manager
        InputManager.IgnoreInput();
        //have ui manager show title card
        UIManager.PlayerTurnIntro();

        foreach (var unit in units)
        {
            unit.GreyIn();
        }
    }

    public void PlayerTurn()
    {
        GameState = GameState.PLAYER_TURN;
        //set no input on input manager
        InputManager.PlayerTurn();
    }

    public void EnemyTurnStart()
    {
        GameState = GameState.ENEMY_TURN_START;
        //set no input on input manager
        InputManager.IgnoreInput();
        //have ui manager show title card
        grid.HideGrid();

        UIManager.EnemyTurnIntro();
    }

    public void EnemyTurn()
    {
        GameState = GameState.ENEMY_TURN;
        //set no input on input manager
        InputManager.IgnoreInput();

        StartCoroutine(TempWait());
    }

    IEnumerator TempWait()
    {
        yield return new WaitForSeconds(3f);

        PlayerTurnStart();
    }

    public void UnitCommitMove()
    {
        (int, int) tileIndex = (((int)units[unitID].EntityTransform.position.x / indexSizeMultiplier), ((int)units[unitID].EntityTransform.position.z / indexSizeMultiplier));
        grid.Tiles[tileIndex.Item1, tileIndex.Item2].SetUnitOccupant(units[unitID].EntityTransform);
        Debug.Log(tileIndex);

        (int, int) tileIndexAnchor = (((int)units[unitID].EntityAnchorTransform.position.x / indexSizeMultiplier), ((int)units[unitID].EntityAnchorTransform.position.z / indexSizeMultiplier));
        grid.Tiles[tileIndexAnchor.Item1, tileIndexAnchor.Item2].FreeTile();
        units[unitID].EntityAnchorTransform.position = units[unitID].EntityTransform.position;

        units[unitID].GreyOut();
        EndTurnCheck();
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
        AttackPromptObject.gameObject.SetActive(false);
        GhostEntity.position = new Vector3(0, 0, 0);
        GhostEntity.gameObject.SetActive(false);
    }

    public void Startbattle(Unit unit, Enemy enemy, InitalAttacker initalAttacker)
    {
        UnitDropped();  //await
        GhostMoveUnitAttack(unit.EntityTransform, unit, enemy, initalAttacker);
    }

    public void StartBattleCallback(Unit unit, Enemy enemy, InitalAttacker initalAttacker)
    {
        UnitCommitMove();  //await
        CameraFollow.MoveToBattle();
        BattleObject.GetComponent<BattleLogic>().Init(unit, enemy, initalAttacker);
    }

    public void BattleOver(int _unitID, int _enemyID, Outcome outcome)
    {
        //units[_unitID].TakeDamage(5);
        //Debug.Log(units[_unitID].EntityStats.CurrentHealth);

        units[_unitID].GreyOut();

        CameraFollow.MoveToGrid();

        Debug.Log(outcome);

        switch (outcome)
        {
            case Outcome.UNIT_FIRST_WIN:

                enemies[_enemyID].EntityTransform.DOMoveY(-2, 0.5f).SetDelay(3f).OnComplete(() =>
                {
                    (int, int) tileIndex = (((int)enemies[_enemyID].EntityTransform.position.x / indexSizeMultiplier), ((int)enemies[_enemyID].EntityTransform.position.z / indexSizeMultiplier));
                    grid.Tiles[tileIndex.Item1, tileIndex.Item2].FreeTile();
                    enemies[_enemyID].EntityDead();
                    BattleOverCallback();
                });

                break;
            case Outcome.UNIT_FIRST_LOOSE:

                units[_unitID].EntityTransform.DOMoveY(-2, 0.5f).SetDelay(3f).OnComplete(() =>
                {
                    (int, int) tileIndex = (((int)units[_unitID].EntityTransform.position.x / indexSizeMultiplier), ((int)units[_unitID].EntityTransform.position.z / indexSizeMultiplier));
                    grid.Tiles[tileIndex.Item1, tileIndex.Item2].FreeTile();
                    units[_unitID].EntityDead();
                    BattleOverCallback();
                });

                break;
            case Outcome.ENEMY_FIRST_WIN:

                units[_unitID].EntityTransform.DOMoveY(-2, 0.5f).SetDelay(3f).OnComplete(() =>
                {
                    (int, int) tileIndex = (((int)units[_unitID].EntityTransform.position.x / indexSizeMultiplier), ((int)units[_unitID].EntityTransform.position.z / indexSizeMultiplier));
                    grid.Tiles[tileIndex.Item1, tileIndex.Item2].FreeTile();
                    units[_unitID].EntityDead();
                    BattleOverCallback();
                });

                break;
            case Outcome.ENEMY_FIRST_LOOSE:

                enemies[_enemyID].EntityTransform.DOMoveY(-2, 0.5f).SetDelay(3f).OnComplete(() =>
                {
                    (int, int) tileIndex = (((int)enemies[_enemyID].EntityTransform.position.x / indexSizeMultiplier), ((int)enemies[_enemyID].EntityTransform.position.z / indexSizeMultiplier));
                    grid.Tiles[tileIndex.Item1, tileIndex.Item2].FreeTile();
                    enemies[_enemyID].EntityDead();
                    BattleOverCallback();
                });

                break;
            case Outcome.STALEMATE:

                BattleOverCallback();

                break;
        }

        EndTurnCheck();
    }

    void BattleOverCallback()
    {
        InputManager.BattleOver();
    }

    void EndTurnCheck()
    {
        foreach (var unit in units)
        {
            if (!unit.ActionUsed)
                return;
        }

        EnemyTurnStart();
    }

    public int UnitIDFromTransform(Transform transform)
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

    public Unit UnitFromTransform(Transform transform)
    {
        Unit rtnUnit = new Unit();

        foreach (Unit unit in units)
        {
            if (unit.EntityTransform == transform)
            {
                rtnUnit = unit;
                return rtnUnit;
            }
        }

        return rtnUnit;
    }

    public Enemy EnemyFromTransform(Transform transform)
    {
        Enemy rtnEnemy = new Enemy();

        foreach (Enemy enemy in enemies)
        {
            if (enemy.EntityTransform == transform)
            {
                rtnEnemy = enemy;
                return rtnEnemy;
            }
        }

        return rtnEnemy;
    }

    public Enemy EnemyFromPos(Vector3 pos)
    {
        Enemy rtnEnemy = new Enemy();

        foreach (Enemy enemy in enemies)
        {
            if (enemy.EntityTransform.position == pos)
            {
                rtnEnemy = enemy;
                return rtnEnemy;
            }
        }

        return rtnEnemy;
    }
}