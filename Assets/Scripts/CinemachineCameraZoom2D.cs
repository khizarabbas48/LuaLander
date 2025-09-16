using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraZoom2D : MonoBehaviour
{
    private const float NORMAL_ORTHOGRAPHIC_SIZE = 10f;
    public static CinemachineCameraZoom2D Instance { get; private set; }
    [SerializeField] private CinemachineCamera cinemachineCamera;
    private float targetOrthographicSize = 10f;

    private void Awake()
    {
        Instance = this; // Singleton pattern to ensure only one instance of CinemachineCameraZoom2D exists
    }

    private void Update()
    {
        float zoomSpeed = 2f; // Speed of zooming
        cinemachineCamera.Lens.OrthographicSize =
        Mathf.Lerp(cinemachineCamera.Lens.OrthographicSize, targetOrthographicSize, Time.deltaTime * zoomSpeed);
    }

    public void SetTargetOrthographicSize(float targetOrthographicSize)
    {
        this.targetOrthographicSize = targetOrthographicSize;
    }

    //public void SetToNormalOrthographicSize()
    public void SetToNormalOrthographicSize()
    {
        targetOrthographicSize = NORMAL_ORTHOGRAPHIC_SIZE;
    }


}
