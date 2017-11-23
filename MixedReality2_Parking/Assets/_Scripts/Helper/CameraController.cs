using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private KeyCode SwitchCamerasButton = KeyCode.C;

    [SerializeField]
    private KeyCode SwitchVRCamera = KeyCode.V;

    [SerializeField]
    private Camera PlayerCamera;

    [SerializeField]
    private Camera DebugCamera;

    [SerializeField]
    private Camera VRCamera;
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
        if (Input.GetKeyDown(SwitchVRCamera))
        {
            EnableNormalCameras(VRCamera.enabled);
            VRCamera.enabled = !VRCamera.enabled;
        }
	}

    private void EnableNormalCameras(bool enable)
    {
        if (!enable)
        {
            PlayerCamera.enabled = false;
            DebugCamera.enabled = false;
        }
        else
        {
            PlayerCamera.enabled = false;
            DebugCamera.enabled = true;
        }
       
    }
}
