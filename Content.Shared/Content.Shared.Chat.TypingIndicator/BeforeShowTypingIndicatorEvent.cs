using System;
using Content.Shared.Inventory;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.TypingIndicator;

[Serializable]
[NetSerializable]
public sealed class BeforeShowTypingIndicatorEvent : IInventoryRelayEvent
{
	private ProtoId<TypingIndicatorPrototype>? _overrideIndicator;

	private TimeSpan? _latestEquipTime;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public BeforeShowTypingIndicatorEvent()
	{
		_overrideIndicator = null;
		_latestEquipTime = null;
	}

	public bool TryUpdateTimeAndIndicator(ProtoId<TypingIndicatorPrototype>? indicator, TimeSpan? equipTime)
	{
		if (equipTime.HasValue && (!_latestEquipTime.HasValue || _latestEquipTime < equipTime))
		{
			_latestEquipTime = equipTime;
			_overrideIndicator = indicator;
			return true;
		}
		return false;
	}

	public ProtoId<TypingIndicatorPrototype>? GetMostRecentIndicator()
	{
		return _overrideIndicator;
	}
}
