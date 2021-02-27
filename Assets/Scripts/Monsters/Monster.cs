using UnityEngine;
using System.Collections.Generic;

public enum Status {
    Neutral,
    Stunned,
    Enraged,
    Depressed,
    Traumatized,
    High,
    Sad,
    Happy,
    Horny,
    Confused,
    Woke,
    Fainted,
}

public class Monster {
    #region Variables

    public string Name { get; }
    public bool Fused { get; }
    public List<AttackMove> MoveSet { get; }
    public int CurHP { get; private set; }
    public int CurMP { get; private set; }
    public int CurDef { get; private set; }
    public int CurAtk { get; private set; }
    public Status CurrentStatus { get; set; }

    private int curLevel, curXP;
    
    private readonly MonsterScriptableObject baseData;
    private readonly float SCALING = 0.2f;
    private readonly int MAX_LEVEL = 30;

    #endregion

    public Monster(MonsterScriptableObject baseData) {
        this.baseData = baseData;
        Name = baseData.Name;
        curLevel = baseData.BaseLevel;
        curXP = GetXPFromLevel(curLevel);
        LevelUp();
        CurrentStatus = Status.Neutral;
        Fused = baseData.Fused;
        MoveSet = baseData.MoveSet;
    }

    public GameObject GetPrefab() => baseData.Prefab;

    #region HP Methods
    public void AddHP(int val) {
        if (CurrentStatus.Equals(Status.Fainted))
            return;

        if (val + CurHP > GetMaxHP())
            CurHP = GetMaxHP();
        else if (val + CurHP <= 0) {
            CurHP = 0;
            CurrentStatus = Status.Fainted;
        }
        else
            CurHP += val;
    }

    public int GetMaxHP() => Mathf.RoundToInt(baseData.BaseHP * Mathf.Pow((1 + SCALING), curLevel));
    
    public void Revive() {
        // Reset all values
        CurrentStatus = Status.Neutral;
        CurHP = GetMaxHP();
        CurAtk = GetMaxAtk();
        CurDef = GetMaxDef();
        CurMP = GetMaxMP();
    }
    
    public int TakeDamage(int enemyAtk) {
        // add crit chance
        var damageMultiplier = 100f / (100 + CurDef);
        var damage = -Mathf.RoundToInt(Mathf.Pow(enemyAtk, 0.75f) * damageMultiplier);
        AddHP(damage);
        return damage;
        //Debug.Log(Name + ": multiplier: " + damageMultiplier + " damage taken: " + damage);
    }

    public void Heal(int? amount = 0) {
        if (amount != null && amount > 0) {
            AddHP((int)amount);
        }
        else {
            amount = Mathf.RoundToInt(GetMaxHP()*0.3f);
            AddHP((int)amount);
        }
    }
    #endregion 

    #region MP Methods
    public void AddMP(int val) {
        if (val + CurMP > GetMaxMP())
            CurMP = GetMaxMP();
        else if (val + CurMP <= 0)
            CurMP = 0;
        else
            CurMP += val;
    }

    public int GetMaxMP() => Mathf.RoundToInt(baseData.BaseMP * Mathf.Pow((1 + SCALING), curLevel));

    #endregion

    #region Def Methods
    public void AddDef(int val) {
        if (val + CurDef <= 0)
            CurDef = 0;
        else
            CurDef += val;
    }

    public int GetMaxDef() => Mathf.RoundToInt(baseData.BaseDefense * (curLevel + 1) * (1 + SCALING));

    #endregion

    #region Atk Methods
    public void AddAtk(int val) {
        if (val + CurAtk <= 0)
            CurAtk = 0;
        else
            CurAtk += val;
    }

    public int GetMaxAtk() => Mathf.RoundToInt(baseData.BaseAttack * Mathf.Pow((1 + SCALING), curLevel));
    #endregion

    #region XP Methods
    public int GetXPFromLevel(int level = 0) => Mathf.RoundToInt(4f * Mathf.Pow(level, 3f) / 5f);

    public int GetCurXP() => curXP;

    public void AddXP(int val) {
        var maxXP = GetXPFromLevel(MAX_LEVEL);
        var prevLevel = curLevel;
        if (val + curXP > maxXP) {
            curXP = maxXP;
            curLevel = MAX_LEVEL;
        }
        else if (val + curXP < 0) {
            curXP = 0;
            curLevel = 0;
        }
        else {
            curXP += val;
            curLevel = GetLevel();
        }

        if (curLevel != prevLevel)
            LevelUp();
    }
    #endregion

