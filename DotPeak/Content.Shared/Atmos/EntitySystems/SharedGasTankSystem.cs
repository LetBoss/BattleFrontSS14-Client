// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.SharedGasTankSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Examine;
using Content.Shared.Timing;
using Content.Shared.Toggleable;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedGasTankSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private SharedInternalsSystem _internals;
  [Dependency]
  protected SharedUserInterfaceSystem UI;
  [Dependency]
  private UseDelaySystem _delay;
  public const string GasTankDelay = "gasTank";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTankComponent, ComponentShutdown>(new EntityEventRefHandler<GasTankComponent, ComponentShutdown>((object) this, __methodptr(OnGasShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTankComponent, BeforeActivatableUIOpenEvent>(new EntityEventRefHandler<GasTankComponent, BeforeActivatableUIOpenEvent>((object) this, __methodptr(BeforeUiOpen)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTankComponent, GetItemActionsEvent>(new ComponentEventHandler<GasTankComponent, GetItemActionsEvent>((object) this, __methodptr(OnGetActions)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTankComponent, ExaminedEvent>(new ComponentEventHandler<GasTankComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTankComponent, ToggleActionEvent>(new EntityEventRefHandler<GasTankComponent, ToggleActionEvent>((object) this, __methodptr(OnActionToggle)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTankComponent, GasTankSetPressureMessage>(new EntityEventRefHandler<GasTankComponent, GasTankSetPressureMessage>((object) this, __methodptr(OnGasTankSetPressure)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTankComponent, GasTankToggleInternalsMessage>(new EntityEventRefHandler<GasTankComponent, GasTankToggleInternalsMessage>((object) this, __methodptr(OnGasTankToggleInternals)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GasTankComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<GasTankComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(OnGetAlternativeVerb)), (Type[]) null, (Type[]) null);
  }

  private void OnGasShutdown(Entity<GasTankComponent> gasTank, ref ComponentShutdown args)
  {
    this.DisconnectFromInternals(gasTank);
  }

  private void OnGasTankToggleInternals(
    Entity<GasTankComponent> ent,
    ref GasTankToggleInternalsMessage args)
  {
    this.ToggleInternals(ent, new EntityUid?(((BaseBoundUserInterfaceEvent) args).Actor));
  }

  private void OnGasTankSetPressure(
    Entity<GasTankComponent> ent,
    ref GasTankSetPressureMessage args)
  {
    float num = Math.Clamp(args.Pressure, 0.0f, ent.Comp.MaxOutputPressure);
    ent.Comp.OutputPressure = num;
    this.Dirty<GasTankComponent>(ent, (MetaDataComponent) null);
    this.UpdateUserInterface(ent);
  }

  public virtual void UpdateUserInterface(Entity<GasTankComponent> ent)
  {
  }

  private void BeforeUiOpen(Entity<GasTankComponent> ent, ref BeforeActivatableUIOpenEvent args)
  {
    this.UpdateUserInterface(ent);
  }

  private void OnGetActions(EntityUid uid, GasTankComponent component, GetItemActionsEvent args)
  {
    args.AddAction(ref component.ToggleActionEntity, EntProtoId.op_Implicit(component.ToggleAction));
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }

  private void OnExamined(EntityUid uid, GasTankComponent component, ExaminedEvent args)
  {
    using (args.PushGroup("GasTankComponent"))
    {
      if (args.IsInDetailsRange)
      {
        ExaminedEvent examinedEvent = args;
        ILocalizationManager loc = this.Loc;
        GasMixture air = component.Air;
        (string, object) valueTuple = ("pressure", (object) Math.Round(air != null ? (double) air.Pressure : 0.0));
        string markup = loc.GetString("comp-gas-tank-examine", valueTuple);
        examinedEvent.PushMarkup(markup);
      }
      if (component.IsConnected)
        args.PushMarkup(this.Loc.GetString("comp-gas-tank-connected"));
      args.PushMarkup(this.Loc.GetString(component.IsValveOpen ? "comp-gas-tank-examine-open-valve" : "comp-gas-tank-examine-closed-valve"));
    }
  }

  private void OnActionToggle(Entity<GasTankComponent> gasTank, ref ToggleActionEvent args)
  {
    if (args.Handled)
      return;
    this.ToggleInternals(gasTank, new EntityUid?(args.Performer));
    args.Handled = true;
  }

  private void OnGetAlternativeVerb(
    EntityUid uid,
    GasTankComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = component.IsValveOpen ? this.Loc.GetString("comp-gas-tank-close-valve") : this.Loc.GetString("comp-gas-tank-open-valve");
    alternativeVerb.Act = (Action) (() =>
    {
      component.IsValveOpen = !component.IsValveOpen;
      this._audio.PlayPredicted(component.ValveSound, uid, new EntityUid?(args.User), new AudioParams?());
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    });
    alternativeVerb.Disabled = component.IsConnected;
    verbs.Add(alternativeVerb);
  }

  public bool CanConnectToInternals(Entity<GasTankComponent> ent)
  {
    InternalsComponent internalsComp;
    this.TryGetInternalsComp(ent, out EntityUid? _, out internalsComp, ent.Comp.User);
    return internalsComp != null && internalsComp.BreathTools.Count != 0 && !ent.Comp.IsValveOpen;
  }

  public bool ConnectToInternals(Entity<GasTankComponent> ent, EntityUid? user = null)
  {
    EntityUid entityUid;
    GasTankComponent gasTankComponent1;
    ent.Deconstruct(ref entityUid, ref gasTankComponent1);
    EntityUid tankEntity = entityUid;
    GasTankComponent gasTankComponent2 = gasTankComponent1;
    if (gasTankComponent2.IsConnected || !this.CanConnectToInternals(ent))
      return false;
    EntityUid? internalsUid;
    InternalsComponent internalsComp;
    this.TryGetInternalsComp(ent, out internalsUid, out internalsComp, ent.Comp.User);
    if (!internalsUid.HasValue || internalsComp == null || !this._delay.TryResetDelay(ent.Owner, true, id: "gasTank"))
      return false;
    if (this._internals.TryConnectTank(Entity<InternalsComponent>.op_Implicit((internalsUid.Value, internalsComp)), tankEntity))
      gasTankComponent2.User = new EntityUid?(internalsUid.Value);
    this.Dirty<GasTankComponent>(ent, (MetaDataComponent) null);
    SharedActionsSystem actions1 = this._actions;
    EntityUid? nullable1 = gasTankComponent2.ToggleActionEntity;
    Entity<ActionComponent>? action1 = nullable1.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable1.GetValueOrDefault())) : new Entity<ActionComponent>?();
    int num = gasTankComponent2.IsConnected ? 1 : 0;
    actions1.SetToggled(action1, num != 0);
    SharedActionsSystem actions2 = this._actions;
    nullable1 = gasTankComponent2.ToggleActionEntity;
    Entity<ActionComponent>? action2 = nullable1.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable1.GetValueOrDefault())) : new Entity<ActionComponent>?();
    TimeSpan cooldown = TimeSpan.FromSeconds(1L);
    actions2.SetCooldown(action2, cooldown);
    if (!gasTankComponent2.IsConnected)
      return false;
    gasTankComponent2.ConnectStream = this._audio.Stop(gasTankComponent2.ConnectStream, (AudioComponent) null);
    GasTankComponent gasTankComponent3 = gasTankComponent2;
    (EntityUid, AudioComponent)? nullable2 = this._audio.PlayPredicted(gasTankComponent2.ConnectSound, tankEntity, user, new AudioParams?());
    ref (EntityUid, AudioComponent)? local = ref nullable2;
    EntityUid? nullable3;
    if (!local.HasValue)
    {
      nullable1 = new EntityUid?();
      nullable3 = nullable1;
    }
    else
      nullable3 = new EntityUid?(local.GetValueOrDefault().Item1);
    gasTankComponent3.ConnectStream = nullable3;
    this.UpdateUserInterface(ent);
    return true;
  }

  private bool TryGetInternalsComp(
    Entity<GasTankComponent> ent,
    out EntityUid? internalsUid,
    out InternalsComponent? internalsComp,
    EntityUid? user = null)
  {
    internalsUid = new EntityUid?();
    internalsComp = (InternalsComponent) null;
    if (this.TerminatingOrDeleted(ent.Owner, (MetaDataComponent) null))
      return false;
    if (!user.HasValue)
      user = ent.Comp.User;
    InternalsComponent internalsComponent1;
    if (this.TryComp<InternalsComponent>(user, ref internalsComponent1))
    {
      internalsUid = user;
      internalsComp = internalsComponent1;
      return true;
    }
    BaseContainer baseContainer;
    InternalsComponent internalsComponent2;
    if (!this._containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ent.Owner, this.Transform(ent.Owner))), ref baseContainer) || !this.TryComp<InternalsComponent>(baseContainer.Owner, ref internalsComponent2))
      return false;
    internalsUid = new EntityUid?(baseContainer.Owner);
    internalsComp = internalsComponent2;
    return true;
  }

  public bool DisconnectFromInternals(Entity<GasTankComponent> ent, EntityUid? user = null, bool forced = false)
  {
    EntityUid entityUid1;
    GasTankComponent gasTankComponent1;
    ent.Deconstruct(ref entityUid1, ref gasTankComponent1);
    EntityUid entityUid2 = entityUid1;
    GasTankComponent gasTankComponent2 = gasTankComponent1;
    if (!gasTankComponent2.User.HasValue || !forced && !this._delay.TryResetDelay(ent.Owner, true, id: "gasTank"))
      return false;
    EntityUid? internalsUid;
    InternalsComponent internalsComp;
    this.TryGetInternalsComp(ent, out internalsUid, out internalsComp, gasTankComponent2.User);
    gasTankComponent2.User = new EntityUid?();
    this.Dirty<GasTankComponent>(ent, (MetaDataComponent) null);
    SharedActionsSystem actions1 = this._actions;
    EntityUid? nullable1 = gasTankComponent2.ToggleActionEntity;
    Entity<ActionComponent>? action1 = nullable1.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable1.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actions1.SetToggled(action1, false);
    UseDelayInfo info;
    if (!forced && this._delay.TryGetDelayInfo(Entity<UseDelayComponent>.op_Implicit(ent.Owner), out info, "gasTank"))
    {
      SharedActionsSystem actions2 = this._actions;
      nullable1 = gasTankComponent2.ToggleActionEntity;
      Entity<ActionComponent>? action2 = nullable1.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable1.GetValueOrDefault())) : new Entity<ActionComponent>?();
      TimeSpan length = info.Length;
      actions2.SetCooldown(action2, length);
    }
    if (internalsUid.HasValue && internalsComp != null)
      this._internals.DisconnectTank(Entity<InternalsComponent>.op_Implicit((internalsUid.Value, internalsComp)), forced);
    gasTankComponent2.DisconnectStream = this._audio.Stop(gasTankComponent2.DisconnectStream, (AudioComponent) null);
    GasTankComponent gasTankComponent3 = gasTankComponent2;
    (EntityUid, AudioComponent)? nullable2 = this._audio.PlayPredicted(gasTankComponent2.DisconnectSound, entityUid2, user, new AudioParams?());
    ref (EntityUid, AudioComponent)? local = ref nullable2;
    EntityUid? nullable3;
    if (!local.HasValue)
    {
      nullable1 = new EntityUid?();
      nullable3 = nullable1;
    }
    else
      nullable3 = new EntityUid?(local.GetValueOrDefault().Item1);
    gasTankComponent3.DisconnectStream = nullable3;
    this.UpdateUserInterface(ent);
    return true;
  }

  private bool ToggleInternals(Entity<GasTankComponent> ent, EntityUid? user = null)
  {
    return ent.Comp.IsConnected ? this.DisconnectFromInternals(ent, user) : this.ConnectToInternals(ent, user);
  }
}
