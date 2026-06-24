// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.OccluderSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.ComponentTrees;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class OccluderSystem : ComponentTreeSystem<OccluderTreeComponent, OccluderComponent>
{
  public const float MaxRaycastRange = 100f;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<OccluderComponent, ComponentGetState>(new ComponentEventRefHandler<OccluderComponent, ComponentGetState>(this.OnGetState));
    this.SubscribeLocalEvent<OccluderComponent, ComponentHandleState>(new ComponentEventRefHandler<OccluderComponent, ComponentHandleState>(this.OnHandleState));
  }

  private void OnGetState(EntityUid uid, OccluderComponent comp, ref ComponentGetState args)
  {
    args.State = (IComponentState) new OccluderComponent.OccluderComponentState(comp.Enabled, comp.BoundingBox);
  }

  private void OnHandleState(EntityUid uid, OccluderComponent comp, ref ComponentHandleState args)
  {
    if (!(args.Current is OccluderComponent.OccluderComponentState current))
      return;
    this.SetEnabled(uid, current.Enabled, comp);
    this.SetBoundingBox(uid, current.BoundingBox, comp);
  }

  protected override bool DoFrameUpdate => true;

  protected override bool DoTickUpdate => true;

  protected override bool Recursive => false;

  protected override Box2 ExtractAabb(in ComponentTreeEntry<OccluderComponent> entry)
  {
    return ((Box2) ref entry.Component.BoundingBox).Translated(entry.Transform.LocalPosition);
  }

  protected override Box2 ExtractAabb(
    in ComponentTreeEntry<OccluderComponent> entry,
    Vector2 pos,
    Angle rot)
  {
    return this.ExtractAabb(in entry);
  }

  public void SetBoundingBox(EntityUid uid, Box2 box, OccluderComponent? comp = null)
  {
    if (!this.Resolve<OccluderComponent>(uid, ref comp))
      return;
    comp.BoundingBox = box;
    this.Dirty(uid, (IComponent) comp);
    if (!comp.TreeUid.HasValue)
      return;
    this.QueueTreeUpdate(uid, comp);
  }

  public virtual void SetEnabled(
    EntityUid uid,
    bool enabled,
    OccluderComponent? comp = null,
    MetaDataComponent? meta = null)
  {
    if (!this.Resolve<OccluderComponent>(uid, ref comp, false) || enabled == comp.Enabled)
      return;
    comp.Enabled = enabled;
    this.Dirty(uid, (IComponent) comp, meta);
    this.QueueTreeUpdate(uid, comp);
  }

  public bool InRangeUnoccluded<TState>(
    MapCoordinates origin,
    MapCoordinates other,
    float range,
    TState state,
    Func<Entity<OccluderComponent, TransformComponent>, TState, bool> ignore)
  {
    float length;
    Ray ray;
    bool result;
    return !this.GetRay(origin, other, range, out length, out ray, out result) ? result : !this.IntersectRay<TState>(origin.MapId, in ray, length, state, ignore).HasValue;
  }

  public bool InRangeUnoccluded(
    MapCoordinates origin,
    MapCoordinates other,
    float range,
    bool ignoreTouching)
  {
    float length;
    Ray ray;
    bool result;
    if (!this.GetRay(origin, other, range, out length, out ray, out result))
      return result;
    if (!ignoreTouching)
      return !this.IntersectRay(origin.MapId, in ray, length).HasValue;
    (SharedTransformSystem, Vector2, Vector2) predicateState = (this.XformSystem, origin.Position, other.Position);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return !this.IntersectRay<(SharedTransformSystem, Vector2, Vector2)>(origin.MapId, in ray, length, predicateState, OccluderSystem.\u003C\u003EO.\u003C0\u003E__IsTouchingEndpoint ?? (OccluderSystem.\u003C\u003EO.\u003C0\u003E__IsTouchingEndpoint = new Func<Entity<OccluderComponent, TransformComponent>, (SharedTransformSystem, Vector2, Vector2), bool>(OccluderSystem.IsTouchingEndpoint))).HasValue;
  }

  private bool GetRay(
    MapCoordinates origin,
    MapCoordinates other,
    float range,
    out float length,
    out Ray ray,
    out bool result)
  {
    ray = new Ray();
    length = 0.0f;
    result = false;
    if (other.MapId != origin.MapId || other.MapId == MapId.Nullspace)
      return false;
    Vector2 vector2 = other.Position - origin.Position;
    length = vector2.Length();
    if (MathHelper.CloseTo(length, 0.0f, 1E-07f))
    {
      result = true;
      return false;
    }
    Vector2 direction = vector2 / length;
    if ((double) range > 0.0 && (double) length > (double) range + 0.0099999997764825821)
      return false;
    if ((double) length > 100.0)
    {
      this.Log.Warning("InRangeUnoccluded check performed over extreme range. Limiting range.");
      length = 100f;
    }
    ray = new Ray(origin.Position, direction);
    return true;
  }

  public static bool IsTouchingEndpoint(
    Entity<OccluderComponent, TransformComponent> ent,
    (SharedTransformSystem Sys, Vector2 Start, Vector2 End) state)
  {
    Box2 box2 = ent.Comp1.BoundingBox;
    box2 = ((Box2) ref box2).Translated(state.Sys.GetWorldPosition(ent.Comp2));
    return ((Box2) ref box2).Contains(state.Start, true) || ((Box2) ref box2).Contains(state.End, true);
  }
}
