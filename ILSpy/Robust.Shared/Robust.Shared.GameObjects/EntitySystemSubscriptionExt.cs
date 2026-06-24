using System;
using Robust.Shared.Configuration;

namespace Robust.Shared.GameObjects;

public static class EntitySystemSubscriptionExt
{
	public static void CVar<T>(this EntitySystem.Subscriptions subs, IConfigurationManager cfg, string name, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		cfg.OnValueChanged(name, onValueChanged, invokeImmediately);
		subs.RegisterUnsubscription(delegate
		{
			cfg.UnsubValueChanged(name, onValueChanged);
		});
	}

	public static void CVar<T>(this EntitySystem.Subscriptions subs, IConfigurationManager cfg, CVarDef<T> cVar, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		cfg.OnValueChanged(cVar, onValueChanged, invokeImmediately);
		subs.RegisterUnsubscription(delegate
		{
			cfg.UnsubValueChanged(cVar, onValueChanged);
		});
	}

	public static void CVar<T>(this EntitySystem.Subscriptions subs, IConfigurationManager cfg, string name, CVarChanged<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		cfg.OnValueChanged(name, onValueChanged, invokeImmediately);
		subs.RegisterUnsubscription(delegate
		{
			cfg.UnsubValueChanged(name, onValueChanged);
		});
	}

	public static void CVar<T>(this EntitySystem.Subscriptions subs, IConfigurationManager cfg, CVarDef<T> cVar, CVarChanged<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		cfg.OnValueChanged(cVar, onValueChanged, invokeImmediately);
		subs.RegisterUnsubscription(delegate
		{
			cfg.UnsubValueChanged(cVar, onValueChanged);
		});
	}
}
