using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Input.Binding;

[NotContentImplementable]
public interface ICommandBindRegistry
{
	void Register<TOwner>(CommandBinds commandBinds);

	void Register(CommandBinds commandBinds, Type owner);

	IEnumerable<InputCmdHandler> GetHandlers(BoundKeyFunction function);

	void Unregister(Type owner);

	void Unregister<TOwner>();
}
