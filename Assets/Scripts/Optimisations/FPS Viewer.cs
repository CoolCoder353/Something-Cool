using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSViewer : MonoBehaviour
{
    [Tooltip("The duration over which the FPS is averaged")]
    public float averageDuration = 1.0f;

    [Tooltip("The text component where the FPS is displayed")]
    public TMP_Text fpsText;

    private int frameCount;
    private float timePassed;

    void Update()
    {
        frameCount++;
        timePassed += Time.deltaTime;

        if (timePassed >= averageDuration)
        {
            // Calculate the average FPS over the last few seconds
            float fps = frameCount / timePassed;

            // Display the FPS
            fpsText.text = $"FPS: {fps}";

            // Reset the counters
            frameCount = 0;
            timePassed = 0;
        }
    }
}