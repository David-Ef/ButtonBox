using System;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCanvas : MonoBehaviour
{
    private ButtonBoxMngr _buttonBoxMngr; 
    
    public Toggle leftBtn;
    public Toggle rightBtn;
    public Slider slider;
    public Text text;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {

        _buttonBoxMngr = ButtonBoxMngr.instance;
        
        // Example of using the callback to trigger some behaviour
        _buttonBoxMngr.onButtonChange = (button, state) => { print($"Btn {button} > {state}"); };
    }

    void LateUpdate()
    {
        if (_buttonBoxMngr.isReady)
        {
            text.text = $"{_buttonBoxMngr.sampleAge}: {_buttonBoxMngr.isLeftButtonPressed},  {_buttonBoxMngr.isRightButtonPressed},  {_buttonBoxMngr.sliderValue}";

            leftBtn.isOn = _buttonBoxMngr.isLeftButtonPressed;
            rightBtn.isOn = _buttonBoxMngr.isRightButtonPressed;
            slider.value = _buttonBoxMngr.sliderValue;
        }
    }
}
