using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, Timer.TimerListener {

    public Text secondsDisplay;
    public Text millisDisplay;

    public float maxTimeFontSizeMultiplier = 1.25f;
    
    public int timeBlockLength = 10;
    public int shortTimeBlockLength = 1;
    public int shortTimeBlockThreshold = 10;

    public AnimationCurve fontSizeCurve;

    private float globalTextScale;

    private Timer timer;
    private int secondsFontSize, millisFontSize;

	// Use this for initialization
	void Start () 
    {
        float horizRatio = Screen.width / 540f;
        float vertRatio = Screen.height / 960f;
        globalTextScale = Mathf.Sqrt(horizRatio * horizRatio + vertRatio * vertRatio);

        secondsFontSize = secondsDisplay.fontSize;
        millisFontSize = millisDisplay.fontSize;

        timer = gameObject.GetComponent<Timer>();
        timer.RegisterListener(this);

        timer.StartTimer(60);
	}

    void Timer.TimerListener.TimerTick(float remainingTime)
    {
        float time = Mathf.Max(remainingTime, 0f);
        secondsDisplay.text = ((int)time).ToString("D2");

        int millis = (int)((time % (int)time) * 100);
        millisDisplay.text = (Mathf.Max(millis, 0).ToString("D2"));

        float timeThisBlock = timeBlockLength - (time % timeBlockLength);
        float fontSizeMultiplier = Mathf.Lerp(1f, maxTimeFontSizeMultiplier, fontSizeCurve.Evaluate(timeThisBlock));

        Logger.SetValue("timeThisBlock", timeThisBlock.ToString());
        Logger.SetValue("fontSizeMultiplier", fontSizeMultiplier.ToString());

        secondsDisplay.fontSize = (int) (secondsFontSize * fontSizeMultiplier * globalTextScale);
        millisDisplay.fontSize = (int)(millisFontSize * fontSizeMultiplier * globalTextScale);

        if (time < shortTimeBlockThreshold + 1)
        {
            timeBlockLength = shortTimeBlockLength;
        }
    }
}
