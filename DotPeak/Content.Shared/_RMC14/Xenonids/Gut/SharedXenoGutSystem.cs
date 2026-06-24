// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Gut.SharedXenoGutSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Gibbing;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Gut;

public sealed class SharedXenoGutSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedBodySystem _bodySystem;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private RMCGibSystem _rmcGib;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoGutComponent, XenoGutActionEvent>(new EntityEventRefHandler<XenoGutComponent, XenoGutActionEvent>(this.OnXenoGutAction));
    this.SubscribeLocalEvent<XenoGutComponent, XenoGutDoAfterEvent>(new EntityEventRefHandler<XenoGutComponent, XenoGutDoAfterEvent>(this.OnXenoGutDoAfterEvent));
  }

  private void OnXenoGutAction(Entity<XenoGutComponent> xeno, ref XenoGutActionEvent args)
  {
    if (args.Target == xeno.Owner || this.HasComp<XenoComponent>(args.Target) || args.Handled)
      return;
    XenoGutAttemptEvent args1 = new XenoGutAttemptEvent();
    this.RaiseLocalEvent<XenoGutAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled)
      return;
    EntityUid target = args.Target;
    if (!this.HasComp<BodyComponent>(target) || !this._xenoPlasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    args.Handled = true;
    XenoGutDoAfterEvent @event = new XenoGutDoAfterEvent();
    DoAfterArgs args2 = new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.Delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno), new EntityUid?(target))
    {
      BreakOnMove = true,
      BlockDuplicate = true,
      DuplicateCondition = DuplicateConditions.SameEvent
    };
    this._popup.PopupPredicted(this.Loc.GetString("rmc-gut-start-self"), this.Loc.GetString("rmc-gut-start-others", ("user", (object) xeno.Owner), ("target", (object) args.Target)), xeno.Owner, new EntityUid?(xeno.Owner), PopupType.LargeCaution);
    this._doAfter.TryStartDoAfter(args2);
    this._jitter.DoJitter(args.Target, xeno.Comp.Delay, true, 14f, 5f, true);
  }

  private void OnXenoGutDoAfterEvent(Entity<XenoGutComponent> xeno, ref XenoGutDoAfterEvent args)
  {
    EntityUid? target;
    if (!args.Cancelled && !args.Handled)
    {
      target = args.Target;
      if (target.HasValue)
      {
        EntityUid valueOrDefault = target.GetValueOrDefault();
        BodyComponent comp;
        if (valueOrDefault == xeno.Owner || this.HasComp<XenoComponent>(valueOrDefault) || !this.TryComp<BodyComponent>(valueOrDefault, out comp) || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
          return;
        args.Handled = true;
        if (this._net.IsServer)
        {
          this._rmcGib.ScatterInventoryItems(valueOrDefault);
          this._bodySystem.GibBody(valueOrDefault, true, comp, splatCone: new Angle());
          this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
        }
        this._popup.PopupPredicted(this.Loc.GetString("rmc-gut-finish-self"), this.Loc.GetString("rmc-gut-finish-others", ("user", (object) xeno.Owner), ("target", (object) args.Target)), xeno.Owner, new EntityUid?(xeno.Owner), PopupType.LargeCaution);
        using (IEnumerator<Entity<ActionComponent>> enumerator = this._rmcActions.GetActionsWithEvent<XenoGutActionEvent>((EntityUid) xeno).GetEnumerator())
        {
          while (enumerator.MoveNext())
            this._actions.SetIfBiggerCooldown(new Entity<ActionComponent>?(enumerator.Current.AsNullable()), xeno.Comp.Cooldown);
          return;
        }
      }
    }
    target = args.Target;
    if (!target.HasValue)
      return;
    this._statusEffects.TryRemoveStatusEffect(target.GetValueOrDefault(), "Jitter");
  }
}
