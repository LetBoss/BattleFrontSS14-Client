using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
[Virtual]
public class ViewVariablesEntitySelector : ViewVariablesObjectSelector
{
	public NetEntity Entity { get; }

	public ViewVariablesEntitySelector(NetEntity entity)
	{
		Entity = entity;
	}
}
