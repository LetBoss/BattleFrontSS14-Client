using System;
using System.Collections.Generic;
using System.Reflection;

namespace Robust.Shared.Configuration;

internal interface IConfigurationManagerInternal : IConfigurationManager
{
	void OverrideConVars(IEnumerable<(string key, string value)> cVars);

	void LoadCVarsFromAssembly(Assembly assembly);

	void LoadCVarsFromType(Type containingType);

	void SetVirtualConfig();

	void Initialize(bool isServer);

	void Shutdown();

	HashSet<string> LoadFromFile(string configFile);

	void SetSaveFile(string configFile);

	void CheckUnusedCVars();
}
