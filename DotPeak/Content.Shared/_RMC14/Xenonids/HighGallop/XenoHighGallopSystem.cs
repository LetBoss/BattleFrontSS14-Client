// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.HighGallop.XenoHighGallopSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared.Maps;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.HighGallop;

public sealed class XenoHighGallopSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private RMCPullingSystem _pulling;
  [Dependency]
  private TagSystem _tags;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private RMCSizeStunSystem _size;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoHighGallopComponent, XenoHighGallopActionEvent>(new EntityEventRefHandler<XenoHighGallopComponent, XenoHighGallopActionEvent>(this.OnHighGallopAction));
  }

  private void OnHighGallopAction(
    Entity<XenoHighGallopComponent> xeno,
    ref XenoHighGallopActionEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? grid = this._transform.GetGrid(args.Target);
    if (!grid.HasValue)
      return;
    EntityUid valueOrDefault = grid.GetValueOrDefault();
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      return;
    args.Handled = true;
    this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.Emote);
    this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    Angle angle = Angle.op_Subtraction(DirectionExtensions.ToAngle(Vector2Helpers.Normalized(args.Target.Position - this._transform.GetMoverCoordinates((EntityUid) xeno).Position)), Angle.FromDegrees(90.0));
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) xeno);
    Box2 box2_1 = Box2.CenteredAround(moverCoordinates.Position, new Vector2(xeno.Comp.Width, xeno.Comp.Height));
    Box2 box2_2 = ((Box2) ref box2_1).Translated(new Vector2(0.0f, (float) ((double) xeno.Comp.Height / 2.0 + 0.5)));
    Box2Rotated box2Rotated;
    // ISSUE: explicit constructor call
    ((Box2Rotated) ref box2Rotated).\u002Ector(box2_2, angle, moverCoordinates.Position);
    Box2 box2_3 = ((Box2Rotated) ref box2Rotated).CalcBoundingBox();
    if (this._net.IsClient)
      return;
    foreach (TileRef turf in this._map.GetTilesIntersecting(valueOrDefault, comp, box2Rotated))
    {
      EntProtoId prototype = xeno.Comp.TelegraphEffect;
      ref Box2 local1 = ref box2_3;
      Box2 box2_4 = Box2.CenteredAround(this._turf.GetTileCenter(turf).Position, Vector2.One);
      ref Box2 local2 = ref box2_4;
      if (!((Box2) ref local1).Encloses(ref local2))
        prototype = xeno.Comp.TelegraphEffectEdge;
      this.SpawnAtPosition((string) prototype, this._turf.GetTileCenter(turf));
    }
    foreach (EntityUid entityUid in this._lookup.GetEntitiesIntersecting(this.Transform((EntityUid) xeno).MapID, box2Rotated))
    {
      if (this._tags.HasTag(entityUid, xeno.Comp.Flingable))
      {
        this._pulling.TryStopAllPullsFromAndOn(entityUid);
        MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
        this._size.KnockBack(entityUid, new MapCoordinates?(mapCoordinates), xeno.Comp.FlingDistance, xeno.Comp.FlingDistance, 10f, true);
      }
      else
      {
        RMCSizes size;
        if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, entityUid) && (!this._size.TryGetSize(entityUid, out size) || size < RMCSizes.Big))
        {
          this._stun.TryParalyze(entityUid, this._xeno.TryApplyXenoDebuffMultiplier(entityUid, xeno.Comp.StunDuration), true);
          this._slow.TrySlowdown(entityUid, this._xeno.TryApplyXenoDebuffMultiplier(entityUid, xeno.Comp.SlowDuration));
        }
      }
    }
  }
}
