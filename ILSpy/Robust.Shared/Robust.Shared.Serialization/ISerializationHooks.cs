using Robust.Shared.Analyzers;

namespace Robust.Shared.Serialization;

[RequiresExplicitImplementation]
public interface ISerializationHooks
{
	void AfterDeserialization()
	{
	}
}
