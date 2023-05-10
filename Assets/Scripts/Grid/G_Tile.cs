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
        Meadows,
        Swamp,
        Forest,
        Graveyard,
        Bridge,
        River
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
        public GameObject TileOutline { get; private set; }

        public G_Tile()
        {

        }

        public G_Tile(int w, int h, int coEff, List<(int, int)> adjList, SpriteRenderer sr, TileType tileType, OccupationStatus occupation, GameObject tileOutline)
        {
            TileWIndex = w;
            TileHIndex = h;

            TileWorldPos = new Vector3(TileWIndex * coEff, -1.4f, TileHIndex * coEff);

            OccupationState = occupation;

            TileType = tileType;

            AdjacencyList = adjList;

            GhostSpriteRend = sr;

            TileOutline = tileOutline;
        }

        public void SetUnitOccupant(Transform occ)
        {
            OccupationState = OccupationStatus.UNITOCCUPIED;
            TileOccupant = occ;
            Debug.Log("Unit occupant set at " + occ.position.x/5 + "," + occ.position.z/5);
        }

        public void SetEnemyOccupant(Transform occ)
        {
            OccupationState = OccupationStatus.ENEMYOCCUPIED;
            TileOccupant = occ;
        }

        public void FreeTile()
        {
            TileOccupant = null;
            OccupationState = OccupationStatus.OPEN;
            Debug.Log(TileWIndex + "," + TileHIndex + " Now free");
        }

        public void DEBUG_OCCUPY(OccupationStatus occ)
        {
            OccupationState = occ;
            Debug.Log(TileWIndex + "," + TileHIndex + " Now occupied: " + occ);
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