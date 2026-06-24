using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.DeviceLinking;

[Prototype(null, 1)]
public sealed class SourcePortPrototype : DevicePortPrototype, IPrototype
{
	[DataField("defaultLinks", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<SinkPortPrototype>))]
	public HashSet<string>? DefaultLinks;
}
