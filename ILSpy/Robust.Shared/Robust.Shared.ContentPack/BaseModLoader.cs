using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Reflection;
using Robust.Shared.Timing;

namespace Robust.Shared.ContentPack;

internal abstract class BaseModLoader : IPostInjectInit
{
	protected sealed class ModInfo
	{
		public Assembly GameAssembly { get; }

		public List<GameShared> EntryPoints { get; }

		public ModInfo(Assembly gameAssembly)
		{
			GameAssembly = gameAssembly;
			EntryPoints = new List<GameShared>();
		}
	}

	[Dependency]
	protected readonly IReflectionManager ReflectionManager;

	[Dependency]
	protected readonly ILogManager LogManager;

	[Dependency]
	private readonly IDependencyCollection _dependencies;

	private readonly List<ModuleTestingCallbacks> _testingCallbacks = new List<ModuleTestingCallbacks>();

	protected readonly List<ModInfo> Mods = new List<ModInfo>();

	protected ISawmill Sawmill { get; private set; }

	public IEnumerable<Assembly> LoadedModules => Mods.Select((ModInfo p) => p.GameAssembly);

	public Assembly GetAssembly(string name)
	{
		return Mods.Select((ModInfo p) => p.GameAssembly).Single((Assembly p) => p.GetName().Name == name);
	}

	protected void InitMod(Assembly assembly)
	{
		ModInfo modInfo = new ModInfo(assembly);
		ReflectionManager.LoadAssemblies(modInfo.GameAssembly);
		foreach (Type item in from t in modInfo.GameAssembly.GetTypes()
			where typeof(GameShared).IsAssignableFrom(t)
			select t)
		{
			GameShared gameShared = (GameShared)Activator.CreateInstance(item);
			gameShared.Dependencies = _dependencies;
			if (_testingCallbacks != null)
			{
				gameShared.SetTestingCallbacks(_testingCallbacks);
			}
			modInfo.EntryPoints.Add(gameShared);
		}
		Mods.Add(modInfo);
	}

	public bool IsContentAssembly(Assembly typeAssembly)
	{
		foreach (ModInfo mod in Mods)
		{
			if (mod.GameAssembly == typeAssembly)
			{
				return true;
			}
		}
		return false;
	}

	public void BroadcastRunLevel(ModRunLevel level)
	{
		foreach (ModInfo mod in Mods)
		{
			foreach (GameShared entryPoint in mod.EntryPoints)
			{
				switch (level)
				{
				case ModRunLevel.PreInit:
					entryPoint.PreInit();
					continue;
				case ModRunLevel.Init:
					entryPoint.Init();
					continue;
				case ModRunLevel.PostInit:
					entryPoint.PostInit();
					continue;
				}
				Sawmill.Error($"Unknown RunLevel: {level}");
			}
		}
	}

	public void BroadcastUpdate(ModUpdateLevel level, FrameEventArgs frameEventArgs)
	{
		foreach (ModInfo mod in Mods)
		{
			foreach (GameShared entryPoint in mod.EntryPoints)
			{
				entryPoint.Update(level, frameEventArgs);
			}
		}
	}

	public void SetModuleBaseCallbacks(ModuleTestingCallbacks testingCallbacks)
	{
		_testingCallbacks.Add(testingCallbacks);
	}

	public void Shutdown()
	{
		foreach (ModInfo mod in Mods)
		{
			foreach (GameShared entryPoint in mod.EntryPoints)
			{
				entryPoint.Shutdown();
			}
			foreach (GameShared entryPoint2 in mod.EntryPoints)
			{
				entryPoint2.Dispose();
			}
		}
	}

	void IPostInjectInit.PostInject()
	{
		Sawmill = LogManager.GetSawmill("res.mod");
	}
}
