using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LandedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleTextMesh;
    [SerializeField] private TextMeshProUGUI statsTextMesh;
    [SerializeField] private TextMeshProUGUI nextButtonTextMesh;
    [SerializeField] private Button nextButton;

    private Action nextButtonClickAction; // Action to be invoked on button click

    private void Awake()
    {
        nextButton.onClick.AddListener(() =>
        {
            nextButtonClickAction();
        });
    }

    public void Start()
    {
        Lander.Instance.onLanded += Lander_onLanded;

        Hide();
    }

    private void Lander_onLanded(object sender, Lander.OnLandedEventArgs e)
    {
        if (e.landingType == Lander.LandingType.Success)
        {
            titleTextMesh.text = "Landed Successfully!";
            nextButtonTextMesh.text = "CONTINUE";
            nextButtonClickAction = GameManager.Instance.GotoNextLevel; // Go to the next level on success
        }
        else
        {
            titleTextMesh.text = "CRASHED!";
            nextButtonTextMesh.text = "RETRY";
            nextButtonClickAction = GameManager.Instance.RetryLevel; // Retry the level on crash
        }

        statsTextMesh.text =
            Mathf.Round(e.landingSpeed * 2f) + "\n" +
            Mathf.Round(e.dotVector * 100f) + "\n" +
            " x " + e.scoreMultiplier + "\n" +
            e.score;

        Show();
    }

    // Show and hide the landed UI
    private void Show()
    {
        gameObject.SetActive(true);
        
        nextButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
