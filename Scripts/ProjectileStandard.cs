using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStandard : MonoBehaviour
{

    public float maxLifeTime = 5f;
    public float speed = 100f;
    public int damage = 20;
    public Transform root;
    public Transform tip;
    public LayerMask hittableLayers = -1;
    public GameObject impactVFX;
    public float impactVFXLifeTime = 5f;
    public float impactVFXSpawnOffset = 0.1f;
    private Vector3 _lastRootPosition;
    private ProjectileBase _projectileBase;
    private Vector3 _velocity;
    private Vector3 _consumedCorrectionVector;
    private Vector3 _showDamageLoc;

    private void OnEnable()
    {
        _projectileBase = GetComponent<ProjectileBase>();
        _projectileBase.onShoot += OnShoot;
        Destroy(gameObject,maxLifeTime);
    }

    private void OnShoot()
    {
        _lastRootPosition = root.position;
        _velocity = transform.forward * speed;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity.normalized;
        RaycastHit hit = new RaycastHit();
        Vector3 displaymentSinceLastFrame = tip.position - _lastRootPosition;   //飞行距离
        //射线检测
         if (Physics.Raycast(_lastRootPosition,
            transform.forward,out hit,
            displaymentSinceLastFrame.magnitude,
            hittableLayers,
            QueryTriggerInteraction.Collide))
         {
             if (IsHitValid(hit))
            {
                if (hit.distance <= 0)
                {
                    hit.point = root.position;
                    hit.normal = -transform.forward;
                }
                OnHit(hit.point, hit.normal, hit.collider);
            }
         }
    }
    

    private bool IsHitValid(RaycastHit hit)
    {
        if (hit.collider.isTrigger)
        {
            return false;
        }
        return true;
    }

    private void OnHit(Vector3 point, Vector3 normal,Collider collider)
    {
        Damageable damageable = collider.GetComponent<Damageable>();
        // 判断是否可被攻击
        if (damageable)
        {
            _showDamageLoc = Camera.main.WorldToScreenPoint(point);
            UIManager.Instance.showDamage(_showDamageLoc, damage);
            damageable.InflictDamage(damage);
        }
        if (impactVFX != null)
        {
            //添加攻击特效
            GameObject impactVFXInstance = Instantiate(impactVFX,
                point + normal * impactVFXSpawnOffset,
                Quaternion.LookRotation(normal));
            if (impactVFXLifeTime > 0)
            {
                Destroy(impactVFXInstance, impactVFXLifeTime);
            }
        }
        Destroy(gameObject);
    }
}
