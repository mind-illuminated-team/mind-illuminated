
using Sensors;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Ensures correct app and scene setup.</summary>
public class VRSceneManager : Singleton<VRSceneManager>
{

    private static readonly string START_GAME_SCENE = "StartGameScene";
    private static readonly string GAME_SCENE = "FridayDemoBackupScene";

    public float gameDurationSeconds = 20.0f;

    private float elapsedTime = 0.0f;
    private bool running = false;

    private SensorDataProvider sensorDataProvider;

    private void Awake()
    {
        Input.backButtonLeavesApp = true;

        sensorDataProvider = SensorDataProvider.Instance;

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
        Debug.Log("Starting game");
        sensorDataProvider.ClearData();
        sensorDataProvider.StartCapture();
        StartCoroutine(LoadSceneAsync(GAME_SCENE));
        running = true;
    }

    public void EndGame()
    {
        Debug.Log("Ending game");
        elapsedTime = 0.0f;
        running = false;
        sensorDataProvider.StopCapture();
        StartCoroutine(LoadSceneAsync(START_GAME_SCENE));
        UploadSensorData();
    }

    IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log(sceneName + " loaded");
    }

    private void UploadSensorData()
    {
        var sensorData = sensorDataProvider.GetAllData();
        var sensorDataString = SensorDataConverter.SensorDataListToString(sensorData);
        BackendService.Instance.UploadSensorDataFile(sensorDataString);
    }

}
