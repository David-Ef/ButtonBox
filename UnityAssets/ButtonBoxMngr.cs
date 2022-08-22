using System;
using UnityEngine;

[RequireComponent(typeof(SerialComMngr))]
public class ButtonBoxMngr : MonoBehaviour
{
    
    public static ButtonBoxMngr instance;
    
    private SerialComMngr _serialComMngr;
    public bool isReady => SerialComMngr.isConnected;

    public bool isLeftButtonPressed { private set; get; }
    public bool isRightButtonPressed { private set; get; }

    // If your code relies on "thisFrame" properties it should check them in lateUpdate calls
    public bool wasLeftButtonPressedThisFrame { private set; get; }
    public bool wasRightButtonPressedThisFrame { private set; get; }

    public bool wasLeftButtonReleasedThisFrame { private set; get; }
    public bool wasRightButtonReleasedThisFrame { private set; get; }
    
    public delegate void ButtonChangeNotice(int button, bool state);
    public ButtonChangeNotice onButtonChange;

    public long sampleAge => _serialComMngr.lastAge;
    
    public float sliderValue { private set; get; }
    

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        _serialComMngr = SerialComMngr.instance;
    }

    void Update()
    {
        wasLeftButtonPressedThisFrame = false;
        wasRightButtonPressedThisFrame = false;
        wasLeftButtonReleasedThisFrame = false;
        wasRightButtonReleasedThisFrame = false;

        if (_serialComMngr.lastMessage == String.Empty) return;
        
        // string rawData = Encoding.UTF8.GetString(_serialComMngr.lastData);
        // if (rawData == String.Empty) return;
        
        // print(rawData);
        // print($"{rawData.Split('\n').Length} lines");
        
        // string[] data = rawData.Split('\n').First().Split('-');
        string[] data = _serialComMngr.lastMessage.Split('-');

        bool newLeftButtonState = data[1] == "0";
        bool newRightButtonState = data[0] == "0";

        if (newLeftButtonState != isLeftButtonPressed)
        {
            if (newLeftButtonState)
            {
                wasLeftButtonPressedThisFrame = true;
            }
            else
            {
                wasLeftButtonReleasedThisFrame = true;
            }

            onButtonChange?.Invoke(0, newLeftButtonState);
        }

        if (newRightButtonState != isRightButtonPressed)
        {
            if (newRightButtonState)
            {
                wasRightButtonPressedThisFrame = true;
            }
            else
            {
                wasRightButtonReleasedThisFrame = true;
            }

            onButtonChange?.Invoke(1, newRightButtonState);
        }

        isLeftButtonPressed = newLeftButtonState;
        isRightButtonPressed = newRightButtonState;
        
        sliderValue = 1f-float.Parse(data[2].Trim());

//        print($"l {isLeftButtonPressed}, r {isRightButtonPressed}, {sliderValue}");
    }
}
