using System.Collections.Generic;
using XUnityLibrary.Converter;
using XUnityLibrary.EventBus;

public class EventLoadAssets : EventBase
{

}

public class EventLoadTable<T> : EventBase
{
    public List<T> DataList { get; private set; }

    public EventLoadTable(Wrapper<T> wrapper)
    {
        DataList = wrapper.list;
    }
}

public class EventUpdateTableLadingProgress : EventBase
{
    public float Progress { get; private set; }

    public EventUpdateTableLadingProgress(float progress)
    {
        Progress = progress;
    }
}
