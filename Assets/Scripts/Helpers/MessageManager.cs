using UniRx;

namespace Helpers
{
    public static class MessageManager
    {
        public static void SendEvent<T>(T ev) where T : GameEvent
        {
            MessageBroker.Default.Publish(ev);
        }

        public static IObservable<T> ReceiveEvent<T>() where T : GameEvent
        {
            return MessageBroker.Default.Receive<T>();
        }
    }

    public class GameEvent { }
}
