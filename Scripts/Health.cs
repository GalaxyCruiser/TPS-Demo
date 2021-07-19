using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int _currentHealth;
    private Animator _animator;
    public bool isDead;

    private void Awake()
    {
        _currentHealth = maxHealth;
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            if (!isDead)
            {
                //每次击杀敌人增加10点经验值
                ClientSettings.exp += 10;
                Msg msg = new Msg("updateCharacter");
                msg.args.Add(ClientSettings.characterName);
                msg.args.Add(ClientSettings.entityID.ToString());
                msg.args.Add("exp");
                msg.args.Add(ClientSettings.exp.ToString());
                StartCoroutine(NetworkHost.GetInstance().Send(msg));
                //每100点经验值提升一级等级
                if (ClientSettings.exp % 100 == 0)
                {
                    ClientSettings.lvl += 1;
                    Msg lvlmsg = new Msg("updateCharacter");
                    lvlmsg.args.Add(ClientSettings.characterName);
                    lvlmsg.args.Add(ClientSettings.entityID.ToString());
                    lvlmsg.args.Add("lvl");
                    lvlmsg.args.Add(ClientSettings.lvl.ToString());
                    StartCoroutine(NetworkHost.GetInstance().Send(lvlmsg));
                }
            }
            
            isDead = true;
            //播放死亡动画
            _animator.SetTrigger("die");
            Destroy(gameObject,3);
        }
    }

    void Update()
    {
        // 更新EXP，LVL显示
        UIManager.Instance.UpdateExpText(ClientSettings.exp);
        UIManager.Instance.UpdateLvlText(ClientSettings.lvl);
    }
}
