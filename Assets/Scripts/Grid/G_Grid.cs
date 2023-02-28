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

        public G_Grid(int width, int height, int  ism, Transform[,] tilePrefabs)
        {
            Width = width;
            Height = height;
            IndexSizeMultiplier = ism;

            Tiles = new G_Tile[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //make adj grid
                    //add one to w then h, minus 1 to w then h
                    //if less than 0 or greater than w/h then skip

                    List<(int, int)> adjList = new List<(int, int)>();

                    //(+1,0)
                    if (i + 1 < width)
                        adjList.Add((i + 1, j));

                    //(-1,0)
                    if (i - 1 > 0)
                        adjList.Add((i - 1, j));    //DO BOUNDS CHECK DURING ALGORITHM

                    //(0,+1)
                    if (j + 1 < height)
                        adjList.Add((i, j + 1));

                    //(0,-1)
                    if (j - 1 > 0)
                        adjList.Add((i, j - 1));
                    
                    G_Tile newTile = new G_Tile(i, j, IndexSizeMultiplier, adjList, tilePrefabs[i,j].Find("GhostSprite").GetComponentInChildren<SpriteRenderer>());  
                    Tiles[i, j] = newTile;
                    Debug.Log(i + ", " + j);
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
                    if (tile.Occupied)
                    {
                        outUnit = tile.TileOccupant;
                        return true;
                    }
                    else
                        break;
                }
            }

            outUnit = null;
            return false;
        }

        public List<(int, int)> GetAdjacency((int, int) tileIndex)
        {
            return Tiles[tileIndex.Item1, tileIndex.Item2].AdjacencyList;
        }
    }
}