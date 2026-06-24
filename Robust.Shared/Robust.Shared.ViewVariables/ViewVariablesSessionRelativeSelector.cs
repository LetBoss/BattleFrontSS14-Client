using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesSessionRelativeSelector : ViewVariablesObjectSelector
{
	public uint SessionId { get; }

	public object[] PropertyIndex { get; }

	public ViewVariablesSessionRelativeSelector(uint sessionId, object[] propertyIndex)
	{
		SessionId = sessionId;
		PropertyIndex = propertyIndex;
	}
}
