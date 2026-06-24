using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Audio;

[Prototype(null, 1)]
public sealed class SoundCollectionPrototype : IPrototype
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("files", false, 1, false, false, null)]
	public List<ResPath> PickFiles { get; private set; } = new List<ResPath>();
}
