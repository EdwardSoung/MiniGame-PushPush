using System;
using UnityEngine;

public class ResourceCriticalException : UnityException
{
    public ResourceCriticalException(string message) : base(message)
    { }

    public ResourceCriticalException(string message, Exception innerException) : base(message, innerException)
    { }
}
