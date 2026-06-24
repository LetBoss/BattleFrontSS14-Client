using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.CrewManifest;

[Serializable]
[NetSerializable]
public sealed class CrewManifestEuiState : EuiStateBase
{
	public string StationName { get; }

	public CrewManifestEntries? Entries { get; }

	public CrewManifestEuiState(string stationName, CrewManifestEntries? entries)
	{
		StationName = stationName;
		Entries = entries;
	}
}
