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
    [SerializeField] SoundManager SM;

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

    int currentEnemyMove = 0;
    int enemyMaxMove = 6;

    System.Random rnd;
    List<ReturnNode>.Enumerator em;

    void Awake()
    {
        indexSizeMultiplier = 5;
        Sprites = new Sprite[2][];

        Sprites[1] = MageSprites;
        Sprites[0] = KnightSprites;

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

        units = new Unit[7];
        (int, int)[] unitIndexes = {(0,2), (3,1), (7,5), (7,3), (13, 6), (22, 0), (24, 0) };
        int[] unitClasses = new int[] { 1, 0, 1, 0, 0, 1, 0 };

        for (int i = 0; i < units.Length; i++)
        {
            Vector3 pos = new Vector3(unitIndexes[i].Item1 * indexSizeMultiplier, -1.4f, unitIndexes[i].Item2 * indexSizeMultiplier);
            Transform entityObject = (Instantiate(EntityPrefab, pos, Quaternion.identity));
            Transform unitAnchor = Instantiate(EntityAnchorPrefab, pos, Quaternion.identity);
            List<Weapon> weapons = new List<Weapon>();
            
            //weapons.Add(new Weapon(1000, 100, 0, "Showcase weapon", AttackType.MELEE, WeaponSprites[1]));
            
            if (unitClasses[i] == 0)
            {
                weapons.Add(new Weapon(29, 100, 15, "Iron Sword", AttackType.MELEE, WeaponSprites[0]));
                weapons.Add(new Weapon(40, 40, 60, "Unwieldy Axe", AttackType.MELEE, WeaponSprites[2]));
            }
            else if (unitClasses[i] == 1)
            {
                weapons.Add(new Weapon(31, 100, 30, "Fire Staff", AttackType.MAGIC, WeaponSprites[3]));
                weapons.Add(new Weapon(41, 60, 15, "Thunder Staff", AttackType.MAGIC, WeaponSprites[4]));
            }


            //weapons.Add(new Weapon(1, 5, 0, "MISS_TEST", AttackType.MELEE, WeaponSprites[1]));
            //weapons.Add(new Weapon(1, 100, 100, "CRIT_TEST", AttackType.MELEE, WeaponSprites[1]));

            EntityClass entityClass = (EntityClass)unitClasses[i];

            #region SPRITE RENDERERS

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

            #endregion

            units[i] = new Unit(i, new Stats(unitClasses[i]), entityObject, unitAnchor, weapons, spriteRenderers, entityClass);
            grid.Tiles[unitIndexes[i].Item1, unitIndexes[i].Item2].SetUnitOccupant(units[i].EntityTransform);
        }

        enemies = new Enemy[9];
        (int, int)[] enemyIndexes = { (1, 4), (5, 5), (9, 3), (8, 4), (9, 5), (17,5), (28,6), (22,9), (22,5)};
        bool[] wander = { true, true, false, false, false, false, false, true, true, true, false };
        int[] enemyClasses = new int[] { 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0 };
        //Stats[] enemyStats = new Stats[] { new Stats(27, 17, 17, 3, 11, 17, 4), new Stats(25, 7, 11, 24, 25, 11, 7) };

        Color color = new Color(1f, 0.5f, 0.5f, 1f);

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 enemyPos = new Vector3(enemyIndexes[i].Item1 * indexSizeMultiplier, -1.4f, enemyIndexes[i].Item2 * indexSizeMultiplier);
            Transform enemyObject = (Instantiate(EntityPrefab, enemyPos, Quaternion.identity));
            Transform enemyAnchor = Instantiate(EntityAnchorPrefab, enemyPos, Quaternion.identity);
            //Stats enemyStats = new Stats(20 + i, 13, 8, 11, 3, 7, 2);
            List<Weapon> weaponsEnemy = new List<Weapon>();
            
            //weaponsEnemy.Add(new Weapon(1400, 120, 1, "Frying Pan", AttackType.MELEE, WeaponSprites[2]));

            if (enemyClasses[i] == 0)
            {
                weaponsEnemy.Add(new Weapon(23, 100, 5, "Cruel Cutter", AttackType.MELEE, WeaponSprites[5]));
            }
            else if (enemyClasses[i] == 1)
            {
                weaponsEnemy.Add(new Weapon(25, 100, 5, "Stygian Staff", AttackType.MAGIC, WeaponSprites[6]));
            }

            EntityClass entityClass = (EntityClass)enemyClasses[i];

            var obj = enemyObject.Find("Parent");

            SpriteRenderer[] spriteRenderers = new SpriteRenderer[6];
            spriteRenderers[0] = obj.Find("HeadSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[0].sprite = Sprites[enemyClasses[i]][0];
            spriteRenderers[0].color = color;
            spriteRenderers[1] = obj.Find("ShoulderSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[1].sprite = Sprites[enemyClasses[i]][1];
            spriteRenderers[1].color = color;
            spriteRenderers[2] = obj.Find("BodySprite").GetComponent<SpriteRenderer>();
            spriteRenderers[2].sprite = Sprites[enemyClasses[i]][2];
            spriteRenderers[2].color = color;
            spriteRenderers[3] = obj.Find("TorsoSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[3].sprite = Sprites[enemyClasses[i]][3];
            spriteRenderers[3].color = color;
            spriteRenderers[4] = obj.Find("LegsSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[4].sprite = Sprites[enemyClasses[i]][4];
            spriteRenderers[4].color = color;
            spriteRenderers[5] = obj.Find("FeetSprite").GetComponent<SpriteRenderer>();
            spriteRenderers[5].sprite = Sprites[enemyClasses[i]][5];
            spriteRenderers[5].color = color;

            enemies[i] = new Enemy(i, new Stats(enemyClasses[i]), enemyObject, enemyAnchor, weaponsEnemy, spriteRenderers, wander[i]);
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

        rnd = new System.Random();
    }

    public bool CalculatePath(Vector3 cursor, Transform unit, bool isUnit)
    {
        RemoveGhostSprites();
        GhostEntity.gameObject.SetActive(false);
        AttackPromptObject.gameObject.SetActive(false);

        Transform Anchor;

        if (isUnit)
        {
            int unitID = UnitIDFromTransform(unit);
            Anchor = units[unitID].EntityAnchorTransform;

            if (cursor == Anchor.position)
                return false;
        }
        else
        {
            int enemyID = (EnemyIDFromTransform(unit));
            Anchor = enemies[enemyID].EntityAnchorTransform;

            if (cursor == Anchor.position)
                return false;
        }

        G_Tile OGPosTile = new G_Tile();

        if (grid.TileAtPos(Anchor.position, out OGPosTile))
        {
            
        }

        G_Tile cursorPosTile = new G_Tile();

        if (grid.TileAtPos(cursor, out cursorPosTile))
        {

        }

        (int, int)? index = (0,0);

        if (grid.IDFromTile(cursorPosTile, out index))
        {
            
        }

        OccupationStatus occupationStatus = grid.Tiles[index.Value.Item1, index.Value.Item2].OccupationState;

        Transform unitSet = null;
        Vector3 pos = new Vector3(index.Value.Item1 * indexSizeMultiplier, -1.4f, index.Value.Item2 * indexSizeMultiplier);

        if (grid.UnitAtPos(pos, out unitSet))
        {

        }

        grid.Tiles[index.Value.Item1, index.Value.Item2].FreeTile();

        var bfs = new BreadthFirstSearch(grid, (OGPosTile.TileWIndex, OGPosTile.TileHIndex));

        if (bfs.HasPathTo((cursorPosTile.TileWIndex, cursorPosTile.TileHIndex)))
        {
            var path = bfs.PathTo((cursorPosTile.TileWIndex, cursorPosTile.TileHIndex));

            int finalMoveDist = 0;

            if (isUnit)
                finalMoveDist = units[unitID].MaxMove;
            else
                finalMoveDist = enemyMaxMove;

            foreach (var item in path)
            {
                TerrainType terrainType = TerrainTypes[(int)grid.Tiles[item.Self.Item1, item.Self.Item2].TileType];
                finalMoveDist -= terrainType.TerrainHinderance;
            }

            if (path.Count > finalMoveDist)
            {
                grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);
            }
            else if (path.Count > 0)
            {
                if (isUnit)
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

                            //grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);

                            grid.Tiles[index.Value.Item1, index.Value.Item2].SetUnitOccupant(unitSet);
                            prevPath = path;
                            SetPrevAndCurrent();

                            return false;

                        case OccupationStatus.ENEMYOCCUPIED:

                            foreach (var item in path)
                            {
                                grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
                                grid.Tiles[item.Self.Item1, item.Self.Item2].GhostSpriteRend.color = new Color(1f, 0f, 0f, 0.4f);
                            }

                            (int, int) intTup = (path.Last().PrevNode.Item1, path.Last().PrevNode.Item2);
                            Vector3 newPos = new Vector3(intTup.Item1 * indexSizeMultiplier, -1.4f,
                                                         intTup.Item2 * indexSizeMultiplier);


                            
                            if (grid.Tiles[intTup.Item1, intTup.Item2].OccupationState == OccupationStatus.OPEN)
                            {
                                GhostEntity.gameObject.SetActive(true);
                                GhostEntity.transform.position = newPos;
                                //GhostEntity.GetComponentInChildren<SpriteRenderer>().sprite = units[unitID].EntityTransform.GetComponentInChildren<SpriteRenderer>().sprite;

                                Enemy enemy;
                                AttackPrompt(unitID, index, out enemy);
                                InputManager.AttackPrompt(unitID, enemy.ID);

                                grid.Tiles[index.Value.Item1, index.Value.Item2].SetEnemyOccupant(enemy.EntityTransform);
                                prevPath = path;
                                SetPrevAndCurrent();
                            }
                            else if (grid.Tiles[intTup.Item1, intTup.Item2].OccupationState == OccupationStatus.UNITOCCUPIED && path.Count == 2)
                            {
                                Enemy enemy;
                                AttackPrompt(unitID, index, out enemy);
                                InputManager.AttackPrompt(unitID, enemy.ID);

                                grid.Tiles[index.Value.Item1, index.Value.Item2].SetEnemyOccupant(enemy.EntityTransform);
                                prevPath = path;
                                SetPrevAndCurrent();
                            }
                            else
                            {
                                (int, int) intTuper = (path.Last().Self.Item1, path.Last().Self.Item2);
                                Vector3 newPoser = new Vector3(intTuper.Item1 * indexSizeMultiplier, -1.4f,
                                                             intTuper.Item2 * indexSizeMultiplier);

                                Transform enemy;
                                grid.EnemyAtPos(newPoser, out enemy);

                                grid.Tiles[index.Value.Item1, index.Value.Item2].SetEnemyOccupant(enemy);
                                prevPath = path;
                                SetPrevAndCurrent();
                            }
                            
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
                }
                else
                {
                    if (occupationStatus == OccupationStatus.UNITOCCUPIED) 
                    { 

                        grid.Tiles[index.Value.Item1, index.Value.Item2].SetUnitOccupant(unitSet);
                    }
                    else
                        grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);

                    prevPath = path;

                    return true;
                }

                //set ghostunit pos to path.last
                //GhostEntity.gameObject.SetActive(true);
                //(int, int) intTup = (path.Last().Self.Item1, path.Last().Self.Item2);
                //Vector3 newPos = new Vector3(intTup.Item1 * indexSizeMultiplier, 0,
                                            // intTup.Item2 * indexSizeMultiplier);
               // GhostEntity.transform.position = newPos;
                //GhostEntity.GetComponentInChildren<SpriteRenderer>().sprite = unit.GetComponentInChildren<SpriteRenderer>().sprite;

                
            }
            else
            {
                if (occupationStatus == OccupationStatus.UNITOCCUPIED)
                {

                    grid.Tiles[index.Value.Item1, index.Value.Item2].SetUnitOccupant(unitSet);
                }
                else
                    grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);

                prevPath = path;
            }

        }
        else
        {
            if (isUnit)
            {
                foreach (var item in prevPath)
                {
                    grid.Tiles[item.Self.Item1, item.Self.Item2].SetGhostSprite(ghostSprites[(int)item.NodeSprite]);
                    grid.Tiles[item.Self.Item1, item.Self.Item2].GhostSpriteRend.color = new Color(1f, 0f, 0f, 0.4f);
                }
            }

            if (occupationStatus == OccupationStatus.UNITOCCUPIED)
            {

                grid.Tiles[index.Value.Item1, index.Value.Item2].SetUnitOccupant(unitSet);
            }
            else
                grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);

            return false;
        }

        if (occupationStatus == OccupationStatus.UNITOCCUPIED)
        {

            grid.Tiles[index.Value.Item1, index.Value.Item2].SetUnitOccupant(unitSet);
        }
        else
            grid.Tiles[index.Value.Item1, index.Value.Item2].DEBUG_OCCUPY(occupationStatus);

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
        float xdiff = unit.EntityTransform.position.x - enemy.EntityTransform.position.x;
        float zdiff = unit.EntityTransform.position.z - enemy.EntityTransform.position.z;

        xdiff = xdiff / indexSizeMultiplier;
        zdiff = zdiff / indexSizeMultiplier;
        
        if (xdiff == 0 && MathF.Abs(zdiff) == 1)
        {
            StartBattleCallBackNoMove(unit, enemy, initalAttacker);
        }
        else if(MathF.Abs(xdiff) == 1 && zdiff == 0)
        {
            StartBattleCallBackNoMove(unit, enemy, initalAttacker);
        }
        else
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
    }

    void UnitMoveAttack(IEnumerator<ReturnNode> _em, Unit unit, Enemy enemy, InitalAttacker initalAttacker)
    {
        if (_em.MoveNext())
        {
            Vector3 newPosition = new Vector3(_em.Current.PrevNode.Item1 * indexSizeMultiplier,
                                      -1.4f,
                                      _em.Current.PrevNode.Item2 * indexSizeMultiplier);

            units[unitID].EntityTransform.DOMove(newPosition, 0.25f).SetEase(Ease.Linear).OnComplete(() => UnitMoveAttack(_em, unit, enemy, initalAttacker));
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

        //Debug.Log(enemyPos);

        G_Tile tile;

        if (grid.TileAtPos(enemyPos, out tile))
        {
            //Debug.Log("true");
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
        CameraFollow.MoveToGrid();
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
        UIManager.OpenCursorFeedback();
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
        StartCoroutine(CalcEnemyMove());
        //StartCoroutine(TempWait());
    }

    public void EndGame(bool win)
    {
        InputManager.IgnoreInput();
        UIManager.GameEnd(win);
    }

    IEnumerator CalcEnemyMove()
    {
        if (currentEnemyMove == enemies.Length)
        {
            currentEnemyMove = 0;
            EnemyMovesDone();
            yield break;
        }

        while (true)
        {
            if (enemies[currentEnemyMove].EntityStatus == EntityStatus.DEAD)
            {
                currentEnemyMove++;

                if (currentEnemyMove == enemies.Length)
                {
                    currentEnemyMove = 0;
                }
            }
            else
                break;
        }

        CameraFollow.MoveToEnemy(enemies[currentEnemyMove].EntityTransform);
        yield return new WaitForSeconds(2f);

        EnemyMove();
    }

    void EnemyMove()
    {
        //unit in area
        while (true)
        {
            List<Unit> unitsNear = new List<Unit>();
            Vector3 moveTo;

            foreach (var unit in units)
            {
                if (unit.EntityStatus == EntityStatus.DEAD)
                    continue;

                bool addUnit = false;

                //if this enemy has a valid path to current unit
                if(CalculatePath(unit.EntityTransform.position, enemies[currentEnemyMove].EntityTransform, false))
                {
                    //spaces next to unit
                    Vector3[] testPos = new Vector3[4];
                    testPos[0] = new Vector3(unit.EntityTransform.position.x + (1 * indexSizeMultiplier), -1.4f, unit.EntityTransform.position.z);
                    testPos[1] = new Vector3(unit.EntityTransform.position.x - (1 * indexSizeMultiplier), -1.4f, unit.EntityTransform.position.z);
                    testPos[2] = new Vector3(unit.EntityTransform.position.x, -1.4f, unit.EntityTransform.position.z + (1 * indexSizeMultiplier));
                    testPos[3] = new Vector3(unit.EntityTransform.position.x, -1.4f, unit.EntityTransform.position.z - (1 * indexSizeMultiplier));

                    for (int i = 0; i < testPos.Length; i++)
                    {
                        G_Tile tile;
                        if (grid.TileAtPos(testPos[i], out tile))
                        {
                            if (tile.OccupationState == OccupationStatus.OPEN && CalculatePath(testPos[i], enemies[currentEnemyMove].EntityTransform, false))
                            {
                                //potential move space found
                                addUnit = true;
                                moveTo = tile.TileWorldPos;
                            }
                        }
                    }
                }

                if (addUnit)
                    unitsNear.Add(unit);
            }

            Unit unitToAttack = null;

            if (unitsNear.Count == 0)
                break;
            else if(unitsNear.Count > 1)  
            {
                //if there is more than one unit, the one with the lowest health is prefered
                enemies[currentEnemyMove].WanderOn();
                unitToAttack = unitsNear[0];

                foreach (var unit in unitsNear)
                {
                    if (unit.EntityStats.CurrentHealth < unitToAttack.EntityStats.CurrentHealth)
                        unitToAttack = unit;
                }
            }
            else if (unitsNear.Count == 1)
            {
                enemies[currentEnemyMove].WanderOn();
                unitToAttack = unitsNear[0];
            }

            em = prevPath.GetEnumerator();
            em.MoveNext();
            EnemyMovingAttack(unitToAttack);
            return;
        }

        if (enemies[currentEnemyMove].Wander)
        {
            while (true)
            {
                //get current pos
                (int, int) currentTile = ((int)enemies[currentEnemyMove].EntityTransform.position.x / indexSizeMultiplier, (int)enemies[currentEnemyMove].EntityTransform.position.z / indexSizeMultiplier);
                //find another pos 3 tiles away
                int newX = rnd.Next(-3, 4);
                int newY = rnd.Next(-3, 4);

                (int, int) newPos = (newX + currentTile.Item1, newY + currentTile.Item2);
                if (newPos == currentTile)
                    continue;

                G_Tile tile;
                Vector3 testPos = new Vector3(newPos.Item1 * indexSizeMultiplier, -1.4f, newPos.Item2 * indexSizeMultiplier);

                if (!grid.TileAtPos(testPos, out tile))
                    continue;

                if (!CalculatePath(testPos, enemies[currentEnemyMove].EntityTransform, false))
                    continue;

                //continue to next iteration if the pos is invalid
                break;
            }

            em = prevPath.GetEnumerator();
            em.MoveNext();
            EnemyMoving();
        }
        else
        {
            currentEnemyMove++;
            StartCoroutine(CalcEnemyMove());
        }
    }

    void EnemyMoving()
    {
        if (em.MoveNext())
        {
            Vector3 newPosition = new Vector3(em.Current.Self.Item1 * indexSizeMultiplier,
                                      -1.4f,
                                      em.Current.Self.Item2 * indexSizeMultiplier);

            enemies[currentEnemyMove].EntityTransform.DOMove(newPosition, 0.25f).SetEase(Ease.Linear).OnComplete(() => EnemyMoving());
        }
        else
        {
            //commit move
            (int, int) tileIndexAnchor = (((int)enemies[currentEnemyMove].EntityAnchorTransform.position.x / indexSizeMultiplier), ((int)enemies[currentEnemyMove].EntityAnchorTransform.position.z / indexSizeMultiplier));
            grid.Tiles[tileIndexAnchor.Item1, tileIndexAnchor.Item2].FreeTile();

            enemies[currentEnemyMove].EntityAnchorTransform.position = enemies[currentEnemyMove].EntityTransform.position;

            (int, int) tileIndexAnchor2 = (((int)enemies[currentEnemyMove].EntityAnchorTransform.position.x / indexSizeMultiplier), ((int)enemies[currentEnemyMove].EntityAnchorTransform.position.z / indexSizeMultiplier));
            grid.Tiles[tileIndexAnchor2.Item1, tileIndexAnchor2.Item2].SetEnemyOccupant(enemies[currentEnemyMove].EntityTransform);

            currentEnemyMove++;
            if (currentEnemyMove == enemies.Length)
            {
                currentEnemyMove = 0;
                EnemyMovesDone();
            }
            else
            {
                StartCoroutine(CalcEnemyMove());
            }
        }
    }

    void EnemyMovingAttack(Unit toAttack)
    {
        if (em.MoveNext())
        {
            Vector3 newPosition = new Vector3(em.Current.Self.Item1 * indexSizeMultiplier,
                                      -1.4f,
                                      em.Current.Self.Item2 * indexSizeMultiplier);

            enemies[currentEnemyMove].EntityTransform.DOMove(newPosition, 0.25f).SetEase(Ease.Linear).OnComplete(() => EnemyMovingAttack(toAttack));
        }
        else
        {
            //commit move
            (int, int) tileIndexAnchor = (((int)enemies[currentEnemyMove].EntityAnchorTransform.position.x / indexSizeMultiplier), ((int)enemies[currentEnemyMove].EntityAnchorTransform.position.z / indexSizeMultiplier));
            grid.Tiles[tileIndexAnchor.Item1, tileIndexAnchor.Item2].FreeTile();

            enemies[currentEnemyMove].EntityAnchorTransform.position = enemies[currentEnemyMove].EntityTransform.position;

            (int, int) tileIndexAnchor2 = (((int)enemies[currentEnemyMove].EntityAnchorTransform.position.x / indexSizeMultiplier), ((int)enemies[currentEnemyMove].EntityAnchorTransform.position.z / indexSizeMultiplier));
            grid.Tiles[tileIndexAnchor2.Item1, tileIndexAnchor2.Item2].SetEnemyOccupant(enemies[currentEnemyMove].EntityTransform);

            Startbattle(toAttack, enemies[currentEnemyMove], InitalAttacker.ENEMY);
        }
    }

    void EnemyMovesDone()
    {
        RemoveGhostSprites();

        if (EndGameCheckEnemy())
        {
            EndGame(true);
            return;
        }
        else if (EndGameCheckUnit())
        {
            EndGame(false);
            return;
        }

        PlayerTurnStart();
    }

    IEnumerator TempWait()
    {
        yield return new WaitForSeconds(3f);

        PlayerTurnStart();
    }

    public void UnitCommitMove(bool battle)
    {
        if (units[unitID].EntityTransform.position == units[unitID].EntityAnchorTransform.position)
        {

        }
        else
        {
            (int, int) tileIndex = (((int)units[unitID].EntityTransform.position.x / indexSizeMultiplier), ((int)units[unitID].EntityTransform.position.z / indexSizeMultiplier));
            grid.Tiles[tileIndex.Item1, tileIndex.Item2].SetUnitOccupant(units[unitID].EntityTransform);
            //Debug.Log(tileIndex);

            (int, int) tileIndexAnchor = (((int)units[unitID].EntityAnchorTransform.position.x / indexSizeMultiplier), ((int)units[unitID].EntityAnchorTransform.position.z / indexSizeMultiplier));
            grid.Tiles[tileIndexAnchor.Item1, tileIndexAnchor.Item2].FreeTile();
            units[unitID].EntityAnchorTransform.position = units[unitID].EntityTransform.position;
        }

        units[unitID].GreyOut();
        EndTurnCheck(battle);
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

        UIManager.CloseCursorFeedback();

        if (initalAttacker == InitalAttacker.UNIT)
            GhostMoveUnitAttack(unit.EntityTransform, unit, enemy, initalAttacker);
        else if (initalAttacker == InitalAttacker.ENEMY)
            StartBattleCallback(unit, enemy, initalAttacker);
    }

    public void StartBattleCallback(Unit unit, Enemy enemy, InitalAttacker initalAttacker)
    {
        if (initalAttacker == InitalAttacker.UNIT)
            UnitCommitMove(true);

        CameraFollow.MoveToBattle();
        Debug.Log("HOW MANY TIMES PLEASE");
        BattleObject.GetComponent<BattleLogic>().Init(unit, enemy, initalAttacker);
    }

    public void StartBattleCallBackNoMove(Unit unit, Enemy enemy, InitalAttacker initalAttacker)
    {
        CameraFollow.MoveToBattle();
        BattleObject.GetComponent<BattleLogic>().Init(unit, enemy, initalAttacker);
    }

    public void BattleOver(int _unitID, int _enemyID, Outcome outcome, InitalAttacker attacker)
    {
        //units[_unitID].TakeDamage(5);
        //Debug.Log(units[_unitID].EntityStats.CurrentHealth);

        if (attacker == InitalAttacker.UNIT)
            units[_unitID].GreyOut();

        CameraFollow.MoveToGrid();
        UIManager.OpenCursorFeedback();

        Debug.Log(outcome);

        switch (outcome)
        {
            case Outcome.UNIT_FIRST_WIN:

                enemies[_enemyID].EntityTransform.DOMoveY(-4.5f, 0.5f).SetDelay(3f).OnComplete(() =>
                {
                    enemies[_enemyID].EntityDeader();
                    BattleOverCallback();
                });

                {
                    (int, int) tileIndex = (((int)enemies[_enemyID].EntityTransform.position.x / indexSizeMultiplier), ((int)enemies[_enemyID].EntityTransform.position.z / indexSizeMultiplier));
                    grid.Tiles[tileIndex.Item1, tileIndex.Item2].FreeTile();
                    enemies[_enemyID].EntityDead();
                }

                break;
            case Outcome.UNIT_FIRST_LOOSE:

                units[_unitID].EntityTransform.DOMoveY(-4.5f, 0.5f).SetDelay(3f).OnComplete(() =>
                {
                    units[_unitID].EntityDeader();
                    BattleOverCallback();
                });

                {
                    (int, int) tileIndex = (((int)units[_unitID].EntityTransform.position.x / indexSizeMultiplier), ((int)units[_unitID].EntityTransform.position.z / indexSizeMultiplier));
                    grid.Tiles[tileIndex.Item1, tileIndex.Item2].FreeTile();
                    units[_unitID].EntityDead();
                }

                break;
            case Outcome.ENEMY_FIRST_WIN:

                units[_unitID].EntityTransform.DOMoveY(-4.5f, 0.5f).SetDelay(3f).OnComplete(() =>
                {
                    units[_unitID].EntityDeader();
                    BattleOverCallback();
                });

                {
                    (int, int) tileIndex = (((int)units[_unitID].EntityTransform.position.x / indexSizeMultiplier), ((int)units[_unitID].EntityTransform.position.z / indexSizeMultiplier));
                    grid.Tiles[tileIndex.Item1, tileIndex.Item2].FreeTile();
                    units[_unitID].EntityDead();
                }

                break;
            case Outcome.ENEMY_FIRST_LOOSE:

                enemies[_enemyID].EntityTransform.DOMoveY(-4.5f, 0.5f).SetDelay(3f).OnComplete(() =>
                {
                    enemies[_enemyID].EntityDeader();
                    BattleOverCallback();
                });

                {
                    (int, int) tileIndex = (((int)enemies[_enemyID].EntityTransform.position.x / indexSizeMultiplier), ((int)enemies[_enemyID].EntityTransform.position.z / indexSizeMultiplier));
                    grid.Tiles[tileIndex.Item1, tileIndex.Item2].FreeTile();
                    enemies[_enemyID].EntityDead();
                }

                break;
            case Outcome.STALEMATE:

                if (attacker == InitalAttacker.UNIT)
                    BattleOverCallback();

                break;
        }

        if (attacker == InitalAttacker.ENEMY)
        {
            currentEnemyMove++;
            StartCoroutine(CalcEnemyMove());
        }
        else
        {
            if (EndGameCheckEnemy())
                EndGame(true);
            else if (EndGameCheckUnit())
                EndGame(false);
            else
                EndTurnCheck(false);
        }
    }

    void BattleOverCallback()
    {
        InputManager.BattleOver();
    }

    void EndTurnCheck(bool battle)
    {
        if (battle)
            return;

        foreach (var unit in units)
        {
            if (!unit.ActionUsed)
                return;
        }

        EnemyTurnStart();
    }

    bool EndGameCheckEnemy()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.EntityStatus != EntityStatus.DEAD)
                return false;
        }

        return true;
    }

    bool EndGameCheckUnit()
    {
        foreach (var unit in units)
        {
            if (unit.EntityStatus != EntityStatus.DEAD)
                return false;
        }

        return true;
    }

    public void UnitSelected(Transform unit)
    {
        unitID = UnitIDFromTransform(unit);
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

    public int EnemyIDFromTransform(Transform transform)
    {
        int rtnInt = 0;

        foreach (Enemy enemy in enemies)
        {
            if (enemy.EntityTransform == transform)
            {
                rtnInt = enemy.ID;
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