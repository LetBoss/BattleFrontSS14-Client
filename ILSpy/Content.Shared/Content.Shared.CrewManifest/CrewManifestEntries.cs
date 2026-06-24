using System;
using Robust.Shared.Serialization;

namespace Content.Shared.CrewManifest;

[Serializable]
[NetSerializable]
public sealed class CrewManifestEntries
{
	public CrewManifestEntry[] Entries = Array.Empty<CrewManifestEntry>();
}
