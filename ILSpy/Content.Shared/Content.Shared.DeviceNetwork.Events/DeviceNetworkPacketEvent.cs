using Robust.Shared.GameObjects;

namespace Content.Shared.DeviceNetwork.Events;

public sealed class DeviceNetworkPacketEvent : EntityEventArgs
{
	public int NetId;

	public readonly uint Frequency;

	public string? Address;

	public readonly string SenderAddress;

	public EntityUid Sender;

	public readonly NetworkPayload Data;

	public DeviceNetworkPacketEvent(int netId, string? address, uint frequency, string senderAddress, EntityUid sender, NetworkPayload data)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		NetId = netId;
		Address = address;
		Frequency = frequency;
		SenderAddress = senderAddress;
		Sender = sender;
		Data = data;
	}
}
