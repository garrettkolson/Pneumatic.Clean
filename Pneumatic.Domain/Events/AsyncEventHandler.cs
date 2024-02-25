namespace Pneumatic.Domain.Events;

public delegate Task AsyncEventHandler<in TEvent>(object sender, TEvent @event)
    where TEvent : EventArgs;

internal static class AsyncEventHandlerExtensions
{
    public static async Task InvokeAsync<TEvent>(this AsyncEventHandler<TEvent>? eventHandler,
        object sender, TEvent @event)
        where TEvent : EventArgs
    {
        if (eventHandler != null)
        {
            // TODO: see if we can configure if the invocation list gets called sync/async
            await Task.WhenAll(eventHandler.GetInvocationList().Select(async invocation =>
            {
                if (invocation is not AsyncEventHandler<TEvent> handlerInstance) return;
                await handlerInstance(sender, @event).ConfigureAwait(false);
            }));
        }
    }
}