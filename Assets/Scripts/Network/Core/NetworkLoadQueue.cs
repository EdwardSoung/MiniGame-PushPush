using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class NetworkLoadQueue
{
    private readonly List<Func<UniTask>> pending = new();

    public void Add<TReq, TRes>(IAPIRequest<TReq, TRes> request, Action<TRes> onSuccess, Action<string> onError = null, Action<string> onNetworkError = null)
        where TReq : BaseReqData
        where TRes : BaseResData
    {
        pending.Add(async () =>
        {
            TRes res;
            try
            {
                res = await NetworkManager.Instance.SendAsync(request);
            }
            catch (NetworkTransportException ex)
            {
                // 서버 응답 자체를 받지 못한 통신 계층 실패 - E_SessionStatus 와는 별개로 처리
                onNetworkError?.Invoke(ex.Message);
                return;
            }

            //TODO: 상태값 Enum으로 빼고 에러 핸들러도 필요할듯
            if (res.Status == 0)
            {
                onSuccess?.Invoke(res);
            }
            else
            {
                onError?.Invoke(res.message);
            }            
        });
    }

    public async UniTask ExecuteAll(Action<float> onProgress = null)
    {
        int total = pending.Count;
        for (int i = 0; i < total; i++)
        {
            await pending[i]();
            onProgress?.Invoke((float)(i + 1) / total);
        }
        pending.Clear();
    }
}
