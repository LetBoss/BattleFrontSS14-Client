using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceLinking.Events;

public sealed class NewLinkEvent : EntityEventArgs
{
	public readonly EntityUid Source;

	public readonly EntityUid Sink;

	public readonly EntityUid? User;

	public readonly string SourcePort;

	public readonly string SinkPort;

	public NewLinkEvent(EntityUid? user, EntityUid source, string sourcePort, EntityUid sink, string sinkPort)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Source = source;
		SourcePort = sourcePort;
		Sink = sink;
		SinkPort = sinkPort;
	}
}
