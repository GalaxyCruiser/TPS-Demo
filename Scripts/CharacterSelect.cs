using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public NetworkHost networkHost;
    public InputField characterName;
    public Text tips;
    public Text chooseTips;
    public GameObject character1;
    public GameObject character2;
    private int _characterCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        networkHost = NetworkHost.GetInstance();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        tips.text = "";
        chooseTips.text = "";
        character1.SetActive(false);
        character2.SetActive(false);
        character1.GetComponent<Button>().onClick.AddListener(gameStart);
        character2.GetComponent<Button>().onClick.AddListener(gameStart);
        Msg msg = new Msg("getCharacters"); //获取当前账户角色
        msg.args.Add(ClientSettings.account);
        StartCoroutine(networkHost.Send(msg));
    }

    // Update is called once per frame
    void Update()
    {
        // 接收处理网络消息
        networkHost.Receive();
        while (networkHost.receivedMessages.Count>0)
        {
            Msg msg  = networkHost.receivedMessages.Dequeue();
            MethodInfo methodInfo = GetType().GetMethod(msg.method);
            methodInfo.Invoke(this, new object[]{msg.args.ToArray<object>()});
        }
    }
    public void createCharacter()
    {
        //当前账号角色数量上线为2个
        if (_characterCount >= 2)
        {
            tips.text = "character slot full";
            return;
        }
        if (characterName.text != "")
        {
            Msg regMsg = new Msg {method = "createCharacter"};
            regMsg.args.Add(characterName.text);
            regMsg.args.Add(ClientSettings.account);
            StartCoroutine(networkHost.Send(regMsg));
        }
    }

    // 显示当前账号的角色
    public void showCharacter(params object[] args)
    {
        _characterCount = args.Length;
        chooseTips.text = "please choose character";
        if (args.Length >= 1)
        {
            character1.SetActive(true);
            character1.GetComponentInChildren<Text>().text = (string)args[0];
        }
        if (args.Length >= 2)
        {
            character2.SetActive(true);
            character2.GetComponentInChildren<Text>().text = (string)args[1];
        }
    }
    
    //加载角色信息
    public void loadCharacterInfo(params object[] args)
    {
        ClientSettings.hp = Int32.Parse((string)args[2]);
        ClientSettings.ammo = Int32.Parse((string)args[3]);
        ClientSettings.ammoInCartridge = Int32.Parse((string)args[4]);
        ClientSettings.lvl = Int32.Parse((string)args[5]);
        ClientSettings.exp = Int32.Parse((string)args[6]);
        SceneManager.LoadScene("MainScence");
    }

    //选择角色后获取角色信息
    public void gameStart()
    {
        var buttonSelf = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        string characterName = buttonSelf.GetComponentInChildren<Text>().text;
        ClientSettings.characterName = characterName;
        Msg msg = new Msg("getCharacterInfo");
        msg.args.Add(characterName);
        StartCoroutine(networkHost.Send(msg));
        
    }
}
