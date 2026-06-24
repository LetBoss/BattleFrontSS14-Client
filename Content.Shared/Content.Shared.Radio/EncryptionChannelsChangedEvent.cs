using Content.Shared.Radio.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Radio;

public sealed class EncryptionChannelsChangedEvent : EntityEventArgs
{
	public readonly EncryptionKeyHolderComponent Component;

	public EncryptionChannelsChangedEvent(EncryptionKeyHolderComponent component)
	{
		Component = component;
	}
}
