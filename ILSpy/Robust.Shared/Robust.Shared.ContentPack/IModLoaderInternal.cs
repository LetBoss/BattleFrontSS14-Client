using System;
using System.Collections.Generic;
using System.IO;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Robust.Shared.ContentPack;

internal interface IModLoaderInternal : IModLoader
{
	Func<string, Stream?>? VerifierExtraLoadHandler { get; set; }

	event ExtraModuleLoad ExtraModuleLoaders;

	bool TryLoadModulesFrom(ResPath mountPath, string filterPrefix);

	bool TryLoadModules(IEnumerable<ResPath> paths);

	void LoadGameAssembly(Stream assembly, Stream? symbols = null, bool skipVerify = false);

	void LoadGameAssembly(string diskPath, bool skipVerify = false);

	void BroadcastRunLevel(ModRunLevel level);

	void BroadcastUpdate(ModUpdateLevel level, FrameEventArgs frameEventArgs);

	bool TryLoadAssembly(string assemblyName);

	void SetUseLoadContext(bool useLoadContext);

	void SetEnableSandboxing(bool sandboxing);

	void AddEngineModuleDirectory(string dir);

	void Shutdown();
}
