using System;
using Robust.Shared.Prototypes;

namespace Robust.Shared.Toolshed.TypeParsers;

[Obsolete("Use ProtoId<T> or EntProtoId, or the prototype directly")]
public readonly record struct Prototype<T>(T Value) : IAsType<string> where T : class, IPrototype
{
	public ProtoId<T> Id => Value.ID;

	public string AsType()
	{
		return Value.ID;
	}
}
