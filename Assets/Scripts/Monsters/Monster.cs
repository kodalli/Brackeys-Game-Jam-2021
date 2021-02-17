using UnityEngine;

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
    public bool Owned { get; set; }
    public string Name { get; }

    private MonsterScriptableObject baseData;

    private int curHP, curMP, curDef, curAtk;
    private int curLevel, curXP;
    private Status CurrentStatus;
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
        Owned = false;
    }

    #region HP Methods
    public void AddHP(int val)
    {
        if (CurrentStatus.Equals(Status.Fainted))
            return;

        if (val + curHP > GetMaxHP())
            curHP = GetMaxHP();
        else if (val + curHP < 0)
        {
            curHP = 0;
            CurrentStatus = Status.Fainted;
        }
        else
            curHP += val;
    }

    public int GetCurrentHP()
    {
        return curHP;
    }

    public int GetMaxHP()
    {
        return Mathf.RoundToInt(baseData.BaseHP * (curLevel + 1) * (1 + SCALING));
    }
    public void Revive()
    {
        CurrentStatus = Status.Neutral;
        curHP = GetMaxHP();
    }
    public void TakeDamage(int enemyAtk)
    {
        // add crit chance
        var damageMultiplier = 100f / (100 + curDef);
        var damage = enemyAtk * damageMultiplier;
        AddHP(-Mathf.RoundToInt(damage));
    }
    #endregion 

    #region MP Methods
    public void AddMP(int val)
    {
        if (val + curMP > GetMaxMP())
            curMP = GetMaxMP();
        else if (val + curMP < 0)
            curMP = 0;
        else
            curMP += val;
    }

    public int GetCurrentMP()
    {
        return curMP;
    }

    public int GetMaxMP()
    {
        return Mathf.RoundToInt(baseData.BaseMP * (curLevel + 1) * (1 + SCALING));
    }
    #endregion

    #region Def Methods
    public void AddDef(int val)
    {
        if (val + curDef < 0)
            curDef = 0;
        else
            curDef += val;
    }

    public int GetCurrentDef()
    {
        return curDef;
    }

    public int GetMaxDef()
    {
        return Mathf.RoundToInt(baseData.BaseDefense * (curLevel + 1) * (1 + SCALING));
    }
    #endregion

    #region Atk Methods
    public void AddAtk(int val)
    {
        if (val + curAtk < 0)
            curAtk = 0;
        else
            curAtk += val;
    }

    public int GetCurrentAtk()
    {
        return curAtk;
    }

    public int GetMaxAtk()
    {
        return Mathf.RoundToInt(baseData.BaseAttack * (curLevel + 1) * (1 + SCALING));
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
        curHP = GetMaxHP();
        curMP = GetMaxMP();
        curDef = GetMaxDef();
        curAtk = GetMaxDef();
        curXP = GetXP(curLevel);
    }
    #endregion

    // Status WIP
    #region Status Methods
    public Status GetStatus()
    {
        return CurrentStatus;
    }
    public void SetStatus(Status NextStatus)
    {
        CurrentStatus = NextStatus;
    }
    #endregion

}
