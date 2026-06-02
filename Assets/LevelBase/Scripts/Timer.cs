
using UnityEngine;
using UnityEngine.UI;


public class Timer : MonoBehaviour
{
    [Header("Sprites 0-9")]
    public Sprite[] digitSprites;

    [Header("Display Slots")]
    public Image hundredsOfSeconds;
    public Image tensOfSeconds;
    public Image unitsOfSeconds;
    public Image tenthsOfMillisecond; 
    public Image hundredthsOfMillisecond;

    [HideInInspector]
    public float timer = 0f;
    private bool isRunning = false;


    void Start()
    {
        TurnOff();
    }
    void Update()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;

        if (timer > 999.99f) timer = 999.99f;

        UpdateDisplay(timer);
    }

    public void TurnOn()
    {
        timer = 0f;
        isRunning = true;
        this.enabled = true;
    }
    public void TurnOff()
    {
        isRunning = false;
        this.enabled = false;
    }

    private void UpdateDisplay(float time)
    {
        int totalSeconds = (int)time;
        int milliseconds = (int)((time - totalSeconds) * 100);

        int secHundreds = (totalSeconds / 100) % 10;
        int secTens = (totalSeconds / 10) % 10;
        int secUnits = totalSeconds % 10;

        int msTenths = (milliseconds / 10) % 10;
        int msHundredths = milliseconds % 10;

        SetSprite(hundredsOfSeconds, secHundreds);
        SetSprite(tensOfSeconds, secTens);
        SetSprite(unitsOfSeconds, secUnits);
        SetSprite(tenthsOfMillisecond, msTenths);
        SetSprite(hundredthsOfMillisecond, msHundredths);
    }
    private void SetSprite(Image imageSlot, int digit)
    {
        if (imageSlot != null && digit < digitSprites.Length)
        {
            imageSlot.sprite = digitSprites[digit];
        }
    }
}
