using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // Requires .NET 4 in Project Settings
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Debug = UnityEngine.Debug;

// Original code borrowed from: https://github.com/SirWilliamT/ArduinoUnitySerialCommunication

public class SerialComMngr : MonoBehaviour
{

    public static SerialComMngr instance;
    
    private SerialPort _serial;
    private List<string> _ports;
    public bool isConnected => _serial != null && _serial.IsOpen;

    public byte[] lastData { private set; get; }
    public string lastMessage;
    public long lastAge => _stopwatch.ElapsedMilliseconds;
    private Stopwatch _stopwatch;
    
    private Thread samplingThread;

    private void Awake()
    {
        instance = this;
        lastData = null;
        lastMessage = String.Empty;
    }

    void Start()
    {
        GetAvailablePorts();
        ConnectToPort();
        
        _stopwatch = new Stopwatch();

        if (isConnected)
        {
            _stopwatch.Start();
            
            samplingThread = new Thread(ReadFromPort);
            samplingThread.Start();
        }
    }

    void ReadFromPort()
    {
        while(isConnected)
        {
            int bytesToRead = _serial.BytesToRead;
            if (bytesToRead > 0)
            {
                // byte[] buff = new byte[bytesToRead];
                string read = _serial.ReadLine();//(buff, 0, bytesToRead);
                if (read != String.Empty)
                {
                    lastMessage = read;
                    _stopwatch.Restart();
                }
                // if (read > 0)
                // {
                //     lastData = buff;
                // }
            }
        }
    }

    public void GetAvailablePorts()
    {

        _ports = new List<string>();

        // Get port names
        foreach (string port_name in SerialPort.GetPortNames())
        {
            // print(port_name);
#if PLATFORM_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            if (port_name.Substring(8, port_name.Length - 9) == "USB")
            {
                _ports.Add(port_name);
            }
#else
            Debug.LogError("Code for windows not implemented yet");
#endif
        }
    }

    public void ConnectToPort()
    {
        string port = _ports[0];
        Disconnect();

        try
        {
            _serial = new SerialPort(port, 9600)
            {
                Encoding = Encoding.UTF8,
                DtrEnable = true
            };
            _serial.Open();

            print($"Connected to {port}");
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void Disconnect()
    {
        if (_serial != null)
        {
            // close the connection if it is open
            if (isConnected)
            {
                _serial.Close();
            }

            // release any resources being used
            _serial.Dispose();
            _serial = null;

            Debug.Log("Disconnected");
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
