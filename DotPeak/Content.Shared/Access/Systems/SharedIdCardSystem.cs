// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Systems.SharedIdCardSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable enable
namespace Content.Shared.Access.Systems;

public abstract class SharedIdCardSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _cfgManager;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAccessSystem _access;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private MetaDataSystem _metaSystem;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private int _maxNameLength;
  private int _maxIdJobLength;
  private readonly List<EntityUid> _toRename = new List<EntityUid>();

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IdCardComponent, MapInitEvent>(new ComponentEventHandler<IdCardComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<TryGetIdentityShortInfoEvent>(new EntityEventHandler<TryGetIdentityShortInfoEvent>(this.OnTryGetIdentityShortInfo), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityRenamedEvent>(new EntityEventRefHandler<EntityRenamedEvent>((object) this, __methodptr(OnRename)), (Type[]) null, (Type[]) null);
    EntitySystemSubscriptionExt.CVar<int>(this.Subs, this._cfgManager, CCVars.MaxNameLength, (Action<int>) (value => this._maxNameLength = value), true);
    EntitySystemSubscriptionExt.CVar<int>(this.Subs, this._cfgManager, CCVars.MaxIdJobLength, (Action<int>) (value => this._maxIdJobLength = value), true);
  }

  private void OnRename(ref EntityRenamedEvent ev)
  {
    if (this.HasComp<IdCardComponent>(((EntityRenamedEvent) ref ev).Uid) || this.HasComp<PdaComponent>(((EntityRenamedEvent) ref ev).Uid))
      return;
    this._toRename.Add(((EntityRenamedEvent) ref ev).Uid);
  }

  private void OnMapInit(EntityUid uid, IdCardComponent id, MapInitEvent args)
  {
    this.UpdateEntityName(uid, id);
  }

  private void OnTryGetIdentityShortInfo(TryGetIdentityShortInfoEvent ev)
  {
    if (ev.Handled)
      return;
    string str = (string) null;
    Entity<IdCardComponent> idCard;
    if (this.TryFindIdCard(ev.ForActor, out idCard) && (!ev.RequestForAccessLogging || !idCard.Comp.BypassLogging))
      str = SharedIdCardSystem.ExtractFullTitle(Entity<IdCardComponent>.op_Implicit(idCard));
    ev.Title = str;
    ev.Handled = true;
  }

  public bool TryFindIdCard(EntityUid uid, out Entity<IdCardComponent> idCard)
  {
    EntityUid? activeItem = this._hands.GetActiveItem(Entity<HandsComponent>.op_Implicit(uid));
    EntityUid? entityUid;
    return activeItem.HasValue && this.TryGetIdCard(activeItem.GetValueOrDefault(), out idCard) || this.TryGetIdCard(uid, out idCard) || this._inventorySystem.TryGetSlotEntity(uid, "id", out entityUid) && this.TryGetIdCard(entityUid.Value, out idCard);
  }

  public bool TryGetIdCard(EntityUid uid, out Entity<IdCardComponent> idCard)
  {
    IdCardComponent idCardComponent;
    if (this.TryComp<IdCardComponent>(uid, ref idCardComponent))
    {
      idCard = Entity<IdCardComponent>.op_Implicit((uid, idCardComponent));
      return true;
    }
    PdaComponent pdaComponent;
    if (this.TryComp<PdaComponent>(uid, ref pdaComponent) && this.TryComp<IdCardComponent>(pdaComponent.ContainedId, ref idCardComponent))
    {
      idCard = Entity<IdCardComponent>.op_Implicit((pdaComponent.ContainedId.Value, idCardComponent));
      return true;
    }
    idCard = new Entity<IdCardComponent>();
    return false;
  }

  public bool TryChangeJobTitle(
    EntityUid uid,
    string? jobTitle,
    IdCardComponent? id = null,
    EntityUid? player = null)
  {
    if (!this.Resolve<IdCardComponent>(uid, ref id, true))
      return false;
    if (!string.IsNullOrWhiteSpace(jobTitle))
    {
      jobTitle = jobTitle.Trim();
      if (jobTitle.Length > this._maxIdJobLength)
        jobTitle = jobTitle.Substring(0, this._maxIdJobLength);
    }
    else
      jobTitle = (string) null;
    if (id.LocalizedJobTitle == jobTitle)
      return true;
    id.LocalizedJobTitle = jobTitle;
    this.Dirty(uid, (IComponent) id, (MetaDataComponent) null);
    this.UpdateEntityName(uid, id);
    if (player.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(35, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player.Value)), nameof (player), "ToPrettyString(player.Value)");
      logStringHandler.AppendLiteral(" has changed the job title of ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" to ");
      logStringHandler.AppendFormatted(jobTitle);
      logStringHandler.AppendLiteral(" ");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Identity, LogImpact.Low, ref local);
    }
    return true;
  }

  public bool TryChangeJobIcon(
    EntityUid uid,
    JobIconPrototype jobIcon,
    IdCardComponent? id = null,
    EntityUid? player = null)
  {
    if (!this.Resolve<IdCardComponent>(uid, ref id, true))
      return false;
    if (ProtoId<JobIconPrototype>.op_Equality(id.JobIcon, ProtoId<JobIconPrototype>.op_Implicit(jobIcon.ID)))
      return true;
    id.JobIcon = ProtoId<JobIconPrototype>.op_Implicit(jobIcon.ID);
    this.Dirty(uid, (IComponent) id, (MetaDataComponent) null);
    if (player.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(34, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player.Value)), nameof (player), "ToPrettyString(player.Value)");
      logStringHandler.AppendLiteral(" has changed the job icon of ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" to ");
      logStringHandler.AppendFormatted<JobIconPrototype>(jobIcon, nameof (jobIcon));
      logStringHandler.AppendLiteral(" ");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Identity, LogImpact.Low, ref local);
    }
    return true;
  }

  public bool TryChangeJobDepartment(EntityUid uid, JobPrototype job, IdCardComponent? id = null)
  {
    if (!this.Resolve<IdCardComponent>(uid, ref id, true))
      return false;
    id.JobDepartments.Clear();
    foreach (DepartmentPrototype enumeratePrototype in this._prototypeManager.EnumeratePrototypes<DepartmentPrototype>())
    {
      if (enumeratePrototype.Roles.Contains(ProtoId<JobPrototype>.op_Implicit(job.ID)))
        id.JobDepartments.Add(ProtoId<DepartmentPrototype>.op_Implicit(enumeratePrototype.ID));
    }
    this.Dirty(uid, (IComponent) id, (MetaDataComponent) null);
    return true;
  }

  public bool TryChangeJobDepartment(
    EntityUid uid,
    List<ProtoId<DepartmentPrototype>> departments,
    IdCardComponent? id = null)
  {
    if (!this.Resolve<IdCardComponent>(uid, ref id, true))
      return false;
    id.JobDepartments.Clear();
    foreach (ProtoId<DepartmentPrototype> department in departments)
      id.JobDepartments.Add(department);
    this.Dirty(uid, (IComponent) id, (MetaDataComponent) null);
    return true;
  }

  public bool TryChangeFullName(
    EntityUid uid,
    string? fullName,
    IdCardComponent? id = null,
    EntityUid? player = null)
  {
    if (!this.Resolve<IdCardComponent>(uid, ref id, true))
      return false;
    if (!string.IsNullOrWhiteSpace(fullName))
    {
      fullName = fullName.Trim();
      if (fullName.Length > this._maxNameLength)
        fullName = fullName.Substring(0, this._maxNameLength);
    }
    else
      fullName = (string) null;
    if (id.FullName == fullName)
      return true;
    id.FullName = fullName;
    this.Dirty(uid, (IComponent) id, (MetaDataComponent) null);
    this.UpdateEntityName(uid, id);
    if (player.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(30, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(player.Value)), nameof (player), "ToPrettyString(player.Value)");
      logStringHandler.AppendLiteral(" has changed the name of ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "entity", "ToPrettyString(uid)");
      logStringHandler.AppendLiteral(" to ");
      logStringHandler.AppendFormatted(fullName);
      logStringHandler.AppendLiteral(" ");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Identity, LogImpact.Low, ref local);
    }
    return true;
  }

  private void UpdateEntityName(EntityUid uid, IdCardComponent? id = null)
  {
    if (!this.Resolve<IdCardComponent>(uid, ref id, true))
      return;
    string str1 = string.IsNullOrWhiteSpace(id.LocalizedJobTitle) ? string.Empty : $" ({id.LocalizedJobTitle})";
    string str2 = string.IsNullOrWhiteSpace(id.FullName) ? this.Loc.GetString(LocId.op_Implicit(id.NameLocId), ("jobSuffix", (object) str1)) : this.Loc.GetString(LocId.op_Implicit(id.FullNameLocId), ("fullName", (object) id.FullName), ("jobSuffix", (object) str1));
    this._metaSystem.SetEntityName(uid, str2, (MetaDataComponent) null, true);
  }

  private static string ExtractFullTitle(IdCardComponent idCardComponent)
  {
    return $"{idCardComponent.FullName} ({CultureInfo.CurrentCulture.TextInfo.ToTitleCase(idCardComponent.LocalizedJobTitle ?? string.Empty)})".Trim();
  }

  public void SetExpireTime(Entity<ExpireIdCardComponent?> ent, TimeSpan time)
  {
    if (!this.Resolve<ExpireIdCardComponent>(Entity<ExpireIdCardComponent>.op_Implicit(ent), ref ent.Comp, true))
      return;
    ent.Comp.ExpireTime = time;
    this.Dirty<ExpireIdCardComponent>(ent, (MetaDataComponent) null);
  }

  public void SetPermanent(Entity<ExpireIdCardComponent?> ent, bool val)
  {
    if (!this.Resolve<ExpireIdCardComponent>(Entity<ExpireIdCardComponent>.op_Implicit(ent), ref ent.Comp, true))
      return;
    ent.Comp.Permanent = val;
    this.Dirty<ExpireIdCardComponent>(ent, (MetaDataComponent) null);
  }

  public virtual void ExpireId(Entity<ExpireIdCardComponent> ent)
  {
    if (ent.Comp.Expired)
      return;
    this._access.TrySetTags(Entity<ExpireIdCardComponent>.op_Implicit(ent), (IEnumerable<ProtoId<AccessLevelPrototype>>) ent.Comp.ExpiredAccess);
    ent.Comp.Expired = true;
    this.Dirty<ExpireIdCardComponent>(ent, (MetaDataComponent) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<ExpireIdCardComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ExpireIdCardComponent>();
    EntityUid entityUid;
    ExpireIdCardComponent expireIdCardComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref expireIdCardComponent))
    {
      if (!expireIdCardComponent.Expired && !expireIdCardComponent.Permanent && !(this._timing.CurTime < expireIdCardComponent.ExpireTime))
        this.ExpireId(Entity<ExpireIdCardComponent>.op_Implicit((entityUid, expireIdCardComponent)));
    }
    try
    {
      foreach (EntityUid uid in this._toRename)
      {
        Entity<IdCardComponent> idCard;
        if (!this.TerminatingOrDeleted(uid, (MetaDataComponent) null) && this.TryFindIdCard(uid, out idCard))
          this.TryChangeFullName(Entity<IdCardComponent>.op_Implicit(idCard), this.Name(uid, (MetaDataComponent) null), Entity<IdCardComponent>.op_Implicit(idCard));
      }
    }
    finally
    {
      this._toRename.Clear();
    }
  }

  public bool TryChangeOriginalOwner(EntityUid uid, EntityUid? player, IdCardComponent? id = null)
  {
    if (!this.Resolve<IdCardComponent>(uid, ref id, true) || !player.HasValue)
      return false;
    EntityUid? originalOwner = id.OriginalOwner;
    EntityUid? nullable = player;
    if ((originalOwner.HasValue == nullable.HasValue ? (originalOwner.HasValue ? (EntityUid.op_Equality(originalOwner.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0)
      return true;
    id.OriginalOwner = new EntityUid?(player.Value);
    this.Dirty(uid, (IComponent) id, (MetaDataComponent) null);
    this.UpdateEntityName(uid, id);
    return true;
  }
}
