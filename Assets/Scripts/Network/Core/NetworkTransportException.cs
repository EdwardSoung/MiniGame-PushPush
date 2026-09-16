using System;

// 서버 응답 자체를 받지 못한 경우(UnityWebRequest 실패, 응답 파싱 실패)에 던지는 예외.
// 서버가 실제로 내려주는 비즈니스 상태값인 E_SessionStatus 와는 별개의 클라이언트 통신 계층 오류.
public class NetworkTransportException : Exception
{
    public NetworkTransportException(string message) : base(message) { }

    public NetworkTransportException(string message, Exception innerException) : base(message, innerException) { }
}
