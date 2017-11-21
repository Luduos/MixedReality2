using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class DisplayController : MonoBehaviour {


    [SerializeField]
    private Text NotificationDisplay;

    [SerializeField]
    private float DisplayTime = 3.0f;
	
	public void OnNoNextTargetSelected()
    {
        StartCoroutine(DisplayNotificationText("Please select a new target by turning towards it", DisplayTime));
    }

    private IEnumerator DisplayNotificationText(string message, float displayTime)
    {
        float deltaTime = 0.0f;
        NotificationDisplay.text = message;
        while(deltaTime < displayTime)
        {
            deltaTime += Time.deltaTime;
            yield return null;
        }
        NotificationDisplay.text = "";
        yield return null;
    }
}
