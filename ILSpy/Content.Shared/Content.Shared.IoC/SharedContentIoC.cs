using Content.Shared._RMC14.Localizations;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Localizations;
using Robust.Shared.IoC;

namespace Content.Shared.IoC;

public static class SharedContentIoC
{
	public static void Register()
	{
		IoCManager.Register<MarkingManager, MarkingManager>(false);
		IoCManager.Register<ContentLocalizationManager, ContentLocalizationManager>(false);
		IoCManager.Register<RMCLocalizationManager>(false);
	}
}
