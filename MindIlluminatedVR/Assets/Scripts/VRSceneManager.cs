
using UnityEngine;

/// <summary>Ensures correct app and scene setup.</summary>
public class VRSceneManager : MonoBehaviour
{
    private void Start()
    {
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
}
