using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkWebRequest
{
    private readonly string baseUrl;

    public NetworkWebRequest(string baseUrl)
    {
        this.baseUrl = baseUrl;
    }

    public async UniTask<TRes> PostAsync<TReq, TRes>(string endpoint, TReq reqData)
        where TReq : BaseReqData
        where TRes : BaseResData
    {
        var json = JsonConvert.SerializeObject(reqData);
        var bytes = Encoding.UTF8.GetBytes(json);

        using var req = new UnityWebRequest(baseUrl + endpoint, "POST");
        req.uploadHandler = new UploadHandlerRaw(bytes);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.disposeCertificateHandlerOnDispose = true;
        req.SetRequestHeader("Content-Type", "application/json");

        await req.SendWebRequest();

        if (!WebRequestResultHandler.Handle(req.result, endpoint))
        {
            throw new NetworkTransportException(req.error);
        }

        try
        {
            return JsonConvert.DeserializeObject<TRes>(req.downloadHandler.text);
        }
        catch (JsonException ex)
        {
            throw WebRequestResultHandler.HandleParseError(ex, endpoint);
        }
    }
}
