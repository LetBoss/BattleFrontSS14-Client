// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.EntityConsoleHost
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Console;

internal sealed class EntityConsoleHost
{
  [Dependency]
  private readonly IConsoleHost _consoleHost;
  [Dependency]
  private readonly IReflectionManager _reflectionManager;
  [Dependency]
  private readonly IEntitySystemManager _entitySystemManager;
  private readonly HashSet<string> _entityCommands = new HashSet<string>();

  public bool DiscoverCommands { get; set; } = true;

  public void Startup()
  {
    if (!this.DiscoverCommands)
      return;
    DependencyCollection dependencyCollection = ((EntitySystemManager) this._entitySystemManager).SystemDependencyCollection;
    this._consoleHost.BeginRegistrationRegion();
    foreach (Type allChild in this._reflectionManager.GetAllChildren<IEntityConsoleCommand>())
    {
      IConsoleCommand instance = (IConsoleCommand) Activator.CreateInstance(allChild);
      dependencyCollection.InjectDependencies((object) instance, true);
      this._entityCommands.Add(instance.Command);
      this._consoleHost.RegisterCommand(instance);
    }
    this._consoleHost.EndRegistrationRegion();
  }

  public void Shutdown()
  {
    foreach (string entityCommand in this._entityCommands)
      this._consoleHost.UnregisterCommand(entityCommand);
    this._entityCommands.Clear();
  }
}
