using System;
using System.Collections.Generic;

namespace Robust.Shared.Configuration;

public sealed class ConfigurationMultiSubscriptionBuilder(IConfigurationManager manager) : IDisposable
{
	private readonly List<Action> _unsubscribeActions = new List<Action>();

	public ConfigurationMultiSubscriptionBuilder OnValueChanged<T>(CVarDef<T> cVar, CVarChanged<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		manager.OnValueChanged(cVar, onValueChanged, invokeImmediately);
		_unsubscribeActions.Add(delegate
		{
			manager.UnsubValueChanged(cVar, onValueChanged);
		});
		return this;
	}

	public ConfigurationMultiSubscriptionBuilder OnValueChanged<T>(string name, CVarChanged<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		manager.OnValueChanged(name, onValueChanged, invokeImmediately);
		_unsubscribeActions.Add(delegate
		{
			manager.UnsubValueChanged(name, onValueChanged);
		});
		return this;
	}

	public ConfigurationMultiSubscriptionBuilder OnValueChanged<T>(CVarDef<T> cVar, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		manager.OnValueChanged(cVar, onValueChanged, invokeImmediately);
		_unsubscribeActions.Add(delegate
		{
			manager.UnsubValueChanged(cVar, onValueChanged);
		});
		return this;
	}

	public ConfigurationMultiSubscriptionBuilder OnValueChanged<T>(string name, Action<T> onValueChanged, bool invokeImmediately = false) where T : notnull
	{
		manager.OnValueChanged(name, onValueChanged, invokeImmediately);
		_unsubscribeActions.Add(delegate
		{
			manager.UnsubValueChanged(name, onValueChanged);
		});
		return this;
	}

	public void Dispose()
	{
		foreach (Action unsubscribeAction in _unsubscribeActions)
		{
			unsubscribeAction();
		}
		_unsubscribeActions.Clear();
	}
}
