// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Systems.EyeClosingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Eye.Blinding.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Eye.Blinding.Systems;

public sealed class EyeClosingSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private BlindableSystem _blindableSystem;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private ISharedPlayerManager _playerManager;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EyeClosingComponent, MapInitEvent>(new EntityEventRefHandler<EyeClosingComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<EyeClosingComponent, ComponentShutdown>(new EntityEventRefHandler<EyeClosingComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<EyeClosingComponent, ToggleEyesActionEvent>(new EntityEventRefHandler<EyeClosingComponent, ToggleEyesActionEvent>(this.OnToggleAction));
    this.SubscribeLocalEvent<EyeClosingComponent, CanSeeAttemptEvent>(new EntityEventRefHandler<EyeClosingComponent, CanSeeAttemptEvent>(this.OnTrySee));
    this.SubscribeLocalEvent<EyeClosingComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<EyeClosingComponent, AfterAutoHandleStateEvent>(this.OnHandleState));
  }

  private void OnMapInit(Entity<EyeClosingComponent> eyelids, ref MapInitEvent args)
  {
    this._actionsSystem.AddAction((EntityUid) eyelids, ref eyelids.Comp.EyeToggleActionEntity, eyelids.Comp.EyeToggleAction);
    this.Dirty<EyeClosingComponent>(eyelids);
  }

  private void OnShutdown(Entity<EyeClosingComponent> eyelids, ref ComponentShutdown args)
  {
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> owner = (Entity<ActionsComponent>) eyelids.Owner;
    EntityUid? toggleActionEntity = eyelids.Comp.EyeToggleActionEntity;
    Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) toggleActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(owner, action);
    this.SetEyelids((Entity<EyeClosingComponent>) (eyelids.Owner, eyelids.Comp), false);
  }

  private void OnToggleAction(Entity<EyeClosingComponent> eyelids, ref ToggleEyesActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.SetEyelids((Entity<EyeClosingComponent>) (eyelids.Owner, eyelids.Comp), !eyelids.Comp.EyesClosed);
  }

  private void OnHandleState(
    Entity<EyeClosingComponent> eyelids,
    ref AfterAutoHandleStateEvent args)
  {
    this.DoAudioFeedback((Entity<EyeClosingComponent>) (eyelids.Owner, eyelids.Comp), eyelids.Comp.EyesClosed);
  }

  private void OnTrySee(Entity<EyeClosingComponent> eyelids, ref CanSeeAttemptEvent args)
  {
    if (!eyelids.Comp.EyesClosed)
      return;
    args.Cancel();
  }

  public bool AreEyesClosed(Entity<EyeClosingComponent?> eyelids)
  {
    return this.Resolve<EyeClosingComponent>((EntityUid) eyelids, ref eyelids.Comp, false) && eyelids.Comp.EyesClosed;
  }

  public void SetEyelids(Entity<EyeClosingComponent?> eyelids, bool value)
  {
    if (!this.Resolve<EyeClosingComponent>((EntityUid) eyelids, ref eyelids.Comp) || eyelids.Comp.EyesClosed == value)
      return;
    eyelids.Comp.EyesClosed = value;
    this.Dirty<EyeClosingComponent>(eyelids);
    if (eyelids.Comp.EyeToggleActionEntity.HasValue)
    {
      SharedActionsSystem actionsSystem = this._actionsSystem;
      EntityUid? toggleActionEntity = eyelids.Comp.EyeToggleActionEntity;
      Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) toggleActionEntity.GetValueOrDefault()) : new Entity<ActionComponent>?();
      int num = eyelids.Comp.EyesClosed ? 1 : 0;
      actionsSystem.SetToggled(action, num != 0);
    }
    this._blindableSystem.UpdateIsBlind((Entity<BlindableComponent>) eyelids.Owner);
    this.DoAudioFeedback(eyelids, eyelids.Comp.EyesClosed);
  }

  public void DoAudioFeedback(Entity<EyeClosingComponent?> eyelids, bool eyelidTarget)
  {
    if (!this.Resolve<EyeClosingComponent>((EntityUid) eyelids, ref eyelids.Comp) || !this._net.IsClient || !this._timing.IsFirstTimePredicted || eyelids.Comp.PreviousEyelidPosition == eyelidTarget)
      return;
    eyelids.Comp.PreviousEyelidPosition = eyelidTarget;
    ICommonSession session;
    if (!this._playerManager.TryGetSessionByEntity((EntityUid) eyelids, out session))
      return;
    this._audio.PlayGlobal(eyelidTarget ? eyelids.Comp.EyeCloseSound : eyelids.Comp.EyeOpenSound, session);
  }

  public void UpdateEyesClosable(Entity<BlindableComponent?> blindable)
  {
    if (!this.Resolve<BlindableComponent>((EntityUid) blindable, ref blindable.Comp, false))
      return;
    GetBlurEvent args = new GetBlurEvent((float) blindable.Comp.EyeDamage);
    this.RaiseLocalEvent<GetBlurEvent>(blindable.Owner, args);
    EyeClosingComponent comp;
    if (this.TryComp<EyeClosingComponent>((EntityUid) blindable, out comp) && !comp.NaturallyCreated)
      return;
    if ((double) args.Blur < 6.0 || (double) args.Blur >= (double) blindable.Comp.MaxDamage)
    {
      this.RemCompDeferred<EyeClosingComponent>((EntityUid) blindable);
    }
    else
    {
      this.EnsureComp<EyeClosingComponent>((EntityUid) blindable).NaturallyCreated = true;
      this.Dirty<BlindableComponent>(blindable);
    }
  }
}
