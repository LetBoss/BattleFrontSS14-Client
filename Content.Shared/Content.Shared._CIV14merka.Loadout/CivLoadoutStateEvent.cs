using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Loadout;

[Serializable]
[NetSerializable]
public sealed class CivLoadoutStateEvent : EntityEventArgs
{
	public readonly Dictionary<string, List<string>> Disabled;

	public readonly Dictionary<string, List<string>> Selections;

	public readonly List<string> Owned;

	public CivLoadoutStateEvent(Dictionary<string, List<string>> disabled, Dictionary<string, List<string>> selections, List<string> owned)
	{
		Disabled = disabled;
		Selections = selections;
		Owned = owned;
	}
}
