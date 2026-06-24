// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.DumpDependencyInjectors
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using System;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class DumpDependencyInjectors : LocalizedCommands
{
  [Dependency]
  private readonly IDependencyCollection _dependencies;

  public override string Command => "dump_dependency_injectors";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    Type[] cachedInjectorTypes = ((DependencyCollection) this._dependencies).GetCachedInjectorTypes();
    foreach (Type type in cachedInjectorTypes)
      shell.WriteLine(type.FullName ?? "");
    shell.WriteLine(this.Loc.GetString("cmd-dump_dependency_injectors-total-count", ("total", (object) cachedInjectorTypes.Length)));
  }
}
