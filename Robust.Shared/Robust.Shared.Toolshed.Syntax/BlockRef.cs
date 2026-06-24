namespace Robust.Shared.Toolshed.Syntax;

public sealed class BlockRef<T>(Block<T> block) : ValueRef<T>
{
	public override T? Evaluate(IInvocationContext ctx)
	{
		return block.Invoke(ctx);
	}
}
