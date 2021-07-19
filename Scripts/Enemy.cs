using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public Transform target;
    public BoxCollider meleeArea;
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Health _health;
    private bool _isChase;
    private bool _isAttack;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        Invoke("ChaseStart",2);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        //设玩家角色位置为target
        target = PlayerCharacterController.instance.transform;
    }

    private void ChaseStart()
    {
        _isChase = true;
        if (_animator)
        {
            _animator.SetBool("isWalk",true);
        }
    }

    private void FreezeVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //当敌人开启导航并存活，追踪玩家角色
        if (_navMeshAgent.enabled &&  !_health.isDead)
        {
            _navMeshAgent.SetDestination(target.position);
            _navMeshAgent.isStopped = !_isChase;
        }
        else
        {
            _navMeshAgent.SetDestination(transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (_isChase)
        {
            FreezeVelocity();
        }
        Targeting();
    }

    private void Targeting()
    {
        //检查前方是否存在玩家角色
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.5f, transform.forward,
             2f, LayerMask.GetMask("Player"));
        if (hits.Length>0 && !_isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    //对玩家发动攻击
    IEnumerator Attack()
    {
        _isChase = false;
        _isAttack = true;
        _animator.SetBool("isAttack",true);
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(1f);
        _isChase = true;
        _isAttack = false;
        _animator.SetBool("isAttack",false);

    }
}
