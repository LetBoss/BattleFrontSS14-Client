// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Access.IdCardSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines;
using Content.Shared.Access.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Access;

public sealed class IdCardSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<IdCardComponent, UseInHandEvent>(new EntityEventRefHandler<IdCardComponent, UseInHandEvent>(this.OnUseInHand), new Type[1]
    {
      typeof (ClothingSystem)
    });
    this.SubscribeLocalEvent<IdCardComponent, ExaminedEvent>(new EntityEventRefHandler<IdCardComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<MarineComponent, InteractUsingEvent>(new EntityEventRefHandler<MarineComponent, InteractUsingEvent>(this.OnInteractUsing));
  }

  private void OnInteractUsing(Entity<MarineComponent> ent, ref InteractUsingEvent args)
  {
    IdCardComponent comp;
    if (args.Handled || !this.TryComp<IdCardComponent>(args.Used, out comp) || comp.OriginalOwner.HasValue)
      return;
    comp.OriginalOwner = new EntityUid?(args.Target);
    this._popup.PopupPredicted($"{this.Name(args.User)} bound an ID to {this.Name(args.Target)}.", args.Target, new EntityUid?(args.User));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(22, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "player", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" has bound the ID ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Used), "entity", "ToPrettyString(args.Used)");
    logStringHandler.AppendLiteral(" to ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Target), "player", "ToPrettyString(args.Target)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.RMCIdModify, LogImpact.High, ref local);
    args.Handled = true;
    this.Dirty<MarineComponent>(ent);
  }

  private void OnExamine(Entity<IdCardComponent> ent, ref ExaminedEvent args)
  {
    if (ent.Comp.OriginalOwner.HasValue)
      return;
    args.PushMarkup("[color=orange]To claim ownership, interact with the card or another person to bind it to them.[/color]");
  }

  private void OnUseInHand(Entity<IdCardComponent> ent, ref UseInHandEvent args)
  {
    if (ent.Comp.OriginalOwner.HasValue || args.Handled)
      return;
    ent.Comp.OriginalOwner = new EntityUid?(args.User);
    args.Handled = true;
    this._popup.PopupClient("Bound ID to yourself.", new EntityUid?(args.User));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(33, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "player", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" has bound the ID ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "entity", "ToPrettyString(ent)");
    logStringHandler.AppendLiteral(" to themselves.");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.RMCIdModify, LogImpact.Medium, ref local);
    this.Dirty<IdCardComponent>(ent);
  }
}
