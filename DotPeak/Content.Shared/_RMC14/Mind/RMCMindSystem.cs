// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mind.RMCMindSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.GameStates;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared._RMC14.Mind;

public sealed class RMCMindSystem : EntitySystem
{
  [Dependency]
  private SharedRMCPvsSystem _rmcPvs;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MindContainerComponent, PlayerAttachedEvent>(new EntityEventRefHandler<MindContainerComponent, PlayerAttachedEvent>(this.OnMindContainerPlayedAttached));
  }

  private void OnMindContainerPlayedAttached(
    Entity<MindContainerComponent> ent,
    ref PlayerAttachedEvent args)
  {
    MindComponent comp;
    if (!this.TryComp<MindComponent>(ent.Comp.Mind, out comp))
      return;
    foreach (EntityUid mindRole in comp.MindRoles)
      this._rmcPvs.AddSessionOverride(mindRole, args.Player);
  }
}
