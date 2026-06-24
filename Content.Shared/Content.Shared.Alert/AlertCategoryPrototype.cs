using Robust.Shared.Prototypes;

namespace Content.Shared.Alert;

[Prototype(null, 1)]
public sealed class AlertCategoryPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
