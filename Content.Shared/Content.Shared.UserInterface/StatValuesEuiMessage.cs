using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.UserInterface;

[Serializable]
[NetSerializable]
public sealed class StatValuesEuiMessage : EuiMessageBase
{
	public string Title = string.Empty;

	public List<string> Headers = new List<string>();

	public List<string[]> Values = new List<string[]>();
}
