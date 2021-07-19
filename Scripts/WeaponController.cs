using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    public GameObject weaponRoot;
    public ProjectileBase projectilePrefab;
    public Transform weaponMuzzle;
    public GameObject muzzleFlashPrefab;
    public float delayBetweenShots = 0.2f;
    private float _lastShotTime = Mathf.NegativeInfinity;
    private int _cartridgeCapacity = 30;
    private float _reloadTime = 2.667f;
    public AudioClip shotClip;
    public AudioClip reloadClip;
    private AudioSource _audioSource;
    public bool IsWeaponActive { get; set; }
    public GameObject Owner { get; set; }
    public GameObject SourcePrefab { get; set; }
    public Vector3 MuzzleWorldVelocity { get; private set; }
 
    public bool HandleShootInputs(bool inputHeld)
    {
        if (inputHeld)
        {
            return TryShoot();
        }
        return false;
    }
    public bool HandleReloadInputs(bool inputHeld)
    {
        if (inputHeld)
        {
            return TryReload();
        }
        return false;
    }

    private bool TryReload()
    {
        if (ClientSettings.ammoInCartridge!=_cartridgeCapacity  && ClientSettings.ammo != 0)
        {
            _audioSource.clip = reloadClip;
            _audioSource.Play();
            StartCoroutine(ReloadRoutine());
            return true;
        }
        return false;
    }

    //换弹
    private IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(_reloadTime);
        int refill = Math.Min(_cartridgeCapacity- ClientSettings.ammoInCartridge, ClientSettings.ammo) ;
        ClientSettings.ammoInCartridge += refill;
        ClientSettings.ammo -= refill;
        Msg msg = new Msg("updateCharacter");
        msg.args.Add(ClientSettings.characterName);
        msg.args.Add(ClientSettings.entityID.ToString());
        msg.args.Add("ammoInCartridge");
        msg.args.Add(ClientSettings.ammoInCartridge.ToString());
        StartCoroutine(NetworkHost.GetInstance().Send(msg));
        Msg ammoMsg = new Msg("updateCharacter");
        ammoMsg.args.Add(ClientSettings.characterName);
        ammoMsg.args.Add(ClientSettings.entityID.ToString());
        ammoMsg.args.Add("ammo");
        ammoMsg.args.Add(ClientSettings.ammo.ToString());
        StartCoroutine(NetworkHost.GetInstance().Send(ammoMsg));
    }
    private bool TryShoot()
    {
        if (_lastShotTime + delayBetweenShots < Time.time && ClientSettings.ammoInCartridge > 0)
        {
            ClientSettings.ammoInCartridge--;
            Msg msg = new Msg("updateCharacter");
            msg.args.Add(ClientSettings.characterName);
            msg.args.Add(ClientSettings.entityID.ToString());
            msg.args.Add("ammoInCartridge");
            msg.args.Add(ClientSettings.ammoInCartridge.ToString());
            StartCoroutine(NetworkHost.GetInstance().Send(msg));
            //空弹匣自动换弹
            if (ClientSettings.ammoInCartridge == 0)
            {
                TryReload();
            }
            _audioSource.clip = shotClip;
            _audioSource.Play();
            HandleShoot();
            return true;
        }
        return false;
    }
    
    //射击
    private void HandleShoot()
    {
        //显示弹道
        if (projectilePrefab != null)
        {
            weaponMuzzle.rotation = Camera.main.transform.rotation;
            Vector3 shotDirection = weaponMuzzle.forward;
            ProjectileBase newProjectile = Instantiate(projectilePrefab, weaponMuzzle.position, 
                weaponMuzzle.rotation);
            newProjectile.Shoot(this);
        }
        //显示枪口火花
        if(muzzleFlashPrefab != null)
        {
            GameObject muzzleFlashInstance = Instantiate(muzzleFlashPrefab, weaponMuzzle.position, 
                weaponMuzzle.rotation,weaponMuzzle.transform);
            Destroy(muzzleFlashInstance, 2);
        }
        _lastShotTime = Time.time;
    }
    private void UpdateUI()
    {
        if ( UIManager.Instance == null) return;
        UIManager.Instance.UpdateAmmoText(ClientSettings.ammoInCartridge, ClientSettings.ammo);
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }
}
