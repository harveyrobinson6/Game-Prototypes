using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N_Entity
{
    public class Enemy : Entity
    {



        public Enemy(int id, Stats stats, SpriteRenderer spriteRenderer, Transform transform)
        {
            ID = id;
            EntityStats = stats;
            EntitySprite = spriteRenderer;
            EntityTransform = transform;
        }
    }
}