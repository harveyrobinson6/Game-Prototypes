using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N_Grid;

public class Cursor : MonoBehaviour
{
    [SerializeField] BattleSceneManager BSM;

    public void CardinalDirections(Direction dir)
    {
        switch (dir)
        {
            case Direction.UP:
                Move(new Vector3(0, 0, 5));
                break;
            case Direction.DOWN:
                Move(new Vector3(0, 0, -5));
                break;
            case Direction.LEFT:
                Move(new Vector3(-5, 0, 0));
                break;
            case Direction.RIGHT:
                Move(new Vector3(5, 0, 0));
                break;
        }
    }
    public void Move(Vector3 dir)
    {
        Vector3 potentialPos = transform.position + dir;
        G_Tile tile = new G_Tile();

        if (BSM.grid.TileAtPos(potentialPos, out tile))
        {
            transform.position = potentialPos;
        }
        else
            return;
    }

    private void FixedUpdate()
    {
        
    }
}
