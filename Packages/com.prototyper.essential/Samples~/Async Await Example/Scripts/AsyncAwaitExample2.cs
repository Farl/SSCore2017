using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SS;
using System.Threading.Tasks;

public class AsyncAwaitExample2 : MonoBehaviour
{
    // Get data from API
    // https://pop-proj.ndc.gov.tw/api/v3/page#/%E4%B8%BB%E8%A6%81%E5%B9%B4%E9%BD%A1%E5%88%A5%E4%BA%BA%E5%8F%A3%E6%95%B8/get_Custom_GetOpenDataExport_ashx_464_ApiClick_A874D2D771BD2D9A
    // like this: https://pop-proj.ndc.gov.tw/Common/Custom/Custom_GetOpenDataExport.ashx/464/ApiClick/A874D2D771BD2D9A?sYear=2000&eYear=2024
    [SerializeField] private Button callAPIAsyncButton;
    [SerializeField] private Button callAPICoroutineButton;
    [SerializeField] private string apiURL = "https://pop-proj.ndc.gov.tw/Common/Custom";
    [SerializeField] private string api = "/Custom_GetOpenDataExport.ashx/464/ApiClick/A874D2D771BD2D9A";
    [SerializeField] private string parameters = "?sYear=2000&eYear=2024";

    private void Start()
    {
        callAPIAsyncButton.onClick.AddListener(HandleCallAPIClick);
        callAPICoroutineButton.onClick.AddListener(HandleCallAPICoroutineClick);
    }

    private void HandleCallAPIClick()
    {
        ProcessAPICall().Forget();
    }

    private void HandleCallAPICoroutineClick()
    {
        StartCoroutine(ProcessAPICallCoroutine());
    }

    private IEnumerator ProcessAPICallCoroutine()
    {
        // Call async function
        var task = ProcessAPICall();
        yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);
        if (task.IsFaulted)
        {
            UnityEngine.Debug.LogError(task.Exception);
        }
        else if (task.IsCompleted)
        {
            UnityEngine.Debug.Log("API call completed");
        }
    }


    private UnityWebRequest CreateRequest(string url, string method = "GET", string body = null)
    {
        var request = new UnityWebRequest($"{url}", method);
        UnityEngine.Debug.Log(url);
        request.downloadHandler = new DownloadHandlerBuffer();
        // https://stackoverflow.com/questions/36364459/unitywebrequest-and-or-httpwebrequest-give-403-on-android-with-put
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("User-Agent", "DefaultBrowser");
        request.SetRequestHeader("Cookie", string.Format("DummyCookie"));

        if (!string.IsNullOrEmpty(body))
        {
            var bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            UnityEngine.Debug.Log(body);
        }

        return request;
    }

    private async Task ProcessAPICall()
    {
        // Start the stopwatch
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Call the API
        using (var webRequest = CreateRequest($"{apiURL}{api}{parameters}"))
        {
            await webRequest.SendWebRequest();

            if (webRequest.error != null)
            {
                UnityEngine.Debug.LogError(webRequest.error);
            }
            {
                UnityEngine.Debug.Log(webRequest.downloadHandler.text);
            }
        }

        // Stop the stopwatch
        stopwatch.Stop();
        UnityEngine.Debug.Log($"API call took {stopwatch.ElapsedMilliseconds}ms");
    }
}
