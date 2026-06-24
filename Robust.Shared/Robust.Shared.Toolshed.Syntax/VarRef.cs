using System;
using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Syntax;

public sealed class VarRef<T>(string varName) : ValueRef<T>
{
	public record BadVarTypeError(Type? Got, Type Expected, string VarName) : IConError
	{
		public string? Expression { get; set; }

		public Vector2i? IssueSpan { get; set; }

		public StackTrace? Trace { get; set; }

		public FormattedMessage DescribeInner()
		{
			return FormattedMessage.FromUnformatted((Got == null) ? $"Variable ${VarName} is not assigned. Expected variable of type {Expected.PrettyName()}." : $"Variable ${VarName} is not of the expected type. Expected {Expected.PrettyName()} but got {Got?.PrettyName()}.");
		}
	}

	public readonly string VarName = varName;

	public override T? Evaluate(IInvocationContext ctx)
	{
		object obj = ctx.ReadVar(VarName);
		if (obj is T)
		{
			return (T)obj;
		}
		BadVarTypeError err = new BadVarTypeError(obj?.GetType(), typeof(T), VarName);
		ctx.ReportError(err);
		return default(T);
	}
}
