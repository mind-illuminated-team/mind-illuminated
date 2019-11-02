
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameScript : MonoBehaviour
{
    public GameObject signInButton;
    public GameObject startGameButton;

    public Text statusText;

    public string webClientId = "<your-web-client-id>";

    private GoogleSignInConfiguration configuration;

    // Defer the configuration creation until Awake so the web Client ID
    // Can be set via the property inspector in the Editor.
    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestAuthCode = true,
            RequestIdToken = true,
            UseGameSignIn = false
        };
    }

    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        statusText.text = "Signing in";
        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnStartGame()
    {
        statusText.text = "Connecting to server...";

        var operation = BackendService.Instance.GetAccessToken();
        operation.completed += OnBackendConnectionFinished;
    }

    internal void OnBackendConnectionFinished(AsyncOperation action)
    {
        UnityWebRequestAsyncOperation operation = (UnityWebRequestAsyncOperation) action;
        if (operation.webRequest.isHttpError || operation.webRequest.isNetworkError)
        {
            statusText.text = "Connection error: " + operation.webRequest.error;
        }
        else
        {
            statusText.text = "Uploading test data...";
            operation = BackendService.Instance.UploadTestFile();
            operation.completed += OnUploadTestFileFinished;
        }
    }

    internal void OnUploadTestFileFinished(AsyncOperation action)
    {
        UnityWebRequestAsyncOperation operation = (UnityWebRequestAsyncOperation) action;
        if (operation.webRequest.isHttpError || operation.webRequest.isNetworkError)
        {
            statusText.text = "Upload error: " + operation.webRequest.error;
        }
        else
        {
            statusText.text = "Downloading test data...";
            operation = BackendService.Instance.DownloadTestData();
            operation.completed += OnDownloadTestDataFinished;
        }
    }

    internal void OnDownloadTestDataFinished(AsyncOperation action)
    {
        UnityWebRequestAsyncOperation operation = (UnityWebRequestAsyncOperation)action;
        if (operation.webRequest.isHttpError || operation.webRequest.isNetworkError)
        {
            statusText.text = "Download error: " + operation.webRequest.error;
        }
        else
        {
            statusText.text = "Test data: " + Encoding.UTF8.GetString(operation.webRequest.downloadHandler.data);
        }
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException) enumerator.Current;
                    statusText.text = "Got Error: " + error.Status + " " + error.Message;
                }
                else
                {
                    statusText.text = "Got Unexpected Exception:" + task.Exception;
                }
            }
        }
        else if (task.IsCanceled)
        {
            statusText.text = "Canceled";
        }
        else
        {
            UserHolder.USER = task.Result;
            statusText.text = "Welcome " + task.Result.DisplayName + "!";
            signInButton.SetActive(false);
            startGameButton.SetActive(true);
        }
    }

}
