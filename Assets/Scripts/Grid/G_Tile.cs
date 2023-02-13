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

        public G_Tile()
        {

        }

        public G_Tile(int w, int h, int coEff)
        {
            TileWIndex = w;
            TileHIndex = h;

            TileWorldPos = new Vector3(TileWIndex * coEff, 0, TileHIndex * coEff);
        }
    }
}