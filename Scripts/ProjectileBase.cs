using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileBase : MonoBehaviour
{
    public GameObject Owner { get; private set; }
    public Vector3 InitialPosition { get; private set; }
    public Vector3 InitialDiriection { get; private set; }
    public Vector3 InheritedMuzzleVelocity { get; private set; }
    public UnityAction onShoot;
    
    // 记录射击初始状态
    public void Shoot(WeaponController controller)
    {
        Owner = controller.Owner;
        InitialPosition = transform.position;
        InitialDiriection = transform.forward;
        InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
        if (onShoot != null)
        {
            onShoot.Invoke();
        }
    }

}
