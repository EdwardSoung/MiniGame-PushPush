using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public static class WebRequestResultHandler
{
    //Web Request 실패시 처리
    public static bool Handle(UnityWebRequest.Result result, string endpoint)
    {
        switch (result)
        {
            case UnityWebRequest.Result.Success:
                return true;

            case UnityWebRequest.Result.ConnectionError:
                // 서버에 도달하지 못함 (오프라인, DNS 실패, 타임아웃 등)
                Debug.LogWarning($"Web reqeust connection error on {endpoint}");
                return false;

            case UnityWebRequest.Result.ProtocolError:
                Debug.LogWarning($"Web request protocol error on {endpoint}");
                return false;

            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogWarning($"Web request data processing error on {endpoint}");
                return false;

            default:
                return false;
        }
    }

    // 데이터 파싱에 실패한 경우
    public static NetworkTransportException HandleParseError(JsonException ex, string endpoint)
    {
        Debug.LogWarning($"[WebRequest] Failed to parse response on {endpoint}: {ex.Message}");
        return new NetworkTransportException(ex.Message, ex);
    }
}
