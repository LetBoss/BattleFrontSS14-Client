// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Plasma.XenoPlasmaSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared.Alert;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Plasma;

public sealed class XenoPlasmaSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedJitteringSystem _jitter;
  private Robust.Shared.GameObjects.EntityQuery<XenoPlasmaComponent> _xenoPlasmaQuery;

  public override void Initialize()
  {
    this._xenoPlasmaQuery = this.GetEntityQuery<XenoPlasmaComponent>();
    this.SubscribeLocalEvent<XenoPlasmaComponent, MapInitEvent>(new EntityEventRefHandler<XenoPlasmaComponent, MapInitEvent>(this.OnXenoPlasmaMapInit));
    this.SubscribeLocalEvent<XenoPlasmaComponent, ComponentRemove>(new EntityEventRefHandler<XenoPlasmaComponent, ComponentRemove>(this.OnXenoPlasmaRemove));
    this.SubscribeLocalEvent<XenoPlasmaComponent, RejuvenateEvent>(new EntityEventRefHandler<XenoPlasmaComponent, RejuvenateEvent>(this.OnXenoRejuvenate));
    this.SubscribeLocalEvent<XenoPlasmaComponent, XenoTransferPlasmaActionEvent>(new EntityEventRefHandler<XenoPlasmaComponent, XenoTransferPlasmaActionEvent>(this.OnXenoTransferPlasmaAction));
    this.SubscribeLocalEvent<XenoPlasmaComponent, XenoTransferPlasmaDoAfterEvent>(new EntityEventRefHandler<XenoPlasmaComponent, XenoTransferPlasmaDoAfterEvent>(this.OnXenoTransferDoAfter));
    this.SubscribeLocalEvent<XenoPlasmaComponent, NewXenoEvolvedEvent>(new EntityEventRefHandler<XenoPlasmaComponent, NewXenoEvolvedEvent>(this.OnNewXenoEvolved));
    this.SubscribeLocalEvent<XenoPlasmaComponent, XenoDevolvedEvent>(new EntityEventRefHandler<XenoPlasmaComponent, XenoDevolvedEvent>(this.OnXenoDevolved));
    this.SubscribeLocalEvent<XenoActionPlasmaComponent, RMCActionUseAttemptEvent>(new EntityEventRefHandler<XenoActionPlasmaComponent, RMCActionUseAttemptEvent>(this.OnXenoActionEnergyUseAttempt));
    this.SubscribeLocalEvent<XenoActionPlasmaComponent, RMCActionUseEvent>(new EntityEventRefHandler<XenoActionPlasmaComponent, RMCActionUseEvent>(this.OnXenoActionEnergyUse));
  }

  private void OnXenoPlasmaMapInit(Entity<XenoPlasmaComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAlert(ent);
  }

  private void OnXenoPlasmaRemove(Entity<XenoPlasmaComponent> ent, ref ComponentRemove args)
  {
    this._alerts.ClearAlert((EntityUid) ent, ent.Comp.Alert);
  }

  private void OnXenoRejuvenate(Entity<XenoPlasmaComponent> xeno, ref RejuvenateEvent args)
  {
    this.RegenPlasma((Entity<XenoPlasmaComponent>) ((EntityUid) xeno, (XenoPlasmaComponent) xeno), (FixedPoint2) xeno.Comp.MaxPlasma);
  }

  private void OnXenoTransferPlasmaAction(
    Entity<XenoPlasmaComponent> xeno,
    ref XenoTransferPlasmaActionEvent args)
  {
    if (xeno.Owner == args.Target)
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-plasma-cannot-self"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    else if (this.HasComp<XenoAttachedOvipositorComponent>(args.Target))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-plasma-ovipositor"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    }
    else
    {
      XenoPlasmaComponent comp;
      if (!this.TryComp<XenoPlasmaComponent>(args.Target, out comp) || comp.MaxPlasma == 0)
      {
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-plasma-other-max-zero", ("target", (object) args.Target)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      }
      else
      {
        if (!this.HasPlasmaPopup((Entity<XenoPlasmaComponent>) ((EntityUid) xeno, (XenoPlasmaComponent) xeno), args.Amount))
          return;
        args.Handled = true;
        XenoTransferPlasmaDoAfterEvent @event = new XenoTransferPlasmaDoAfterEvent(args.Amount);
        this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.PlasmaTransferDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno), new EntityUid?(args.Target))
        {
          BreakOnMove = true,
          DistanceThreshold = new float?(args.Range),
          TargetEffect = (EntProtoId?) "RMCEffectHealPlasma"
        });
      }
    }
  }

  private void OnXenoTransferDoAfter(
    Entity<XenoPlasmaComponent> self,
    ref XenoTransferPlasmaDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    XenoPlasmaComponent comp;
    if (self.Owner == valueOrDefault || this.HasComp<XenoAttachedOvipositorComponent>(args.Target) || !this.TryComp<XenoPlasmaComponent>(valueOrDefault, out comp) || comp.Plasma == comp.MaxPlasma || !this.TryRemovePlasma((Entity<XenoPlasmaComponent>) ((EntityUid) self, (XenoPlasmaComponent) self), args.Amount))
      return;
    args.Handled = true;
    this.RegenPlasma((Entity<XenoPlasmaComponent>) valueOrDefault, args.Amount);
    this._jitter.DoJitter(valueOrDefault, TimeSpan.FromSeconds(1L), true, 80f, 8f, true);
    if (this._net.IsClient)
      return;
    this._popup.PopupEntity(this.Loc.GetString("cm-xeno-plasma-transferred-to-other", ("plasma", (object) args.Amount), ("target", (object) valueOrDefault), ("total", (object) self.Comp.Plasma)), (EntityUid) self, (EntityUid) self);
    this._popup.PopupEntity(this.Loc.GetString("cm-xeno-plasma-transferred-to-self", ("plasma", (object) args.Amount), ("target", (object) self.Owner), ("total", (object) comp.Plasma)), valueOrDefault, valueOrDefault);
    this._audio.PlayPredicted(self.Comp.PlasmaTransferSound, (EntityUid) self, new EntityUid?((EntityUid) self));
    if (!(comp.Plasma != comp.MaxPlasma))
      return;
    args.Repeat = true;
  }

  private void OnNewXenoEvolved(Entity<XenoPlasmaComponent> newXeno, ref NewXenoEvolvedEvent args)
  {
    this.EvolutionTransferPlasma((EntityUid) args.OldXeno, newXeno);
  }

  private void OnXenoDevolved(Entity<XenoPlasmaComponent> newXeno, ref XenoDevolvedEvent args)
  {
    this.EvolutionTransferPlasma(args.OldXeno, newXeno);
  }

  private void OnXenoActionEnergyUseAttempt(
    Entity<XenoActionPlasmaComponent> action,
    ref RMCActionUseAttemptEvent args)
  {
    if (args.Cancelled || this.HasPlasmaPopup((Entity<XenoPlasmaComponent>) args.User, (FixedPoint2) action.Comp.Cost))
      return;
    args.Cancelled = true;
  }

  private void OnXenoActionEnergyUse(
    Entity<XenoActionPlasmaComponent> action,
    ref RMCActionUseEvent args)
  {
    XenoPlasmaComponent comp;
    if (!this.TryComp<XenoPlasmaComponent>(args.User, out comp))
      return;
    this.RemovePlasma((Entity<XenoPlasmaComponent>) (args.User, comp), (FixedPoint2) action.Comp.Cost);
  }

  private void EvolutionTransferPlasma(EntityUid oldXeno, Entity<XenoPlasmaComponent> newXeno)
  {
    XenoPlasmaComponent comp;
    if (!this.TryComp<XenoPlasmaComponent>(oldXeno, out comp))
      return;
    FixedPoint2 maxPlasma = (FixedPoint2) newXeno.Comp.MaxPlasma;
    if (comp.MaxPlasma > 0)
      maxPlasma *= comp.Plasma / (float) comp.MaxPlasma;
    this.SetPlasma(newXeno, maxPlasma);
  }

  private void UpdateAlert(Entity<XenoPlasmaComponent> xeno)
  {
    if (xeno.Comp.MaxPlasma == 0)
      return;
    float actual = MathF.Max(0.0f, xeno.Comp.Plasma.Float());
    short maxSeverity = this._alerts.GetMaxSeverity(xeno.Comp.Alert);
    int num = (int) maxSeverity - ContentHelpers.RoundToLevels((double) actual, (double) xeno.Comp.MaxPlasma, (int) maxSeverity + 1);
    string str1 = $"{((int) xeno.Comp.Plasma).ToString()} / {xeno.Comp.MaxPlasma.ToString()}";
    AlertsSystem alerts = this._alerts;
    EntityUid euid = (EntityUid) xeno;
    ProtoId<AlertPrototype> alert = xeno.Comp.Alert;
    short? severity = new short?((short) num);
    string str2 = str1;
    (TimeSpan, TimeSpan)? cooldown = new (TimeSpan, TimeSpan)?();
    string dynamicMessage = str2;
    alerts.ShowAlert(euid, alert, severity, cooldown, dynamicMessage: dynamicMessage);
  }

  public bool HasPlasma(Entity<XenoPlasmaComponent> xeno, FixedPoint2 plasma)
  {
    return xeno.Comp.Plasma >= plasma;
  }

  public bool HasPlasmaPopup(Entity<XenoPlasmaComponent?> xeno, FixedPoint2 plasma, bool predicted = true)
  {
    if (!this.Resolve<XenoPlasmaComponent>((EntityUid) xeno, ref xeno.Comp, false))
    {
      DoPopup();
      return false;
    }
    if (this.HasPlasma((Entity<XenoPlasmaComponent>) ((EntityUid) xeno, xeno.Comp), plasma))
      return true;
    DoPopup();
    return false;

    void DoPopup()
    {
      string message = this.Loc.GetString("cm-xeno-not-enough-plasma");
      if (predicted)
        this._popup.PopupClient(message, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      else
        this._popup.PopupEntity(message, (EntityUid) xeno, (EntityUid) xeno);
    }
  }

  public void RegenPlasma(Entity<XenoPlasmaComponent?> xeno, FixedPoint2 amount)
  {
    if (!this._xenoPlasmaQuery.Resolve((EntityUid) xeno, ref xeno.Comp, false))
      return;
    FixedPoint2 plasma1 = xeno.Comp.Plasma;
    xeno.Comp.Plasma = FixedPoint2.Min(xeno.Comp.Plasma + amount, (FixedPoint2) xeno.Comp.MaxPlasma);
    FixedPoint2 plasma2 = xeno.Comp.Plasma;
    if (plasma1 == plasma2)
      return;
    this.Dirty<XenoPlasmaComponent>(xeno);
    this.UpdateAlert((Entity<XenoPlasmaComponent>) ((EntityUid) xeno, xeno.Comp));
  }

  public void RemovePlasma(Entity<XenoPlasmaComponent> xeno, FixedPoint2 plasma)
  {
    xeno.Comp.Plasma = FixedPoint2.Max(FixedPoint2.Zero, xeno.Comp.Plasma - plasma);
    this.Dirty<XenoPlasmaComponent>(xeno);
    this.UpdateAlert(xeno);
  }

  public void SetPlasma(Entity<XenoPlasmaComponent> xeno, FixedPoint2 plasma)
  {
    xeno.Comp.Plasma = plasma;
    this.Dirty<XenoPlasmaComponent>(xeno);
    this.UpdateAlert(xeno);
  }

  public bool TryRemovePlasma(Entity<XenoPlasmaComponent?> xeno, FixedPoint2 plasma)
  {
    if (!this.Resolve<XenoPlasmaComponent>((EntityUid) xeno, ref xeno.Comp, false) || !this.HasPlasma((Entity<XenoPlasmaComponent>) ((EntityUid) xeno, xeno.Comp), plasma))
      return false;
    this.RemovePlasma((Entity<XenoPlasmaComponent>) ((EntityUid) xeno, xeno.Comp), plasma);
    return true;
  }

  public bool TryRemovePlasmaPopup(
    Entity<XenoPlasmaComponent?> xeno,
    FixedPoint2 plasma,
    bool predicted = true)
  {
    if (!this.Resolve<XenoPlasmaComponent>((EntityUid) xeno, ref xeno.Comp, false))
      return false;
    if (this.TryRemovePlasma((Entity<XenoPlasmaComponent>) ((EntityUid) xeno, xeno.Comp), plasma))
      return true;
    if (predicted)
      this._popup.PopupClient(this.Loc.GetString("cm-xeno-not-enough-plasma"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    else
      this._popup.PopupEntity(this.Loc.GetString("cm-xeno-not-enough-plasma"), (EntityUid) xeno, (EntityUid) xeno);
    return false;
  }
}
