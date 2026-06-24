using System.Collections.Generic;
using System.Reflection;
using Robust.Shared.Analyzers;

namespace Robust.Shared.ContentPack;

[NotContentImplementable]
public interface IModLoader
{
	IEnumerable<Assembly> LoadedModules { get; }

	Assembly GetAssembly(string name);

	void SetModuleBaseCallbacks(ModuleTestingCallbacks testingCallbacks);

	bool IsContentAssembly(Assembly typeAssembly);
}
