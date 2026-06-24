// Decompiled with JetBrains decompiler
// Type: Content.Shared.Examine.ExamineSystemShared
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Overwatch;
using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Examine;

public abstract class ExamineSystemShared : EntitySystem
{
  [Dependency]
  private OccluderSystem _occluder;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  protected MobStateSystem MobStateSystem;
  [Dependency]
  private QueenEyeSystem _queenEye;
  public const float MaxRaycastRange = 100f;
  public const float CritExamineRange = 1.3f;
  public const float DeadExamineRange = 0.75f;
  public const float ExamineRange = 16f;
  protected const float ExamineDetailsRange = 8f;
  protected const float ExamineBlurrinessMult = 2.5f;
  private Robust.Shared.GameObjects.EntityQuery<GhostComponent> _ghostQuery;
  public const string DefaultIconTexture = "/Textures/Interface/examine-star.png";

  public abstract void SendExamineTooltip(
    EntityUid player,
    EntityUid target,
    FormattedMessage message,
    bool getVerbs,
    bool centerAtCursor);

  public bool IsInDetailsRange(EntityUid examiner, EntityUid entity)
  {
    if (this.IsClientSide(entity) || this._ghostQuery.HasComp(examiner))
      return true;
    if (this.MobStateSystem.IsIncapacitated(examiner) || !this.InRangeUnOccluded(examiner, entity, 8f))
      return false;
    return this._containerSystem.IsInSameOrTransparentContainer((Entity<TransformComponent, MetaDataComponent>) examiner, (Entity<TransformComponent, MetaDataComponent>) entity, userSeeInsideSelf: true) || this._interactionSystem.CanAccessViaStorage(examiner, entity);
  }

  public bool CanExamine(EntityUid examiner, EntityUid examined)
  {
    if (this.IsClientSide(examined))
      return true;
    return !this.Deleted(examined) && this.CanExamine(examiner, this._transform.GetMapCoordinates(examined), (SharedInteractionSystem.Ignored) (entity => entity == examiner || entity == examined), new EntityUid?(examined));
  }

  public virtual bool CanExamine(
    EntityUid examiner,
    MapCoordinates target,
    SharedInteractionSystem.Ignored? predicate = null,
    EntityUid? examined = null,
    ExaminerComponent? examinerComp = null)
  {
    if (!this.Resolve<ExaminerComponent>(examiner, ref examinerComp, false))
      return false;
    if (examinerComp.SkipChecks)
      return true;
    if (examined.HasValue)
    {
      ExamineAttemptEvent args = new ExamineAttemptEvent(examiner);
      this.RaiseLocalEvent<ExamineAttemptEvent>(examined.Value, args);
      if (args.Cancelled)
        return false;
    }
    if (!examinerComp.CheckInRangeUnOccluded)
      return true;
    if (this.Comp<TransformComponent>(examiner).MapID != target.MapId && !this.HasComp<OverwatchWatchingComponent>(examiner) && !this.HasComp<XenoWatchingComponent>(examiner))
      return false;
    if (!examined.HasValue)
      return this.InRangeUnOccluded(examiner, target, this.GetExaminerRange(examiner), predicate);
    QueenEyeActionComponent comp1;
    if (this.TryComp<QueenEyeActionComponent>(examiner, out comp1) && comp1.Eye.HasValue)
      return this._queenEye.CanSeeTarget((Entity<QueenEyeActionComponent>) (examiner, comp1), examined.Value);
    OverwatchWatchingComponent comp2;
    if (this.TryComp<OverwatchWatchingComponent>(examiner, out comp2))
    {
      EntityUid? watching = comp2.Watching;
      if (watching.HasValue)
      {
        EntityUid valueOrDefault = watching.GetValueOrDefault();
        return this.InRangeUnOccluded(valueOrDefault, examined.Value, this.GetExaminerRange(valueOrDefault), predicate);
      }
    }
    XenoWatchingComponent comp3;
    if (this.TryComp<XenoWatchingComponent>(examiner, out comp3))
    {
      EntityUid? watching = comp3.Watching;
      if (watching.HasValue)
      {
        EntityUid valueOrDefault = watching.GetValueOrDefault();
        return this.InRangeUnOccluded(valueOrDefault, examined.Value, this.GetExaminerRange(valueOrDefault), predicate);
      }
    }
    return this.InRangeUnOccluded(examiner, examined.Value, this.GetExaminerRange(examiner), predicate);
  }

