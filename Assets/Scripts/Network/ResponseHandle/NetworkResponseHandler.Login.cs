using UnityEngine;

public partial class NetworkResponseHandler
{
    public void OnLogin(ResLogin resData)
    {
        NetworkManager.Instance.SetToken(resData.userToken);

        //로그인 완료...
    }
}
