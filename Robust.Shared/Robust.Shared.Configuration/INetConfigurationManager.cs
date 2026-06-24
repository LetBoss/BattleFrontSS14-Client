using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.Network;

namespace Robust.Shared.Configuration;

[NotContentImplementable]
public interface INetConfigurationManager : IConfigurationManager
{
	void SetupNetworking();

	List<(string name, object value)> GetReplicatedVars(bool all = false);

	void TickProcessMessages();

	void FlushMessages();

	T GetClientCVar<T>(INetChannel channel, string name);

	T GetClientCVar<T>(INetChannel channel, CVarDef<T> definition) where T : notnull
	{
		return GetClientCVar<T>(channel, definition.Name);
	}
}
