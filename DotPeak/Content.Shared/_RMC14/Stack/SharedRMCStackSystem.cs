// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stack.SharedRMCStackSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Stacks;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;

#nullable enable
namespace Content.Shared._RMC14.Stack;

public abstract class SharedRMCStackSystem : EntitySystem
{
  [Dependency]
  private SharedStackSystem _stack;

  public virtual EntityUid? Split(
    Entity<StackComponent?> stack,
    int amount,
    EntityCoordinates spawnPosition)
  {
    this._stack.Use((EntityUid) stack, amount);
    return new EntityUid?();
  }
}
