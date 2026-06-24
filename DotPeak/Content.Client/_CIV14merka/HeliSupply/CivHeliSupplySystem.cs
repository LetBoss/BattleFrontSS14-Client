// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.HeliSupply.CivHeliSupplySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Commander;
using Content.Shared._CIV14merka.HeliSupply;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliSupplySystem : EntitySystem
{
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IInputManager _input;
  [Dependency]
  private IOverlayManager _overlays;
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private SharedTransformSystem _transform;
  private CivHeliFlybyOverlay? _flybyOverlay;
  private CivHeliRouteOverlay? _routeOverlay;
  private bool _isRouteMode;
  private MapId _routeMapId = MapId.Nullspace;
  private readonly List<Vector2> _routePoints = new List<Vector2>();

  public event Action? OnOpenReceived;

  public event Action<CivHeliStateMessage>? OnStateReceived;

  public event Action<bool>? OnRouteModeEnded;

  public CivHeliStateMessage? LastState { get; private set; }

  public bool IsRouteMode => this._isRouteMode;

  public MapId RouteMapId => this._routeMapId;

  public IReadOnlyList<Vector2> RoutePoints => (IReadOnlyList<Vector2>) this._routePoints;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivHeliOpenMessage>(new EntityEventHandler<CivHeliOpenMessage>(this.OnOpen), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivHeliStateMessage>(new EntityEventHandler<CivHeliStateMessage>(this.OnState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CivHeliFlybyEvent>(new EntityEventHandler<CivHeliFlybyEvent>(this.OnFlyby), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    // ISSUE: method pointer
    CommandBinds.Builder.BindBefore(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleUse)), true, true), new Type[2]
    {
      typeof (CivCommanderBotControlSystem),
      typeof (CivCommanderPurchasePlacementSystem)
    }).BindBefore(EngineKeyFunctions.UseSecondary, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate2((object) this, __methodptr(HandleSecondary)), true, true), new Type[2]
    {
      typeof (CivCommanderBotControlSystem),
      typeof (CivCommanderPurchasePlacementSystem)
    }).Register<CivHeliSupplySystem>();
    this._routeOverlay = new CivHeliRouteOverlay(this);
    this._overlays.AddOverlay((Overlay) this._routeOverlay);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<CivHeliSupplySystem>();
    if (this._routeOverlay != null)
    {
      this._overlays.RemoveOverlay((Overlay) this._routeOverlay);
      this._routeOverlay = (CivHeliRouteOverlay) null;
    }
    if (this._flybyOverlay != null)
    {
      foreach (CivHeliFlybyInstance instance in this._flybyOverlay.Instances)
        this.StopSound(instance);
      if (this._overlays.HasOverlay<CivHeliFlybyOverlay>())
        this._overlays.RemoveOverlay((Overlay) this._flybyOverlay);
    }
    this._flybyOverlay = (CivHeliFlybyOverlay) null;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._flybyOverlay == null)
      return;
    TimeSpan realTime = this._timing.RealTime;
    List<CivHeliFlybyInstance> instances = this._flybyOverlay.Instances;
    for (int index = instances.Count - 1; index >= 0; --index)
    {
      CivHeliFlybyInstance inst = instances[index];
      float cost = (float) (realTime - inst.StartTime).TotalSeconds * inst.Speed;
      bool done = (double) cost > (double) inst.Path.TotalCost;
      if (done)
        this.StopSound(inst);
      this.UpdateDust(inst, cost, done, frameTime);
      if (done && inst.Dust.Count == 0)
      {
        this.StopSound(inst);
        instances.RemoveAt(index);
      }
      else if (!done)
      {
        EntityUid? audioEntity = inst.AudioEntity;
        if (audioEntity.HasValue)
        {
          EntityUid valueOrDefault = audioEntity.GetValueOrDefault();
          Vector2 pos;
          inst.Path.SampleByCost(cost, out pos, out Vector2 _);
          if (this.Exists(valueOrDefault))
            this._transform.SetWorldPosition(valueOrDefault, pos);
        }
      }
    }
  }

  private void UpdateDust(CivHeliFlybyInstance inst, float cost, bool done, float frameTime)
  {
    for (int index = inst.Dust.Count - 1; index >= 0; --index)
    {
      CivHeliDustParticle heliDustParticle = inst.Dust[index];
      heliDustParticle.Age += frameTime;
      if ((double) heliDustParticle.Age >= (double) heliDustParticle.Life)
        inst.Dust.RemoveAt(index);
      else
        heliDustParticle.Pos += heliDustParticle.Vel * frameTime;
    }
    if (done || (double) inst.DustRate <= 0.0)
      return;
    inst.DustAccumulator += inst.DustRate * frameTime;
    if ((double) inst.DustAccumulator < 1.0)
      return;
    Vector2 pos;
    inst.Path.SampleByCost(cost, out pos, out Vector2 _);
    while ((double) inst.DustAccumulator >= 1.0)
    {
      --inst.DustAccumulator;
      float x = (float) ((double) this._random.NextFloat() * 3.1415927410125732 * 2.0);
      Vector2 vector2 = new Vector2(MathF.Cos(x), MathF.Sin(x));
      inst.Dust.Add(new CivHeliDustParticle()
      {
        Pos = pos + vector2 * this._random.NextFloat() * 0.9f * inst.Scale,
        Vel = vector2 * (float) (0.5 + (double) this._random.NextFloat() * 1.2000000476837158),
        Life = (float) (0.60000002384185791 + (double) this._random.NextFloat() * 0.60000002384185791),
        Size = (float) (0.699999988079071 + (double) this._random.NextFloat() * 0.60000002384185791)
      });
    }
  }

  private void StopSound(CivHeliFlybyInstance inst)
  {
    EntityUid? audioEntity = inst.AudioEntity;
    if (!audioEntity.HasValue)
      return;
    EntityUid valueOrDefault = audioEntity.GetValueOrDefault();
    if (this.Exists(valueOrDefault) && !this.TerminatingOrDeleted(valueOrDefault, (MetaDataComponent) null))
      this.QueueDel(new EntityUid?(valueOrDefault));
    inst.AudioEntity = new EntityUid?();
  }

  private void OnOpen(CivHeliOpenMessage msg)
  {
    Action onOpenReceived = this.OnOpenReceived;
    if (onOpenReceived == null)
      return;
    onOpenReceived();
  }

  private void OnState(CivHeliStateMessage msg)
  {
    this.LastState = msg;
    Action<CivHeliStateMessage> onStateReceived = this.OnStateReceived;
    if (onStateReceived == null)
      return;
    onStateReceived(msg);
  }

  private void OnFlyby(CivHeliFlybyEvent ev)
  {
    if (this._flybyOverlay == null)
    {
      this._flybyOverlay = new CivHeliFlybyOverlay();
      if (!this._overlays.HasOverlay<CivHeliFlybyOverlay>())
        this._overlays.AddOverlay((Overlay) this._flybyOverlay);
    }
    CivHeliConfigPrototype config = this.GetConfig();
    CivHeliPath civHeliPath = CivHeliRouteMath.Build((IReadOnlyList<Vector2>) ev.Points, config.Smoothing, config.SmoothPasses, config.TurnSlowdown, ev.DropIndex, config.DropSlowZone, config.DropSlowFactor);
    if ((double) civHeliPath.TotalCost <= 0.0 || (double) config.Speed <= 0.0)
      return;
    float num = ev.DropIndex < 0 || ev.DropIndex >= civHeliPath.PointDist.Length ? civHeliPath.Total : civHeliPath.PointDist[ev.DropIndex];
    CivHeliFlybyInstance heliFlybyInstance1 = new CivHeliFlybyInstance()
    {
      Path = civHeliPath,
      DropDist = num,
      Speed = config.Speed,
      Alpha = config.Alpha,
      Scale = config.Scale,
      TakeoffScale = config.TakeoffScale,
      DropScale = config.DropScale,
      TakeoffZone = config.TakeoffZone,
      DropZone = config.DropZone,
      ClimbZone = config.ClimbZone,
      AngleOffset = (float) ((double) config.SpriteAngleDeg * 3.1415927410125732 / 180.0),
      DustRate = config.DustRate,
      Side = ev.Side,
      MapId = ev.MapId,
      StartTime = this._timing.RealTime
    };
    EntityUid? nullable1;
    if (config.FlySound != null && this._map.TryGetMap(new MapId?(ev.MapId), ref nullable1))
    {
      Vector2 pos;
      civHeliPath.SampleByCost(0.0f, out pos, out Vector2 _);
      EntityCoordinates entityCoordinates;
      // ISSUE: explicit constructor call
      ((EntityCoordinates) ref entityCoordinates).\u002Ector(nullable1.Value, pos);
      AudioParams audioParams1 = config.FlySound.Params;
      AudioParams audioParams2 = ((AudioParams) ref audioParams1).WithLoop(true);
      CivHeliFlybyInstance heliFlybyInstance2 = heliFlybyInstance1;
      (EntityUid, AudioComponent)? nullable2 = this._audio.PlayStatic(config.FlySound, Filter.Local(), entityCoordinates, false, new AudioParams?(audioParams2));
      ref (EntityUid, AudioComponent)? local = ref nullable2;
      EntityUid? nullable3 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
      heliFlybyInstance2.AudioEntity = nullable3;
    }
    this._flybyOverlay.Instances.Add(heliFlybyInstance1);
  }

  public void RequestAdd(string protoId, int amount = 1)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivHeliCargoAddMessage()
    {
      ProtoId = protoId,
      Amount = amount
    });
  }

  public void RequestRemove(string protoId)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivHeliCargoRemoveMessage()
    {
      ProtoId = protoId
    });
  }

  public void RequestCancel()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivHeliCancelMessage());
  }

  public void StartRouteMode()
  {
    this._isRouteMode = true;
    this._routeMapId = MapId.Nullspace;
    this._routePoints.Clear();
  }

  public void CancelRouteMode() => this.EndRouteMode(false);

  public void FinishRoute()
  {
    bool launched = this._isRouteMode && this._routePoints.Count > 0 && MapId.op_Inequality(this._routeMapId, MapId.Nullspace);
    if (launched)
      this.RaiseNetworkEvent((EntityEventArgs) new CivHeliLaunchMessage()
      {
        Points = new List<Vector2>((IEnumerable<Vector2>) this._routePoints),
        MapId = this._routeMapId
      });
    this.EndRouteMode(launched);
  }

  private void EndRouteMode(bool launched)
  {
    if (!this._isRouteMode)
      return;
    this._isRouteMode = false;
    this._routePoints.Clear();
    Action<bool> onRouteModeEnded = this.OnRouteModeEnded;
    if (onRouteModeEnded == null)
      return;
    onRouteModeEnded(launched);
  }

  public float? EstimateEta()
  {
    if (this._routePoints.Count == 0)
      return new float?();
    CivHeliConfigPrototype config = this.GetConfig();
    if ((double) config.Speed <= 0.0)
      return new float?();
    List<Vector2> points = new List<Vector2>(this._routePoints.Count + 2);
    CivHeliStateMessage lastState = this.LastState;
    if (lastState != null && lastState.HasOrigin && MapId.op_Equality(lastState.OriginMapId, this._routeMapId))
      points.Add(lastState.Origin);
    points.AddRange((IEnumerable<Vector2>) this._routePoints);
    if (points.Count < 2)
      return new float?();
    List<Vector2> vector2List1 = points;
    Vector2 vector2_1 = vector2List1[vector2List1.Count - 1];
    List<Vector2> vector2List2 = points;
    Vector2 vector2_2 = vector2List2[vector2List2.Count - 2];
    Vector2 vector2_3 = vector2_1 - vector2_2;
    float num = vector2_3.Length();
    vector2_3 = (double) num > 1.0 / 1000.0 ? vector2_3 / num : new Vector2(0.0f, 1f);
    List<Vector2> vector2List3 = points;
    List<Vector2> vector2List4 = points;
    Vector2 vector2_4 = vector2List4[vector2List4.Count - 1] + vector2_3 * 10f;
    vector2List3.Add(vector2_4);
    int fixedIndex = points.Count - 2;
    CivHeliPath civHeliPath = CivHeliRouteMath.Build((IReadOnlyList<Vector2>) points, config.Smoothing, config.SmoothPasses, config.TurnSlowdown, fixedIndex, config.DropSlowZone, config.DropSlowFactor);
    return new float?(civHeliPath.CostAtDist(civHeliPath.PointDist[fixedIndex]) / config.Speed);
  }

  public CivHeliConfigPrototype GetConfig()
  {
    CivHeliConfigPrototype heliConfigPrototype;
    return this._proto.TryIndex<CivHeliConfigPrototype>("CivHeliDefault", ref heliConfigPrototype) ? heliConfigPrototype : new CivHeliConfigPrototype();
  }

  public void GetActiveHeliBlips(MapId mapId, List<HeliMapBlip> into)
  {
    if (this._flybyOverlay == null)
      return;
    TimeSpan realTime = this._timing.RealTime;
    foreach (CivHeliFlybyInstance instance in this._flybyOverlay.Instances)
    {
      if (!MapId.op_Inequality(instance.MapId, mapId))
      {
        float cost = (float) (realTime - instance.StartTime).TotalSeconds * instance.Speed;
        if ((double) cost >= 0.0 && (double) cost <= (double) instance.Path.TotalCost)
        {
          Vector2 pos;
          Vector2 tangent;
          instance.Path.SampleByCost(cost, out pos, out tangent);
          into.Add(new HeliMapBlip(pos, tangent, instance.Side));
        }
      }
    }
  }

  public Vector2 GetCursorWorldPosition()
  {
    return this._eye.PixelToMap(this._input.MouseScreenPosition).Position;
  }

  public MapId GetCursorMapId() => this._eye.PixelToMap(this._input.MouseScreenPosition).MapId;

  private bool HandleUse(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!this._isRouteMode || args.State != 1 || !this.IsViewportHover())
      return false;
    MapCoordinates map = this._eye.PixelToMap(this._input.MouseScreenPosition);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace))
      return false;
    if (this._routePoints.Count == 0)
      this._routeMapId = map.MapId;
    else if (MapId.op_Inequality(map.MapId, this._routeMapId))
      return true;
    CivHeliStateMessage lastState = this.LastState;
    if (this._routePoints.Count >= (lastState != null ? lastState.MaxWaypoints : 12))
      return true;
    this._routePoints.Add(map.Position);
    return true;
  }

  private bool HandleSecondary(in PointerInputCmdHandler.PointerInputCmdArgs args)
  {
    if (!this._isRouteMode || args.State != 1 || !this.IsViewportHover())
      return false;
    if (this._routePoints.Count > 0)
      this._routePoints.RemoveAt(this._routePoints.Count - 1);
    return true;
  }

  private bool IsViewportHover()
  {
    return this._ui.CurrentlyHovered == null || this._ui.CurrentlyHovered is IViewportControl;
  }
}
