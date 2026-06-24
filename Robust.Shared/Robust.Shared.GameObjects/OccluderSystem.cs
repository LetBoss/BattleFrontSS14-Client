using System;
using System.Numerics;
using Robust.Shared.ComponentTrees;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;

namespace Robust.Shared.GameObjects;

public abstract class OccluderSystem : ComponentTreeSystem<OccluderTreeComponent, OccluderComponent>
{
	public const float MaxRaycastRange = 100f;

	protected override bool DoFrameUpdate => true;

	protected override bool DoTickUpdate => true;

	protected override bool Recursive => false;

	public override void Initialize()
	{
		base.Initialize();
		SubscribeLocalEvent<OccluderComponent, ComponentGetState>(OnGetState);
		SubscribeLocalEvent<OccluderComponent, ComponentHandleState>(OnHandleState);
	}

	private void OnGetState(EntityUid uid, OccluderComponent comp, ref ComponentGetState args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		args.State = new OccluderComponent.OccluderComponentState(comp.Enabled, comp.BoundingBox);
	}

	private void OnHandleState(EntityUid uid, OccluderComponent comp, ref ComponentHandleState args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (args.Current is OccluderComponent.OccluderComponentState occluderComponentState)
		{
			SetEnabled(uid, occluderComponentState.Enabled, comp);
			SetBoundingBox(uid, occluderComponentState.BoundingBox, comp);
		}
	}

	protected override Box2 ExtractAabb(in ComponentTreeEntry<OccluderComponent> entry)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return ((Box2)(ref entry.Component.BoundingBox)).Translated(entry.Transform.LocalPosition);
	}

	protected override Box2 ExtractAabb(in ComponentTreeEntry<OccluderComponent> entry, Vector2 pos, Angle rot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ExtractAabb(in entry);
	}

	public void SetBoundingBox(EntityUid uid, Box2 box, OccluderComponent? comp = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (Resolve(uid, ref comp))
		{
			comp.BoundingBox = box;
			Dirty(uid, comp);
			if (comp.TreeUid.HasValue)
			{
				QueueTreeUpdate(uid, comp);
			}
		}
	}

	public virtual void SetEnabled(EntityUid uid, bool enabled, OccluderComponent? comp = null, MetaDataComponent? meta = null)
	{
		if (Resolve(uid, ref comp, logMissing: false) && enabled != comp.Enabled)
		{
			comp.Enabled = enabled;
			Dirty(uid, comp, meta);
			QueueTreeUpdate(uid, comp);
		}
	}

	public bool InRangeUnoccluded<TState>(MapCoordinates origin, MapCoordinates other, float range, TState state, Func<Entity<OccluderComponent, TransformComponent>, TState, bool> ignore)
	{
		if (!GetRay(origin, other, range, out var length, out var ray, out var result))
		{
			return result;
		}
		return !IntersectRay(origin.MapId, in ray, length, state, ignore).HasValue;
	}

	public bool InRangeUnoccluded(MapCoordinates origin, MapCoordinates other, float range, bool ignoreTouching)
	{
		if (!GetRay(origin, other, range, out var length, out var ray, out var result))
		{
			return result;
		}
		if (!ignoreTouching)
		{
			return !IntersectRay(origin.MapId, in ray, length).HasValue;
		}
		return !IntersectRay(predicateState: (XformSystem, origin.Position, other.Position), mapId: origin.MapId, ray: in ray, length: length, ignore: IsTouchingEndpoint).HasValue;
	}

	private bool GetRay(MapCoordinates origin, MapCoordinates other, float range, out float length, out Ray ray, out bool result)
	{
		ray = default(Ray);
		length = 0f;
		result = false;
		if (other.MapId != origin.MapId || other.MapId == MapId.Nullspace)
		{
			return false;
		}
		Vector2 vector = other.Position - origin.Position;
		length = vector.Length();
		if (MathHelper.CloseTo(length, 0f, 1E-07f))
		{
			result = true;
			return false;
		}
		Vector2 direction = vector / length;
		if (range > 0f && length > range + 0.01f)
		{
			return false;
		}
		if (length > 100f)
		{
			base.Log.Warning("InRangeUnoccluded check performed over extreme range. Limiting range.");
			length = 100f;
		}
		ray = new Ray(origin.Position, direction);
		return true;
	}

	public static bool IsTouchingEndpoint(Entity<OccluderComponent, TransformComponent> ent, (SharedTransformSystem Sys, Vector2 Start, Vector2 End) state)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Box2 val = ent.Comp1.BoundingBox;
		val = ((Box2)(ref val)).Translated(state.Sys.GetWorldPosition(ent.Comp2));
		if (!((Box2)(ref val)).Contains(state.Start, true))
		{
			return ((Box2)(ref val)).Contains(state.End, true);
		}
		return true;
	}
}
