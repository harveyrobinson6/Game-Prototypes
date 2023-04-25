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

        public Unit(int id, Stats stats, Transform transform, Transform anchor, List<Weapon> weapons, SpriteRenderer[] spriteRenderers)
        {
            ID = id;
            SelectedWeapon = 0;
            EntityStats = stats;
            //EntitySprite = spriteRenderer;
            EntityTransform = transform;
            EntityAnchorTransform = anchor;

            MaxMove = 10;
            EntityName = "Sneed";
            EntityClass = EntityClass.Knight;

            Weapons = weapons;

            Sprites = spriteRenderers;
            ActionUsed = false;
        }
    }
}