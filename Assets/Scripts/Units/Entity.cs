using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N_Entity
{
    public enum EntityClass  //COULD MAKE THIS ITS OWN CLASS LATER
    {
        Knight,
        Mage
    }

    public enum EntityStatus
    {
        ALIVE,
        STASIS,
        DEAD
    }

    public abstract class Entity
    {
        public int ID { get; protected set; }
        public int MaxMove { get; protected set; }
        public int SelectedWeapon { get; protected set; }
        public string EntityName { get; protected set; }
        public EntityClass EntityClass { get; protected set; }
        public EntityStatus EntityStatus { get; protected set; }
        public Stats EntityStats { get; protected set; }
        public List<Weapon> Weapons { get; protected set; }
        public SpriteRenderer[] Sprites { get; protected set; }
        public Transform EntityTransform { get; protected set; }
        public Transform EntityAnchorTransform { get; protected set; }
        public bool ActionUsed { get; protected set; }

        public void NewWeaponSelected(int ID)
        {
            SelectedWeapon = ID;
        }

        public void EntityDead()
        {
            //set enum
            EntityStatus = EntityStatus.DEAD;
            //move transform and anchor somewhere
            
        }

        public void EntityDeader()
        {
            EntityTransform.position = new Vector3(-100, 0, 0);
            EntityAnchorTransform.position = new Vector3(-100, 0, 0);
        }

        public void TakeDamage(int _damage)
        {
            int damage = (int)Mathf.Clamp(_damage, 0, EntityStats.CurrentHealth);

            EntityStats.TakeDamage(damage);
        }

        public void GreyOut()
        {
            ActionUsed = true;

            foreach (var sr in Sprites)
            {
                Color color = new Color(0.5f, 0.5f, 0.5f, 1f);
                sr.color = color;
            }
        }

        public void GreyIn()
        {
            ActionUsed = false;

            foreach (var sr in Sprites)
            {
                Color color = new Color(1f, 1f, 1f, 1f);
                sr.color = color;
            }
        }

    }

    public class Stats
    {
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; set; }
        public int Attack { get; private set; }
        public int Defence { get; private set; }
        public int Aether { get; private set; }
        public int Faith { get; private set; }
        public int Dexterity { get; private set; }
        public int Forfeit { get; private set; }

        public Stats(int entityClass)
        {
            if (entityClass == 0)
            {
                MaxHealth = 27;
                Attack = 17;
                Defence = 17;
                Aether = 3;
                Faith = 11;
                Dexterity = 17;
                Forfeit = 4;
            }
            else if (entityClass == 1)
            {
                MaxHealth = 25;
                Attack = 7;
                Defence = 11;
                Aether = 24;
                Faith = 25;
                Dexterity = 11;
                Forfeit = 7;
            }

            CurrentHealth = MaxHealth;
        }
        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
        }
    }
}