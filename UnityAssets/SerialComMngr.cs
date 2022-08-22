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

    public bool verbose = true; 

    public static SerialComMngr instance;
    
    private SerialPort _serial;
    public static string serialName => isConnected ? instance._serial.PortName : String.Empty;

    private List<string> _ports;
    private int _nport = 0;
    public static bool isConnected => instance._serial != null && instance._serial.IsOpen;

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
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    private void Update()
    {
        
        // Continuously check when was last data received
        //    Try to reconnect if last sample is too old
        
        if (lastAge > 2000)
        {
            if (verbose) Debug.LogWarning($"[ButtonBox] Port {_ports[_nport]} has not sent any data in the last 1000ms. Switching to next port in list.");
            
            _nport = ++_nport % _ports.Count;

            Disconnect();
        }

        if (!isConnected)
        {
            GetAvailablePorts();
            
            // Try last port first
            ConnectToPort(_ports.Count-_nport-1);

            if (!isConnected)
            {
                if (verbose) Debug.LogWarning($"[ButtonBox] Could not connect to Button box on serial port {_ports[_nport]}.");
            } else
            {
                _stopwatch.Restart();
                samplingThread = new Thread(ReadFromPort);
                samplingThread.Start();
            }
        }
    }

    void ReadFromPort()
    {
        while(isConnected)
        {
            int bytesToRead = _serial.BytesToRead;
            if (bytesToRead > 0)
            {
                string read = _serial.ReadLine();
                if (read != String.Empty)
                {
                    lastMessage = read;
                    _stopwatch.Restart();
                }
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
#else
            if (port_name.Substring(0, 3) == "COM")
#endif
            {
                _ports.Add(port_name);
            }
        }
    }

    public void ConnectToPort(int nport)
    {
        string port = _ports[nport];
        
        if (verbose) print($"[ButtonBox] Trying to connect to serial port {port} ({nport+1} / {_ports.Count})");
        
        Disconnect();

        try
        {
            _serial = new SerialPort(port, 9600)
            {
                Encoding = Encoding.UTF8,
                DtrEnable = true
            };
            _serial.Open();

            if (verbose) print($"[ButtonBox] Connected to serial port {port}.");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[ButtonBox] {e.Message}");
        }
    }

    public void Disconnect()
    {
        if (_serial != null)
        {
            string serialName = _serial.PortName;
            // close the connection if it is open
            if (isConnected)
            {
                _serial.Close();
            }

            // release any resources used
            _serial.Dispose();
            _serial = null;

            if (verbose) Debug.Log($"[ButtonBox] Disconnected from serial port {serialName}");
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
