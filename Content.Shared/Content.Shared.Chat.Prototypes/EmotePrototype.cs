using System.Collections.Generic;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Chat.Prototypes;

[Prototype(null, 1)]
public sealed class EmotePrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public string Name;

	[DataField(null, false, 1, false, false, null)]
	public bool Available = true;

	[DataField(null, false, 1, false, false, null)]
	public EmoteCategory Category = EmoteCategory.General;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier Icon = (SpriteSpecifier)new Texture(new ResPath("/Textures/Interface/Actions/scream.png"));

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? Blacklist;

	[DataField(null, false, 1, false, false, null)]
	public List<string> ChatMessages = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<string> ChatTriggers = new HashSet<string>();

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
