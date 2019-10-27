
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Ensures correct app and scene setup.</summary>
public class VRSceneManager : MonoBehaviour
{

    private void Start()
    {
        SceneManager.LoadScene("StartGameUI", LoadSceneMode.Additive);
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        Input.backButtonLeavesApp = true;
    }

    private void Update()
    {
        // Exit when (X) is tapped.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void OnSceneUnloaded(Scene current)
    {
        Debug.Log("Unloaded scene: " + current);
        // TODO load game scene if StartGameUI scene successfully unloaded
    }

}
