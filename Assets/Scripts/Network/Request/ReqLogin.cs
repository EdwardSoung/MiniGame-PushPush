using UnityEngine;

public class ReqLogin : BaseReqData
{
    public string userId;
}


public class LoginRequest : IAPIRequest<ReqLogin, ResLogin>
{
    public string Endpoint => "/api/login";

    public ReqLogin ReqData { get; set; }
}