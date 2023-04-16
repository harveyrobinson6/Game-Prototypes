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

        public Unit(int id, Stats stats, SpriteRenderer spriteRenderer, Transform transform, Transform anchor, List<Weapon> weapons)
        {
            ID = id;
            SelectedWeapon = 0;
            EntityStats = stats;
            EntitySprite = spriteRenderer;
            EntityTransform = transform;
            EntityAnchorTransform = anchor;

            MaxMove = 10;
            EntityName = "Sneed";
            EntityClass = EntityClass.Knight;

            Weapons = weapons;
        }
    }
}