using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._CIV14merka.Commander;
using Content.Shared._CIV14merka.HeliSupply;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Audio;
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

	public CivHeliStateMessage? LastState { get; private set; }

	public bool IsRouteMode => _isRouteMode;

	public MapId RouteMapId => _routeMapId;

	public IReadOnlyList<Vector2> RoutePoints => _routePoints;

	public event Action? OnOpenReceived;

	public event Action<CivHeliStateMessage>? OnStateReceived;

	public event Action<bool>? OnRouteModeEnded;

	public override void Initialize()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<CivHeliOpenMessage>((EntityEventHandler<CivHeliOpenMessage>)OnOpen, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivHeliStateMessage>((EntityEventHandler<CivHeliStateMessage>)OnState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<CivHeliFlybyEvent>((EntityEventHandler<CivHeliFlybyEvent>)OnFlyby, (Type[])null, (Type[])null);
		CommandBinds.Builder.BindBefore(EngineKeyFunctions.Use, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleUse), true, true), new Type[2]
		{
			typeof(CivCommanderBotControlSystem),
			typeof(CivCommanderPurchasePlacementSystem)
		}).BindBefore(EngineKeyFunctions.UseSecondary, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate2(HandleSecondary), true, true), new Type[2]
		{
			typeof(CivCommanderBotControlSystem),
			typeof(CivCommanderPurchasePlacementSystem)
		}).Register<CivHeliSupplySystem>();
		_routeOverlay = new CivHeliRouteOverlay(this);
		_overlays.AddOverlay((Overlay)(object)_routeOverlay);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CommandBinds.Unregister<CivHeliSupplySystem>();
		if (_routeOverlay != null)
		{
			_overlays.RemoveOverlay((Overlay)(object)_routeOverlay);
			_routeOverlay = null;
		}
		if (_flybyOverlay != null)
		{
			foreach (CivHeliFlybyInstance instance in _flybyOverlay.Instances)
			{
				StopSound(instance);
			}
			if (_overlays.HasOverlay<CivHeliFlybyOverlay>())
			{
				_overlays.RemoveOverlay((Overlay)(object)_flybyOverlay);
			}
		}
		_flybyOverlay = null;
	}

	public override void Update(float frameTime)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_flybyOverlay == null)
		{
			return;
		}
		TimeSpan realTime = _timing.RealTime;
		List<CivHeliFlybyInstance> instances = _flybyOverlay.Instances;
		for (int num = instances.Count - 1; num >= 0; num--)
		{
			CivHeliFlybyInstance civHeliFlybyInstance = instances[num];
			float num2 = (float)(realTime - civHeliFlybyInstance.StartTime).TotalSeconds * civHeliFlybyInstance.Speed;
			bool flag = num2 > civHeliFlybyInstance.Path.TotalCost;
			if (flag)
			{
				StopSound(civHeliFlybyInstance);
			}
			UpdateDust(civHeliFlybyInstance, num2, flag, frameTime);
			if (flag && civHeliFlybyInstance.Dust.Count == 0)
			{
				StopSound(civHeliFlybyInstance);
				instances.RemoveAt(num);
			}
			else if (!flag)
			{
				EntityUid? audioEntity = civHeliFlybyInstance.AudioEntity;
				if (audioEntity.HasValue)
				{
					EntityUid valueOrDefault = audioEntity.GetValueOrDefault();
					civHeliFlybyInstance.Path.SampleByCost(num2, out var pos, out var _);
					if (((EntitySystem)this).Exists(valueOrDefault))
					{
						_transform.SetWorldPosition(valueOrDefault, pos);
					}
				}
			}
		}
	}

	private void UpdateDust(CivHeliFlybyInstance inst, float cost, bool done, float frameTime)
	{
		for (int num = inst.Dust.Count - 1; num >= 0; num--)
		{
			CivHeliDustParticle civHeliDustParticle = inst.Dust[num];
			civHeliDustParticle.Age += frameTime;
			if (civHeliDustParticle.Age >= civHeliDustParticle.Life)
			{
				inst.Dust.RemoveAt(num);
			}
			else
			{
				civHeliDustParticle.Pos += civHeliDustParticle.Vel * frameTime;
			}
		}
		if (done || inst.DustRate <= 0f)
		{
			return;
		}
		inst.DustAccumulator += inst.DustRate * frameTime;
		if (!(inst.DustAccumulator < 1f))
		{
			inst.Path.SampleByCost(cost, out var pos, out var _);
			while (inst.DustAccumulator >= 1f)
			{
				inst.DustAccumulator -= 1f;
				float x = _random.NextFloat() * MathF.PI * 2f;
				Vector2 vector = new Vector2(MathF.Cos(x), MathF.Sin(x));
				inst.Dust.Add(new CivHeliDustParticle
				{
					Pos = pos + vector * _random.NextFloat() * 0.9f * inst.Scale,
					Vel = vector * (0.5f + _random.NextFloat() * 1.2f),
					Life = 0.6f + _random.NextFloat() * 0.6f,
					Size = 0.7f + _random.NextFloat() * 0.6f
				});
			}
		}
	}

	private void StopSound(CivHeliFlybyInstance inst)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? audioEntity = inst.AudioEntity;
		if (audioEntity.HasValue)
		{
			EntityUid valueOrDefault = audioEntity.GetValueOrDefault();
			if (((EntitySystem)this).Exists(valueOrDefault) && !((EntitySystem)this).TerminatingOrDeleted(valueOrDefault, (MetaDataComponent)null))
			{
				((EntitySystem)this).QueueDel((EntityUid?)valueOrDefault);
			}
			inst.AudioEntity = null;
		}
	}

	private void OnOpen(CivHeliOpenMessage msg)
	{
		this.OnOpenReceived?.Invoke();
	}

	private void OnState(CivHeliStateMessage msg)
	{
		LastState = msg;
		this.OnStateReceived?.Invoke(msg);
	}

	private void OnFlyby(CivHeliFlybyEvent ev)
	{
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		if (_flybyOverlay == null)
		{
			_flybyOverlay = new CivHeliFlybyOverlay();
			if (!_overlays.HasOverlay<CivHeliFlybyOverlay>())
			{
				_overlays.AddOverlay((Overlay)(object)_flybyOverlay);
			}
		}
		CivHeliConfigPrototype config = GetConfig();
		CivHeliPath civHeliPath = CivHeliRouteMath.Build(ev.Points, config.Smoothing, config.SmoothPasses, config.TurnSlowdown, ev.DropIndex, config.DropSlowZone, config.DropSlowFactor);
		if (!(civHeliPath.TotalCost <= 0f) && !(config.Speed <= 0f))
		{
			float dropDist = ((ev.DropIndex >= 0 && ev.DropIndex < civHeliPath.PointDist.Length) ? civHeliPath.PointDist[ev.DropIndex] : civHeliPath.Total);
			CivHeliFlybyInstance civHeliFlybyInstance = new CivHeliFlybyInstance
			{
				Path = civHeliPath,
				DropDist = dropDist,
				Speed = config.Speed,
				Alpha = config.Alpha,
				Scale = config.Scale,
				TakeoffScale = config.TakeoffScale,
				DropScale = config.DropScale,
				TakeoffZone = config.TakeoffZone,
				DropZone = config.DropZone,
				ClimbZone = config.ClimbZone,
				AngleOffset = config.SpriteAngleDeg * MathF.PI / 180f,
				DustRate = config.DustRate,
				Side = ev.Side,
				MapId = ev.MapId,
				StartTime = _timing.RealTime
			};
			EntityUid? val = default(EntityUid?);
			if (config.FlySound != null && _map.TryGetMap((MapId?)ev.MapId, ref val))
			{
				civHeliPath.SampleByCost(0f, out var pos, out var _);
				EntityCoordinates val2 = default(EntityCoordinates);
				((EntityCoordinates)(ref val2))._002Ector(val.Value, pos);
				AudioParams val3 = config.FlySound.Params;
				AudioParams value = ((AudioParams)(ref val3)).WithLoop(true);
				civHeliFlybyInstance.AudioEntity = _audio.PlayStatic(config.FlySound, Filter.Local(), val2, false, (AudioParams?)value)?.Item1;
			}
			_flybyOverlay.Instances.Add(civHeliFlybyInstance);
		}
	}

	public void RequestAdd(string protoId, int amount = 1)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivHeliCargoAddMessage
		{
			ProtoId = protoId,
			Amount = amount
		});
	}

	public void RequestRemove(string protoId)
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivHeliCargoRemoveMessage
		{
			ProtoId = protoId
		});
	}

	public void RequestCancel()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivHeliCancelMessage());
	}

	public void StartRouteMode()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_isRouteMode = true;
		_routeMapId = MapId.Nullspace;
		_routePoints.Clear();
	}

	public void CancelRouteMode()
	{
		EndRouteMode(launched: false);
	}

	public void FinishRoute()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = _isRouteMode && _routePoints.Count > 0 && _routeMapId != MapId.Nullspace;
		if (flag)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new CivHeliLaunchMessage
			{
				Points = new List<Vector2>(_routePoints),
				MapId = _routeMapId
			});
		}
		EndRouteMode(flag);
	}

	private void EndRouteMode(bool launched)
	{
		if (_isRouteMode)
		{
			_isRouteMode = false;
			_routePoints.Clear();
			this.OnRouteModeEnded?.Invoke(launched);
		}
	}

	public float? EstimateEta()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (_routePoints.Count == 0)
		{
			return null;
		}
		CivHeliConfigPrototype config = GetConfig();
		if (config.Speed <= 0f)
		{
			return null;
		}
		List<Vector2> list = new List<Vector2>(_routePoints.Count + 2);
		CivHeliStateMessage lastState = LastState;
		if (lastState != null && lastState.HasOrigin && lastState.OriginMapId == _routeMapId)
		{
			list.Add(lastState.Origin);
		}
		list.AddRange(_routePoints);
		if (list.Count < 2)
		{
			return null;
		}
		Vector2 vector = list[list.Count - 1] - list[list.Count - 2];
		float num = vector.Length();
		vector = ((num > 0.001f) ? (vector / num) : new Vector2(0f, 1f));
		list.Add(list[list.Count - 1] + vector * 10f);
		int num2 = list.Count - 2;
		CivHeliPath civHeliPath = CivHeliRouteMath.Build(list, config.Smoothing, config.SmoothPasses, config.TurnSlowdown, num2, config.DropSlowZone, config.DropSlowFactor);
		return civHeliPath.CostAtDist(civHeliPath.PointDist[num2]) / config.Speed;
	}

	public CivHeliConfigPrototype GetConfig()
	{
		CivHeliConfigPrototype result = default(CivHeliConfigPrototype);
		if (_proto.TryIndex<CivHeliConfigPrototype>("CivHeliDefault", ref result))
		{
			return result;
		}
		return new CivHeliConfigPrototype();
	}

	public void GetActiveHeliBlips(MapId mapId, List<HeliMapBlip> into)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (_flybyOverlay == null)
		{
			return;
		}
		TimeSpan realTime = _timing.RealTime;
		foreach (CivHeliFlybyInstance instance in _flybyOverlay.Instances)
		{
			if (!(instance.MapId != mapId))
			{
				float num = (float)(realTime - instance.StartTime).TotalSeconds * instance.Speed;
				if (!(num < 0f) && !(num > instance.Path.TotalCost))
				{
					instance.Path.SampleByCost(num, out var pos, out var tangent);
					into.Add(new HeliMapBlip(pos, tangent, instance.Side));
				}
			}
		}
	}

	public Vector2 GetCursorWorldPosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return _eye.PixelToMap(_input.MouseScreenPosition).Position;
	}

	public MapId GetCursorMapId()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return _eye.PixelToMap(_input.MouseScreenPosition).MapId;
	}

	private bool HandleUse(in PointerInputCmdArgs args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!_isRouteMode || (int)args.State != 1 || !IsViewportHover())
		{
			return false;
		}
		MapCoordinates val = _eye.PixelToMap(_input.MouseScreenPosition);
		if (val.MapId == MapId.Nullspace)
		{
			return false;
		}
		if (_routePoints.Count == 0)
		{
			_routeMapId = val.MapId;
		}
		else if (val.MapId != _routeMapId)
		{
			return true;
		}
		int num = LastState?.MaxWaypoints ?? 12;
		if (_routePoints.Count >= num)
		{
			return true;
		}
		_routePoints.Add(val.Position);
		return true;
	}

	private bool HandleSecondary(in PointerInputCmdArgs args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		if (!_isRouteMode || (int)args.State != 1 || !IsViewportHover())
		{
			return false;
		}
		if (_routePoints.Count > 0)
		{
			_routePoints.RemoveAt(_routePoints.Count - 1);
		}
		return true;
	}

	private bool IsViewportHover()
	{
		if (_ui.CurrentlyHovered != null)
		{
			return _ui.CurrentlyHovered is IViewportControl;
		}
		return true;
	}
}
