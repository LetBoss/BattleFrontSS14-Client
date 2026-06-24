// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fluids.SpraySafetySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Fluids.Components;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Fluids;

public sealed class SpraySafetySystem : EntitySystem
{
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SpraySafetyComponent, SolutionTransferAttemptEvent>(new EntityEventRefHandler<SpraySafetyComponent, SolutionTransferAttemptEvent>(this.OnTransferAttempt));
    this.SubscribeLocalEvent<SpraySafetyComponent, SolutionTransferredEvent>(new EntityEventRefHandler<SpraySafetyComponent, SolutionTransferredEvent>(this.OnTransferred));
    this.SubscribeLocalEvent<SpraySafetyComponent, SprayAttemptEvent>(new EntityEventRefHandler<SpraySafetyComponent, SprayAttemptEvent>(this.OnSprayAttempt));
  }

  private void OnTransferAttempt(
    Entity<SpraySafetyComponent> ent,
    ref SolutionTransferAttemptEvent args)
  {
    (EntityUid entityUid, SpraySafetyComponent comp) = ent;
    if (!(entityUid == args.To) || this._toggle.IsActivated((Entity<ItemToggleComponent>) entityUid))
      return;
    args.Cancel(this.Loc.GetString((string) comp.Popup));
  }

  private void OnTransferred(Entity<SpraySafetyComponent> ent, ref SolutionTransferredEvent args)
  {
    this._audio.PlayPredicted(ent.Comp.RefillSound, (EntityUid) ent, args.User);
  }

  private void OnSprayAttempt(Entity<SpraySafetyComponent> ent, ref SprayAttemptEvent args)
  {
    if (this._toggle.IsActivated((Entity<ItemToggleComponent>) ent.Owner))
      return;
    this._popup.PopupEntity(this.Loc.GetString((string) ent.Comp.Popup), (EntityUid) ent, args.User);
    args.Cancel();
  }
}
