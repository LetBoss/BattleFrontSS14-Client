using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesSelectorKeyValuePair
{
	public bool Key { get; set; }
}
