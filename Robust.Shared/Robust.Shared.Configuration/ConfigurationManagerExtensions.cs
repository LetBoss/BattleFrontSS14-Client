namespace Robust.Shared.Configuration;

public static class ConfigurationManagerExtensions
{
	public static ConfigurationMultiSubscriptionBuilder SubscribeMultiple(this IConfigurationManager manager)
	{
		return new ConfigurationMultiSubscriptionBuilder(manager);
	}
}
