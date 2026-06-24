namespace Robust.Shared.Prototypes;

public interface IInheritingPrototype
{
	string[]? Parents { get; }

	bool Abstract { get; }
}
