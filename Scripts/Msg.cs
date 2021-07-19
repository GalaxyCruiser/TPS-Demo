


using System;
using System.Collections.Generic;
using System.Linq;


//消息类
[Serializable]
public class Msg
{
    public string method;       //调用函数名
    public List<string> args;   //参数列表
    
    public Msg()
    {
        args = new List<string>();
    }
    public Msg(string method, params string[] args)
    {
        this.method = method;
        this.args = new List<string>();
        for (int i = 0; i < args.Length; i++)
        {
            this.args.Add(args[i]);
        }
    }
}