using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class GameLevel : MonoBehaviour
{
    [SerializeField] private int levelNumber;

    [SerializeField] private float zoomedOutOrthographicSize;

    [SerializeField] private Transform landerStartPositionTransform; // Reference to the transform representing the lander's start position
    [SerializeField] private Transform cameraStartPositionTransform; // Reference to the transform representing the camera's start position

    // Method to get the level number
    public int GetLevelNumber()
    {
        return levelNumber;
    }

    // Method to get the lander's start position
    public Vector3 GetLanderStartPosition()
    {
        return landerStartPositionTransform.position;
    }

    // Method to get the camera's start target transform
    public Transform GetCameraStartTargetTransform()
    {
        return cameraStartPositionTransform.transform;
    }

    public float GetZoomedOutOrthographicSize()
    {
        return zoomedOutOrthographicSize;
    }
}
