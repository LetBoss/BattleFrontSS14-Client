using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.Laws;

[Prototype(null, 1)]
public sealed class SiliconLawsetPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, typeof(PrototypeIdListSerializer<SiliconLawPrototype>))]
	public List<string> Laws = new List<string>();

	[DataField(null, false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ObeysTo = string.Empty;

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
