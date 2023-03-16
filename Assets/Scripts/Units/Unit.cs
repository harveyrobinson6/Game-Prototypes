using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N_Entity
{
    public class Unit : Entity
    {


        public Unit()
        {

        }

        public Unit(int id, Stats stats, SpriteRenderer spriteRenderer, Transform transform)
        {
            ID = id;
            EntityStats = stats;
            EntitySprite = spriteRenderer;
            EntityTransform = transform;
        }
    }
}