using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.PDA;

[Serializable]
[NetSerializable]
public struct PdaIdInfoText
{
	public string? ActualOwnerName;

	public string? IdOwner;

	public string? JobTitle;

	public string? StationAlertLevel;

	public Color StationAlertColor;
}
