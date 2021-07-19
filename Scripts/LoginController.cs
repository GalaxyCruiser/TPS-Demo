using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class LoginController : MonoBehaviour
{

    public NetworkHost networkHost;
    private bool _loginSuccess;
    public InputField account;
    public InputField pwd;
    public Text tips;
    
    // Start is called before the first frame update
    void Start()
    {
        networkHost = NetworkHost.GetInstance();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        tips.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(_loginSuccess)return;
        networkHost.Receive();
        while (networkHost.receivedMessages.Count>0)
        {
            Msg msg  = networkHost.receivedMessages.Dequeue();
            MethodInfo methodInfo = this.GetType().GetMethod(msg.method);
            methodInfo.Invoke(this, msg.args.ToArray<object>());
        }
    }
    
    //发送注册消息
    public void Regisger()
    {
        Msg regMsg = new Msg();
        regMsg.method = "register";
        regMsg.args.Add(account.text);
        regMsg.args.Add(pwd.text);
        StartCoroutine(networkHost.Send(regMsg));
    }
    
    //发送登录消息
    public void Login()
    {
        Msg loginMsg = new Msg();
        loginMsg.method = "login";
        loginMsg.args.Add(account.text);
        loginMsg.args.Add(pwd.text);
        StartCoroutine(networkHost.Send(loginMsg));
    }
    public void Login1()
    {
        Msg loginMsg = new Msg();
        loginMsg.method = "login";
        loginMsg.args.Add("netease1");
        loginMsg.args.Add("123");
        StartCoroutine(networkHost.Send(loginMsg));
    }
    public void Login2()
    {
        Msg loginMsg = new Msg();
        loginMsg.method = "login";
        loginMsg.args.Add("netease2");
        loginMsg.args.Add("123");
        StartCoroutine(networkHost.Send(loginMsg));
    }
    public void Login3()
    {
        Msg loginMsg = new Msg();
        loginMsg.method = "login";
        loginMsg.args.Add("netease3");
        loginMsg.args.Add("123");
        StartCoroutine(networkHost.Send(loginMsg));
    }
    
    //注册成功提示
    public void registerSuccess(string account )
    {
        tips.text = account+ " Register Success";
    }
    
    //用户名冲突提示accountExist
    public void accountExist(string account)
    {
        tips.text = account+ " Already Exist";
    }
    
    //数据库错误提示
    public void databaseError(string account)
    {
        tips.text = "DatabaseError when create "+account ;
    }

    //登录错误提示
    public void loginFail(string account)
    {
        tips.text = "wrong account or password ";
    }
    
    //登录成功后，进入角色选择界面
    public void loginSuccess(string account)
    {
        _loginSuccess = true;
        ClientSettings.account = account;
        SceneManager.LoadScene("CharacterSelect");
    }

    //设置clientID
    public void setClientID(string hid)
    {
        ClientSettings.clientID = Int32.Parse(hid);
    }
}
