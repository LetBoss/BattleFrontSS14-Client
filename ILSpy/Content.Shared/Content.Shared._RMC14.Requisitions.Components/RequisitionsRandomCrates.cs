using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared._RMC14.Requisitions.Components;

[Serializable]
[DataRecord]
[NetSerializable]
public sealed class RequisitionsRandomCrates
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan Every;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int Minimum;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int MinimumFor;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public List<EntProtoId> Choices = new List<EntProtoId>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan Next;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int Given;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public double Fraction;
}
