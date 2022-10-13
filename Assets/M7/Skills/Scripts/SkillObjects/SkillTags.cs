using System;

[Flags]
public enum SkillTags
{
    None = 0,
    Buff = 1,
    Debuff = 1 << 1,
    Damage = 1 << 2,
    Heal = 1 << 3,
    Stun = 1 << 4,
    Poison = 1 << 5
}