    #region Level Methods
    public int GetLevel() => Mathf.RoundToInt(5f * Mathf.Pow(curXP, 1 / 3f) / 4f);

    public void AddLevel(int val) {
        if (val + curLevel > MAX_LEVEL) {
            curXP = GetXPFromLevel(MAX_LEVEL);
            curLevel = MAX_LEVEL;
        }
        else if (val + curLevel < 0) {
            curXP = 0;
            curLevel = 0;
        }
        else {
            curLevel += val;
            curXP = GetXPFromLevel(curLevel);
        }
        LevelUp();
    }

    // Sets props to max value scaled with level
    public void LevelUp(int? level = null) {
        curLevel = level ?? curLevel;
        CurHP = GetMaxHP();
        CurMP = GetMaxMP();
        CurDef = GetMaxDef();
        CurAtk = GetMaxDef();
        curXP = GetXPFromLevel(curLevel);
    }
    #endregion

    #region Status Methods

    public void ApplyStatusEffect(Status appliedStatus) {
        //Neutral,
        //Stunned,
        //Enraged,
        //Depressed,
        //Traumatized,
        //High,
        //Sad,
        //Happy,
        //Horny,
        //Confused,
        //Woke,
        //Fainted,

        switch(appliedStatus) {
            case Status.Neutral:
                ApplyNeutral();
                break;
            case Status.Stunned:
                ApplyStunned();
                break;
            case Status.Enraged:
                ApplyEnraged();
                break;
            case Status.Depressed:
                ApplyDepressed();
                break;
            case Status.Traumatized:
                ApplyTraumatized();
                break;
            case Status.High:
                ApplyHigh();
                break;
            case Status.Sad:
                ApplySad();
                break;
            case Status.Happy:
                ApplyHappy();
                break;
            case Status.Horny:
                ApplyHorny();
                break;
            case Status.Confused:
                ApplyConfused();
                break;
            case Status.Woke:
                ApplyWoke();
                break;
            case Status.Fainted:
                ApplyFainted();
                break;
            default:
                Debug.LogError("Not valid status");
                break;
        }
    }

    private void ApplyNeutral() {
        var previousHP = CurHP;
        var previousMP = CurMP;
        Revive();
        CurHP = previousHP;
        CurMP = previousMP;
        CurrentStatus = Status.Neutral;
    }

    private void ApplyStunned() {
        CurrentStatus = Status.Stunned;
    }

    private void ApplyEnraged() {
        // inc atk
        CurAtk = Mathf.RoundToInt(CurAtk + 1.5f * curLevel + 1);
        CurrentStatus = Status.Enraged;

    }

    private void ApplyDepressed() {
        // dec def and atk 
        CurDef = Mathf.RoundToInt(CurDef + 0.8f);
        CurAtk = Mathf.RoundToInt(CurAtk + 0.8f);
        CurrentStatus = Status.Depressed;

    }

    private void ApplyTraumatized() {
        // dec def and atk medium
        CurDef = Mathf.RoundToInt(CurDef * 0.5f);
        CurAtk = Mathf.RoundToInt(CurAtk * 0.5f);
        CurrentStatus = Status.Traumatized;

    }

    private void ApplyHigh() {
        // inc def alot
        CurDef = Mathf.RoundToInt(CurDef + 4f * curLevel + 1);
        CurrentStatus = Status.High;

    }

    private void ApplyHappy() {
        // inc def and atk medium
        CurDef = Mathf.RoundToInt(CurDef + 1.5f * curLevel + 1);
        CurAtk = Mathf.RoundToInt(CurAtk + 1.5f * curLevel + 1);
        CurrentStatus = Status.Happy;

    }

    private void ApplySad() {
        // lower def
        CurDef = Mathf.RoundToInt(CurDef * 0.8f);
        CurrentStatus = Status.Sad;

    }

    private void ApplyHorny() {
        // inc def, lower atk alot
        CurAtk = Mathf.RoundToInt(CurAtk + 2f * curLevel + 1);
        CurDef = Mathf.RoundToInt(CurDef * 0.2f);
        CurrentStatus = Status.Horny;

    }

    private void ApplyConfused() {
        // miss attack
        CurrentStatus = Status.Confused;

    }

    private void ApplyWoke() {
        // can attack agian
        CurrentStatus = Status.Woke;
        CurDef = Mathf.RoundToInt(CurDef * 0.2f);

    }

    private void ApplyFainted() {
        CurHP = 0;
        CurrentStatus = Status.Fainted;

    }

    #endregion

}
