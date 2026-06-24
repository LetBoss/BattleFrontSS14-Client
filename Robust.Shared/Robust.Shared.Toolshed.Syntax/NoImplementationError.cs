using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Syntax;

public sealed class NoImplementationError(ParserContext ctx) : ConError
{
	public readonly ToolshedEnvironment Env = ctx.Environment;

	public readonly string Cmd = ctx.Bundle.Command;

	public readonly string? SubCommand = ctx.Bundle.SubCommand;

	public readonly Type[]? Types = ctx.Bundle.TypeArguments;

	public readonly Type? PipedType = ctx.Bundle.PipedType;

	public override FormattedMessage DescribeInner()
	{
		FormattedMessage formattedMessage = FormattedMessage.FromUnformatted($"Could not find an implementation of the '{Cmd}' command given the input type '{PipedType?.PrettyName() ?? "void"}'.\n");
		HashSet<Type> source = Env.GetCommand(Cmd).AcceptedTypes(SubCommand);
		if (source.Any((Type x) => x.IsGenericParameter))
		{
			return formattedMessage;
		}
		if (source.Any((Type x) => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>) && x.GetGenericArguments()[0].IsGenericParameter))
		{
			return formattedMessage;
		}
		formattedMessage.AddText("Accepted types: '" + string.Join("','", source.Select((Type x) => x.PrettyName())) + "'.\n");
		return formattedMessage;
	}
}
