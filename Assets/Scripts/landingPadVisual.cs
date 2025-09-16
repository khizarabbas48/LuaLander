using TMPro;
using UnityEngine;

public class landingPadVisual : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreMultiplierTextMesh;

    // Visuals for text mesh pro like x15 etc.
    private void Awake()
    {
        LandingPad landingPad = GetComponent<LandingPad>(); // Access the GetscoreMultiplier method from LandingPad
        scoreMultiplierTextMesh.text = "x" + landingPad.GetscoreMultiplier();
    }
}
