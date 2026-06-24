using System;
using Robust.Shared.Serialization;

namespace Content.Shared.CrewManifest;

[Serializable]
[NetSerializable]
public sealed class CrewManifestEntry
{
	public string Name { get; }

	public string JobTitle { get; }

	public string JobIcon { get; }

	public string JobPrototype { get; }

	public string? Squad { get; }

	public CrewManifestEntry(string name, string jobTitle, string jobIcon, string jobPrototype)
	{
		Name = name;
		JobTitle = jobTitle;
		JobIcon = jobIcon;
		JobPrototype = jobPrototype;
	}

	public CrewManifestEntry(string name, string jobTitle, string jobIcon, string jobPrototype, string? squad)
	{
		Name = name;
		JobTitle = jobTitle;
		JobIcon = jobIcon;
		JobPrototype = jobPrototype;
		Squad = squad;
	}
}
