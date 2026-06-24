using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

[Serializable]
[NetSerializable]
public sealed class ViewVariablesBlobMetadata : ViewVariablesBlob
{
	public List<object> Traits { get; set; }

	public string ObjectTypePretty { get; set; }

	public string ObjectType { get; set; }

	public string Stringified { get; set; }
}
