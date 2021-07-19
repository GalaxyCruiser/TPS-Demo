using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//可被攻击对象
public class Damageable : MonoBehaviour
{
    private Health _health;
    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    public void InflictDamage(int damage)
    {
        _health.TakeDamage(damage);
    }


}
