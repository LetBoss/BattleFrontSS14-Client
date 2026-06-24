// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Mobs.RMCAdminGhostDragPossessionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dialog;
using Content.Shared.Administration;
using Content.Shared.Administration.Logs;
using Content.Shared.Administration.Managers;
using Content.Shared.Database;
using Content.Shared.DragDrop;
using Content.Shared.Mind;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Mobs;

public sealed class RMCAdminGhostDragPossessionSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private ISharedAdminManager _adminManager;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private SharedMindSystem _mind;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<CMGhostComponent, CanDragEvent>(new EntityEventRefHandler<CMGhostComponent, CanDragEvent>(this.OnCanDrag));
    this.SubscribeLocalEvent<CMGhostComponent, CanDropDraggedEvent>(new EntityEventRefHandler<CMGhostComponent, CanDropDraggedEvent>(this.OnCanDropDragged));
    this.SubscribeLocalEvent<CMGhostComponent, DragDropDraggedEvent>(new EntityEventRefHandler<CMGhostComponent, DragDropDraggedEvent>(this.OnGhostDraggedDropped));
    this.SubscribeLocalEvent<CMGhostComponent, CanDropTargetEvent>(new EntityEventRefHandler<CMGhostComponent, CanDropTargetEvent>(this.OnCanDropTarget));
    this.SubscribeLocalEvent<CMGhostComponent, GhostPossessionConfirmEvent>(new EntityEventRefHandler<CMGhostComponent, GhostPossessionConfirmEvent>(this.OnPossessionConfirmation));
  }

  private void OnCanDrag(Entity<CMGhostComponent> ent, ref CanDragEvent args)
  {
    args.Handled = true;
  }

  private void OnCanDropDragged(Entity<CMGhostComponent> ent, ref CanDropDraggedEvent args)
  {
    if (!this.Exists((EntityUid) ent) || !this._adminManager.HasAdminFlag(args.User, AdminFlags.Fun))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnCanDropTarget(Entity<CMGhostComponent> ent, ref CanDropTargetEvent args)
  {
    if (!this.Exists((EntityUid) ent) || !this._adminManager.HasAdminFlag(args.User, AdminFlags.Fun))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnGhostDraggedDropped(Entity<CMGhostComponent> ent, ref DragDropDraggedEvent args)
  {
    if (!this.Exists((EntityUid) ent) || !this._adminManager.HasAdminFlag(args.User, AdminFlags.Fun) || ent.Owner == args.Target)
      return;
    args.Handled = true;
    GhostPossessionConfirmEvent ev = new GhostPossessionConfirmEvent(this.GetNetEntity(args.User), this.GetNetEntity((EntityUid) ent), this.GetNetEntity(args.Target));
    this._dialog.OpenConfirmation(args.User, "Are you sure?", $"Are you sure you want [Bold][Italic]{this.MetaData((EntityUid) ent).EntityName} | {ent.Owner.Id}[/Bold][/Italic] to possess [Bold][Italic]{this.MetaData(args.Target).EntityName} | {args.Target.Id}[/Bold][/Italic]", (object) ev);
  }

  private void OnPossessionConfirmation(
    Entity<CMGhostComponent> ent,
    ref GhostPossessionConfirmEvent args)
  {
    EntityUid entity1 = this.GetEntity(args.Actor);
    EntityUid entity2 = this.GetEntity(args.Possessor);
    EntityUid entity3 = this.GetEntity(args.ToPossess);
    this._mind.ControlMob(entity2, entity3);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(24, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity1), "player", "ToPrettyString(actor)");
    logStringHandler.AppendLiteral(" has forced ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity2), "entity", "ToPrettyString(possessor)");
    logStringHandler.AppendLiteral(" to possess ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity3), "player", "ToPrettyString(toPossess)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCAdminCommandLogging, LogImpact.High, ref local);
  }
}
