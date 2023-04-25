using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N_Entity;
using DG.Tweening;

public class BattleLogic : MonoBehaviour
{
    [SerializeField] BattleSceneManager BSM;
    [SerializeField] Transform UnitTransform;
    [SerializeField] Transform EnemyTransform;
    [SerializeField] SpriteRenderer UnitSprite;
    [SerializeField] SpriteRenderer EnemySprite;
    [SerializeField] SpriteRenderer UnitWeapon;
    [SerializeField] SpriteRenderer EnemyWeapon;

    public void Init(Unit _unit, Enemy _enemy, InitalAttacker attacker)
    {
        Battle currentBattle = new Battle(_unit, _enemy, attacker);

        UnitTransform.localPosition = new Vector3(-5, -1.3f, 0);
        EnemyTransform.localPosition = new Vector3(5, -1.3f, 0);

        UnitSprite.sprite = _unit.Sprites[0].sprite;
        EnemySprite.sprite = _enemy.Sprites[0].sprite;

        UnitWeapon.sprite = _unit.Weapons[_unit.SelectedWeapon].WeaponSprite;
        EnemyWeapon.sprite = _enemy.Weapons[_enemy.SelectedWeapon].WeaponSprite;

        var sequence = DOTween.Sequence();
        sequence.PrependInterval(2f);

        switch (currentBattle.InitalAttacker)
        {
            case InitalAttacker.UNIT:

                switch (currentBattle.Outcome)
                {
                    case Outcome.UNIT_FIRST_WIN:
                        Debug.Log("wipwip");
                        //unit attacks enemy
                        sequence.Append(UnitTransform.DOLocalMoveX(5, 1f));
                        sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f));
                        //enemy sinks through floor
                        sequence.Append(EnemyTransform.DOLocalMoveY(0, 0.5f).SetDelay(2f));
                        //DO DAMAGE
                        break;
                    case Outcome.UNIT_FIRST_LOOSE:
                        //unit attacks enemy
                        sequence.Append(UnitTransform.DOLocalMoveX(5, 1f));
                        sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f));
                        //enemy counterattacks
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f).SetDelay(2f));
                        sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f));
                        //unit sinks through floor
                        sequence.Append(UnitTransform.DOLocalMoveY(0, 0.5f).SetDelay(2f));
                        break;
                    case Outcome.STALEMATE:
                        //unit attacks enemy
                        sequence.Append(UnitTransform.DOLocalMoveX(5, 1f));
                        sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f));
                        //enemy counterattacks
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f).SetDelay(2f));
                        sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f));
                        BSM.units[_unit.ID].TakeDamage(currentBattle.defenderDamage);
                        break;
                }

                break;
            case InitalAttacker.ENEMY:

                switch (currentBattle.Outcome)
                {
                    case Outcome.ENEMY_FIRST_WIN:
                        //enemy attacks unit
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f));
                        sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f));
                        //unit sinks through floor
                        sequence.Append(UnitTransform.DOLocalMoveY(0, 0.5f).SetDelay(2f));
                        break;
                    case Outcome.ENEMY_FIRST_LOOSE:
                        //enemy attacks unit
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f));
                        sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).SetDelay(1));
                        //unit counterattacks
                        sequence.Append(UnitTransform.DOLocalMoveX(5, 1f).SetDelay(2f));
                        sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f));
                        //enemy sinks through floor
                        sequence.Append(EnemyTransform.DOLocalMoveY(0, 0.5f).SetDelay(2f));
                        break;
                    case Outcome.STALEMATE:
                        //enemy attacks unit
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f));
                        sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f));
                        //unit counterattacks
                        sequence.Append(UnitTransform.DOLocalMoveX(5, 1f).SetDelay(2f));
                        sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f));
                        break;
                }

                break;
        }

        //don't need to start sequence, it starts automatically at end of scope
        sequence.AppendCallback(() => { BSM.BattleOver(_unit.ID, _enemy.ID, currentBattle.Outcome); });
        //set health
    }



    public class Battle
    {
        public Unit unit { get; private set; }
        public Enemy enemy { get; private set; }
        public InitalAttacker InitalAttacker { get; private set; }
        public Outcome Outcome { get; private set; }
        public int attackerDamage { get; private set; }
        public int defenderDamage { get; private set; }
        public bool attackerHit { get; private set; }
        public bool defenderHit { get; private set; }
        public bool attackerCrit { get; private set; }
        public bool defenderCrit { get; private set; }

        public Battle(Unit _unit, Enemy _enemy, InitalAttacker attacker)
        {
            unit = _unit;
            enemy = _enemy;
            InitalAttacker = attacker;

            switch (InitalAttacker)
            {
                case InitalAttacker.UNIT:

                    attackerDamage = CalculateDamage(unit, enemy);
                    defenderDamage = CalculateDamage(enemy, unit);

                    attackerHit = CalculateHit(unit, enemy);
                    defenderHit = CalculateHit(enemy, unit);

                    attackerCrit = CalacualteCrit(unit, enemy);
                    defenderCrit = CalacualteCrit(enemy, unit);

                    if (attackerCrit)
                        attackerDamage = (int)(attackerDamage * 1.5f);
                    if (defenderCrit)
                        defenderDamage = (int)(defenderDamage * 1.5f);

                    Outcome = outcome();
                    //Outcome = Outcome.UNIT_FIRST_WIN;

                    break;

                case InitalAttacker.ENEMY:

                    attackerDamage = CalculateDamage(enemy, unit);
                    defenderDamage = CalculateDamage(unit, enemy);

                    attackerHit = CalculateHit(enemy, unit);
                    defenderHit = CalculateHit(unit, enemy);

                    attackerCrit = CalacualteCrit(enemy, unit);
                    defenderCrit = CalacualteCrit(unit, enemy);

                    if (attackerCrit)
                        attackerDamage = (int)(attackerDamage * 1.5f);
                    if (defenderCrit)
                        defenderDamage = (int)(defenderDamage * 1.5f);

                    Outcome = outcome();

                    break;
            }
        }

        int CalculateDamage(Entity attacker, Entity defender)
        {
            Weapon attackerWeapon = attacker.Weapons[attacker.SelectedWeapon];
            Weapon defenderWeapon = defender.Weapons[defender.SelectedWeapon];

            switch (attackerWeapon.AttackType)
            {
                case AttackType.MELEE:

                    {
                        int damageVal = ((attacker.EntityStats.Attack + attackerWeapon.Power) / 2) - defender.EntityStats.Defence;
                        return damageVal;
                    }

                case AttackType.MAGIC:

                    {
                        int damageVal = ((attacker.EntityStats.Aether + attackerWeapon.Power) / 2) - defender.EntityStats.Faith;
                        return damageVal;
                    }
            }

            return 0;
        }

        bool CalculateHit(Entity attacker, Entity defender)
        {
            Weapon attackerWeapon = attacker.Weapons[attacker.SelectedWeapon];
            Weapon defenderWeapon = defender.Weapons[defender.SelectedWeapon];

            int accuracyVal = (attacker.EntityStats.Dexterity + attackerWeapon.Accuracy);
            accuracyVal = Mathf.Clamp(accuracyVal, 0, 100);

            if (accuracyVal == 100)
                return true;
            else if (accuracyVal == 0)
                return false;

            System.Random RandomGen = new System.Random();
            int rng = RandomGen.Next(101);

            if (rng <= accuracyVal)
                return true;
            else
                return false;
        }

        bool CalacualteCrit(Entity attacker, Entity defender)
        {
            Weapon attackerWeapon = attacker.Weapons[attacker.SelectedWeapon];
            Weapon defenderWeapon = defender.Weapons[defender.SelectedWeapon];

            int criticalVal = ((attacker.EntityStats.Dexterity - attacker.EntityStats.Forfeit * 2) + attackerWeapon.Crit);
            criticalVal = Mathf.Clamp(criticalVal, 0, 100);

            if (criticalVal == 100)
                return true;
            else if (criticalVal == 0)
                return false;

            System.Random RandomGen = new System.Random();
            int rng = RandomGen.Next(101);

            if (rng <= criticalVal)
                return true;
            else
                return false;
        }

        Outcome outcome()
        {
            Debug.Log(defenderHit);
            Debug.Log(defenderDamage);
            Debug.Log(unit.EntityStats.CurrentHealth);

            switch (InitalAttacker)
            {
                case InitalAttacker.UNIT:  //unit first, then enemy

                    if (attackerHit == false && defenderHit == false)  //both miss
                        return Outcome.STALEMATE;

                    if (attackerHit == true && attackerDamage >= enemy.EntityStats.CurrentHealth)  //unit hits first and lands killing blow
                        return Outcome.UNIT_FIRST_WIN;

                    if (defenderHit == true && defenderDamage >= unit.EntityStats.CurrentHealth)  //unit hits first and doesn't kill. enemy counter attacks and kills
                        return Outcome.UNIT_FIRST_LOOSE;

                    break;
                case InitalAttacker.ENEMY:  //enemy first, then unit

                    if (attackerHit == false && defenderHit == false)  //both miss
                        return Outcome.STALEMATE;

                    if (attackerHit == true && attackerDamage >= unit.EntityStats.CurrentHealth)  //enemy hits first and lands killing blow
                        return Outcome.ENEMY_FIRST_WIN;

                    if (defenderHit == true && defenderDamage >= enemy.EntityStats.CurrentHealth)  //enemy hits first and doesn't kill. unit counter attacks and kills
                        return Outcome.ENEMY_FIRST_LOOSE;

                    break;
            }

            return Outcome.STALEMATE;
        }
    }
}
public enum Outcome
{
    UNIT_FIRST_WIN,
    UNIT_FIRST_LOOSE,
    ENEMY_FIRST_WIN,
    ENEMY_FIRST_LOOSE,
    STALEMATE
}

public enum InitalAttacker
{
    UNIT,
    ENEMY
}