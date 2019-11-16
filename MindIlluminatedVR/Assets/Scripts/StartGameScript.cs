
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using UnityEngine;
using UnityEngine.UI;

public class StartGameScript : MonoBehaviour, IBackendServiceListener
{
    public GameObject signInButton;
    public GameObject startGameButton;
    
    public Text statusText;
    private string status;

    public string webClientId = "<your-web-client-id>";

    private VRSceneManager sceneManager;

    private int listenerIdx;

    // Defer the configuration creation until Awake so the web Client ID
    // Can be set via the property inspector in the Editor.
    void Awake()
    {
        sceneManager = FindObjectOfType<VRSceneManager>();
        listenerIdx = BackendService.Instance.RegisterListener(this);
    }

    void Start()
    {
        CheckSignedInUser();
    }

    void Update()
    {
        statusText.text = status;
    }

    void OnDestroy()
    {
        BackendService.Instance.UnregisterListener(listenerIdx);
    }

    public void OnSignIn()
    {
        status = "Signing in...";
        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(this.OnAuthenticationFinished);
    }

    public void OnStartGame()
    {
        sceneManager.StartGame();
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
                    status = "Got Error: " + error.Status + " " + error.Message;
                }
                else
                {
                    status = "Got Unexpected Exception:" + task.Exception;
                }
            }
        }
        else if (task.IsCanceled)
        {
            status = "Canceled";
        }
        else
        {
            GoogleSignInHelper.User = task.Result;
            BackendService.Instance.GetAccessToken();
        }
    }

    internal void CheckSignedInUser()
    {
        if (GoogleSignInHelper.User != null)
        {
            status = "Welcome " + GoogleSignInHelper.User.DisplayName + "!";
            //status = "Auth code: " + UserHolder.USER.AuthCode;
            Debug.Log(status);
            signInButton.SetActive(false);
            startGameButton.SetActive(true);
        }
    }

    public void Listen(string message)
    {
        status = message;
        Invoke("CheckSignedInUser", 3);
    }

}
