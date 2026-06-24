// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.AttachableTemporarySpeedModsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Slow;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class AttachableTemporarySpeedModsSystem : EntitySystem
{
  [Dependency]
  private AttachableHolderSystem _attachableHolderSystem;
  [Dependency]
  private RMCSlowSystem _slow;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<AttachableTemporarySpeedModsComponent, AttachableAlteredEvent>(new EntityEventRefHandler<AttachableTemporarySpeedModsComponent, AttachableAlteredEvent>(this.OnAttachableAltered));
  }

  private void OnAttachableAltered(
    Entity<AttachableTemporarySpeedModsComponent> attachable,
    ref AttachableAlteredEvent args)
  {
    EntityUid? userUid;
    if ((attachable.Comp.Alteration & args.Alteration) != attachable.Comp.Alteration || !this._attachableHolderSystem.TryGetUser(attachable.Owner, out userUid))
      return;
    this._slow.TrySlowdown(userUid.Value, attachable.Comp.SlowDuration);
    this._slow.TrySuperSlowdown(userUid.Value, attachable.Comp.SuperSlowDuration);
  }
}
