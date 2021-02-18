﻿using UnityEngine;
using System.Collections.Generic;

public enum Status
{
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

public class Monster
{
    #region Variables
    public string Name { get; }
    public bool Fused { get; }
    public List<AttackMove> MoveSet { get; }
    public int CurHP { get; private set; }
    public int CurMP { get; private set; }
    public int CurDef { get; private set; }
    public int CurAtk { get; private set; }
    public Status CurrentStatus { get; set; }

    private MonsterScriptableObject baseData;
    private int curLevel, curXP;
    
    private readonly float SCALING = 0.2f;
    private readonly int MAX_LEVEL = 30;
    #endregion
    public Monster(MonsterScriptableObject baseData)
    {
        this.baseData = baseData;
        Name = baseData.Name;
        curLevel = baseData.BaseLevel;
        curXP = GetXP(curLevel);
        LevelUp();
        CurrentStatus = Status.Neutral;
        Fused = baseData.Fused;
        MoveSet = baseData.MoveSet;
    }

    #region HP Methods
    public void AddHP(int val)
    {
        if (CurrentStatus.Equals(Status.Fainted))
            return;

        if (val + CurHP > GetMaxHP())
            CurHP = GetMaxHP();
        else if (val + CurHP <= 0)
        {
            CurHP = 0;
            CurrentStatus = Status.Fainted;
        }
        else
            CurHP += val;
    }

    public int GetMaxHP()
    {
        return Mathf.RoundToInt(baseData.BaseHP  * Mathf.Pow((1 + SCALING), curLevel));
    }
    public void Revive()
    {
        CurrentStatus = Status.Neutral;
        CurHP = GetMaxHP();
    }
    public void TakeDamage(int enemyAtk)
    {
        // add crit chance
        var damageMultiplier = 100f / (100 + CurDef);
        var damage = -Mathf.RoundToInt(enemyAtk * damageMultiplier);
        AddHP(damage);
        Debug.Log(Name + ": multiplier: " + damageMultiplier + " damage taken: " + damage);
    }
    #endregion 

    #region MP Methods
    public void AddMP(int val)
    {
        if (val + CurMP > GetMaxMP())
            CurMP = GetMaxMP();
        else if (val + CurMP <= 0)
            CurMP = 0;
        else
            CurMP += val;
    }

    public int GetMaxMP()
    {
        return Mathf.RoundToInt(baseData.BaseMP * Mathf.Pow((1 + SCALING), curLevel));
    }
    #endregion

    #region Def Methods
    public void AddDef(int val)
    {
        if (val + CurDef <= 0)
            CurDef = 0;
        else
            CurDef += val;
    }

    public int GetMaxDef()
    {
        return Mathf.RoundToInt(baseData.BaseDefense * (curLevel + 1) * (1 + SCALING));
    }
    #endregion

    #region Atk Methods
    public void AddAtk(int val)
    {
        if (val + CurAtk <= 0)
            CurAtk = 0;
        else
            CurAtk += val;
    }

    public int GetMaxAtk()
    {
        return Mathf.RoundToInt(baseData.BaseAttack * Mathf.Pow((1 + SCALING), curLevel));
    }
    #endregion

    #region XP Methods
    public int GetXP(int? level = null)
    {
        level = level ?? curLevel;
        return Mathf.RoundToInt(4f * Mathf.Pow((int)level, 3f) / 5f);
    }

    public void AddXP(int val)
    {
        var maxXP = GetXP(MAX_LEVEL);
        var prevLevel = curLevel;
        if (val + curXP > maxXP)
        {
            curXP = maxXP;
            curLevel = MAX_LEVEL;
        }
        else if (val + curXP < 0)
        {
            curXP = 0;
            curLevel = 0;
        }
        else
        {
            curXP += val;
            curLevel = GetLevel(curXP);
        }

        if (curLevel != prevLevel)
            LevelUp();
    }
    #endregion

    #region Level Methods
    public int GetLevel(int? xp = null)
    {
        xp = xp ?? curXP;
        return Mathf.RoundToInt(5f * Mathf.Pow((int)xp, 1/3f) / 4f);
    }

    public void AddLevel(int val)
    {
        if (val + curLevel > MAX_LEVEL)
        {
            curXP = GetXP(MAX_LEVEL);
            curLevel = MAX_LEVEL;
        }
        else if (val + curLevel < 0)
        {
            curXP = 0;
            curLevel = 0;
        }
        else
        {
            curLevel += val;
            curXP = GetXP(curLevel);
        }
        LevelUp();
    }

    // Sets props to max value scaled with level
    public void LevelUp(int? level = null)
    {
        curLevel = level ?? curLevel;
        CurHP = GetMaxHP();
        CurMP = GetMaxMP();
        CurDef = GetMaxDef();
        CurAtk = GetMaxDef();
        curXP = GetXP(curLevel);
    }
    #endregion

    // Status WIP
    #region Status Methods

    #endregion

}
