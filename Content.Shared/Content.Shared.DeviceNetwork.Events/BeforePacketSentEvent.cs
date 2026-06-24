using System.Numerics;
using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceNetwork.Events;

public sealed class BeforePacketSentEvent : CancellableEntityEventArgs
{
	public readonly EntityUid Sender;

	public readonly TransformComponent SenderTransform;

	public readonly Vector2 SenderPosition;

	public readonly string NetworkId;

	public BeforePacketSentEvent(EntityUid sender, TransformComponent xform, Vector2 senderPosition, string networkId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Sender = sender;
		SenderTransform = xform;
		SenderPosition = senderPosition;
		NetworkId = networkId;
	}
}
