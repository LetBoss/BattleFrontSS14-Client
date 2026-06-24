using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Overwatch;
using Content.Shared._RMC14.Xenonids.Eye;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Verbs;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Utility;

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

	private EntityQuery<GhostComponent> _ghostQuery;

	public const string DefaultIconTexture = "/Textures/Interface/examine-star.png";

	public abstract void SendExamineTooltip(EntityUid player, EntityUid target, FormattedMessage message, bool getVerbs, bool centerAtCursor);

	public bool IsInDetailsRange(EntityUid examiner, EntityUid entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).IsClientSide(entity, (MetaDataComponent)null))
		{
			return true;
		}
		if (_ghostQuery.HasComp(examiner))
		{
			return true;
		}
		if (MobStateSystem.IsIncapacitated(examiner))
		{
			return false;
		}
		if (!InRangeUnOccluded(examiner, entity, 8f))
		{
			return false;
		}
		if (_containerSystem.IsInSameOrTransparentContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(examiner), Entity<TransformComponent, MetaDataComponent>.op_Implicit(entity), (BaseContainer)null, (BaseContainer)null, true))
		{
			return true;
		}
		return _interactionSystem.CanAccessViaStorage(examiner, entity);
	}

	public bool CanExamine(EntityUid examiner, EntityUid examined)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).IsClientSide(examined, (MetaDataComponent)null))
		{
			return true;
		}
		if (!((EntitySystem)this).Deleted(examined, (MetaDataComponent)null))
		{
			return CanExamine(examiner, _transform.GetMapCoordinates(examined, (TransformComponent)null), (EntityUid entity) => entity == examiner || entity == examined, examined);
		}
		return false;
	}

	public virtual bool CanExamine(EntityUid examiner, MapCoordinates target, SharedInteractionSystem.Ignored? predicate = null, EntityUid? examined = null, ExaminerComponent? examinerComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ExaminerComponent>(examiner, ref examinerComp, false))
		{
			return false;
		}
		if (examinerComp.SkipChecks)
		{
			return true;
		}
		if (examined.HasValue)
		{
			ExamineAttemptEvent ev = new ExamineAttemptEvent(examiner);
			((EntitySystem)this).RaiseLocalEvent<ExamineAttemptEvent>(examined.Value, ev, false);
			if (((CancellableEntityEventArgs)ev).Cancelled)
			{
				return false;
			}
		}
		if (!examinerComp.CheckInRangeUnOccluded)
		{
			return true;
		}
		if (((EntitySystem)this).Comp<TransformComponent>(examiner).MapID != target.MapId && !((EntitySystem)this).HasComp<OverwatchWatchingComponent>(examiner) && !((EntitySystem)this).HasComp<XenoWatchingComponent>(examiner))
		{
			return false;
		}
		if (examined.HasValue)
		{
			QueenEyeActionComponent queen = default(QueenEyeActionComponent);
			if (((EntitySystem)this).TryComp<QueenEyeActionComponent>(examiner, ref queen) && queen.Eye.HasValue)
			{
				return _queenEye.CanSeeTarget(Entity<QueenEyeActionComponent>.op_Implicit((examiner, queen)), examined.Value);
			}
			OverwatchWatchingComponent overwatcher = default(OverwatchWatchingComponent);
			if (((EntitySystem)this).TryComp<OverwatchWatchingComponent>(examiner, ref overwatcher))
			{
				EntityUid? watching = overwatcher.Watching;
				if (watching.HasValue)
				{
					EntityUid overwatched = watching.GetValueOrDefault();
					return InRangeUnOccluded(overwatched, examined.Value, GetExaminerRange(overwatched), predicate);
				}
			}
			XenoWatchingComponent watcher = default(XenoWatchingComponent);
			if (((EntitySystem)this).TryComp<XenoWatchingComponent>(examiner, ref watcher))
			{
				EntityUid? watching = watcher.Watching;
				if (watching.HasValue)
				{
					EntityUid watched = watching.GetValueOrDefault();
					return InRangeUnOccluded(watched, examined.Value, GetExaminerRange(watched), predicate);
				}
			}
			return InRangeUnOccluded(examiner, examined.Value, GetExaminerRange(examiner), predicate);
		}
		return InRangeUnOccluded(examiner, target, GetExaminerRange(examiner), predicate);
	}

	public float GetExaminerRange(EntityUid examiner, MobStateComponent? mobState = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MobStateComponent>(examiner, ref mobState, false))
		{
			if (MobStateSystem.IsDead(examiner, mobState))
			{
				return 0.75f;
			}
			BlindableComponent blind = default(BlindableComponent);
			if (MobStateSystem.IsCritical(examiner, mobState) || (((EntitySystem)this).TryComp<BlindableComponent>(examiner, ref blind) && blind.IsBlind))
			{
				return 1.3f;
			}
			BlurryVisionComponent blurry = default(BlurryVisionComponent);
			if (((EntitySystem)this).TryComp<BlurryVisionComponent>(examiner, ref blurry))
			{
				return Math.Clamp(16f - blurry.Magnitude * 2.5f, 2f, 16f);
			}
		}
		return 16f;
	}

	public bool IsOccluded(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		EyeComponent eye = default(EyeComponent);
		if (((EntitySystem)this).TryComp<EyeComponent>(uid, ref eye))
		{
			return eye.DrawFov;
		}
		return false;
	}

	public bool InRangeUnOccluded(MapCoordinates origin, MapCoordinates other, float range, SharedInteractionSystem.Ignored? predicate, bool ignoreInsideBlocker = true, IEntityManager? entMan = null)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Func<EntityUid, SharedInteractionSystem.Ignored, bool> wrapped = (EntityUid uid, SharedInteractionSystem.Ignored? ignored) => ignored?.Invoke(uid) ?? false;
		return InRangeUnOccluded<SharedInteractionSystem.Ignored>(origin, other, range, predicate, wrapped, ignoreInsideBlocker, entMan);
	}

	public bool InRangeUnOccluded<TState>(MapCoordinates origin, MapCoordinates other, float range, TState state, Func<EntityUid, TState, bool> predicate, bool ignoreInsideBlocker = true, IEntityManager? entMan = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		if (other.MapId != origin.MapId || other.MapId == MapId.Nullspace)
		{
			return false;
		}
		Vector2 dir = other.Position - origin.Position;
		float length = dir.Length();
		if (range > 0f && length > range + 0.01f)
		{
			return false;
		}
		if (MathHelper.CloseTo(length, 0f, 1E-07f))
		{
			return true;
		}
		if (length > 100f)
		{
			((EntitySystem)this).Log.Warning("InRangeUnOccluded check performed over extreme range. Limiting CollisionRay size.");
			length = 100f;
		}
		Ray ray = default(Ray);
		((Ray)(ref ray))._002Ector(origin.Position, Vector2Helpers.Normalized(dir));
		List<RayCastResults> rayResults = ((ComponentTreeSystem<OccluderTreeComponent, OccluderComponent>)(object)_occluder).IntersectRayWithPredicate<TState>(origin.MapId, ref ray, length, state, predicate, false);
		if (rayResults.Count == 0)
		{
			return true;
		}
		if (!ignoreInsideBlocker)
		{
			return false;
		}
		OccluderComponent o = default(OccluderComponent);
		foreach (RayCastResults item in rayResults)
		{
			RayCastResults result = item;
			if (((EntitySystem)this).TryComp<OccluderComponent>(((RayCastResults)(ref result)).HitEntity, ref o))
			{
				Box2 bBox = o.BoundingBox;
				bBox = ((Box2)(ref bBox)).Translated(_transform.GetWorldPosition(((RayCastResults)(ref result)).HitEntity));
				if (!((Box2)(ref bBox)).Contains(origin.Position, true) && !((Box2)(ref bBox)).Contains(other.Position, true))
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool InRangeUnOccluded(EntityUid origin, EntityUid other, float range = 16f, SharedInteractionSystem.Ignored? predicate = null, bool ignoreInsideBlocker = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		InRangeOverrideEvent ev = new InRangeOverrideEvent(origin, other);
		((EntitySystem)this).RaiseLocalEvent<InRangeOverrideEvent>(origin, ref ev, false);
		if (ev.Handled)
		{
			return ev.InRange;
		}
		MapCoordinates originPos = _transform.GetMapCoordinates(origin, (TransformComponent)null);
		MapCoordinates otherPos = _transform.GetMapCoordinates(other, (TransformComponent)null);
		return InRangeUnOccluded(originPos, otherPos, range, predicate, ignoreInsideBlocker);
	}

	public bool InRangeUnOccluded(EntityUid origin, EntityCoordinates other, float range = 16f, SharedInteractionSystem.Ignored? predicate = null, bool ignoreInsideBlocker = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates originPos = _transform.GetMapCoordinates(origin, (TransformComponent)null);
		MapCoordinates otherPos = _transform.ToMapCoordinates(other, true);
		return InRangeUnOccluded(originPos, otherPos, range, predicate, ignoreInsideBlocker);
	}

	public bool InRangeUnOccluded(EntityUid origin, MapCoordinates other, float range = 16f, SharedInteractionSystem.Ignored? predicate = null, bool ignoreInsideBlocker = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		MapCoordinates originPos = _transform.GetMapCoordinates(origin, (TransformComponent)null);
		return InRangeUnOccluded(originPos, other, range, predicate, ignoreInsideBlocker);
	}

	public FormattedMessage GetExamineText(EntityUid entity, EntityUid? examiner)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage message = new FormattedMessage();
		if (!examiner.HasValue)
		{
			return message;
		}
		bool hasDescription = false;
		MetaDataComponent metadata = ((EntitySystem)this).MetaData(entity);
		if (!string.IsNullOrEmpty(metadata.EntityDescription))
		{
			message.AddText(metadata.EntityDescription);
			hasDescription = true;
		}
		message.PushColor(Color.DarkGray);
		bool isInDetailsRange = IsInDetailsRange(examiner.Value, entity);
		ExaminedEvent examinedEvent = new ExaminedEvent(message, entity, examiner.Value, isInDetailsRange, hasDescription);
		((EntitySystem)this).RaiseLocalEvent<ExaminedEvent>(entity, examinedEvent, false);
		FormattedMessage totalMessage = examinedEvent.GetTotalMessage();
		totalMessage.Pop();
		return totalMessage;
	}

	public override void Initialize()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<GroupExamineComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<GroupExamineComponent, GetVerbsEvent<ExamineVerb>>)OnGroupExamineVerb, (Type[])null, (Type[])null);
		_ghostQuery = ((EntitySystem)this).GetEntityQuery<GhostComponent>();
	}

	private void OnGroupExamineVerb(EntityUid uid, GroupExamineComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		foreach (ExamineGroup group in component.Group)
		{
			if (EntityHasComponent(uid, group.Components))
			{
				ExamineVerb examineVerb = new ExamineVerb
				{
					Act = delegate
					{
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						//IL_0026: Unknown result type (might be due to invalid IL or missing references)
						SendExamineGroup(args.User, args.Target, group);
						group.Entries.Clear();
					},
					Text = base.Loc.GetString(LocId.op_Implicit(group.ContextText)),
					Message = base.Loc.GetString(group.HoverMessage),
					Category = VerbCategory.Examine,
					Icon = group.Icon
				};
				args.Verbs.Add(examineVerb);
			}
		}
	}

	public bool EntityHasComponent(EntityUid uid, List<string> components)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		ComponentRegistration componentRegistration = default(ComponentRegistration);
		foreach (string comp in components)
		{
			if (((EntitySystem)this).Factory.TryGetRegistration(comp, ref componentRegistration, false) && ((EntitySystem)this).HasComp(uid, componentRegistration.Type))
			{
				return true;
			}
		}
		return false;
	}

	public void SendExamineGroup(EntityUid user, EntityUid target, ExamineGroup group)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage message = new FormattedMessage();
		if (group.Title != null)
		{
			message.AddMarkupOrThrow(base.Loc.GetString(group.Title));
			message.PushNewline();
		}
		message.AddMessage(GetFormattedMessageFromExamineEntries(group.Entries));
		SendExamineTooltip(user, target, message, getVerbs: false, centerAtCursor: false);
	}

	public static FormattedMessage GetFormattedMessageFromExamineEntries(List<ExamineEntry> entries)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		FormattedMessage formattedMessage = new FormattedMessage();
		entries.Sort((ExamineEntry a, ExamineEntry b) => b.Priority.CompareTo(a.Priority));
		bool first = true;
		foreach (ExamineEntry entry in entries)
		{
			if (!first)
			{
				formattedMessage.PushNewline();
			}
			else
			{
				first = false;
			}
			formattedMessage.AddMessage(entry.Message);
		}
		return formattedMessage;
	}

	public void AddDetailedExamineVerb(GetVerbsEvent<ExamineVerb> verbsEvent, Component component, List<ExamineEntry> entries, string verbText, string iconTexture = "/Textures/Interface/examine-star.png", string hoverMessage = "", bool isHoverExamine = false)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		GroupExamineComponent groupExamine = default(GroupExamineComponent);
		if (((EntitySystem)this).TryComp<GroupExamineComponent>(verbsEvent.Target, ref groupExamine))
		{
			string componentName = ((EntitySystem)this).Factory.GetComponentName(((object)component).GetType());
			foreach (ExamineGroup examineGroup in groupExamine.Group)
			{
				if (!examineGroup.Components.Contains(componentName))
				{
					continue;
				}
				foreach (ExamineEntry entry2 in examineGroup.Entries)
				{
					if (entry2.Component == componentName)
					{
						return;
					}
				}
				{
					foreach (ExamineEntry entry in entries)
					{
						examineGroup.Entries.Add(entry);
					}
					return;
				}
			}
		}
		FormattedMessage formattedMessage = GetFormattedMessageFromExamineEntries(entries);
		Action act = delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			SendExamineTooltip(verbsEvent.User, verbsEvent.Target, formattedMessage, getVerbs: false, centerAtCursor: false);
		};
		if (isHoverExamine)
		{
			act = delegate
			{
			};
		}
		ExamineVerb examineVerb = new ExamineVerb
		{
			Act = act,
			Text = verbText,
			Message = hoverMessage,
			Category = VerbCategory.Examine,
			Icon = (SpriteSpecifier?)new Texture(new ResPath(iconTexture)),
			HoverVerb = isHoverExamine
		};
		verbsEvent.Verbs.Add(examineVerb);
	}

	public void AddDetailedExamineVerb(GetVerbsEvent<ExamineVerb> verbsEvent, Component component, ExamineEntry entry, string verbText, string iconTexture = "/Textures/Interface/examine-star.png", string hoverMessage = "", bool isHoverExamine = false)
	{
		AddDetailedExamineVerb(verbsEvent, component, new List<ExamineEntry> { entry }, verbText, iconTexture, hoverMessage, isHoverExamine);
	}

	public void AddDetailedExamineVerb(GetVerbsEvent<ExamineVerb> verbsEvent, Component component, FormattedMessage message, string verbText, string iconTexture = "/Textures/Interface/examine-star.png", string hoverMessage = "", bool isHoverExamine = false)
	{
		string componentName = ((EntitySystem)this).Factory.GetComponentName(((object)component).GetType());
		AddDetailedExamineVerb(verbsEvent, component, new ExamineEntry(componentName, 0f, message), verbText, iconTexture, hoverMessage, isHoverExamine);
	}

	public void AddHoverExamineVerb(GetVerbsEvent<ExamineVerb> verbsEvent, Component component, string verbText, string hoverMessage, string iconTexture = "/Textures/Interface/examine-star.png")
	{
		AddDetailedExamineVerb(verbsEvent, component, FormattedMessage.Empty, verbText, iconTexture, hoverMessage, isHoverExamine: true);
	}
}
