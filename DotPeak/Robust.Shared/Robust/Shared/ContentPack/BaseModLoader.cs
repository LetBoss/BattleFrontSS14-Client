// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.BaseModLoader
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Reflection;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Robust.Shared.ContentPack;

internal abstract class BaseModLoader : IPostInjectInit
{
  [Dependency]
  protected readonly IReflectionManager ReflectionManager;
  [Dependency]
  protected readonly ILogManager LogManager;
  [Dependency]
  private readonly IDependencyCollection _dependencies;
  private readonly List<ModuleTestingCallbacks> _testingCallbacks = new List<ModuleTestingCallbacks>();
  protected readonly List<BaseModLoader.ModInfo> Mods = new List<BaseModLoader.ModInfo>();

  protected ISawmill Sawmill { get; private set; }

  public IEnumerable<Assembly> LoadedModules
  {
    get
    {
      return this.Mods.Select<BaseModLoader.ModInfo, Assembly>((Func<BaseModLoader.ModInfo, Assembly>) (p => p.GameAssembly));
    }
  }

  public Assembly GetAssembly(string name)
  {
    return this.Mods.Select<BaseModLoader.ModInfo, Assembly>((Func<BaseModLoader.ModInfo, Assembly>) (p => p.GameAssembly)).Single<Assembly>((Func<Assembly, bool>) (p => p.GetName().Name == name));
  }

  protected void InitMod(Assembly assembly)
  {
    BaseModLoader.ModInfo modInfo = new BaseModLoader.ModInfo(assembly);
    this.ReflectionManager.LoadAssemblies(modInfo.GameAssembly);
    foreach (Type type in ((IEnumerable<Type>) modInfo.GameAssembly.GetTypes()).Where<Type>((Func<Type, bool>) (t => typeof (GameShared).IsAssignableFrom(t))))
    {
      GameShared instance = (GameShared) Activator.CreateInstance(type);
      instance.Dependencies = this._dependencies;
      if (this._testingCallbacks != null)
        instance.SetTestingCallbacks(this._testingCallbacks);
      modInfo.EntryPoints.Add(instance);
    }
    this.Mods.Add(modInfo);
  }

  public bool IsContentAssembly(Assembly typeAssembly)
  {
    foreach (BaseModLoader.ModInfo mod in this.Mods)
    {
      if (mod.GameAssembly == typeAssembly)
        return true;
    }
    return false;
  }

  public void BroadcastRunLevel(ModRunLevel level)
  {
    foreach (BaseModLoader.ModInfo mod in this.Mods)
    {
      foreach (GameShared entryPoint in mod.EntryPoints)
      {
        switch (level)
        {
          case ModRunLevel.Init:
            entryPoint.Init();
            continue;
          case ModRunLevel.PostInit:
            entryPoint.PostInit();
            continue;
          case ModRunLevel.PreInit:
            entryPoint.PreInit();
            continue;
          default:
            this.Sawmill.Error($"Unknown RunLevel: {level}");
            continue;
        }
      }
    }
  }

  public void BroadcastUpdate(ModUpdateLevel level, FrameEventArgs frameEventArgs)
  {
    foreach (BaseModLoader.ModInfo mod in this.Mods)
    {
      foreach (GameShared entryPoint in mod.EntryPoints)
        entryPoint.Update(level, frameEventArgs);
    }
  }

  public void SetModuleBaseCallbacks(ModuleTestingCallbacks testingCallbacks)
  {
    this._testingCallbacks.Add(testingCallbacks);
  }

  public void Shutdown()
  {
    foreach (BaseModLoader.ModInfo mod in this.Mods)
    {
      foreach (GameShared entryPoint in mod.EntryPoints)
        entryPoint.Shutdown();
      foreach (GameShared entryPoint in mod.EntryPoints)
        entryPoint.Dispose();
    }
  }

  void IPostInjectInit.PostInject() => this.Sawmill = this.LogManager.GetSawmill("res.mod");

  protected sealed class ModInfo
  {
    public ModInfo(Assembly gameAssembly)
    {
      this.GameAssembly = gameAssembly;
      this.EntryPoints = new List<GameShared>();
    }

    public Assembly GameAssembly { get; }

    public List<GameShared> EntryPoints { get; }
  }
}
