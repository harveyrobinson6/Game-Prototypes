using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using N_Entity;
using DG.Tweening;

public class BattleLogic : MonoBehaviour
{
    [SerializeField] BattleSceneManager BSM;
    [SerializeField] UIManager UIManager;
    [SerializeField] Transform UnitTransform;
    [SerializeField] Transform EnemyTransform;
    [SerializeField] SpriteRenderer UnitSprite;
    [SerializeField] SpriteRenderer EnemySprite;
    [SerializeField] SpriteRenderer UnitWeapon;
    [SerializeField] SpriteRenderer EnemyWeapon;

    [SerializeField] SpriteRenderer UnitWeaponSprite;
    [SerializeField] SpriteRenderer UnitTorsoSprite;
    [SerializeField] SpriteRenderer UnitLegsSprite;
    [SerializeField] SpriteRenderer UnitBodySprite;
    [SerializeField] SpriteRenderer UnitHeadSprite;
    [SerializeField] SpriteRenderer UnitFeetSprite;
    [SerializeField] SpriteRenderer UnitShoulderSprite;

    [SerializeField] SpriteRenderer EnemyWeaponSprite;
    [SerializeField] SpriteRenderer EnemyTorsoSprite;
    [SerializeField] SpriteRenderer EnemyLegsSprite;
    [SerializeField] SpriteRenderer EnemyBodySprite;
    [SerializeField] SpriteRenderer EnemyHeadSprite;
    [SerializeField] SpriteRenderer EnemyFeetSprite;
    [SerializeField] SpriteRenderer EnemyShoulderSprite;

    [SerializeField] SoundManager SM;

