﻿using System.Threading.Channels;
using StoryPoker.Client.Web.Api.Abstractions;

namespace StoryPoker.Client.Web.Api.Infrastructure.BackgroundServices.GrainObserver.Channels;

public record GrainSubscription(Guid RoomId) : IGrainSubscriptionMessage
{
    public CancellationTokenSource ResubscribeStoppingToken { get; init; } = new ();
}
public record GrainUnsubscription(Guid RoomId): IGrainSubscriptionMessage;
internal class GrainsSubscriptionBus : IGrainSubscriptionBus
{
    private readonly Channel<IGrainSubscriptionMessage> _queue =
        Channel.CreateBounded<IGrainSubscriptionMessage>(1);

    public async Task EnqueueAsync(IGrainSubscriptionMessage message) => await _queue.Writer.WriteAsync(message);

    public IAsyncEnumerable<IGrainSubscriptionMessage> ReadAsync(CancellationToken token)
        => _queue.Reader.ReadAllAsync(token);
}
