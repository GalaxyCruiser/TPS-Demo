using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    NetworkHost _networkHost = NetworkHost.GetInstance();
    private bool _playerRegistered;
    private Queue<Msg> _queue = new Queue<Msg>();
    public GameObject enemy;
    
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Spawn",20,20);
    }

    // Update is called once per frame
    void Update()
    {
        // 用户未注册且网络连接时发送注册请求
        if (!_playerRegistered && _networkHost.connected)
        {
            Msg msg = new Msg("playerRegister");
            msg.args.Add(ClientSettings.characterName);
            StartCoroutine(_networkHost.Send(msg));
            _playerRegistered = true;
        }
        
        // 接收并处理消息
        _networkHost.Receive();
        while (_networkHost.receivedMessages.Count>0)
        {
            Msg recievedMsg  = _networkHost.receivedMessages.Dequeue();
            _queue.Enqueue(recievedMsg);
        }
        while (_queue.Count>0)
        {
            Msg msg = _queue.Dequeue();
            MethodInfo methodInfo = GetType().GetMethod(msg.method);
            if (methodInfo != null)
            {
                methodInfo.Invoke(this, msg.args.ToArray<object>());
            }
            else
            {
                Debug.Log(msg.method);
            }
        }

    }

    // 设置角色entityID
    public void setEntityID(string eid)
    {
        ClientSettings.entityID = Int32.Parse(eid);
    }
    // 设置角色entityID
    public void setClientID(string cid)
    {
        ClientSettings.clientID = Int32.Parse(cid);
    }

    //刷新敌人
    void Spawn()
    {
        Instantiate (enemy, new Vector3(Random.Range(-50,50),0,Random.Range(-50,50)), Quaternion.Euler(0, 0, 0));
    }
}


