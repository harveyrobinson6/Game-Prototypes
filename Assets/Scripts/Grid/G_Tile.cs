using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N_Grid
{
    public class G_Tile
    {
        public int TileWIndex { get; private set; }
        public int TileHIndex { get; private set; }
        public Vector3 TileWorldPos { get; private set; }
        public Transform TileOccupant { get; private set; }
        public bool Occupied { get; private set; }
        public List<(int x, int y)> AdjacencyList { get; private set; }
        public SpriteRenderer GhostSpriteRend { get; private set; }

        public G_Tile()
        {

        }

        public G_Tile(int w, int h, int coEff, List<(int, int)> adjList, SpriteRenderer sr)
        {
            TileWIndex = w;
            TileHIndex = h;

            TileWorldPos = new Vector3(TileWIndex * coEff, 0, TileHIndex * coEff);

            Occupied = false;

            AdjacencyList = adjList;

            GhostSpriteRend = sr;
        }

        public void SetOccupant(Transform occ)
        {
            TileOccupant = occ;
            Occupied = true;
        }

        public void SetGhostSprite(Sprite sprite)
        {
            GhostSpriteRend.sprite = sprite;
        }

        public void ClearGhostSprite()
        {
            GhostSpriteRend.sprite = null;
        }
    }
}