namespace Pneumatic.Clean.Domain.Events;

public delegate Task AsyncEventHandler<in TEvent>(object sender, TEvent @event)
    where TEvent : EventArgs;

public static class AsyncEventHandlerExtensions
{
    public static async Task InvokeAsync<TEvent>(this AsyncEventHandler<TEvent>? eventHandler,
        object sender, TEvent @event)
        where TEvent : EventArgs
    {
        if (eventHandler != null)
        {
            foreach (var invocation in eventHandler.GetInvocationList())
            {
                if (invocation is not AsyncEventHandler<TEvent> handlerInstance) return;
                await handlerInstance(sender, @event).ConfigureAwait(false);
            }
        }
    }

    public static async Task InvokeConcurrent<TEvent>(this AsyncEventHandler<TEvent>? eventHandler,
        object sender, TEvent @event)
        where TEvent : EventArgs
    {
        if (eventHandler != null)
        {
            await Task.WhenAll(eventHandler.GetInvocationList().Select(async invocation =>
            {
                if (invocation is not AsyncEventHandler<TEvent> handlerInstance) return;
                await handlerInstance(sender, @event).ConfigureAwait(false);
            }));
        }
    }
}