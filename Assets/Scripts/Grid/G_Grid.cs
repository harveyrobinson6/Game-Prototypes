using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N_Grid
{
    public class G_Grid
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int IndexSizeMultiplier { get; private set; }
        public G_Tile[,] Tiles { get; private set; }
        public bool SpritesDisplay { get; private set; }

        public G_Grid(int width, int height, int  ism, Transform[,] tilePrefabs)
        {
            Width = width;
            Height = height;
            IndexSizeMultiplier = ism;
            SpritesDisplay = true;

            (int, int)[] forests = new (int, int)[] { (2, 7), (3, 7), (4, 7), (5, 7), (1, 8), (2, 8), (3, 8), (4, 8),
                                                     (5, 8), (6, 8), (1, 9), (2, 9), (3, 9), (4, 9), (5, 9),
                                                     (6, 9), (2, 10),
                                                     (20, 7), (21, 7), (22, 7), (23, 7), (24, 7), (25, 8), (24, 8),
                                                     (23, 8), (22, 8), (21, 8), (20, 8), (19, 8), (19, 9), (20, 9),
                                                     (21, 9), (22, 9), (23, 9), (24, 9), (25, 9), (25, 10), (24, 10),
                                                     (23, 10), (22, 10), (21, 10), (20, 10), (19, 10), (18, 10)};
            (int, int)[] swamps = new (int, int)[] { (5, 2), (5, 3), (5, 4), (5, 5), (6, 1), (6, 2), (6, 3), (6, 4),
                                                     (6, 5), (7, 0), (7, 1), (7, 2), (7, 3), (7, 4), (7, 5), (7, 6),
                                                     (8, 0), (8, 1), (8, 2), (8, 3), (8, 4), (8, 5), (8, 6),
                                                     (9, 1), (9, 2), (9, 3), (9, 4), (9, 5), (9, 6), (10, 2), (10, 3),
                                                     (10, 4), (10, 5),
                                                     (11, 0), (11, 1), (12, 1), (12, 0), (13, 0), (13, 1), (13, 2), (14, 2),
                                                     (14, 1), (14, 0), (15, 0), (15, 1), (17, 3), (17, 2), (18, 0), (18, 1),
                                                     (18, 2), (18, 3), (19, 3), (19, 2), (19, 1), (19, 0), (20, 0), (20, 1),
                                                     (20, 2), (20, 3)};
            (int, int)[] grave = new (int, int)[] {  (27, 4), (28, 4), (29, 4), (30, 4), (27, 5), (28, 5), (29, 5), (30, 5),
                                                     (26, 5), (26, 6), (27, 6), (28, 6), (29, 6), (30, 6), (26, 7), (27, 7),
                                                     (28, 7), (29, 7), (30, 7), (27, 8), (28, 8), (29, 8) };
            (int, int)[] bridge = new (int, int)[] { (14, 6), (15, 6), (16, 6) };
            (int, int)[] river = new (int, int)[] {  (15, 10), (14, 9), (15, 9), (14, 8), (15, 8), (14, 7), (15, 7), (15, 5),
                                                     (16, 5), (15, 4), (16, 4), (15, 3), (16, 3), (15, 2), (16, 2), (16, 1),
                                                     (17, 1), (16, 0), (17, 0) };
            (int, int)[] nogo = new (int, int)[] {   (27, 5), (27, 6), (27, 7), (29, 7) };

            Tiles = new G_Tile[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    List<(int, int)> adjList = new List<(int, int)>();

                    //(+1,0)
                    if (i + 1 < width)
                        adjList.Add((i + 1, j));
                    //(-1,0)
                    if (i - 1 > -1)
                        adjList.Add((i - 1, j));
                    //(0,+1)
                    if (j + 1 < height)
                        adjList.Add((i, j + 1));
                    //(0,-1)
                    if (j - 1 > -1)
                        adjList.Add((i, j - 1));

                    TileType tileType = TileType.Meadows;
                    OccupationStatus occupationStatus = OccupationStatus.OPEN;

                    foreach (var index in forests)
                    {
                        if (i == index.Item1 && j == index.Item2)
                        {
                            tileType = TileType.Forest;
                            break;
                        }
                    }

                    foreach (var index in swamps)
                    {
                        if (i == index.Item1 && j == index.Item2)
                        {
                            tileType = TileType.Swamp;
                            break;
                        }
                    }

                    foreach (var index in grave)
                    {
                        if (i == index.Item1 && j == index.Item2)
                        {
                            tileType = TileType.Graveyard;
                            break;
                        }
                    }

                    foreach (var index in bridge)
                    {
                        if (i == index.Item1 && j == index.Item2)
                        {
                            tileType = TileType.Bridge;
                            break;
                        }
                    }

                    foreach (var index in river)
                    {
                        if (i == index.Item1 && j == index.Item2)
                        {
                            tileType = TileType.River;
                            occupationStatus = OccupationStatus.OUTOFBOUNDS;
                            break;
                        }
                    }

                    foreach (var index in nogo)
                    {
                        if (i == index.Item1 && j == index.Item2)
                        {
                            occupationStatus = OccupationStatus.OUTOFBOUNDS;
                            break;
                        }
                    }

                    G_Tile newTile = new G_Tile(i, j, IndexSizeMultiplier, adjList, tilePrefabs[i, j].Find("GhostSprite").GetComponentInChildren<SpriteRenderer>(), tileType, occupationStatus, tilePrefabs[i, j].Find("BaseTile").gameObject);

                    Tiles[i, j] = newTile;
                    //Debug.Log(i + ", " + j);
                }
            }
        }

        public bool TileAtPos(Vector3 pos, out G_Tile outTile)
        {
            foreach (G_Tile tile in Tiles)
            {
                if (tile.TileWorldPos == pos)
                {
                    outTile = tile;
                    return true;
                }
            }

            outTile = new G_Tile();
            return false;
        }

        public bool UnitAtPos(Vector3 pos, out Transform outUnit)
        {
            foreach (G_Tile tile in Tiles)
            {
                if (tile.TileWorldPos == pos)
                {
                    if (tile.OccupationState == OccupationStatus.UNITOCCUPIED)
                    {
                        outUnit = tile.TileOccupant;
                        Debug.Log(tile.OccupationState);
                        return true;
                    }
                    else
                    {
                        Debug.Log(tile.OccupationState);
                        break;
                    }

                }
            }

            outUnit = null;
            return false;
        }

        public bool EnemyAtPos(Vector3 pos, out Transform outEnemy)
        {
            foreach (G_Tile tile in Tiles)
            {
                if (tile.TileWorldPos == pos)
                {
                    if (tile.OccupationState == OccupationStatus.ENEMYOCCUPIED)
                    {
                        outEnemy = tile.TileOccupant;
                        return true;
                    }
                    else
                        break;
                }
            }

            outEnemy = null;
            return false;
        }

        public bool OccupyTile(Vector3 pos, Transform occupant)
        {
            foreach (G_Tile tile in Tiles)
            {
                if (tile.TileWorldPos == pos)
                {
                    if (tile.OccupationState == OccupationStatus.OPEN)
                    {
                        tile.SetUnitOccupant(occupant);
                        return true;
                    }
                    else
                        return false;
                }
            }
            return false;
        }

        public bool IDFromTile(G_Tile tile, out (int, int)? ID)
        {
            for (int i = 0; i < Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < Tiles.GetLength(1); j++)
                {
                    if (Tiles[i,j] == tile)
                    {
                        ID = (i, j);
                        return true;
                    }
                }
            }

            ID = null;
            return false;
        }

        public void ToggleSprites()
        {
            if (SpritesDisplay)
            {
                foreach (var tile in Tiles)
                {
                    tile.TileOutline.SetActive(false);
                }
            }
            else
            {
                foreach (var tile in Tiles)
                {
                    tile.TileOutline.SetActive(true);
                }
            }

            SpritesDisplay = !SpritesDisplay;
        }

        public void HideGrid()
        {
            foreach (var tile in Tiles)
            {
                tile.TileOutline.SetActive(false);
            }

            SpritesDisplay = false;
        }
        
        public List<(int, int)> GetAdjacency((int, int) tileIndex)
        {
            return Tiles[tileIndex.Item1, tileIndex.Item2].AdjacencyList;
        }
    }
}