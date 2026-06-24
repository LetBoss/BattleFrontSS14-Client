// Decompiled with JetBrains decompiler
// Type: Content.Shared.Emag.Systems.EmagSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Administration.Logs;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Database;
using Content.Shared.Emag.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Emag.Systems;

public sealed class EmagSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private SharedChargesSystem _sharedCharges;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedAudioSystem _audio;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EmagComponent, AfterInteractEvent>(new ComponentEventHandler<EmagComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<EmaggedComponent, OnAccessOverriderAccessUpdatedEvent>(new EntityEventRefHandler<EmaggedComponent, OnAccessOverriderAccessUpdatedEvent>(this.OnAccessOverriderAccessUpdated));
  }

  private void OnAccessOverriderAccessUpdated(
    Entity<EmaggedComponent> entity,
    ref OnAccessOverriderAccessUpdatedEvent args)
  {
    if (!this.CompareFlag(entity.Comp.EmagType, EmagType.Access))
      return;
    entity.Comp.EmagType &= ~EmagType.Access;
    this.Dirty<EmaggedComponent>(entity);
  }

  private void OnAfterInteract(EntityUid uid, EmagComponent comp, AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    args.Handled = this.TryEmagEffect((Entity<EmagComponent>) (uid, comp), args.User, valueOrDefault);
  }

  public bool TryEmagEffect(
    Entity<EmagComponent?> ent,
    EntityUid user,
    EntityUid target,
    EmagType? customEmagType = null)
  {
    if (!this.Resolve<EmagComponent>((EntityUid) ent, ref ent.Comp, false) || this._tag.HasTag(target, ent.Comp.EmagImmuneTag))
      return false;
    Entity<LimitedChargesComponent> owner = (Entity<LimitedChargesComponent>) ent.Owner;
    if (this._sharedCharges.IsEmpty(owner))
    {
      this._popup.PopupClient(this.Loc.GetString("emag-no-charges"), user, new EntityUid?(user));
      return false;
    }
    EmagType Type = (EmagType) ((int) customEmagType ?? (int) ent.Comp.EmagType);
    GotEmaggedEvent args = new GotEmaggedEvent(user, Type);
    this.RaiseLocalEvent<GotEmaggedEvent>(target, ref args);
    if (!args.Handled)
      return false;
    this._popup.PopupPredicted(this.Loc.GetString("emag-success", (nameof (target), (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), user, new EntityUid?(user), PopupType.Medium);
    this._audio.PlayPredicted(ent.Comp.EmagSound, (EntityUid) ent, new EntityUid?((EntityUid) ent));
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(24, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "player", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" emagged ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral(" with flag(s): ");
    logStringHandler.AppendFormatted<EmagType>(Type, "typeToUse");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Emag, LogImpact.High, ref local);
    if (args.Handled)
      this._sharedCharges.TryUseCharge(owner);
    if (!args.Repeatable)
    {
      EmaggedComponent comp;
      this.EnsureComp<EmaggedComponent>(target, out comp);
      comp.EmagType |= Type;
      this.Dirty(target, (IComponent) comp);
    }
    return args.Handled;
  }

  public bool CheckFlag(EntityUid target, EmagType flag)
  {
    EmaggedComponent comp;
    return this.TryComp<EmaggedComponent>(target, out comp) && (comp.EmagType & flag) == flag;
  }

  public bool CompareFlag(EmagType target, EmagType flag) => (target & flag) == flag;
}
