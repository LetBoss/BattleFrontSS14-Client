using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Spreader;

[Prototype(null, 1)]
public sealed class EdgeSpreaderPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public int UpdatesPerSecond;

	[DataField(null, false, 1, false, false, null)]
	public bool PreventSpreadOnSpaced = true;

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
