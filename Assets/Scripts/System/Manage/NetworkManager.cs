using Cysharp.Threading.Tasks;
using UnityEngine;
using XSystem.Singleton;

public class NetworkManager : UnitySingleton<NetworkManager>
{
    private NetworkWebRequest http;
    private string token;

    protected override void AwakeSingleton()
    {
        base.AwakeSingleton();
        http = new NetworkWebRequest("http://localhost:5041");
    }

    public void SetToken(string token) => this.token = token;

    public UniTask<TRes> SendAsync<TReq, TRes>(IAPIRequest<TReq, TRes> request)
        where TReq : BaseReqData
        where TRes : BaseResData
    {
        //request.ReqData.Token = token;
        //토큰으로 생각했는데... 뭔가 접속에 필요한 값 확인 필요
        return http.PostAsync<TReq, TRes>(request.Endpoint, request.ReqData);
    }
}
