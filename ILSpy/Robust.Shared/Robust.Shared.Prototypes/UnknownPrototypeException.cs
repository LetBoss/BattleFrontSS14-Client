using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Prototypes;

[Serializable]
[Virtual]
public class UnknownPrototypeException : Exception
{
	public readonly string Prototype;

	public readonly Type Kind;

	public override string Message => "Unknown " + Kind.Name + " prototype: " + Prototype;

	public UnknownPrototypeException(string prototype, Type kind)
	{
		Prototype = prototype;
		Kind = kind;
	}
}
