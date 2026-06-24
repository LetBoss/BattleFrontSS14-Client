using Robust.Shared.GameObjects;

namespace Content.Shared.Configurable;

public sealed class ConfigurationUpdatedEvent : EntityEventArgs
{
	public ConfigurationComponent Configuration;

	public ConfigurationUpdatedEvent(ConfigurationComponent configuration)
	{
		Configuration = configuration;
	}
}
