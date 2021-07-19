using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponManager : MonoBehaviour
{
    public enum ShootingMode
    {
        Single,
        Auto
    }
    public WeaponController startingWeapons;
    public Camera weaponCamera;
    private Animator _animator;
    private ShootingMode _shootingMode;
   

    // Start is called before the first frame update
    void Start()
    {
        startingWeapons.Owner = gameObject;
        _animator = GetComponent<Animator>();
        _shootingMode = ShootingMode.Auto;
    }

    void Update()
    {
        WeaponController activeWeapon = startingWeapons;
        if (activeWeapon)
        {
            //射击成功，播放动画
            if (activeWeapon.HandleShootInputs(PlayerInputHandler.instance.GetFireInputHeld(_shootingMode)))
            {
                _animator.SetTrigger("Shoot");
            }
            //换弹成功，播放动画
            if (activeWeapon.HandleReloadInputs(PlayerInputHandler.instance.GetReloadInputHeld()))
            {
                _animator.SetTrigger("Reload");
            }
            //切换射击模式
            if (Input.GetButtonDown("Switch"))
            {
                _shootingMode = _shootingMode == ShootingMode.Auto ? ShootingMode.Single : ShootingMode.Auto;
                string mode =  _shootingMode == ShootingMode.Auto? "AUTO" : "SINGLE";
                UIManager.Instance.UpdateModeText(mode);
            }
        }
    }
}
