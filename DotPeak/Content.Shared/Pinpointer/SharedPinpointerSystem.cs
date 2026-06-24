// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pinpointer.SharedPinpointerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Shared.Pinpointer;

public abstract class SharedPinpointerSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private EmagSystem _emag;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PinpointerComponent, GotEmaggedEvent>(new ComponentEventRefHandler<PinpointerComponent, GotEmaggedEvent>(this.OnEmagged));
    this.SubscribeLocalEvent<PinpointerComponent, AfterInteractEvent>(new ComponentEventHandler<PinpointerComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<PinpointerComponent, ExaminedEvent>(new ComponentEventHandler<PinpointerComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnAfterInteract(
    EntityUid uid,
    PinpointerComponent component,
    AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    target.GetValueOrDefault();
    if (!component.CanRetarget || component.IsActive)
      return;
    args.Handled = true;
    component.Target = args.Target;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(19, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "player", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" set target of ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "pinpointer", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) component.Target.Value), "target", "ToPrettyString(component.Target.Value)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    if (!component.UpdateTargetName)
      return;
    component.TargetName = !component.Target.HasValue ? (string) null : (string) Identity.Name(component.Target.Value, (IEntityManager) this.EntityManager);
  }

  public virtual void SetTarget(EntityUid uid, EntityUid? target, PinpointerComponent? pinpointer = null)
  {
    if (!this.Resolve<PinpointerComponent>(uid, ref pinpointer))
      return;
    EntityUid? target1 = pinpointer.Target;
    EntityUid? nullable = target;
    if ((target1.HasValue == nullable.HasValue ? (target1.HasValue ? (target1.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    pinpointer.Target = target;
    if (pinpointer.UpdateTargetName)
      pinpointer.TargetName = !target.HasValue ? (string) null : (string) Identity.Name(target.Value, (IEntityManager) this.EntityManager);
    if (!pinpointer.IsActive)
      return;
    this.UpdateDirectionToTarget(uid, pinpointer);
  }

  protected virtual void UpdateDirectionToTarget(EntityUid uid, PinpointerComponent? pinpointer = null)
  {
  }

  private void OnExamined(EntityUid uid, PinpointerComponent component, ExaminedEvent args)
  {
    if (!args.IsInDetailsRange || component.TargetName == null)
      return;
    args.PushMarkup(this.Loc.GetString("examine-pinpointer-linked", ("target", (object) component.TargetName)));
  }

  public void SetDistance(EntityUid uid, Distance distance, PinpointerComponent? pinpointer = null)
  {
    if (!this.Resolve<PinpointerComponent>(uid, ref pinpointer) || distance == pinpointer.DistanceToTarget)
      return;
    pinpointer.DistanceToTarget = distance;
    this.Dirty(uid, (IComponent) pinpointer);
  }

  public bool TrySetArrowAngle(EntityUid uid, Angle arrowAngle, PinpointerComponent? pinpointer = null)
  {
    if (!this.Resolve<PinpointerComponent>(uid, ref pinpointer) || ((Angle) ref pinpointer.ArrowAngle).EqualsApprox(arrowAngle, pinpointer.Precision))
      return false;
    pinpointer.ArrowAngle = arrowAngle;
    this.Dirty(uid, (IComponent) pinpointer);
    return true;
  }

  public void SetActive(EntityUid uid, bool isActive, PinpointerComponent? pinpointer = null)
  {
    if (!this.Resolve<PinpointerComponent>(uid, ref pinpointer) || isActive == pinpointer.IsActive)
      return;
    pinpointer.IsActive = isActive;
    this.Dirty(uid, (IComponent) pinpointer);
  }

  public virtual bool TogglePinpointer(EntityUid uid, PinpointerComponent? pinpointer = null)
  {
    if (!this.Resolve<PinpointerComponent>(uid, ref pinpointer))
      return false;
    bool isActive = !pinpointer.IsActive;
    this.SetActive(uid, isActive, pinpointer);
    return isActive;
  }

  private void OnEmagged(EntityUid uid, PinpointerComponent component, ref GotEmaggedEvent args)
  {
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || this._emag.CheckFlag(uid, EmagType.Interaction) || component.CanRetarget)
      return;
    args.Handled = true;
    component.CanRetarget = true;
  }
}
