using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Areas;
using Content.Shared.GameTicking;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Camera;

public abstract class SharedRMCCameraSystem : EntitySystem
{
	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private MetaDataSystem _metaData;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	private readonly HashSet<EntProtoId> _refresh = new HashSet<EntProtoId>();

	private readonly Dictionary<string, int> _cameraNames = new Dictionary<string, int>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCCameraComponent, MapInitEvent>((EntityEventRefHandler<RMCCameraComponent, MapInitEvent>)OnCameraMapInit, (Type[])null, new Type[1] { typeof(AreaSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCCameraComponent, ComponentRemove>((EntityEventRefHandler<RMCCameraComponent, ComponentRemove>)OnCameraRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCCameraComponent, EntityTerminatingEvent>((EntityEventRefHandler<RMCCameraComponent, EntityTerminatingEvent>)OnCameraTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCCameraComputerComponent, MapInitEvent>((EntityEventRefHandler<RMCCameraComputerComponent, MapInitEvent>)OnComputerMapInit, (Type[])null, new Type[1] { typeof(AreaSystem) });
		((EntitySystem)this).SubscribeLocalEvent<RMCCameraWatcherComponent, ComponentRemove>((EntityEventRefHandler<RMCCameraWatcherComponent, ComponentRemove>)OnWatcherRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCCameraWatcherComponent, EntityTerminatingEvent>((EntityEventRefHandler<RMCCameraWatcherComponent, EntityTerminatingEvent>)OnWatcherTerminating, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<RMCCameraComputerComponent>(((EntitySystem)this).Subs, (object)RMCCameraUiKey.Key, (BuiEventSubscriber<RMCCameraComputerComponent>)delegate(Subscriber<RMCCameraComputerComponent> subs)
		{
			subs.Event<BoundUIOpenedEvent>((EntityEventRefHandler<RMCCameraComputerComponent, BoundUIOpenedEvent>)OnComputerBuiOpened);
			subs.Event<BoundUIClosedEvent>((EntityEventRefHandler<RMCCameraComputerComponent, BoundUIClosedEvent>)OnComputerBuiClosed);
			subs.Event<RMCCameraWatchBuiMsg>((EntityEventRefHandler<RMCCameraComputerComponent, RMCCameraWatchBuiMsg>)OnComputerWatchBuiMsg);
			subs.Event<RMCCameraPreviousBuiMsg>((EntityEventRefHandler<RMCCameraComputerComponent, RMCCameraPreviousBuiMsg>)OnComputerPreviousBuiMsg);
			subs.Event<RMCCameraNextBuiMsg>((EntityEventRefHandler<RMCCameraComputerComponent, RMCCameraNextBuiMsg>)OnComputerNextBuiMsg);
		});
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_cameraNames.Clear();
	}

	private void OnCameraMapInit(Entity<RMCCameraComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		EntProtoId? id = ent.Comp.Id;
		if (id.HasValue)
		{
			EntProtoId id2 = id.GetValueOrDefault();
			_refresh.Add(id2);
		}
		if (ent.Comp.Rename)
		{
			if (_area.TryGetArea(Entity<RMCCameraComponent>.op_Implicit(ent), out Entity<AreaComponent>? _, out EntityPrototype areaProto))
			{
				string areaName = areaProto.Name;
				int count = _cameraNames.GetValueOrDefault(areaName);
				_metaData.SetEntityName(Entity<RMCCameraComponent>.op_Implicit(ent), $"{areaName} #{++count}", (MetaDataComponent)null, true);
				_cameraNames[areaName] = count;
			}
		}
		else
		{
			string name = ((EntitySystem)this).Name(Entity<RMCCameraComponent>.op_Implicit(ent), (MetaDataComponent)null);
			int count2 = _cameraNames.GetValueOrDefault(name);
			_cameraNames[name] = count2;
		}
	}

	private void OnCameraRemove(Entity<RMCCameraComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnCameraRemoved(ent);
	}

	private void OnCameraTerminating(Entity<RMCCameraComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnCameraRemoved(ent);
	}

	private void OnComputerMapInit(Entity<RMCCameraComputerComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.CameraIds.Clear();
		ent.Comp.CameraNames.Clear();
		EntityQueryEnumerator<RMCCameraComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RMCCameraComponent>();
		EntityUid uid = default(EntityUid);
		RMCCameraComponent camera = default(RMCCameraComponent);
		while (query.MoveNext(ref uid, ref camera))
		{
			foreach (EntProtoId protoId in ent.Comp.ProtoIds)
			{
				EntProtoId? id = camera.Id;
				EntProtoId val = protoId;
				if (id.HasValue && !(id.GetValueOrDefault() != val))
				{
					ent.Comp.CameraIds.Add(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null));
					ent.Comp.CameraNames.Add(((EntitySystem)this).Name(uid, (MetaDataComponent)null));
				}
			}
		}
		((EntitySystem)this).Dirty<RMCCameraComputerComponent>(ent, (MetaDataComponent)null);
	}

	private void OnWatcherRemove(Entity<RMCCameraWatcherComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnWatcherRemoved(ent);
	}

	private void OnWatcherTerminating(Entity<RMCCameraWatcherComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		OnWatcherRemoved(ent);
	}

	private void OnComputerBuiOpened(Entity<RMCCameraComputerComponent> ent, ref BoundUIOpenedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
			ent.Comp.Watchers.Add(actor);
			((EntitySystem)this).Dirty<RMCCameraComputerComponent>(ent, (MetaDataComponent)null);
			RMCCameraWatcherComponent watcher = ((EntitySystem)this).EnsureComp<RMCCameraWatcherComponent>(actor);
			watcher.Computer = null;
			((EntitySystem)this).Dirty(actor, (IComponent)(object)watcher, (MetaDataComponent)null);
			Refresh(ent, null);
		}
	}

	private void OnComputerBuiClosed(Entity<RMCCameraComputerComponent> ent, ref BoundUIClosedEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
			ent.Comp.Watchers.Remove(actor);
			((EntitySystem)this).Dirty<RMCCameraComputerComponent>(ent, (MetaDataComponent)null);
			((EntitySystem)this).RemCompDeferred<RMCCameraWatcherComponent>(actor);
		}
	}

	private void OnComputerWatchBuiMsg(Entity<RMCCameraComputerComponent> ent, ref RMCCameraWatchBuiMsg args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? camera = default(EntityUid?);
		if (!_timing.ApplyingState && ((EntitySystem)this).TryGetEntity(args.Camera, ref camera) && ent.Comp.CameraIds.Contains(args.Camera))
		{
			EntityUid? old = ent.Comp.CurrentCamera;
			ent.Comp.CurrentCamera = camera;
			Refresh(ent, old);
		}
	}

	private void OnComputerPreviousBuiMsg(Entity<RMCCameraComputerComponent> ent, ref RMCCameraPreviousBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? old = ent.Comp.CurrentCamera;
		int index = 0;
		NetEntity? netCamera = default(NetEntity?);
		if (old.HasValue && ((EntitySystem)this).TryGetNetEntity(old, ref netCamera, (MetaDataComponent)null))
		{
			index = ent.Comp.CameraIds.IndexOf(netCamera.Value) - 1;
			if (index < 0 || index >= ent.Comp.CameraIds.Count)
			{
				index = ent.Comp.CameraIds.Count - 1;
			}
		}
		EntityUid? camera = default(EntityUid?);
		if (index >= 0 && index < ent.Comp.CameraIds.Count && ((EntitySystem)this).TryGetEntity(ent.Comp.CameraIds[index], ref camera))
		{
			ent.Comp.CurrentCamera = camera;
		}
		Refresh(ent, old);
	}

	private void OnComputerNextBuiMsg(Entity<RMCCameraComputerComponent> ent, ref RMCCameraNextBuiMsg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? old = ent.Comp.CurrentCamera;
		int index = 0;
		NetEntity? netCamera = default(NetEntity?);
		if (old.HasValue && ((EntitySystem)this).TryGetNetEntity(old, ref netCamera, (MetaDataComponent)null))
		{
			index = ent.Comp.CameraIds.IndexOf(netCamera.Value) + 1;
			if (index < 0 || index >= ent.Comp.CameraIds.Count)
			{
				index = 0;
			}
		}
		EntityUid? camera = default(EntityUid?);
		if (index >= 0 && index < ent.Comp.CameraIds.Count && ((EntitySystem)this).TryGetEntity(ent.Comp.CameraIds[index], ref camera))
		{
			ent.Comp.CurrentCamera = camera;
		}
		Refresh(ent, old);
	}

	protected virtual void Refresh(Entity<RMCCameraComputerComponent> ent, EntityUid? old)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Dirty<RMCCameraComputerComponent>(ent, (MetaDataComponent)null);
	}

	protected virtual void OnWatcherRemoved(Entity<RMCCameraWatcherComponent> watcher)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		RMCCameraComputerComponent computer = default(RMCCameraComputerComponent);
		if (((EntitySystem)this).TryComp<RMCCameraComputerComponent>(watcher.Comp.Computer, ref computer))
		{
			computer.Watchers.Remove(Entity<RMCCameraWatcherComponent>.op_Implicit(watcher));
			((EntitySystem)this).Dirty(watcher.Comp.Computer.Value, (IComponent)(object)computer, (MetaDataComponent)null);
		}
	}

	public bool GetComputerCameraName(Entity<RMCCameraComputerComponent> computer, EntityUid camera, [NotNullWhen(true)] out string? name)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		int index = computer.Comp.CameraIds.IndexOf(((EntitySystem)this).GetNetEntity(camera, (MetaDataComponent)null));
		if (index < 0)
		{
			name = null;
			return false;
		}
		name = computer.Comp.CameraNames[index];
		return true;
	}

