using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Prototypes;

public static class PrototypeManagerExt
{
	[return: NotNullIfNotNull("protoId")]
	public static T? Index<T>(this IPrototypeManager prototypeManager, ProtoId<T>? protoId) where T : class, IPrototype
	{
		if (protoId.HasValue)
		{
			return prototypeManager.Index(protoId.Value);
		}
		return null;
	}
}
