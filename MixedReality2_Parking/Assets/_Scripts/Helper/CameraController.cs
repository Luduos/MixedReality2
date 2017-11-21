using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private KeyCode SwitchCamerasButton = KeyCode.C;

    [SerializeField]
    private Camera PlayerCamera;

    [SerializeField]
    private Camera DebugCamera;
    // Use this for initialization
    void Start () {
        PlayerCamera.enabled = true;
        DebugCamera.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(SwitchCamerasButton))
        {
            PlayerCamera.enabled = !PlayerCamera.enabled;
            DebugCamera.enabled = !DebugCamera.enabled;
        }
	}
}