	private void OnCameraRemoved(Entity<RMCCameraComponent> camera)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		NetEntity netCamera = ((EntitySystem)this).GetNetEntity(Entity<RMCCameraComponent>.op_Implicit(camera), (MetaDataComponent)null);
		EntityQueryEnumerator<RMCCameraComputerComponent> computers = ((EntitySystem)this).EntityQueryEnumerator<RMCCameraComputerComponent>();
		EntityUid uid = default(EntityUid);
		RMCCameraComputerComponent comp = default(RMCCameraComputerComponent);
		while (computers.MoveNext(ref uid, ref comp))
		{
			foreach (EntProtoId protoId in comp.ProtoIds)
			{
				EntProtoId? id = camera.Comp.Id;
				if (id.HasValue && !(protoId != id.GetValueOrDefault()) && !((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null))
				{
					int index = comp.CameraIds.IndexOf(netCamera);
					if (index >= 0)
					{
						comp.CameraIds.RemoveAt(index);
						comp.CameraNames.RemoveAt(index);
					}
					EntityUid? currentCamera = comp.CurrentCamera;
					EntityUid val = Entity<RMCCameraComponent>.op_Implicit(camera);
					if (currentCamera.HasValue && currentCamera.GetValueOrDefault() == val)
					{
						comp.CurrentCamera = null;
					}
					((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
				}
			}
		}
	}

	public void AddProtoId(RMCCameraComputerComponent computer, EntProtoId protoId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		computer.ProtoIds.Add(protoId);
	}

	public void RemoveProtoId(RMCCameraComputerComponent computer, EntProtoId protoId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		computer.ProtoIds.Remove(protoId);
		EntityQueryEnumerator<RMCCameraComponent> cameraQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCCameraComponent>();
		EntityUid uid = default(EntityUid);
		RMCCameraComponent camera = default(RMCCameraComponent);
		while (cameraQuery.MoveNext(ref uid, ref camera))
		{
			EntProtoId? id = camera.Id;
			if (id.HasValue && !(id.GetValueOrDefault() != protoId))
			{
				computer.CameraIds.Remove(((EntitySystem)this).GetNetEntity(uid, (MetaDataComponent)null));
				computer.CameraNames.Remove(((EntitySystem)this).Name(uid, (MetaDataComponent)null));
			}
		}
	}

	public void RefreshCameras(EntProtoId protoId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_refresh.Add(protoId);
	}

	public override void Update(float frameTime)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		if (_refresh.Count == 0)
		{
			return;
		}
		if (_net.IsClient)
		{
			_refresh.Clear();
			return;
		}
		HashSet<Entity<RMCCameraComputerComponent>> monitors = new HashSet<Entity<RMCCameraComputerComponent>>();
		EntityUid uid = default(EntityUid);
		RMCCameraComputerComponent computer = default(RMCCameraComputerComponent);
		EntityUid uid2 = default(EntityUid);
		RMCCameraComponent camera = default(RMCCameraComponent);
		foreach (EntProtoId refresh in _refresh)
		{
			monitors.Clear();
			EntityQueryEnumerator<RMCCameraComputerComponent> monitorQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCCameraComputerComponent>();
			while (monitorQuery.MoveNext(ref uid, ref computer))
			{
				foreach (EntProtoId protoId in computer.ProtoIds)
				{
					if (protoId == refresh)
					{
						monitors.Add(Entity<RMCCameraComputerComponent>.op_Implicit((uid, computer)));
					}
				}
			}
			if (monitors.Count == 0)
			{
				continue;
			}
			List<NetEntity> cameraIds = new List<NetEntity>();
			List<string> cameraNames = new List<string>();
			EntityQueryEnumerator<RMCCameraComponent> cameraQuery = ((EntitySystem)this).EntityQueryEnumerator<RMCCameraComponent>();
			while (cameraQuery.MoveNext(ref uid2, ref camera))
			{
				EntProtoId? id = camera.Id;
				EntProtoId val = refresh;
				if (id.HasValue && !(id.GetValueOrDefault() != val))
				{
					cameraIds.Add(((EntitySystem)this).GetNetEntity(uid2, (MetaDataComponent)null));
					cameraNames.Add(((EntitySystem)this).Name(uid2, (MetaDataComponent)null));
				}
			}
			foreach (Entity<RMCCameraComputerComponent> monitor in monitors)
			{
				foreach (NetEntity camera2 in cameraIds)
				{
					if (!monitor.Comp.CameraIds.Contains(camera2))
					{
						monitor.Comp.CameraIds.Add(camera2);
					}
				}
				foreach (string name in cameraNames)
				{
					if (!monitor.Comp.CameraNames.Contains(name))
					{
						monitor.Comp.CameraNames.Add(name);
					}
				}
				((EntitySystem)this).Dirty<RMCCameraComputerComponent>(monitor, (MetaDataComponent)null);
			}
		}
		_refresh.Clear();
	}
}
