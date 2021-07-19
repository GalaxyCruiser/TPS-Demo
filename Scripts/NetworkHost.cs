using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class NetworkHost
{
    private TcpClient _client;
    private IPAddress _serverAddress;
    private int _serverPort;
    private NetworkStream _networkStream;
    private StreamWriter _streamWriter;
    private StreamReader _streamReader;
    public bool connected;
    private byte[] _receiveBuffer;
    private byte[] _dataBuffer;
    private int _dataBufferLength;
    public Queue<Msg> receivedMessages;

    private static NetworkHost _networkHostInstance;

    public static NetworkHost GetInstance()
    {
        if (_networkHostInstance == null)
        {
            _networkHostInstance = new NetworkHost();
        }

        return _networkHostInstance;
    }

    private NetworkHost()
    {
        _dataBufferLength = 0;
        _receiveBuffer = new byte[1024 * 8];
        _dataBuffer = new byte[1024 * 16];
        receivedMessages = new Queue<Msg>();

        _serverAddress = IPAddress.Parse("127.0.0.1");
        _serverPort = 2000;
        _client = new TcpClient();
        Thread connectTread = new Thread((ConnectServer));
        connectTread.IsBackground = true;
        connectTread.Start();
    }

    //连接服务器
    private void ConnectServer()
    {
        try
        {
            _client.Connect(_serverAddress, _serverPort);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        connected = true;
    }

    //发送消息
    public IEnumerator Send(Msg msg)
    {
        String json = JsonUtility.ToJson(msg);
        byte[] data = Encoding.ASCII.GetBytes(json);
        int len = 4 + data.Length;
        byte[] lenByte = BitConverter.GetBytes(len);    //添加消息长度
        byte[] sendData = new byte[len];        ////添加消息体
        lenByte.CopyTo(sendData,0);
        data.CopyTo(sendData,lenByte.Length);
        bool sendSuccess = false;
        if (connected)
        {
            _networkStream = _client.GetStream();
            if (_networkStream.CanWrite)
            {
                try
                {
                    _networkStream.Write(sendData,0,sendData.Length);
                    sendSuccess = true;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    sendSuccess = false;
                    connected = false;
                }
            }
        }
        yield return sendSuccess;
    }

    //接收消息
    public void Receive()
    {
        if (connected)
        {
            try
            {
                _networkStream = _client.GetStream();
            }
            catch (InvalidOperationException e)
            {
                Debug.Log(e);
                connected = false;
            }
            finally
            {
                int length;
                while (_networkStream.DataAvailable)
                {
                    length = _networkStream.Read(_receiveBuffer, 0, _receiveBuffer.Length);
                    //将receiveBuffer内数据放入dataBuffer
                    Array.Copy(_receiveBuffer,0,_dataBuffer,_dataBufferLength,length);
                    _dataBufferLength += length;
                    int begin = 0;
                    while (begin < _dataBufferLength)
                    {
                        //获取消息体长度
                        int msgLen = BitConverter.ToInt32(_dataBuffer, begin);
                        if (begin + msgLen > _dataBufferLength)
                        {
                            break;;
                        }
                        begin += 4;
                        int dataLen = msgLen - 4;
                        Msg msg = JsonUtility.FromJson<Msg>(Encoding.ASCII.GetString(_dataBuffer, begin, dataLen));
                        receivedMessages.Enqueue(msg);
                        begin += dataLen;
                    }

                    if (begin < _dataBufferLength)
                    {
                        Array.Copy(_dataBuffer,begin,_dataBuffer,0,_dataBufferLength-begin);
                    }

                    _dataBufferLength -= begin;
                }
            }
        }
    }
}