  public float GetExaminerRange(EntityUid examiner, MobStateComponent? mobState = null)
  {
    if (this.Resolve<MobStateComponent>(examiner, ref mobState, false))
    {
      if (this.MobStateSystem.IsDead(examiner, mobState))
        return 0.75f;
      BlindableComponent comp1;
      if (this.MobStateSystem.IsCritical(examiner, mobState) || this.TryComp<BlindableComponent>(examiner, out comp1) && comp1.IsBlind)
        return 1.3f;
      BlurryVisionComponent comp2;
      if (this.TryComp<BlurryVisionComponent>(examiner, out comp2))
        return Math.Clamp((float) (16.0 - (double) comp2.Magnitude * 2.5), 2f, 16f);
    }
    return 16f;
  }

  public bool IsOccluded(EntityUid uid)
  {
    EyeComponent comp;
    return this.TryComp<EyeComponent>(uid, out comp) && comp.DrawFov;
  }

  public bool InRangeUnOccluded(
    MapCoordinates origin,
    MapCoordinates other,
    float range,
    SharedInteractionSystem.Ignored? predicate,
    bool ignoreInsideBlocker = true,
    IEntityManager? entMan = null)
  {
    Func<EntityUid, SharedInteractionSystem.Ignored, bool> predicate1 = (Func<EntityUid, SharedInteractionSystem.Ignored, bool>) ((uid, wrapped) => wrapped != null && wrapped(uid));
    return this.InRangeUnOccluded<SharedInteractionSystem.Ignored>(origin, other, range, predicate, predicate1, ignoreInsideBlocker, entMan);
  }

  public bool InRangeUnOccluded<TState>(
    MapCoordinates origin,
    MapCoordinates other,
    float range,
    TState state,
    Func<EntityUid, TState, bool> predicate,
    bool ignoreInsideBlocker = true,
    IEntityManager? entMan = null)
  {
    if (other.MapId != origin.MapId || other.MapId == MapId.Nullspace)
      return false;
    Vector2 vector2 = other.Position - origin.Position;
    float maxLength = vector2.Length();
    if ((double) range > 0.0 && (double) maxLength > (double) range + 0.0099999997764825821)
      return false;
    if (MathHelper.CloseTo(maxLength, 0.0f, 1E-07f))
      return true;
    if ((double) maxLength > 100.0)
    {
      this.Log.Warning("InRangeUnOccluded check performed over extreme range. Limiting CollisionRay size.");
      maxLength = 100f;
    }
    Ray ray = new Ray(origin.Position, Vector2Helpers.Normalized(vector2));
    List<RayCastResults> rayCastResultsList = this._occluder.IntersectRayWithPredicate<TState>(origin.MapId, in ray, maxLength, state, predicate, false);
    if (rayCastResultsList.Count == 0)
      return true;
    if (!ignoreInsideBlocker)
      return false;
    foreach (RayCastResults rayCastResults in rayCastResultsList)
    {
      OccluderComponent comp;
      if (this.TryComp<OccluderComponent>(rayCastResults.HitEntity, out comp))
      {
        Box2 boundingBox = comp.BoundingBox;
        Box2 box2 = ((Box2) ref boundingBox).Translated(this._transform.GetWorldPosition(rayCastResults.HitEntity));
        if (!((Box2) ref box2).Contains(origin.Position, true) && !((Box2) ref box2).Contains(other.Position, true))
          return false;
      }
    }
    return true;
  }

  public bool InRangeUnOccluded(
    EntityUid origin,
    EntityUid other,
    float range = 16f,
    SharedInteractionSystem.Ignored? predicate = null,
    bool ignoreInsideBlocker = true)
  {
    InRangeOverrideEvent args = new InRangeOverrideEvent(origin, other);
    this.RaiseLocalEvent<InRangeOverrideEvent>(origin, ref args);
    return args.Handled ? args.InRange : this.InRangeUnOccluded(this._transform.GetMapCoordinates(origin), this._transform.GetMapCoordinates(other), range, predicate, ignoreInsideBlocker);
  }

