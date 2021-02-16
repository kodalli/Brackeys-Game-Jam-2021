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

public class Monster : MonoBehaviour
{
    public bool Owned { get; set; }
    public string Name { get; }

    private MonsterScriptableObject baseData;

    private int curHP, curMP, curDef, curAtk;
    private int curLevel, curXP;
    private Status curStatus;
    private readonly float SCALING = 0.2f;
    private readonly int MAX_LEVEL = 30;

    public Monster(MonsterScriptableObject baseData)
    {
        this.baseData = baseData;
        Name = baseData.Name;
        curHP = baseData.BaseHP;
        curMP = baseData.BaseMP;
        curDef = baseData.BaseDefense;
        curAtk = baseData.BaseAttack;
        curLevel = baseData.BaseLevel;
        curXP = GetXP(curLevel);
        curStatus = Status.Neutral;
        Owned = false;
    }

    #region HP Methods
    public void AddHP(int val)
    {
        if (val + curHP > GetMaxHP())
            curHP = GetMaxHP();
        else if (val + curHP < 0)
            curHP = 0;
        else
            curHP += val;
    }

    public int GetCurrentHP()
    {
        return curHP;
    }

    public int GetMaxHP()
    {
        return Mathf.RoundToInt(baseData.BaseHP * curLevel * (1 + SCALING));
    }
    #endregion Functions

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
        return Mathf.RoundToInt(baseData.BaseMP * curLevel * (1 + SCALING));
    }
    #endregion

    #region Def Methods
    public void AddDef(int val)
    {
        if (val + curDef > GetMaxDef())
            curDef = GetMaxDef();
        else if (val + curDef < 0)
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
        return Mathf.RoundToInt(baseData.BaseDefense * curLevel * (1 + SCALING));
    }
    #endregion

    #region Atk Methods
    public void AddAtk(int val)
    {
        if (val + curAtk > GetMaxAtk())
            curAtk = GetMaxAtk();
        else if (val + curAtk < 0)
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
        return Mathf.RoundToInt(baseData.BaseAttack * curLevel * (1 + SCALING));
    }
    #endregion

    #region XP Methods
    public int GetXP(int level)
    {
        return Mathf.RoundToInt(4f * Mathf.Pow(level, 3f) / 5f);
    }

    public void AddXP(int val)
    {
        var maxXP = GetXP(MAX_LEVEL);
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
    }
    #endregion

    #region Level Methods
    public int GetLevel(int xp)
    {
        return Mathf.RoundToInt(5f * Mathf.Pow(xp, 1/3f) / 4f);
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
    }
    #endregion

    // Status WIP
    #region Status Methods
    public void ChangeStatus(Status status)
    {
        curStatus = status;
    }
    #endregion
}
