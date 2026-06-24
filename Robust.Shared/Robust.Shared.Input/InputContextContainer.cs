using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Robust.Shared.Input;

internal sealed class InputContextContainer : IInputContextContainer
{
	public const string DefaultContextName = "common";

	private readonly Dictionary<string, InputCmdContext> _contexts = new Dictionary<string, InputCmdContext>();

	private InputCmdContext _activeContext;

	private InputCmdContext? _deferredContextSwitch;

	private bool _deferringEnabled;

	public IInputCmdContext ActiveContext => _activeContext;

	public bool DeferringEnabled
	{
		get
		{
			return _deferringEnabled;
		}
		set
		{
			_deferringEnabled = value;
			if (!value && _deferredContextSwitch != null)
			{
				InputCmdContext deferredContextSwitch = _deferredContextSwitch;
				_deferredContextSwitch = null;
				_setActiveContextImmediately(deferredContextSwitch);
			}
		}
	}

	public event EventHandler<ContextChangedEventArgs>? ContextChanged;

	public InputContextContainer()
	{
		_contexts.Add("common", new InputCmdContext("common"));
		SetActiveContext("common");
	}

	public IInputCmdContext New(string uniqueName, string parentName)
	{
		if (string.IsNullOrWhiteSpace(uniqueName))
		{
			throw new ArgumentException("String is null or whitespace.", "uniqueName");
		}
		if (string.IsNullOrWhiteSpace(parentName))
		{
			throw new ArgumentException("String is null or whitespace.", "parentName");
		}
		if (!_contexts.TryGetValue(parentName, out InputCmdContext value))
		{
			throw new ArgumentException("Parent does not exist.", "parentName");
		}
		if (_contexts.ContainsKey(uniqueName))
		{
			throw new ArgumentException("Context with name " + uniqueName + " already exists.", "uniqueName");
		}
		InputCmdContext inputCmdContext = new InputCmdContext(value, uniqueName);
		_contexts.Add(uniqueName, inputCmdContext);
		return inputCmdContext;
	}

	public IInputCmdContext New(string uniqueName, IInputCmdContext parent)
	{
		if (string.IsNullOrWhiteSpace(uniqueName))
		{
			throw new ArgumentException("String is null or whitespace.", "uniqueName");
		}
		if (_contexts.ContainsKey(uniqueName))
		{
			throw new ArgumentException("Context with name " + uniqueName + " already exists.", "uniqueName");
		}
		if (parent == null)
		{
			throw new ArgumentNullException("parent");
		}
		InputCmdContext inputCmdContext = new InputCmdContext(parent, uniqueName);
		_contexts.Add(uniqueName, inputCmdContext);
		return inputCmdContext;
	}

	public bool Exists(string uniqueName)
	{
		return _contexts.ContainsKey(uniqueName);
	}

	public IInputCmdContext GetContext(string uniqueName)
	{
		return _contexts[uniqueName];
	}

	public bool TryGetContext(string uniqueName, [NotNullWhen(true)] out IInputCmdContext? context)
	{
		if (_contexts.TryGetValue(uniqueName, out InputCmdContext value))
		{
			context = value;
			return true;
		}
		context = null;
		return false;
	}

	public void Remove(string uniqueName)
	{
		if (uniqueName == "common")
		{
			throw new ArgumentException("The default context cannot be removed.", "uniqueName");
		}
		_contexts.Remove(uniqueName);
	}

	public void SetActiveContext(string uniqueName)
	{
		if (!DeferringEnabled)
		{
			_setActiveContextImmediately(_contexts[uniqueName]);
		}
		else
		{
			_deferredContextSwitch = _contexts[uniqueName];
		}
	}

	private void _setActiveContextImmediately(InputCmdContext icc)
	{
		ContextChangedEventArgs e = new ContextChangedEventArgs(_activeContext, icc);
		_activeContext = icc;
		this.ContextChanged?.Invoke(this, e);
	}
}
