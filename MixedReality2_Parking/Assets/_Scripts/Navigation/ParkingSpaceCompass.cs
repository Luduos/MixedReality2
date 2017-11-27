using UnityEngine;

public class ParkingSpaceCompass : MonoBehaviour {

    [SerializeField]
    private Vector3 OffsetFromCamera = new Vector3(0.0f, 0.0f, 5.0f);

    [SerializeField]
    private GameObject CompassPointerPrefab;

    public Vector3 Target { get; set; }

    private Camera[] AllCameras;
    private GameObject CompassPointer;

	// Use this for initialization
	void Start () {
        AllCameras = Camera.allCameras;
        CompassPointer = Instantiate(CompassPointerPrefab);
        EnableCompassPointer(false);
    }
	
	// Update is called once per frame
	void Update () {
        // TODO : probably improve this, kinda wasteful
		foreach(Camera cam in AllCameras)
        {
            if (cam.isActiveAndEnabled)
            {
                CompassPointer.transform.parent = cam.transform;
                CompassPointer.transform.localPosition = OffsetFromCamera;
                CompassPointer.transform.LookAt(new Vector3(Target.x, cam.transform.position.y - OffsetFromCamera.y, Target.z), Vector3.up);
                break;
            }
        }
	}

    public void EnableCompassPointer(bool enable)
    {
        this.enabled = enable;
        CompassPointer.SetActive(enable);
    }
}
