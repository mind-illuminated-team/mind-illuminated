using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public sealed class BackendService
{

    public static BackendService Instance { get; } = new BackendService();

    private static readonly string BackendUrl = "http://mind-illuminated-backend.herokuapp.com";

    private static readonly string AccessHeaderName = "X-Access-Token";

    private static readonly string AuthHeaderName = "X-Auth-Code";

    public string AccessToken { get; set; }

    public string TestData { get; set; }

    public string GetFilesData { get; set; }

    static BackendService() { }
    private BackendService() { }

    public UnityWebRequestAsyncOperation GetAccessToken()
    {
        UnityWebRequest request = UnityWebRequest.Get(BackendUrl + "/security/access-token");
        request.SetRequestHeader(AuthHeaderName, UserHolder.USER.AuthCode);
        var operation = request.SendWebRequest();

        operation.completed += (action) => {
            HandleResponse(request, "Access token received");
        };
        return operation;
    }

    public UnityWebRequestAsyncOperation DownloadTestData()
    {
        return DownloadFile("test-data", "test-upload.txt");
    }

    public UnityWebRequestAsyncOperation DownloadFile(string folder, string name)
    {
        UnityWebRequest request = UnityWebRequest.Get(BackendUrl + "/files/download?folder=" + folder + "&name=" + name);
        request.SetRequestHeader(AccessHeaderName, AccessToken);
        var operation = request.SendWebRequest();

        operation.completed += (action) => {
            HandleResponse(request, "File downloaded: ");
            TestData = Encoding.UTF8.GetString(request.downloadHandler.data);
        };
        return operation;
    }

    public UnityWebRequestAsyncOperation GetFiles()
    {
        UnityWebRequest request = UnityWebRequest.Get(BackendUrl + "/files/list");
        request.SetRequestHeader(AccessHeaderName, AccessToken);
        var operation = request.SendWebRequest();

        operation.completed += (action) => {
            HandleResponse(request, "File list received: ");
            GetFilesData = Encoding.UTF8.GetString(request.downloadHandler.data);
        };
        return operation;
    }

    public UnityWebRequestAsyncOperation UploadTestFile()
    {
        var data = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("folder", "test-data"),
            new MultipartFormFileSection("file", Encoding.ASCII.GetBytes("data"), "test-upload.txt", "text/plain")
        };

        return UploadFile(data);
    }

    public UnityWebRequestAsyncOperation UploadFile(List<IMultipartFormSection> data)
    {
        UnityWebRequest request = UnityWebRequest.Post(BackendUrl + "/files/upload", data);
        request.SetRequestHeader(AccessHeaderName, AccessToken);
        var operation = request.SendWebRequest();

        operation.completed += (action) => {
            HandleResponse(request, "File upload completed");
        };
        return operation;
    }

    private void HandleResponse(UnityWebRequest request, string successDebugLogMessage)
    {
        if (request.isHttpError || request.isNetworkError)
        {
            Debug.Log(request.error);
            if (request.responseCode == 401)
            {
                GetAccessToken();
            }
        }
        else
        {
            AccessToken = request.GetResponseHeader(AccessHeaderName);
            Debug.Log(successDebugLogMessage + request.downloadHandler.text);
        }
    }
}
