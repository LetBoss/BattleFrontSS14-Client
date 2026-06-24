// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.Scanner.HealthScannerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Temperature;
using Content.Shared.Chemistry.Components;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Timing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Medical.Scanner;

public sealed class HealthScannerSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedEntityStorageSystem _entityStorage;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCBloodstreamSystem _rmcBloodstream;
  [Dependency]
  private RMCHandsSystem _rmcHands;
  [Dependency]
  private SharedRMCTemperatureSystem _rmcTemperature;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private UseDelaySystem _useDelay;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<HealthScannerComponent, AfterInteractEvent>(new EntityEventRefHandler<HealthScannerComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<HealthScannerComponent, DoAfterAttemptEvent<HealthScannerDoAfterEvent>>(new EntityEventRefHandler<HealthScannerComponent, DoAfterAttemptEvent<HealthScannerDoAfterEvent>>(this.OnDoAfterAttempt));
    this.SubscribeLocalEvent<HealthScannerComponent, HealthScannerDoAfterEvent>(new EntityEventRefHandler<HealthScannerComponent, HealthScannerDoAfterEvent>(this.OnDoAfter));
  }

  private void OnAfterInteract(Entity<HealthScannerComponent> scanner, ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (!this.CanUseHealthScannerPopup(scanner, args.User, ref valueOrDefault))
      return;
    TimeSpan delay = this._skills.GetDelay(args.User, (EntityUid) scanner);
    HealthScannerDoAfterEvent @event = new HealthScannerDoAfterEvent();
    DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, args.User, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) scanner), new EntityUid?(valueOrDefault), new EntityUid?((EntityUid) scanner))
    {
      BreakOnMove = true,
      AttemptFrequency = AttemptFrequency.EveryTick
    };
    if (delay > TimeSpan.Zero)
      this._popup.PopupClient($"You start fumbling around with {this.Loc.GetString("zzzz-the", ("ent", (object) valueOrDefault))}...", valueOrDefault, new EntityUid?(args.User));
    this._doAfter.TryStartDoAfter(args1);
  }

  private void OnDoAfterAttempt(
    Entity<HealthScannerComponent> ent,
    ref DoAfterAttemptEvent<HealthScannerDoAfterEvent> args)
  {
    DoAfterArgs args1 = args.DoAfter.Args;
    EntityUid? target = args1.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (!this.CanUseHealthScannerPopup(ent, args1.User, ref valueOrDefault))
    {
      args.Cancel();
    }
    else
    {
      if (this._transform.InRange(this.Transform(args1.User).Coordinates, args.DoAfter.UserPosition, args1.MovementThreshold))
        return;
      args.Cancel();
    }
  }

  private void OnDoAfter(Entity<HealthScannerComponent> scanner, ref HealthScannerDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    args.Handled = true;
    UseDelayComponent comp;
    if (this.TryComp<UseDelayComponent>((EntityUid) scanner, out comp))
      this._useDelay.TryResetDelay((Entity<UseDelayComponent>) ((EntityUid) scanner, comp));
    scanner.Comp.Target = new EntityUid?(valueOrDefault);
    this.Dirty<HealthScannerComponent>(scanner);
    this._audio.PlayPredicted(scanner.Comp.Sound, (EntityUid) scanner, new EntityUid?(args.User));
    this._ui.OpenUi((Entity<UserInterfaceComponent>) scanner.Owner, (Enum) HealthScannerUIKey.Key, new EntityUid?(args.User));
    this.UpdateUI(scanner);
  }

  private bool CanUseHealthScannerPopup(
    Entity<HealthScannerComponent> scanner,
    EntityUid user,
    ref EntityUid target)
  {
    SharedEntityStorageComponent component = (SharedEntityStorageComponent) null;
    if (this.HasComp<HealthScannableContainerComponent>(target) && this._entityStorage.ResolveStorage(target, ref component))
    {
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) component.Contents.ContainedEntities)
      {
        if (this.HasComp<DamageableComponent>(containedEntity) && this.HasComp<MobStateComponent>(containedEntity) && this.HasComp<MobThresholdsComponent>(containedEntity))
        {
          target = containedEntity;
          break;
        }
      }
    }
    if (!this.HasComp<DamageableComponent>(target) || !this.HasComp<MobStateComponent>(target) || !this.HasComp<MobThresholdsComponent>(target))
    {
      this._popup.PopupClient("You can't analyze that!", target, new EntityUid?(user));
      return false;
    }
    UseDelayComponent comp;
    if (this.TryComp<UseDelayComponent>((EntityUid) scanner, out comp) && this._useDelay.IsDelayed((Entity<UseDelayComponent>) ((EntityUid) scanner, comp)))
      return false;
    HealthScannerAttemptTargetEvent args = new HealthScannerAttemptTargetEvent();
    this.RaiseLocalEvent<HealthScannerAttemptTargetEvent>(target, ref args);
    if (!args.Cancelled)
      return true;
    if (args.Popup != null)
      this._popup.PopupClient(args.Popup, target, new EntityUid?(user));
    return false;
  }

  private void UpdateUI(Entity<HealthScannerComponent> scanner)
  {
    EntityUid? target = scanner.Comp.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (this.TerminatingOrDeleted(valueOrDefault))
    {
      if (!this.TerminatingOrDeleted((EntityUid) scanner))
        this._ui.CloseUi((Entity<UserInterfaceComponent>) scanner.Owner, (Enum) HealthScannerUIKey.Key);
      scanner.Comp.Target = new EntityUid?();
    }
    else
    {
      if (!this._rmcHands.TryGetHolder((EntityUid) scanner, out EntityUid _))
        return;
      FixedPoint2 blood = (FixedPoint2) 0;
      FixedPoint2 maxBlood = (FixedPoint2) 0;
      Solution solution1;
      if (this._rmcBloodstream.TryGetBloodSolution(valueOrDefault, out solution1))
      {
        blood = solution1.Volume;
        maxBlood = solution1.MaxVolume;
      }
      Solution solution2;
      this._rmcBloodstream.TryGetChemicalSolution(valueOrDefault, out Entity<SolutionComponent> _, out solution2);
      float temperature;
      this._rmcTemperature.TryGetCurrentTemperature(valueOrDefault, out temperature);
      bool bleeding = this._rmcBloodstream.IsBleeding(valueOrDefault);
      HealthScannerBuiState state = new HealthScannerBuiState(this.GetNetEntity(valueOrDefault), blood, maxBlood, new float?(temperature), solution2, bleeding);
      this._ui.SetUiState((Entity<UserInterfaceComponent>) scanner.Owner, (Enum) HealthScannerUIKey.Key, (BoundUserInterfaceState) state);
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<HealthScannerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HealthScannerComponent>();
    EntityUid uid;
    HealthScannerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.UpdateAt))
      {
        comp1.UpdateAt = curTime + comp1.UpdateCooldown;
        this.UpdateUI((Entity<HealthScannerComponent>) (uid, comp1));
      }
    }
  }
}
