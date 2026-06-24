using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesBlobEnumerable : ViewVariablesBlob
{
	public List<object> Objects { get; set; }
}
