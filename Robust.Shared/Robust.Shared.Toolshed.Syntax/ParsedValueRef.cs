namespace Robust.Shared.Toolshed.Syntax;

internal sealed class ParsedValueRef<T>(T? value) : ValueRef<T>
{
	public readonly T? Value = value;

	public override T? Evaluate(IInvocationContext ctx)
	{
		return Value;
	}
}
