// Decompiled with JetBrains decompiler
// Type: Content.Shared.Paper.PaperSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Dataset;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Interaction;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.Tag;
using Content.Shared.UserInterface;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Paper;

public sealed class PaperSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private IPrototypeManager _protoMan;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private TagSystem _tagSystem;
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;
  [Dependency]
  private MetaDataSystem _metaSystem;
  [Dependency]
  private SharedAudioSystem _audio;
  private static readonly ProtoId<TagPrototype> WriteIgnoreStampsTag = (ProtoId<TagPrototype>) "WriteIgnoreStamps";
  private static readonly ProtoId<TagPrototype> WriteTag = (ProtoId<TagPrototype>) "Write";
  private Robust.Shared.GameObjects.EntityQuery<PaperComponent> _paperQuery;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PaperComponent, MapInitEvent>(new EntityEventRefHandler<PaperComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<PaperComponent, ComponentInit>(new EntityEventRefHandler<PaperComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<PaperComponent, BeforeActivatableUIOpenEvent>(new EntityEventRefHandler<PaperComponent, BeforeActivatableUIOpenEvent>(this.BeforeUIOpen));
    this.SubscribeLocalEvent<PaperComponent, ExaminedEvent>(new EntityEventRefHandler<PaperComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<PaperComponent, InteractUsingEvent>(new EntityEventRefHandler<PaperComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<PaperComponent, PaperComponent.PaperInputTextMessage>(new EntityEventRefHandler<PaperComponent, PaperComponent.PaperInputTextMessage>(this.OnInputTextMessage));
    this.SubscribeLocalEvent<RandomPaperContentComponent, MapInitEvent>(new EntityEventRefHandler<RandomPaperContentComponent, MapInitEvent>(this.OnRandomPaperContentMapInit));
    this.SubscribeLocalEvent<ActivateOnPaperOpenedComponent, PaperWriteEvent>(new EntityEventRefHandler<ActivateOnPaperOpenedComponent, PaperWriteEvent>(this.OnPaperWrite));
    this.SubscribeLocalEvent<PaperComponent, PaperComponent.PaperSignatureRequestMessage>(new EntityEventRefHandler<PaperComponent, PaperComponent.PaperSignatureRequestMessage>(this.OnSignatureRequest));
    this._paperQuery = this.GetEntityQuery<PaperComponent>();
  }

  private void OnMapInit(Entity<PaperComponent> entity, ref MapInitEvent args)
  {
    if (string.IsNullOrEmpty(entity.Comp.Content))
      return;
    this.SetContent(entity, this.Loc.GetString(entity.Comp.Content));
  }

  private void OnInit(Entity<PaperComponent> entity, ref ComponentInit args)
  {
    entity.Comp.Mode = PaperComponent.PaperAction.Read;
    this.UpdateUserInterface(entity);
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) entity, out comp))
      return;
    if (entity.Comp.Content != "")
      this._appearance.SetData((EntityUid) entity, (Enum) PaperComponent.PaperVisuals.Status, (object) PaperComponent.PaperStatus.Written, comp);
    if (entity.Comp.StampState == null)
      return;
    this._appearance.SetData((EntityUid) entity, (Enum) PaperComponent.PaperVisuals.Stamp, (object) entity.Comp.StampState, comp);
  }

  private void BeforeUIOpen(Entity<PaperComponent> entity, ref BeforeActivatableUIOpenEvent args)
  {
    entity.Comp.Mode = PaperComponent.PaperAction.Read;
    this.UpdateUserInterface(entity);
  }

  private void OnExamined(Entity<PaperComponent> entity, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    using (args.PushGroup("PaperComponent"))
    {
      if (entity.Comp.Content != "")
        args.PushMarkup(this.Loc.GetString("paper-component-examine-detail-has-words", ("paper", (object) entity)));
      if (entity.Comp.StampedBy.Count <= 0)
        return;
      string str = string.Join(", ", entity.Comp.StampedBy.Select<StampDisplayInfo, string>((Func<StampDisplayInfo, string>) (s => this.Loc.GetString(s.StampedName))));
      args.PushMarkup(this.Loc.GetString("paper-component-examine-detail-stamped-by", ("paper", (object) entity), ("stamps", (object) str)));
    }
  }

  private void OnInteractUsing(Entity<PaperComponent> entity, ref InteractUsingEvent args)
  {
    bool flag = entity.Comp.StampedBy.Count == 0 || this._tagSystem.HasTag(args.Used, PaperSystem.WriteIgnoreStampsTag);
    if (this._tagSystem.HasTag(args.Used, PaperSystem.WriteTag))
    {
      if (flag)
      {
        if (entity.Comp.EditingDisabled)
        {
          this._popupSystem.PopupClient(this.Loc.GetString("paper-tamper-proof-modified-message"), (EntityUid) entity, new EntityUid?(args.User));
          args.Handled = true;
          return;
        }
        PaperWriteAttemptEvent args1 = new PaperWriteAttemptEvent(entity.Owner);
        this.RaiseLocalEvent<PaperWriteAttemptEvent>(args.User, ref args1);
        if (args1.Cancelled)
        {
          if (args1.FailReason != null)
            this._popupSystem.PopupClient(this.Loc.GetString(args1.FailReason), entity.Owner, new EntityUid?(args.User));
          args.Handled = true;
          return;
        }
        PaperWriteEvent args2 = new PaperWriteEvent(args.User, (EntityUid) entity);
        this.RaiseLocalEvent<PaperWriteEvent>(args.Used, ref args2);
        entity.Comp.Mode = PaperComponent.PaperAction.Write;
        this._uiSystem.OpenUi((Entity<UserInterfaceComponent>) entity.Owner, (Enum) PaperComponent.PaperUiKey.Key, new EntityUid?(args.User));
        this.UpdateUserInterface(entity);
      }
      args.Handled = true;
    }
    else
    {
      StampComponent comp;
      if (!this.TryComp<StampComponent>(args.Used, out comp) || !this.TryStamp(entity, PaperSystem.GetStampInfo(comp), comp.StampState))
        return;
      this._popupSystem.PopupEntity(this.Loc.GetString("paper-component-action-stamp-paper-other", ("user", (object) args.User), ("target", (object) args.Target), ("stamp", (object) args.Used)), args.User, Filter.PvsExcept(args.User, entityManager: (IEntityManager) this.EntityManager), true);
      this._popupSystem.PopupClient(this.Loc.GetString("paper-component-action-stamp-paper-self", ("target", (object) args.Target), ("stamp", (object) args.Used)), args.User, new EntityUid?(args.User));
      this._audio.PlayPredicted(comp.Sound, (EntityUid) entity, new EntityUid?(args.User));
      this.UpdateUserInterface(entity);
    }
  }

  private static StampDisplayInfo GetStampInfo(StampComponent stamp)
  {
    return new StampDisplayInfo()
    {
      StampedName = stamp.StampedName,
      StampedColor = stamp.StampedColor
    };
  }

  private void OnInputTextMessage(
    Entity<PaperComponent> entity,
    ref PaperComponent.PaperInputTextMessage args)
  {
    PaperWriteAttemptEvent args1 = new PaperWriteAttemptEvent(entity.Owner);
    this.RaiseLocalEvent<PaperWriteAttemptEvent>(args.Actor, ref args1);
    if (args1.Cancelled)
      return;
    if (args.Text.Length <= entity.Comp.ContentSize)
    {
      this.SetContent(entity, args.Text);
      PaperComponent.PaperStatus paperStatus = string.IsNullOrWhiteSpace(args.Text) ? PaperComponent.PaperStatus.Blank : PaperComponent.PaperStatus.Written;
      AppearanceComponent comp1;
      if (this.TryComp<AppearanceComponent>((EntityUid) entity, out comp1))
        this._appearance.SetData((EntityUid) entity, (Enum) PaperComponent.PaperVisuals.Status, (object) paperStatus, comp1);
      MetaDataComponent comp2;
      if (this.TryComp((EntityUid) entity, out comp2))
        this._metaSystem.SetEntityDescription((EntityUid) entity, "", comp2);
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(37, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
      logStringHandler.AppendLiteral(" has written on ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) entity)), nameof (entity), "ToPrettyString(entity)");
      logStringHandler.AppendLiteral(" the following text: ");
      logStringHandler.AppendFormatted(args.Text);
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Chat, LogImpact.Low, ref local);
      this._audio.PlayPvs(entity.Comp.Sound, (EntityUid) entity);
    }
    entity.Comp.Mode = PaperComponent.PaperAction.Read;
    this.UpdateUserInterface(entity);
  }

  private void OnRandomPaperContentMapInit(
    Entity<RandomPaperContentComponent> ent,
    ref MapInitEvent args)
  {
    PaperComponent component;
    if (!this._paperQuery.TryComp((EntityUid) ent, out component))
    {
      this.Log.Warning($"{this.ToPrettyString(new EntityUid?((EntityUid) ent))} has a {"RandomPaperContentComponent"} but no {"PaperComponent"}!");
      this.RemCompDeferred((EntityUid) ent, (IComponent) ent.Comp);
    }
    else
    {
      string messageId = RandomExtensions.Pick<string>(this._random, (IReadOnlyList<string>) this._protoMan.Index<LocalizedDatasetPrototype>(ent.Comp.Dataset).Values);
      this._metaSystem.SetEntityName((EntityUid) ent, this.Loc.GetString(messageId));
      this._metaSystem.SetEntityDescription((EntityUid) ent, this.Loc.GetString(messageId + ".desc"));
      this.SetContent((Entity<PaperComponent>) ((EntityUid) ent, component), this.Loc.GetString(messageId + ".content"));
      this.RemCompDeferred((EntityUid) ent, (IComponent) ent.Comp);
    }
  }

  private void OnPaperWrite(Entity<ActivateOnPaperOpenedComponent> entity, ref PaperWriteEvent args)
  {
    this._interaction.UseInHandInteraction(args.User, (EntityUid) entity);
  }

  public bool TryStamp(
    Entity<PaperComponent> entity,
    StampDisplayInfo stampInfo,
    string spriteStampState)
  {
    if (!entity.Comp.StampedBy.Contains(stampInfo))
    {
      entity.Comp.StampedBy.Add(stampInfo);
      string content = PaperSystem.CleanUnfilledTags(entity.Comp.Content);
      if (content != entity.Comp.Content)
        this.SetContent(entity, content);
      this.Dirty<PaperComponent>(entity);
      AppearanceComponent comp;
      if (entity.Comp.StampState == null && this.TryComp<AppearanceComponent>((EntityUid) entity, out comp))
      {
        entity.Comp.StampState = spriteStampState;
        this._appearance.SetData((EntityUid) entity, (Enum) PaperComponent.PaperVisuals.Stamp, (object) entity.Comp.StampState, comp);
      }
    }
    return true;
  }

  public void CopyStamps(Entity<PaperComponent?> source, Entity<PaperComponent?> target)
  {
    if (!this.Resolve<PaperComponent>((EntityUid) source, ref source.Comp) || !this.Resolve<PaperComponent>((EntityUid) target, ref target.Comp))
      return;
    target.Comp.StampedBy = new List<StampDisplayInfo>((IEnumerable<StampDisplayInfo>) source.Comp.StampedBy);
    target.Comp.StampState = source.Comp.StampState;
    this.Dirty<PaperComponent>(target);
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) target, out comp))
      return;
    this._appearance.SetData((EntityUid) target, (Enum) PaperComponent.PaperVisuals.Stamp, (object) (target.Comp.StampState ?? ""), comp);
  }

  public void SetContent(EntityUid entity, string content)
  {
    PaperComponent comp;
    if (!this.TryComp<PaperComponent>(entity, out comp))
      return;
    this.SetContent((Entity<PaperComponent>) (entity, comp), content);
  }

  public void SetContent(Entity<PaperComponent> entity, string content)
  {
    entity.Comp.Content = content;
    this.Dirty<PaperComponent>(entity);
    this.UpdateUserInterface(entity);
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) entity, out comp))
      return;
    PaperComponent.PaperStatus paperStatus = string.IsNullOrWhiteSpace(content) ? PaperComponent.PaperStatus.Blank : PaperComponent.PaperStatus.Written;
    this._appearance.SetData((EntityUid) entity, (Enum) PaperComponent.PaperVisuals.Status, (object) paperStatus, comp);
  }

  private void UpdateUserInterface(Entity<PaperComponent> entity)
  {
    this._uiSystem.SetUiState((Entity<UserInterfaceComponent>) entity.Owner, (Enum) PaperComponent.PaperUiKey.Key, (BoundUserInterfaceState) new PaperComponent.PaperBoundUserInterfaceState(entity.Comp.Content, entity.Comp.StampedBy, entity.Comp.Mode));
  }

  private void OnSignatureRequest(
    Entity<PaperComponent> entity,
    ref PaperComponent.PaperSignatureRequestMessage args)
  {
    string playerSignature = this.GetPlayerSignature(args.Actor);
    string content = PaperSystem.ReplaceNthSignatureTag(entity.Comp.Content, args.SignatureIndex, playerSignature);
    this.SetContent(entity, content);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(25, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
    logStringHandler.AppendLiteral(" signed ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) entity)), nameof (entity), "ToPrettyString(entity)");
    logStringHandler.AppendLiteral(" with signature: ");
    logStringHandler.AppendFormatted(playerSignature);
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Chat, LogImpact.Low, ref local);
  }

  private string GetPlayerSignature(EntityUid player)
  {
    string empty1 = string.Empty;
    string str = string.Empty;
    string empty2 = string.Empty;
    EntityUid uid = player;
    IdentityComponent comp1;
    if (this.TryComp<IdentityComponent>(player, out comp1))
    {
      EntityUid? containedEntity = comp1.IdentityEntitySlot.ContainedEntity;
      if (containedEntity.HasValue)
        uid = containedEntity.GetValueOrDefault();
    }
    string entityName = this.MetaData(uid).EntityName;
    if (this.TryComp<RankComponent>(player, out RankComponent _))
      str = this.EntityManager.System<SharedRankSystem>().GetRankString(player, true) ?? string.Empty;
    MindContainerComponent comp2;
    if (this.TryComp<MindContainerComponent>(player, out comp2))
    {
      EntityUid? mind1 = comp2.Mind;
      if (mind1.HasValue)
      {
        SharedRoleSystem sharedRoleSystem = this.EntityManager.System<SharedRoleSystem>();
        mind1 = comp2.Mind;
        Entity<MindComponent> mind2 = (Entity<MindComponent>) (mind1.Value, (MindComponent) null);
        List<RoleInfo> allRoleInfo = sharedRoleSystem.MindGetAllRoleInfo(mind2);
        if (allRoleInfo.Count > 0)
          empty2 = this.Loc.GetString(allRoleInfo[0].Name);
      }
    }
    string empty3 = string.Empty;
    string playerSignature;
    if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(entityName) && !string.IsNullOrEmpty(empty2))
      playerSignature = $"{str} {entityName}, {empty2}";
    else
      playerSignature = string.IsNullOrEmpty(str) || string.IsNullOrEmpty(entityName) ? (string.IsNullOrEmpty(entityName) || string.IsNullOrEmpty(empty2) ? entityName : $"{entityName}, {empty2}") : $"{str} {entityName}";
    return playerSignature;
  }

  private static string ReplaceNthSignatureTag(string text, int index, string replacement)
  {
    int num = 0;
    int length;
    for (int startIndex = 0; startIndex < text.Length; startIndex = length + "[signature]".Length)
    {
      length = text.IndexOf("[signature]", startIndex);
      if (length != -1)
      {
        if (num == index)
          return text.Substring(0, length) + replacement + text.Substring(length + "[signature]".Length);
        ++num;
      }
      else
        break;
    }
    return text;
  }

  private static string CleanUnfilledTags(string text)
  {
    return text.Replace("[form]", string.Empty).Replace("[signature]", string.Empty).Replace("[check]", "☐");
  }
}
