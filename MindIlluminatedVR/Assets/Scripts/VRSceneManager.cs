
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/// <summary>Ensures correct app and scene setup.</summary>
public class VRSceneManager : MonoBehaviour
{

    private void Start()
    {
        SceneManager.LoadSceneAsync("StartGameUI", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("UDPData", LoadSceneMode.Additive);
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

        if (current.name == "StartGameUI")
        {
            UploadTestFile();
        }
    }

    void UploadTestFile()
    {
        var data = new List<IMultipartFormSection>
        {
            new MultipartFormFileSection("file", Encoding.ASCII.GetBytes("data"), "test-data.txt", "text/plain")
        };

        UnityWebRequest request = UnityWebRequest.Post("http://mind-illuminated-backend.herokuapp.com/upload-file", data);
        request.SetRequestHeader("X-Mind-Illuminated-Token", "");
        var operation = request.SendWebRequest();

        //response
        operation.completed += (action) => {
            if (!request.isHttpError && !request.isNetworkError)
            {
                Debug.Log(request.downloadHandler.text);
            }
        };
    }

}
