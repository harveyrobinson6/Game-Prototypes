using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N_Entity
{
    public class Enemy : Entity
    {
        public Enemy()
        {

        }

        public Enemy(int id, Stats stats, SpriteRenderer spriteRenderer, Transform transform, Transform anchor, List<Weapon> weapons)
        {
            ID = id;
            SelectedWeapon = 0;
            EntityStats = stats;
            EntitySprite = spriteRenderer;
            EntityTransform = transform;
            EntityAnchorTransform = anchor;

            Weapons = weapons;
        }
    }
}