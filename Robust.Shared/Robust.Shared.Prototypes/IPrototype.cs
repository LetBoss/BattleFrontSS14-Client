using Robust.Shared.ViewVariables;

namespace Robust.Shared.Prototypes;

public interface IPrototype
{
	[ViewVariables(VVAccess.ReadOnly)]
	string ID { get; }
}
