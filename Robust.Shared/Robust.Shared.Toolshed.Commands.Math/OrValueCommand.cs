using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "or?")]
public sealed class OrValueCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T OrValue<T>(IInvocationContext ctx, [PipedArgument] T? value, ValueRef<T> alternate) where T : class
	{
		return value ?? alternate.Evaluate(ctx);
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T OrValue<T>(IInvocationContext ctx, [PipedArgument] T? value, ValueRef<T> alternate) where T : unmanaged
	{
		if (!value.HasValue)
		{
			return alternate.Evaluate(ctx);
		}
		return value.Value;
	}
}
