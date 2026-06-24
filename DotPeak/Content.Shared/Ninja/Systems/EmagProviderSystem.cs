// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Systems.EmagProviderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Emag.Systems;
using Content.Shared.Interaction;
using Content.Shared.Ninja.Components;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Ninja.Systems;

public sealed class EmagProviderSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedNinjaGlovesSystem _gloves;
  [Dependency]
  private TagSystem _tag;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EmagProviderComponent, BeforeInteractHandEvent>(new EntityEventRefHandler<EmagProviderComponent, BeforeInteractHandEvent>(this.OnBeforeInteractHand));
  }

  private void OnBeforeInteractHand(
    Entity<EmagProviderComponent> ent,
    ref BeforeInteractHandEvent args)
  {
    EntityUid target;
    if (args.Handled || !this._gloves.AbilityCheck((EntityUid) ent, args, out target))
      return;
    (EntityUid entityUid, EmagProviderComponent comp) = ent;
    if (this._whitelist.IsWhitelistFail(comp.Whitelist, target) || this._tag.HasTag(target, comp.AccessBreakerImmuneTag))
      return;
    GotEmaggedEvent args1 = new GotEmaggedEvent(entityUid, EmagType.Access);
    this.RaiseLocalEvent<GotEmaggedEvent>(args.Target, ref args1);
    if (!args1.Handled)
      return;
    this._audio.PlayPredicted(comp.EmagSound, entityUid, new EntityUid?(entityUid));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(24, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid), "player", "ToPrettyString(uid)");
    logStringHandler.AppendLiteral(" emagged ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), "target", "ToPrettyString(target)");
    logStringHandler.AppendLiteral(" with flag(s): ");
    logStringHandler.AppendFormatted<EmagType>(ent.Comp.EmagType, "ent.Comp.EmagType");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Emag, LogImpact.High, ref local);
    EmaggedSomethingEvent args2 = new EmaggedSomethingEvent(target);
    this.RaiseLocalEvent<EmaggedSomethingEvent>(entityUid, ref args2);
    args.Handled = true;
  }
}
