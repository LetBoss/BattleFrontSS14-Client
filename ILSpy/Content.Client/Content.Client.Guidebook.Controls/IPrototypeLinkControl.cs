using Robust.Shared.Prototypes;

namespace Content.Client.Guidebook.Controls;

public interface IPrototypeLinkControl
{
	IPrototype? LinkedPrototype { get; }

	void EnablePrototypeLink();
}
