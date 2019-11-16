using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public sealed class BackendService
{

    public static BackendService Instance { get; } = new BackendService();

    private static readonly string BackendUrl = "https://mind-illuminated-backend.herokuapp.com";

    private static readonly string AccessHeaderName = "X-Access-Token";

    private static readonly string AuthHeaderName = "X-Auth-Code";

    public string AccessToken { get; set; }

    public string TestData { get; set; }

    public string GetFilesData { get; set; }

    static BackendService() { }

    private BackendService() { }

    private readonly List<IBackendServiceListener> listeners = new List<IBackendServiceListener>();

    public int RegisterListener(IBackendServiceListener listener)
    {
        listeners.Add(listener);
        return listeners.Count - 1;
    }

    public void UnregisterListener(int listenerIdx)
    {
        listeners.RemoveAt(listenerIdx);
    }

    public void GetAccessToken()
    {
        TriggerListeners("Retrieving access token...");
        UnityWebRequest request = UnityWebRequest.Get(BackendUrl + "/security/access-token");
        request.SetRequestHeader(AuthHeaderName, GoogleSignInHelper.User.AuthCode);
        var operation = request.SendWebRequest();

        operation.completed += (action) => {
            HandleResponse(request, "Access token received");
        };
    }

    public void DownloadTestData()
    {
        DownloadFile("test-data", "test-upload.txt");
    }

    public void DownloadFile(string folder, string name)
    {
        TriggerListeners("Downloading file...");
        UnityWebRequest request = UnityWebRequest.Get(BackendUrl + "/files/download?folder=" + folder + "&name=" + name);
        request.SetRequestHeader(AccessHeaderName, AccessToken);
        var operation = request.SendWebRequest();

        operation.completed += (action) => {
            HandleResponse(request, "File downloaded: ");
            TestData = Encoding.UTF8.GetString(request.downloadHandler.data);
        };
    }

    public void GetFiles()
    {
        TriggerListeners("Listing files...");
        UnityWebRequest request = UnityWebRequest.Get(BackendUrl + "/files/list");
        request.SetRequestHeader(AccessHeaderName, AccessToken);
        var operation = request.SendWebRequest();

        operation.completed += (action) => {
            HandleResponse(request, "File list received: ");
            GetFilesData = Encoding.UTF8.GetString(request.downloadHandler.data);
        };
    }

    public void UploadSensorDataFile(string sensorData)
    {
        var data = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("folder",
                GoogleSignInHelper.User.UserId),
            new MultipartFormFileSection("file",
                Encoding.ASCII.GetBytes(sensorData),
                GoogleSignInHelper.User.UserId + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmm")  + ".txt",
                "text/plain")
        };

        UploadFile(data);
    }

    public void UploadTestFile()
    {
        var data = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("folder", "test-data"),
            new MultipartFormFileSection("file", Encoding.ASCII.GetBytes("data"), "test-upload.txt", "text/plain")
        };

        UploadFile(data);
    }

    public void UploadFile(List<IMultipartFormSection> data)
    {
        TriggerListeners("Uploading file...");
        UnityWebRequest request = UnityWebRequest.Post(BackendUrl + "/files/upload", data);
        request.SetRequestHeader(AccessHeaderName, AccessToken);
        var operation = request.SendWebRequest();

        operation.completed += (action) => {
            HandleResponse(request, "File upload completed!");
        };
    }

    private void HandleResponse(UnityWebRequest request, string successDebugLogMessage)
    {
        string message;
        if (request.isHttpError || request.isNetworkError)
        {
            message = "Communication error: " + request.error + " - " + request.responseCode;
        }
        else
        {
            message = successDebugLogMessage + request.downloadHandler.text;
            AccessToken = request.GetResponseHeader(AccessHeaderName);
        }
        Debug.Log(message);
        TriggerListeners(message);
    }

    internal void TriggerListeners(string message)
    {
        listeners.ForEach(l => l.Listen(message));
    }

    internal void CheckAccessToken()
    {
        if (AccessToken == null)
        {
            GetAccessToken();
        }
    }
}
