// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Utility.Systems.SharedMedevacSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Events;
using Content.Shared._RMC14.Medical.MedevacStretcher;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Utility.Systems;

public abstract class SharedMedevacSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private DropshipUtilitySystem _dropshipUtility;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private MedevacStretcherSystem _stretcher;
  [Dependency]
  private UseDelaySystem _useDelay;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MedevacComponent, MapInitEvent>(new EntityEventRefHandler<MedevacComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<MedevacComponent, InteractHandEvent>(new EntityEventRefHandler<MedevacComponent, InteractHandEvent>(this.OnInteract));
  }

  private void OnMapInit(Entity<MedevacComponent> ent, ref MapInitEvent args)
  {
    this._useDelay.SetLength((Entity<UseDelayComponent>) ent.Owner, ent.Comp.DelayLength, "medevac_system_delay");
  }

  private void OnInteract(Entity<MedevacComponent> ent, ref InteractHandEvent args)
  {
    DropshipUtilityComponent comp;
    if (args.Target == ent.Owner || ent.Comp.IsActivated || !this.TryComp<DropshipUtilityComponent>(ent.Owner, out comp) || !this.HasComp<DropshipUtilityPointComponent>(args.Target))
      return;
    EntityCoordinates coordinates = ent.Owner.ToCoordinates();
    if (!comp.Target.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-medevac-no-target"), coordinates, new EntityUid?(args.User));
    }
    else
    {
      string popup;
      if (!this._dropshipUtility.IsActivatable(new Entity<DropshipUtilityComponent>(ent.Owner, comp), args.User, out popup))
      {
        if (!this._net.IsServer)
          return;
        this._popup.PopupCoordinates(popup, coordinates, args.User);
      }
      else
      {
        PrepareMedevacEvent args1 = new PrepareMedevacEvent(this.GetNetEntity(args.Target));
        this.RaiseLocalEvent<PrepareMedevacEvent>(comp.Target.Value, args1);
        if (args1.ReadyForMedevac)
        {
          this._appearance.SetData(args.Target, (Enum) DropshipUtilityVisuals.State, (object) "medevac_system_active");
          ent.Comp.IsActivated = true;
          this._useDelay.TryResetDelay(ent.Owner, id: "medevac_system_delay");
        }
        else
          this._popup.PopupClient(this.Loc.GetString("rmc-medevac-stretcher-failure"), coordinates, new EntityUid?(args.User));
      }
    }
  }

  private void AfterMedevac(Entity<MedevacComponent> ent)
  {
    DropshipUtilityComponent comp1;
    if (!this.TryComp<DropshipUtilityComponent>(ent.Owner, out comp1))
      return;
    EntityUid? target = comp1.Target;
    MedevacStretcherComponent comp2;
    if (!target.HasValue || !this.TryComp<MedevacStretcherComponent>(target, out comp2))
      return;
    EntityUid? attachmentPoint = comp1.AttachmentPoint;
    (EntityUid, DropshipUtilityComponent) ent1 = (ent.Owner, comp1);
    (EntityUid, MedevacStretcherComponent) ent2 = (target.Value, comp2);
    ent.Comp.IsActivated = false;
    DropshipAttachedSpriteComponent comp3;
    if (this.TryComp<DropshipAttachedSpriteComponent>((EntityUid) ent, out comp3) && comp3.Sprite != null && attachmentPoint.HasValue)
      this._appearance.SetData(attachmentPoint.Value, (Enum) DropshipUtilityVisuals.State, (object) comp3.Sprite.RsiState);
    this._stretcher.Medevac((Entity<MedevacStretcherComponent>) ent2, ent.Owner);
    this._dropshipUtility.ResetActivationCooldown((Entity<DropshipUtilityComponent>) ent1);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    AllEntityQueryEnumerator<MedevacComponent> entityQueryEnumerator = this.AllEntityQuery<MedevacComponent>();
    EntityUid uid;
    MedevacComponent comp1;
    UseDelayComponent comp;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1) && this.TryComp<UseDelayComponent>(uid, out comp))
    {
      if (comp1.IsActivated && !this._useDelay.IsDelayed((Entity<UseDelayComponent>) (uid, comp), "medevac_system_delay"))
        this.AfterMedevac((Entity<MedevacComponent>) (uid, comp1));
    }
  }
}
