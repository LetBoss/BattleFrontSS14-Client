// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.ShakeStun.StunShakeableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stamina;
using Content.Shared._RMC14.Standing;
using Content.Shared._RMC14.Tackle;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.ShakeStun;

public sealed class StunShakeableSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogs;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCStandingSystem _rmcStanding;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private IGameTiming _timing;
  private static readonly ProtoId<StatusEffectPrototype> Stun = (ProtoId<StatusEffectPrototype>) nameof (Stun);
  private static readonly ProtoId<StatusEffectPrototype> KnockedDown = (ProtoId<StatusEffectPrototype>) nameof (KnockedDown);
  private static readonly ProtoId<StatusEffectPrototype> Unconscious = (ProtoId<StatusEffectPrototype>) nameof (Unconscious);

  public override void Initialize()
  {
    this.SubscribeLocalEvent<StunShakeableComponent, InteractHandEvent>(new EntityEventRefHandler<StunShakeableComponent, InteractHandEvent>(this.OnStunShakeableInteractHand), new Type[1]
    {
      typeof (InteractionPopupSystem)
    });
  }

  private void OnStunShakeableInteractHand(
    Entity<StunShakeableComponent> ent,
    ref InteractHandEvent args)
  {
    if (args.Handled)
      return;
    EntityUid user = args.User;
    StunShakeableUserComponent comp1;
    if (user == args.Target || !this.TryComp<StunShakeableUserComponent>(user, out comp1))
      return;
    EntityUid target = args.Target;
    RMCRestComponent rmcRestComponent = this.CompOrNull<RMCRestComponent>(target);
    if (!this._statusEffects.HasStatusEffect(target, (string) StunShakeableSystem.Stun) && !this._statusEffects.HasStatusEffect(target, (string) StunShakeableSystem.KnockedDown) && !this._statusEffects.HasStatusEffect(target, (string) StunShakeableSystem.Unconscious) && !this.HasComp<TackledRecentlyByComponent>(target) && (rmcRestComponent == null || !rmcRestComponent.Resting))
      return;
    args.Handled = true;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < comp1.LastShake + comp1.Cooldown)
      return;
    comp1.LastShake = curTime;
    this.Dirty(user, (IComponent) comp1);
    RMCStaminaComponent comp2;
    if (this.TryComp<RMCStaminaComponent>((EntityUid) ent, out comp2) && comp2.Level >= 4)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-shake-awake-stamina", ("target", (object) target)), target, new EntityUid?(user));
    }
    else
    {
      this._rmcStanding.SetRest((Entity<RMCRestComponent>) target, false);
      this._statusEffects.TryRemoveTime(target, (string) StunShakeableSystem.Stun, ent.Comp.DurationRemoved);
      this._statusEffects.TryRemoveTime(target, (string) StunShakeableSystem.KnockedDown, ent.Comp.DurationRemoved);
      this._statusEffects.TryRemoveTime(target, (string) StunShakeableSystem.Unconscious, ent.Comp.DurationRemoved);
      this.RemCompDeferred<TackledRecentlyByComponent>(target);
      this._popup.PopupClient(this.Loc.GetString("rmc-shake-awake-user", ("target", (object) target)), target, new EntityUid?(user));
      this._popup.PopupEntity(this.Loc.GetString("rmc-shake-awake-target", ("user", (object) user)), target, target);
      if (this._net.IsServer)
        this._audio.PlayEntity(ent.Comp.ShakeSound, Filter.Pvs(target), target, false);
      string message = this.Loc.GetString("rmc-shake-awake-others", ("user", (object) user), ("target", (object) target));
      Filter filter = Filter.PvsExcept(target).RemovePlayerByAttachedEntity(user);
      this._popup.PopupEntity(message, target, filter, true);
      ISharedAdminLogManager adminLogs = this._adminLogs;
      LogStringHandler logStringHandler = new LogStringHandler(22, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" shook ");
      logStringHandler.AppendFormatted<EntityUid>(target, "target");
      logStringHandler.AppendLiteral(" out of a stun.");
      ref LogStringHandler local = ref logStringHandler;
      adminLogs.Add(LogType.RMCStunShake, ref local);
    }
  }
}
