public interface IEvent
{
    bool IsCompleted { get; }

    void StartEvent();

    void ResetEvent();
}