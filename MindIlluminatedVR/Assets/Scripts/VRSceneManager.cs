
using Sensors;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Ensures correct app and scene setup.</summary>
public class VRSceneManager : MonoBehaviour
{

    public float gameDurationSeconds = 10.0f;

    private float elapsedTime = 0.0f;
    private bool running = false;

    private SensorDataProvider sensorDataProvider;

    private void Awake()
    {
        Input.backButtonLeavesApp = true;

        sensorDataProvider = GetComponent<SensorDataProvider>();

        StartCoroutine(LoadStartSceneAsync());
    }

    private void Update()
    {
        // Exit when (X) is tapped.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        ApplyTiming();
    }

    private void ApplyTiming()
    {
        if (running)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > gameDurationSeconds)
            {
                EndGame();
            }
        }
    }

    public void StartGame()
    {
        SceneManager.UnloadSceneAsync("StartGameUI");
        sensorDataProvider.ClearData();
        sensorDataProvider.StartCapture();
        running = true;
    }

    public void EndGame()
    {
        elapsedTime = 0.0f;
        running = false;
        sensorDataProvider.StopCapture();
        StartCoroutine(LoadStartSceneAsync());
        UploadSensorData();
    }

    IEnumerator LoadStartSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("StartGameUI", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void UploadSensorData()
    {
        var sensorData = sensorDataProvider.GetAllData();
        var sensorDataString = SensorDataConverter.SensorDataListToString(sensorData);
        BackendService.Instance.UploadSensorDataFile(sensorDataString);
    }

}
