using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Loadout;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.GameObjects;

namespace Content.Client._CIV14merka.Lobby;

public sealed class CivLoadoutSystem : EntitySystem
{
	private CivLoadoutStateEvent _state = new CivLoadoutStateEvent(new Dictionary<string, List<string>>(), new Dictionary<string, List<string>>(), new List<string>());

	public event Action<CivLoadoutStateEvent>? StateUpdated;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivLoadoutStateEvent>((EntitySessionEventHandler<CivLoadoutStateEvent>)OnState, (Type[])null, (Type[])null);
	}

	public CivLoadoutStateEvent GetState()
	{
		return _state;
	}

	public void RequestState()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivLoadoutRequestStateEvent());
	}

	public void SetItem(string faction, CivTdmClass cls, string itemKey, bool disabled)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivLoadoutSetItemRequestEvent(faction, cls, itemKey, disabled));
		string key = CivLoadoutKeys.Combo(faction, cls);
		if (!_state.Disabled.TryGetValue(key, out List<string> value))
		{
			if (!disabled)
			{
				return;
			}
			value = new List<string>();
			_state.Disabled[key] = value;
		}
		if (disabled)
		{
			if (!value.Contains(itemKey))
			{
				value.Add(itemKey);
			}
		}
		else
		{
			value.Remove(itemKey);
		}
	}

	public void SetSlotChoice(string faction, CivTdmClass cls, string slot, string proto)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivLoadoutSetSlotChoiceRequestEvent(faction, cls, slot, proto));
		string key = CivLoadoutKeys.Combo(faction, cls);
		if (!_state.Selections.TryGetValue(key, out List<string> value))
		{
			value = new List<string>();
			_state.Selections[key] = value;
		}
		value.RemoveAll((string entry) => CivLoadoutKeys.TryParseSlotChoice(entry, out string slot2, out string _) && slot2 == slot);
		value.Add(CivLoadoutKeys.SlotChoice(slot, proto));
	}

	private void OnState(CivLoadoutStateEvent msg, EntitySessionEventArgs args)
	{
		_state = msg;
		this.StateUpdated?.Invoke(msg);
	}
}
