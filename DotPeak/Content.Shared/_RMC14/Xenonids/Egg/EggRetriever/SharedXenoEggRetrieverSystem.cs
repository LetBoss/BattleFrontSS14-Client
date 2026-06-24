// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Egg.EggRetriever.SharedXenoEggRetrieverSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Egg.EggRetriever;

public abstract class SharedXenoEggRetrieverSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  protected SharedAppearanceSystem _appearance;
  [Dependency]
  private INetManager _net;
  [Dependency]
  protected SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoEggRetrieverComponent, ExaminedEvent>(new EntityEventRefHandler<XenoEggRetrieverComponent, ExaminedEvent>(this.OnEggRetrieverExamine));
    this.SubscribeLocalEvent<XenoGenerateEggsComponent, XenoGenerateEggsActionEvent>(new EntityEventRefHandler<XenoGenerateEggsComponent, XenoGenerateEggsActionEvent>(this.OnXenoProduceEggsAction));
    this.SubscribeLocalEvent<XenoGenerateEggsComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoGenerateEggsComponent, MobStateChangedEvent>(this.OnXenoProduceEggsDeath));
  }

  private void OnEggRetrieverExamine(
    Entity<XenoEggRetrieverComponent> retriever,
    ref ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    using (args.PushGroup("XenoEggRetrieverComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-xeno-retrieve-egg-current", ("xeno", (object) retriever), ("cur_eggs", (object) retriever.Comp.CurEggs), ("max_eggs", (object) retriever.Comp.MaxEggs)));
  }

  private void OnXenoProduceEggsAction(
    Entity<XenoGenerateEggsComponent> xeno,
    ref XenoGenerateEggsActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((InstantActionEvent) args))
      return;
    args.Handled = true;
    this.ToggleProduceEggs((EntityUid) xeno, xeno.Comp);
    if (!xeno.Comp.Active)
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-produce-eggs-start"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
  }

  protected void ToggleProduceEggs(EntityUid xeno, XenoGenerateEggsComponent produce)
  {
    if (produce.Active && this._net.IsServer)
    {
      produce.NextDrain = new TimeSpan?();
      produce.NextEgg = new TimeSpan?();
    }
    produce.Active = !produce.Active;
    this._appearance.SetData(xeno, (Enum) XenoEggStorageVisuals.Active, (object) produce.Active);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoGenerateEggsActionEvent>(xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), produce.Active);
    this.Dirty(xeno, (IComponent) produce);
  }

  private void OnXenoProduceEggsDeath(
    Entity<XenoGenerateEggsComponent> xeno,
    ref MobStateChangedEvent args)
  {
    if (this._timing.ApplyingState || args.NewMobState != MobState.Dead || !xeno.Comp.Active)
      return;
    this.ToggleProduceEggs((EntityUid) xeno, xeno.Comp);
  }
}
