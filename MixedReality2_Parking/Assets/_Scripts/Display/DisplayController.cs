using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class DisplayController : MonoBehaviour {
    [SerializeField]
    ParkingSpaceCompass Compass;

    [SerializeField]
    private Text NotificationDisplay;

    [SerializeField]
    private float DisplayTime = 3.0f;

    [SerializeField]
    private Text[] ParkingSpaceDisplays;

    public void ShowCompassTo(Vector3 target)
    {
        Compass.Target = target;
        Compass.EnableCompassPointer(true);
    }

    public void HideCompass()
    {
        Compass.EnableCompassPointer(false);
    }

    public void ShowSelectableParkingSpaces(List<OSMWayFindingInfo> parkingSpacesSorted)
    {
        for(int i = 0; i < ParkingSpaceDisplays.Length; ++i)
        {
            Text display = ParkingSpaceDisplays[i];
            if(i < parkingSpacesSorted.Count)
            {
                string parkingSpaceName = parkingSpacesSorted[i].Way.Name;
                if (parkingSpaceName.Equals(""))
                {
                    parkingSpaceName = "Name Unknown";
                }
                display.text = "Option " + (i + 1) + ": " + parkingSpaceName + 
                    "\nDistance: " + Mathf.Sqrt( parkingSpacesSorted[i].DistanceSqr) + " m";
            }
        }
    }

    public int GetValidParkingSpaceOptionCount()
    {
        return ParkingSpaceDisplays.Length;
    }

    public void ShowStillTravelingNotification()
    {
        StartCoroutine(DisplayNotificationText("Still traveling, please wait until next spot.", DisplayTime));
    }

    public void ShowInvalidOptionSelectionNotify()
    {
        StartCoroutine(DisplayNotificationText("Invalid Option, please try again.", DisplayTime));
    }

    public void HideSelectableParkingSpaces()
    {
        foreach(Text display in ParkingSpaceDisplays)
        {
            display.text = "";
        }
    }

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
