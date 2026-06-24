using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesBlobAllValidComponents : ViewVariablesBlob
{
	public List<string> ComponentTypes { get; set; } = new List<string>();
}
