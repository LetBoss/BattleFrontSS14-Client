using System.Collections.Generic;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Actions;

public sealed class GetItemActionsEvent : EntityEventArgs
{
	private readonly ActionContainerSystem _system;

	public readonly SortedSet<EntityUid> Actions = new SortedSet<EntityUid>();

	public EntityUid User;

	public EntityUid Provider;

	public SlotFlags? SlotFlags;

	public bool InHands => !SlotFlags.HasValue;

	public GetItemActionsEvent(ActionContainerSystem system, EntityUid user, EntityUid provider, SlotFlags? slotFlags = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		_system = system;
		User = user;
		Provider = provider;
		SlotFlags = slotFlags;
	}

	public void AddAction(ref EntityUid? actionId, string prototypeId, EntityUid container)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (_system.EnsureAction(container, ref actionId, prototypeId))
		{
			Actions.Add(actionId.Value);
		}
	}

	public void AddAction(ref EntityUid? actionId, string prototypeId)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		AddAction(ref actionId, prototypeId, Provider);
	}

	public void AddAction(EntityUid? actionId)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (actionId.HasValue)
		{
			Actions.Add(actionId.Value);
		}
	}
}
