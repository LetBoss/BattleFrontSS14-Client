using System.Diagnostics.CodeAnalysis;
using Content.Shared.Mind;
using Content.Shared.Objectives.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Objectives.Systems;

public abstract class SharedObjectivesSystem : EntitySystem
{
	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private IPrototypeManager _protoMan;

	private EntityQuery<MetaDataComponent> _metaQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_metaQuery = ((EntitySystem)this).GetEntityQuery<MetaDataComponent>();
	}

	public bool CanBeAssigned(EntityUid uid, EntityUid mindId, MindComponent mind, ObjectiveComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ObjectiveComponent>(uid, ref comp, true))
		{
			return false;
		}
		RequirementCheckEvent ev = new RequirementCheckEvent(mindId, mind);
		((EntitySystem)this).RaiseLocalEvent<RequirementCheckEvent>(uid, ref ev, false);
		if (ev.Cancelled)
		{
			return false;
		}
		if (comp.Unique)
		{
			EntityPrototype entityPrototype = _metaQuery.GetComponent(uid).EntityPrototype;
			string proto = ((entityPrototype != null) ? entityPrototype.ID : null);
			foreach (EntityUid objective in mind.Objectives)
			{
				EntityPrototype entityPrototype2 = _metaQuery.GetComponent(objective).EntityPrototype;
				if (((entityPrototype2 != null) ? entityPrototype2.ID : null) == proto)
				{
					return false;
				}
			}
		}
		return true;
	}

	public EntityUid? TryCreateObjective(EntityUid mindId, MindComponent mind, string proto)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		if (!_protoMan.HasIndex<EntityPrototype>(proto))
		{
			return null;
		}
		EntityUid uid = ((EntitySystem)this).Spawn(proto, (ComponentRegistry)null, true);
		ObjectiveComponent comp = default(ObjectiveComponent);
		if (!((EntitySystem)this).TryComp<ObjectiveComponent>(uid, ref comp))
		{
			((EntitySystem)this).Del((EntityUid?)uid);
			((EntitySystem)this).Log.Error("Invalid objective prototype " + proto + ", missing ObjectiveComponent");
			return null;
		}
		if (!CanBeAssigned(uid, mindId, mind, comp))
		{
			((EntitySystem)this).Log.Warning($"Objective {proto} did not match the requirements for {_mind.MindOwnerLoggingString(mind)}, deleted it");
			return null;
		}
		ObjectiveAssignedEvent ev = new ObjectiveAssignedEvent(mindId, mind);
		((EntitySystem)this).RaiseLocalEvent<ObjectiveAssignedEvent>(uid, ref ev, false);
		if (ev.Cancelled)
		{
			((EntitySystem)this).Del((EntityUid?)uid);
			((EntitySystem)this).Log.Warning("Could not assign objective " + proto + ", deleted it");
			return null;
		}
		ObjectiveAfterAssignEvent afterEv = new ObjectiveAfterAssignEvent(mindId, mind, comp, ((EntitySystem)this).MetaData(uid));
		((EntitySystem)this).RaiseLocalEvent<ObjectiveAfterAssignEvent>(uid, ref afterEv, false);
		((EntitySystem)this).Log.Debug($"Created objective {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)):objective}");
		return uid;
	}

	public bool TryCreateObjective(Entity<MindComponent> mind, EntProtoId proto, [NotNullWhen(true)] out EntityUid? objective)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		objective = TryCreateObjective(mind.Owner, mind.Comp, EntProtoId.op_Implicit(proto));
		return objective.HasValue;
	}

	public ObjectiveInfo? GetInfo(EntityUid uid, EntityUid mindId, MindComponent? mind = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MindComponent>(mindId, ref mind, true))
		{
			return null;
		}
		float? progress = GetProgress(uid, Entity<MindComponent>.op_Implicit((mindId, mind)));
		if (progress.HasValue)
		{
			float progress2 = progress.GetValueOrDefault();
			ObjectiveComponent comp = ((EntitySystem)this).Comp<ObjectiveComponent>(uid);
			MetaDataComponent obj = ((EntitySystem)this).MetaData(uid);
			string title = obj.EntityName;
			string description = obj.EntityDescription;
			if (comp.Icon == null)
			{
				((EntitySystem)this).Log.Error($"An objective {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)):objective} of {_mind.MindOwnerLoggingString(mind)} is missing an icon!");
				return null;
			}
			return new ObjectiveInfo(title, description, comp.Icon, progress2);
		}
		return null;
	}

	public float? GetProgress(EntityUid uid, Entity<MindComponent> mind)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		ObjectiveGetProgressEvent ev = new ObjectiveGetProgressEvent(Entity<MindComponent>.op_Implicit(mind), mind.Comp);
		((EntitySystem)this).RaiseLocalEvent<ObjectiveGetProgressEvent>(uid, ref ev, false);
		if (ev.Progress.HasValue)
		{
			return ev.Progress;
		}
		((EntitySystem)this).Log.Error($"Objective {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)):objective} of {_mind.MindOwnerLoggingString(mind.Comp)} didn't set a progress value!");
		return null;
	}

	public bool IsCompleted(EntityUid uid, Entity<MindComponent> mind)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetProgress(uid, mind).GetValueOrDefault() >= 0.999f;
	}

	public void SetIcon(EntityUid uid, SpriteSpecifier icon, ObjectiveComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ObjectiveComponent>(uid, ref comp, true))
		{
			comp.Icon = icon;
		}
	}
}
