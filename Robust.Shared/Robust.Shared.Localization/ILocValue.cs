namespace Robust.Shared.Localization;

public interface ILocValue
{
	object? Value { get; }

	string Format(LocContext ctx);
}
