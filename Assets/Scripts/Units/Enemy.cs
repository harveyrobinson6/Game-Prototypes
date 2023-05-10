using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N_Entity
{
    public class Enemy : Entity
    {
        public bool Wander { get; private set; }

        public Enemy()
        {

        }

        public Enemy(int id, Stats stats, Transform transform, Transform anchor, List<Weapon> weapons, SpriteRenderer[] spriteRenderers, bool wander)
        {
            ID = id;
            SelectedWeapon = 0;
            EntityStats = stats;
            //EntitySprite = spriteRenderer;
            EntityTransform = transform;
            EntityAnchorTransform = anchor;

            Weapons = weapons;
            EntityName = "Enemy";
            Sprites = spriteRenderers;
            ActionUsed = false;
            Wander = wander;
        }

        public void WanderOn()
        {
            Wander = true;
        }
    }
}