using UnityEngine.UI;
using UnityEngine;

public class DisplaySampleAge : MonoBehaviour
{
    private ButtonBoxMngr _buttonBoxMngr; 
    private Image[] images;

    private int iMovingAvgArr;
    private int[] movingAvgArr;
    void Start()
    {
        _buttonBoxMngr = ButtonBoxMngr.instance;
        
        images = new Image[10];
        for (int ichild = 0; ichild < 10; ichild++)
        {
            images[ichild] = transform.GetChild(ichild).GetComponent<Image>();
        }

        iMovingAvgArr = 0;
        movingAvgArr = new int[20];
    }

    // Update is called once per frame
    void Update()
    {
        if (!_buttonBoxMngr.isReady) return;
        
        movingAvgArr[iMovingAvgArr++] = (int)_buttonBoxMngr.sampleAge;
        iMovingAvgArr %= 20;

        float avg = 0;
        foreach (int val in movingAvgArr)
        {
            avg += val;
        }

        avg /= 20;
        
        float value = avg > 50 ? 50 : avg;
        int normVal = (int)(value/5f);

        for (int iImg = 0; iImg < images.Length; iImg++)
        {
            if (iImg < normVal)
            {
                Color color;
                if (iImg < 3) color = Color.green;
                else if (iImg < 6) color = Color.yellow;
                else color = Color.red;

                images[iImg].color = color;
            }
            else
            {
                images[iImg].color = Color.white;                
            }
        }
    }
}
