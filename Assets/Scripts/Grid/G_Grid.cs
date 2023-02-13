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

        public G_Grid(int width, int height, int  ism)
        {
            Width = width;
            Height = height;
            IndexSizeMultiplier = ism;

            Tiles = new G_Tile[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    G_Tile newTile = new G_Tile(i, j, IndexSizeMultiplier);  
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
    }
}