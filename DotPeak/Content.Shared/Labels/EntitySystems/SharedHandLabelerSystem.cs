// Decompiled with JetBrains decompiler
// Type: Content.Shared.Labels.EntitySystems.SharedHandLabelerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Interaction;
using Content.Shared.Labels.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Shared.Labels.EntitySystems;

public abstract class SharedHandLabelerSystem : EntitySystem
{
  [Dependency]
  protected SharedUserInterfaceSystem UserInterfaceSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private LabelSystem _labelSystem;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<HandLabelerComponent, AfterInteractEvent>(new ComponentEventHandler<HandLabelerComponent, AfterInteractEvent>(this.AfterInteractOn));
    this.SubscribeLocalEvent<HandLabelerComponent, GetVerbsEvent<UtilityVerb>>(new ComponentEventHandler<HandLabelerComponent, GetVerbsEvent<UtilityVerb>>(this.OnUtilityVerb));
    this.SubscribeLocalEvent<HandLabelerComponent, HandLabelerLabelChangedMessage>(new ComponentEventHandler<HandLabelerComponent, HandLabelerLabelChangedMessage>(this.OnHandLabelerLabelChanged));
    this.SubscribeLocalEvent<HandLabelerComponent, ComponentGetState>(new EntityEventRefHandler<HandLabelerComponent, ComponentGetState>(this.OnGetState));
    this.SubscribeLocalEvent<HandLabelerComponent, ComponentHandleState>(new EntityEventRefHandler<HandLabelerComponent, ComponentHandleState>(this.OnHandleState));
  }

  private void OnGetState(Entity<HandLabelerComponent> ent, ref ComponentGetState args)
  {
    args.State = (IComponentState) new HandLabelerComponentState(ent.Comp.AssignedLabel)
    {
      MaxLabelChars = ent.Comp.MaxLabelChars
    };
  }

  private void OnHandleState(Entity<HandLabelerComponent> ent, ref ComponentHandleState args)
  {
    if (!(args.Current is HandLabelerComponentState current))
      return;
    ent.Comp.MaxLabelChars = current.MaxLabelChars;
    if (ent.Comp.AssignedLabel == current.AssignedLabel)
      return;
    ent.Comp.AssignedLabel = current.AssignedLabel;
    this.UpdateUI(ent);
  }

  protected virtual void UpdateUI(Entity<HandLabelerComponent> ent)
  {
  }

  private void AddLabelTo(
    EntityUid uid,
    HandLabelerComponent? handLabeler,
    EntityUid target,
    out string? result)
  {
    if (!this.Resolve<HandLabelerComponent>(uid, ref handLabeler))
      result = (string) null;
    else if (handLabeler.AssignedLabel == string.Empty)
    {
      if (this._netManager.IsServer)
        this._labelSystem.Label(target, (string) null);
      result = this.Loc.GetString("hand-labeler-successfully-removed");
    }
    else
    {
      if (this._netManager.IsServer)
        this._labelSystem.Label(target, handLabeler.AssignedLabel);
      result = this.Loc.GetString("hand-labeler-successfully-applied");
    }
  }

  private void OnUtilityVerb(
    EntityUid uid,
    HandLabelerComponent handLabeler,
    GetVerbsEvent<UtilityVerb> args)
  {
    EntityUid target = args.Target;
    if (!target.Valid || this._whitelistSystem.IsWhitelistFail(handLabeler.Whitelist, target) || !args.CanAccess)
      return;
    string str = handLabeler.AssignedLabel == string.Empty ? this.Loc.GetString("hand-labeler-remove-label-text") : this.Loc.GetString("hand-labeler-add-label-text");
    UtilityVerb utilityVerb1 = new UtilityVerb();
    utilityVerb1.Act = (Action) (() => this.Labeling(uid, target, args.User, handLabeler));
    utilityVerb1.Text = str;
    UtilityVerb utilityVerb2 = utilityVerb1;
    args.Verbs.Add(utilityVerb2);
  }

  private void AfterInteractOn(
    EntityUid uid,
    HandLabelerComponent handLabeler,
    AfterInteractEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (!valueOrDefault.Valid || this._whitelistSystem.IsWhitelistFail(handLabeler.Whitelist, valueOrDefault) || !args.CanReach)
      return;
    this.Labeling(uid, valueOrDefault, args.User, handLabeler);
  }

  private void Labeling(
    EntityUid uid,
    EntityUid target,
    EntityUid User,
    HandLabelerComponent handLabeler)
  {
    string result;
    this.AddLabelTo(uid, handLabeler, target, out result);
    if (result == null)
      return;
    this._popupSystem.PopupClient(result, User, new EntityUid?(User));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(15, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) User), "user", "ToPrettyString(User)");
    logStringHandler.AppendLiteral(" labeled ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral(" with ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "labeler", "ToPrettyString(uid)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
  }

  private void OnHandLabelerLabelChanged(
    EntityUid uid,
    HandLabelerComponent handLabeler,
    HandLabelerLabelChangedMessage args)
  {
    string str = args.Label.Trim();
    handLabeler.AssignedLabel = str.Substring(0, Math.Min(handLabeler.MaxLabelChars, str.Length));
    this.UpdateUI((Entity<HandLabelerComponent>) (uid, handLabeler));
    this.Dirty(uid, (IComponent) handLabeler);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(23, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "user", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" set ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "labeler", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" to apply label \"");
    logStringHandler.AppendFormatted(handLabeler.AssignedLabel);
    logStringHandler.AppendLiteral("\"");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
  }
}
