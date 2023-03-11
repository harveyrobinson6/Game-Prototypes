using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N_Grid
{
    public enum OccupationStatus
    {
        OPEN,
        UNITOCCUPIED,
        ENEMYOCCUPIED,
        EDIFICE,
        OUTOFBOUNDS
    }

    public enum TileType
    {
        MEADOWS,
        SWAMP,
        MOUNTAIN
    }

    public class G_Tile
    {
        public int TileWIndex { get; private set; }
        public int TileHIndex { get; private set; }
        public Vector3 TileWorldPos { get; private set; }
        public Transform TileOccupant { get; private set; }
        public OccupationStatus OccupationState { get; private set; }
        public TileType TileType { get; private set; }
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

            OccupationState = OccupationStatus.OPEN; //CHANGE THIS
            TileType = TileType.MEADOWS;

            AdjacencyList = adjList;

            GhostSpriteRend = sr;
        }

        public void SetOccupant(Transform occ)
        {
            OccupationState = OccupationStatus.UNITOCCUPIED;
            TileOccupant = occ;
        }

        public void FreeTile()
        {
            TileOccupant = null;
            OccupationState = OccupationStatus.OPEN;
        }

        public void DEBUG_OCCUPY()
        {
            OccupationState = OccupationStatus.UNITOCCUPIED;
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