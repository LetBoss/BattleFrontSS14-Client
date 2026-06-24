using System;

namespace Robust.Shared.Analyzers;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoGenerateComponentStateAttribute : Attribute
{
	public readonly bool RaiseAfterAutoHandleState;

	public readonly bool FieldDeltas;

	public AutoGenerateComponentStateAttribute(bool raiseAfterAutoHandleState = false, bool fieldDeltas = false)
	{
		RaiseAfterAutoHandleState = raiseAfterAutoHandleState;
		FieldDeltas = fieldDeltas;
	}
}
