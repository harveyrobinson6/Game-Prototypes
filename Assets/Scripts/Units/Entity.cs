using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N_Entity
{
    public abstract class Entity
    {
        public int ID { get; protected set; }
        public Stats EntityStats { get; protected set; }
        public SpriteRenderer EntitySprite { get; protected set; }
        public Transform EntityTransform { get; protected set; }
        public Transform EntityAnchorTransform { get; protected set; }

        //ANIMATION STUFF

    }

    public struct Stats
    {
        public int MaxHealth { get; private set; }
        private int _currentHealth;
        public int CurrentHealth 
        { 
            get => _currentHealth;
            set 
            {
                int newHealth = _currentHealth - (int)value;

                if (newHealth < 0)
                    newHealth = 0;
                else
                    _currentHealth = newHealth;
            } 
        }
        public int Attack { get; private set; }
        public int Defence { get; private set; }
        public int Aether { get; private set; }
        public int Faith { get; private set; }
        public int Dexterity { get; private set; }
        public int Forfeit { get; private set; }

        public Stats(int health, int attack, int defence, int aether, int faith, int dexterity, int forfeit)
        {
            MaxHealth = health;
            Attack = attack;
            Defence = defence;
            Aether = aether;
            Faith = faith;
            Dexterity = dexterity;
            Forfeit = forfeit;

            _currentHealth = MaxHealth;
            CurrentHealth = _currentHealth;
        }
    }
}