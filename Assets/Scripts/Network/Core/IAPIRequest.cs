public interface IAPIRequest<TReq, TRes> where TReq : BaseReqData where TRes : BaseResData
{
    string Endpoint { get; }

    TReq ReqData { get; set; }
}
