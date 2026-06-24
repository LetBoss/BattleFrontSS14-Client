using System.Collections.Generic;
using Content.Shared._RMC14.Prototypes;
using Content.Shared.Guidebook;
using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Roles;

[Prototype(null, 1)]
public sealed class AntagPrototype : IPrototype, ICMSpecific
{
	[DataField(null, false, 1, false, false, null)]
	[Access(/*Could not decode attribute arguments.*/)]
	public HashSet<JobRequirement>? Requirements;

	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<GuideEntryPrototype>>? Guides;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("name", false, 1, false, false, null)]
	public string Name { get; private set; } = "";

	[DataField("objective", false, 1, true, false, null)]
	public string Objective { get; private set; } = "";

	[DataField("antagonist", false, 1, false, false, null)]
	public bool Antagonist { get; private set; }

	[DataField("setPreference", false, 1, false, false, null)]
	public bool SetPreference { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool IsCM { get; private set; }
}
