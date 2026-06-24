using Robust.Shared.Serialization;

namespace Robust.Shared.Localization;

internal interface ILocalizationManagerInternal : ILocalizationManager
{
	void AddLoadedToStringSerializer(IRobustMappedStringSerializer serializer);
}
