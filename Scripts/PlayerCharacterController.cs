using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacterController : MonoBehaviour
{
    public static PlayerCharacterController instance;
    public Camera playerCamera;
    public float maxSpeedOnGround = 8f;
    public float moveSharpnessGround = 15f;
    public float rotationSpeed = 200f;
    private CharacterController _characterController;
    private PlayerInputHandler _inputHandler;
    private float _targetCharacterHeight = 1.8f;
    private float _jumpVelocity = 20f;
    private float _currentVelocityY;
    private float _cameraVerticalAngel = 0f;
    private Animator _animator;
    
    public Vector3 CharacterVelocity { get; set; }
        private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _inputHandler = GetComponent<PlayerInputHandler>();
        _characterController.enableOverlapRecovery = true;
        _animator = GetComponent<Animator>();
        UpdateCharcterHeight();
    }
    
    private void UpdateCharcterHeight()
    {
        _characterController.height = _targetCharacterHeight;
        _characterController.center = Vector3.up * _characterController.height * 0.5f;
        playerCamera.transform.localPosition = Vector3.up * _characterController.height * 1.5f + Vector3.back*1.5f;
    }
    // Update is called once per frame
    void Update()
    {

        HandleCharacterMovement();
        UpdateUI();
    }
    
    //处理角色移动消息
    private void HandleCharacterMovement()
    {
        transform.Rotate(new Vector3(0,_inputHandler.GetMouseLookHorizontal()*rotationSpeed,0),Space.Self);
        _cameraVerticalAngel += _inputHandler.GetMouseLookVertical() * rotationSpeed;
        _cameraVerticalAngel = Mathf.Clamp(_cameraVerticalAngel, -89f, 89f);
        Vector3 localMove = _inputHandler.GetMoveInput();
        Vector3 worldSpaceMoveInout = transform.TransformVector(localMove);
        playerCamera.transform.localEulerAngles = new Vector3(-_cameraVerticalAngel, 0, 0);
        //设置相机局部位置
        playerCamera.transform.localPosition = new Vector3(0,
            -2f*(float)Math.Sin(_cameraVerticalAngel/90f) + 2f,
            -2f*(float)Math.Cos(_cameraVerticalAngel/90f));
        if (_characterController.isGrounded)
        {
            _currentVelocityY = 0;
            if (_inputHandler.GetJumpInput())
            {
                _animator.SetTrigger("Jump");
                _currentVelocityY = _jumpVelocity;
            }
            Vector3 targetVeolocity = worldSpaceMoveInout * maxSpeedOnGround + Vector3.up * _currentVelocityY;
            CharacterVelocity = Vector3.Lerp(CharacterVelocity, targetVeolocity,
                moveSharpnessGround * Time.deltaTime);
        }
        else
        {
            CharacterVelocity = CharacterVelocity + Vector3.up * (Physics.gravity.y * Time.deltaTime);
        }
        _characterController.Move(CharacterVelocity * Time.deltaTime);
        _animator.SetFloat("Horizontal",localMove.x);
        _animator.SetFloat("Vertical",localMove.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        //进入敌人近战区域
        if (other.CompareTag("MeleeArea"))
        {
            ClientSettings.hp -= 10;
            if (ClientSettings.hp <= 0)
            {
                //角色死亡后重置状态
                ClientSettings.hp = 100;
                ClientSettings.ammo = 90;
                ClientSettings.ammoInCartridge = 30;
                Msg hp = new Msg("updateCharacter");
                hp.args.Add("hp");
                hp.args.Add("100");
                StartCoroutine(NetworkHost.GetInstance().Send(hp));
                Msg ammo = new Msg("updateCharacter");
                hp.args.Add("ammo");
                hp.args.Add("90");
                StartCoroutine(NetworkHost.GetInstance().Send(ammo));
                Msg ammoInCartridge = new Msg("updateCharacter");
                hp.args.Add("ammoInCartridge");
                hp.args.Add("30");
                StartCoroutine(NetworkHost.GetInstance().Send(ammoInCartridge));
                SceneManager.LoadScene("MainScence");
            }
        }
    }
    private void UpdateUI()
    {
        if ( UIManager.Instance == null) return;
        UIManager.Instance.UpdateHPBar(ClientSettings.hp);
    }
}
