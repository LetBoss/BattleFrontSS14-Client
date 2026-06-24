using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared.NameModifier.EntitySystems;

[ByRefEvent]
public sealed class RefreshNameModifiersEvent : IInventoryRelayEvent
{
	public readonly string BaseName;

	private readonly List<(LocId LocId, int Priority, (string, object)[] ExtraArgs)> _modifiers = new List<(LocId, int, (string, object)[])>();

	public SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;

	public int ModifierCount => _modifiers.Count;

	public RefreshNameModifiersEvent(string baseName)
	{
		BaseName = baseName;
	}

	public void AddModifier(LocId locId, int priority = 0, params (string, object)[] extraArgs)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_modifiers.Add((locId, priority, extraArgs));
	}

	public string GetModifiedName()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		string name = BaseName;
		foreach (var item in _modifiers.OrderBy<(LocId, int, (string, object)[]), int>(((LocId LocId, int Priority, (string, object)[] ExtraArgs) n) => n.Priority))
		{
			(string, object)[] args = item.Item3;
			Array.Resize(ref args, args.Length + 1);
			args[^1] = ("baseName", name);
			name = Loc.GetString(LocId.op_Implicit(item.Item1), args);
		}
		return name;
	}
}