    public void Init(Unit _unit, Enemy _enemy, InitalAttacker attacker)
    {
        Battle currentBattle = new Battle(_unit, _enemy, attacker);

        UnitTransform.localPosition = new Vector3(-5, 1, -85);
        EnemyTransform.localPosition = new Vector3(5, 1, -85);

        UnitWeaponSprite.sprite = null;
        UnitTorsoSprite.sprite = null;
        UnitLegsSprite.sprite = null;
        UnitBodySprite.sprite = null;
        UnitHeadSprite.sprite = null;
        UnitFeetSprite.sprite = null;
        UnitShoulderSprite.sprite = null;

        EnemyWeaponSprite.sprite = null;
        EnemyTorsoSprite.sprite = null;
        EnemyLegsSprite.sprite = null;
        EnemyBodySprite.sprite = null;
        EnemyHeadSprite.sprite = null;
        EnemyFeetSprite.sprite = null;
        EnemyShoulderSprite.sprite = null;

        UnitWeaponSprite.sprite = _unit.Weapons[_unit.SelectedWeapon].WeaponSprite;
        UnitTorsoSprite.sprite = _unit.Sprites[3].sprite;
        UnitLegsSprite.sprite = _unit.Sprites[4].sprite;
        UnitBodySprite.sprite = _unit.Sprites[2].sprite;
        UnitHeadSprite.sprite = _unit.Sprites[0].sprite;
        UnitFeetSprite.sprite = _unit.Sprites[5].sprite;
        UnitShoulderSprite.sprite = _unit.Sprites[1].sprite;

        EnemyWeaponSprite.sprite = _enemy.Weapons[_enemy.SelectedWeapon].WeaponSprite;
        EnemyTorsoSprite.sprite = _enemy.Sprites[3].sprite;
        EnemyLegsSprite.sprite = _enemy.Sprites[4].sprite;
        EnemyBodySprite.sprite = _enemy.Sprites[2].sprite;
        EnemyHeadSprite.sprite = _enemy.Sprites[0].sprite;
        EnemyFeetSprite.sprite = _enemy.Sprites[5].sprite;
        EnemyShoulderSprite.sprite = _enemy.Sprites[1].sprite;

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

                        if (!currentBattle.attackerHit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyMiss()));
                        }   
                        else if (currentBattle.attackerCrit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyCrt(currentBattle.attackerDamage)));
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }
                        else
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyTakeDamage(currentBattle.attackerDamage)));
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }

                        //enemy sinks through floor
                        sequence.Append(EnemyTransform.DOLocalMoveY(-4.5f, 0.5f).SetDelay(2f));
                        break;
                    case Outcome.UNIT_FIRST_LOOSE:
                        //unit attacks enemy
                        sequence.Append(UnitTransform.DOLocalMoveX(5, 1f));

                        if (!currentBattle.attackerHit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyMiss()));
                        }
                        else if (currentBattle.attackerCrit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyCrt(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.defenderDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }
                        else
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyTakeDamage(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.defenderDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }

                        //enemy counterattacks
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f).SetDelay(2f));

                        if (!currentBattle.defenderHit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitMiss()));
                        }
                        else if (currentBattle.defenderCrit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitCrt(currentBattle.defenderDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.defenderDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }
                        else
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitTakeDamage(currentBattle.defenderDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.defenderDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }

                        //unit sinks through floor
                        sequence.Append(UnitTransform.DOLocalMoveY(-4.5f, 0.5f).SetDelay(2f));
                        break;
                    case Outcome.STALEMATE:
                        //unit attacks enemy
                        sequence.Append(UnitTransform.DOLocalMoveX(5, 1f));

                        if (!currentBattle.attackerHit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyMiss()));
                        }
                        else if (currentBattle.attackerCrit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyCrt(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.defenderDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }
                        else
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyTakeDamage(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.defenderDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }

                        //enemy counterattacks
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f).SetDelay(2f));

                        if (!currentBattle.defenderHit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitMiss()));
                        }
                        else if (currentBattle.defenderCrit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitCrt(currentBattle.defenderDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.defenderDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }
                        else
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitTakeDamage(currentBattle.defenderDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.defenderDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.attackerDamage);
                        }

                        break;
                }

                break;
            case InitalAttacker.ENEMY:

                switch (currentBattle.Outcome)
                {
                    case Outcome.ENEMY_FIRST_WIN:
                        //enemy attacks unit
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f));

                        if (!currentBattle.attackerHit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitMiss()));
                        }
                        else if (currentBattle.attackerCrit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitCrt(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                        }
                        else
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitTakeDamage(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                        }

                        //unit sinks through floor
                        sequence.Append(UnitTransform.DOLocalMoveY(-4.5f, 0.5f).SetDelay(2f));
                        break;
                    case Outcome.ENEMY_FIRST_LOOSE:
                        //enemy attacks unit
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f));

                        if (!currentBattle.attackerHit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitMiss()));
                        }
                        else if (currentBattle.attackerCrit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitCrt(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.defenderDamage);
                        }
                        else
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitTakeDamage(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.defenderDamage);
                        }

                        //unit counterattacks
                        sequence.Append(UnitTransform.DOLocalMoveX(5, 1f).SetDelay(2f));

                        if (!currentBattle.defenderHit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyMiss()));
                        }
                        else if (currentBattle.defenderCrit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyCrt(currentBattle.defenderDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.defenderDamage);
                        }
                        else
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyTakeDamage(currentBattle.defenderDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.defenderDamage);
                        }

                        //enemy sinks through floor
                        sequence.Append(EnemyTransform.DOLocalMoveY(-4.5f, 0.5f).SetDelay(2f));
                        break;
                    case Outcome.STALEMATE:
                        //enemy attacks unit
                        sequence.Append(EnemyTransform.DOLocalMoveX(-5, 1f));

                        if (!currentBattle.attackerHit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitMiss()));
                        }
                        else if (currentBattle.attackerCrit)
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitCrt(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.defenderDamage);
                        }
                        else
                        {
                            sequence.Append(EnemyTransform.DOLocalMoveX(5, 0.25f).OnComplete(() => UIManager.UnitTakeDamage(currentBattle.attackerDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.defenderDamage);
                        }

                        //unit counterattacks
                        sequence.Append(UnitTransform.DOLocalMoveX(5, 1f).SetDelay(2f));

                        if (!currentBattle.defenderHit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyMiss()));
                        }
                        else if (currentBattle.defenderCrit)
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyCrt(currentBattle.defenderDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.defenderDamage);
                        }
                        else
                        {
                            sequence.Append(UnitTransform.DOLocalMoveX(-5, 0.25f).OnComplete(() => UIManager.EnemyTakeDamage(currentBattle.defenderDamage)));
                            BSM.units[_unit.ID].TakeDamage(currentBattle.attackerDamage);
                            BSM.enemies[_enemy.ID].TakeDamage(currentBattle.defenderDamage);
                        }

                        break;
                }

                break;
        }

        //don't need to start sequence, it starts automatically at end of scope
        sequence.AppendInterval(2f);
        sequence.AppendCallback(() => { BSM.BattleOver(_unit.ID, _enemy.ID, currentBattle.Outcome, attacker); });
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
                        if (damageVal < 0)
                            damageVal = 0;
                        return damageVal;
                    }

                case AttackType.MAGIC:

                    {
                        int damageVal = ((attacker.EntityStats.Aether + attackerWeapon.Power) / 2) - defender.EntityStats.Faith;
                        if (damageVal < 0)
                            damageVal = 0;
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