
using System.Collections;
using System.Collections.Generic;
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
        statusText.text = "Starting game...";
        SceneManager.UnloadSceneAsync("StartGameUI");
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
            statusText.text = "Welcome " + task.Result.DisplayName + "!";
            signInButton.SetActive(false);
            startGameButton.SetActive(true);
        }
    }

}
