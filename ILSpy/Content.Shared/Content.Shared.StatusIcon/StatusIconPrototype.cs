using Robust.Shared.Prototypes;

namespace Content.Shared.StatusIcon;

public abstract class StatusIconPrototype : StatusIconData, IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
