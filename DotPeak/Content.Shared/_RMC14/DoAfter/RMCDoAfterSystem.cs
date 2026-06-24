// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.DoAfter.RMCDoAfterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.DoAfter;

public sealed class RMCDoAfterSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfter;

  public bool ShouldCancel(Content.Shared.DoAfter.DoAfter doAfter)
  {
    return doAfter.Args.BreakOnRest && this.HasComp<XenoRestingComponent>(doAfter.Args.User);
  }

  public void TryCancel(Entity<DoAfterComponent?> ent, ushort? doAfterIndex)
  {
    if (!doAfterIndex.HasValue || !this.Resolve<DoAfterComponent>((EntityUid) ent, ref ent.Comp, false) || !ent.Comp.DoAfters.ContainsKey(doAfterIndex.Value))
      return;
    this._doAfter.Cancel((EntityUid) ent, doAfterIndex.Value, (DoAfterComponent) ent);
  }
}
