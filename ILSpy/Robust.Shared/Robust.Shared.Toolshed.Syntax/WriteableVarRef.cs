namespace Robust.Shared.Toolshed.Syntax;

public sealed class WriteableVarRef<T>(VarRef<T> inner) : ValueRef<T>
{
	public readonly VarRef<T> Inner = inner;

	public override T? Evaluate(IInvocationContext ctx)
	{
		return Inner.Evaluate(ctx);
	}
}
