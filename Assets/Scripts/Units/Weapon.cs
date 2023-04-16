using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public int Power { get; private set; }
    public int Accuracy { get; private set; }
    public int Crit { get; private set; }
    public string Name { get; private set; }
    public AttackType AttackType { get; private set; }
    public Sprite WeaponSprite { get; private set; }

    public Weapon(int power, int accuracy, int crit, string name, AttackType attackType, Sprite sprite)
    {
        Power = power;
        Accuracy = accuracy;
        Crit = crit;
        Name = name;
        AttackType = attackType;
        WeaponSprite = sprite;
    }
}

public enum AttackType
{
    MELEE,
    MAGIC
}
