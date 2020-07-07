using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData
{
    public enum AttackType
    {
        Melee,
        Hitscan,
        Projectile
    }

    public float damage;
    public float knockBackDist;
    public float range;

    AttackData(float dam, float kbd, float ran)
    {
        damage = dam;
        knockBackDist = kbd;
        range = ran;
    }
}
