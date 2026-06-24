using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesComponentSelector : ViewVariablesEntitySelector
{
	public string ComponentType { get; }

	public ViewVariablesComponentSelector(NetEntity uid, string componentType)
		: base(uid)
	{
		ComponentType = componentType;
	}
}
