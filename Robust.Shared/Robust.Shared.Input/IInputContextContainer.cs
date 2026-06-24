using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Input;

[NotContentImplementable]
public interface IInputContextContainer
{
	IInputCmdContext ActiveContext { get; }

	bool DeferringEnabled { get; set; }

	event EventHandler<ContextChangedEventArgs> ContextChanged;

	IInputCmdContext New(string uniqueName, string parentName);

	IInputCmdContext New(string uniqueName, IInputCmdContext parent);

	bool Exists(string uniqueName);

	IInputCmdContext GetContext(string uniqueName);

	bool TryGetContext(string uniqueName, [NotNullWhen(true)] out IInputCmdContext? context);

	void Remove(string uniqueName);

	void SetActiveContext(string uniqueName);
}