  public bool InRangeUnOccluded(
    EntityUid origin,
    EntityCoordinates other,
    float range = 16f,
    SharedInteractionSystem.Ignored? predicate = null,
    bool ignoreInsideBlocker = true)
  {
    return this.InRangeUnOccluded(this._transform.GetMapCoordinates(origin), this._transform.ToMapCoordinates(other), range, predicate, ignoreInsideBlocker);
  }

  public bool InRangeUnOccluded(
    EntityUid origin,
    MapCoordinates other,
    float range = 16f,
    SharedInteractionSystem.Ignored? predicate = null,
    bool ignoreInsideBlocker = true)
  {
    return this.InRangeUnOccluded(this._transform.GetMapCoordinates(origin), other, range, predicate, ignoreInsideBlocker);
  }

  public FormattedMessage GetExamineText(EntityUid entity, EntityUid? examiner)
  {
    FormattedMessage message = new FormattedMessage();
    if (!examiner.HasValue)
      return message;
    bool hasDescription = false;
    MetaDataComponent metaDataComponent = this.MetaData(entity);
    if (!string.IsNullOrEmpty(metaDataComponent.EntityDescription))
    {
      message.AddText(metaDataComponent.EntityDescription);
      hasDescription = true;
    }
    message.PushColor(Color.DarkGray);
    bool isInDetailsRange = this.IsInDetailsRange(examiner.Value, entity);
    ExaminedEvent args = new ExaminedEvent(message, entity, examiner.Value, isInDetailsRange, hasDescription);
    this.RaiseLocalEvent<ExaminedEvent>(entity, args);
    FormattedMessage totalMessage = args.GetTotalMessage();
    totalMessage.Pop();
    return totalMessage;
  }

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GroupExamineComponent, GetVerbsEvent<ExamineVerb>>(new ComponentEventHandler<GroupExamineComponent, GetVerbsEvent<ExamineVerb>>(this.OnGroupExamineVerb));
    this._ghostQuery = this.GetEntityQuery<GhostComponent>();
  }

  private void OnGroupExamineVerb(
    EntityUid uid,
    GroupExamineComponent component,
    GetVerbsEvent<ExamineVerb> args)
  {
    foreach (ExamineGroup examineGroup in component.Group)
    {
      ExamineGroup group = examineGroup;
      if (this.EntityHasComponent(uid, group.Components))
      {
        ExamineVerb examineVerb1 = new ExamineVerb();
        examineVerb1.Act = (Action) (() =>
        {
          this.SendExamineGroup(args.User, args.Target, group);
          group.Entries.Clear();
        });
        examineVerb1.Text = this.Loc.GetString((string) group.ContextText);
        examineVerb1.Message = this.Loc.GetString(group.HoverMessage);
        examineVerb1.Category = VerbCategory.Examine;
        examineVerb1.Icon = group.Icon;
        ExamineVerb examineVerb2 = examineVerb1;
        args.Verbs.Add(examineVerb2);
      }
    }
  }

  public bool EntityHasComponent(EntityUid uid, List<string> components)
  {
    foreach (string component in components)
    {
      ComponentRegistration registration;
      if (this.Factory.TryGetRegistration(component, out registration) && this.HasComp(uid, registration.Type))
        return true;
    }
    return false;
  }

  public void SendExamineGroup(EntityUid user, EntityUid target, ExamineGroup group)
  {
    FormattedMessage message = new FormattedMessage();
    if (group.Title != null)
    {
      message.AddMarkupOrThrow(this.Loc.GetString(group.Title));
      message.PushNewline();
    }
    message.AddMessage(ExamineSystemShared.GetFormattedMessageFromExamineEntries(group.Entries));
    this.SendExamineTooltip(user, target, message, false, false);
  }

  public static FormattedMessage GetFormattedMessageFromExamineEntries(List<ExamineEntry> entries)
  {
    FormattedMessage fromExamineEntries = new FormattedMessage();
    entries.Sort((Comparison<ExamineEntry>) ((a, b) => b.Priority.CompareTo(a.Priority)));
    bool flag = true;
    foreach (ExamineEntry entry in entries)
    {
      if (!flag)
        fromExamineEntries.PushNewline();
      else
        flag = false;
      fromExamineEntries.AddMessage(entry.Message);
    }
    return fromExamineEntries;
  }

  public void AddDetailedExamineVerb(
    GetVerbsEvent<ExamineVerb> verbsEvent,
    Component component,
    List<ExamineEntry> entries,
    string verbText,
    string iconTexture = "/Textures/Interface/examine-star.png",
    string hoverMessage = "",
    bool isHoverExamine = false)
  {
    GroupExamineComponent comp;
    if (this.TryComp<GroupExamineComponent>(verbsEvent.Target, out comp))
    {
      string componentName = this.Factory.GetComponentName(component.GetType());
      foreach (ExamineGroup examineGroup in comp.Group)
      {
        if (examineGroup.Components.Contains(componentName))
        {
          foreach (ExamineEntry entry in examineGroup.Entries)
          {
            if (entry.Component == componentName)
              return;
          }
          using (List<ExamineEntry>.Enumerator enumerator = entries.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              ExamineEntry current = enumerator.Current;
              examineGroup.Entries.Add(current);
            }
            return;
          }
        }
      }
    }
    FormattedMessage formattedMessage = ExamineSystemShared.GetFormattedMessageFromExamineEntries(entries);
    Action action = (Action) (() => this.SendExamineTooltip(verbsEvent.User, verbsEvent.Target, formattedMessage, false, false));
    if (isHoverExamine)
      action = (Action) (() => { });
    ExamineVerb examineVerb1 = new ExamineVerb();
    examineVerb1.Act = action;
    examineVerb1.Text = verbText;
    examineVerb1.Message = hoverMessage;
    examineVerb1.Category = VerbCategory.Examine;
    examineVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath(iconTexture));
    examineVerb1.HoverVerb = isHoverExamine;
    ExamineVerb examineVerb2 = examineVerb1;
    verbsEvent.Verbs.Add(examineVerb2);
  }

  public void AddDetailedExamineVerb(
    GetVerbsEvent<ExamineVerb> verbsEvent,
    Component component,
    ExamineEntry entry,
    string verbText,
    string iconTexture = "/Textures/Interface/examine-star.png",
    string hoverMessage = "",
    bool isHoverExamine = false)
  {
    GetVerbsEvent<ExamineVerb> verbsEvent1 = verbsEvent;
    Component component1 = component;
    List<ExamineEntry> entries = new List<ExamineEntry>();
    entries.Add(entry);
    string verbText1 = verbText;
    string iconTexture1 = iconTexture;
    string hoverMessage1 = hoverMessage;
    int num = isHoverExamine ? 1 : 0;
    this.AddDetailedExamineVerb(verbsEvent1, component1, entries, verbText1, iconTexture1, hoverMessage1, num != 0);
  }

  public void AddDetailedExamineVerb(
    GetVerbsEvent<ExamineVerb> verbsEvent,
    Component component,
    FormattedMessage message,
    string verbText,
    string iconTexture = "/Textures/Interface/examine-star.png",
    string hoverMessage = "",
    bool isHoverExamine = false)
  {
    string componentName = this.Factory.GetComponentName(component.GetType());
    this.AddDetailedExamineVerb(verbsEvent, component, new ExamineEntry(componentName, 0.0f, message), verbText, iconTexture, hoverMessage, isHoverExamine);
  }

  public void AddHoverExamineVerb(
    GetVerbsEvent<ExamineVerb> verbsEvent,
    Component component,
    string verbText,
    string hoverMessage,
    string iconTexture = "/Textures/Interface/examine-star.png")
  {
    this.AddDetailedExamineVerb(verbsEvent, component, FormattedMessage.Empty, verbText, iconTexture, hoverMessage, true);
  }
}
