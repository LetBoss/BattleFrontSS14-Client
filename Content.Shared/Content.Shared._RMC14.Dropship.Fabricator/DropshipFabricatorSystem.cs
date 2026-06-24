using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.PowerLoader;
using Content.Shared.Coordinates;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Dropship.Fabricator;

public sealed class DropshipFabricatorSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private PowerLoaderSystem _powerLoader;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private int _startingPoints;

	private TimeSpan _gainEvery;

	public ImmutableArray<EntProtoId<DropshipFabricatorPrintableComponent>> Printables { get; private set; }

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PrototypesReloadedEventArgs>((EntityEventHandler<PrototypesReloadedEventArgs>)OnPrototypesReloaded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipFabricatorComponent, MapInitEvent>((EntityEventRefHandler<DropshipFabricatorComponent, MapInitEvent>)OnFabricatorMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DropshipFabricatorComponent, DropshipFabricatoreRecycleDoafterEvent>((EntityEventRefHandler<DropshipFabricatorComponent, DropshipFabricatoreRecycleDoafterEvent>)OnDropshipPartRecycled, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<DropshipFabricatorComponent>(((EntitySystem)this).Subs, (object)DropshipFabricatorUi.Key, (BuiEventSubscriber<DropshipFabricatorComponent>)delegate(Subscriber<DropshipFabricatorComponent> subs)
		{
			subs.Event<DropshipFabricatorPrintMsg>((EntityEventRefHandler<DropshipFabricatorComponent, DropshipFabricatorPrintMsg>)OnPrintMsg);
		});
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCDropshipFabricatorStartingPoints, (Action<int>)delegate(int v)
		{
			_startingPoints = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCDropshipFabricatorGainEverySeconds, (Action<float>)delegate(float v)
		{
			_gainEvery = TimeSpan.FromSeconds(v);
		}, true);
		ReloadPrototypes();
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs ev)
	{
		if (ev.WasModified<EntityPrototype>())
		{
			ReloadPrototypes();
		}
	}

	private void OnFabricatorMapInit(Entity<DropshipFabricatorComponent> ent, ref MapInitEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsServer)
		{
			ent.Comp.Account = Entity<DropshipFabricatorPointsComponent>.op_Implicit(EnsurePoints());
		}
	}

	private void OnDropshipPartRecycled(Entity<DropshipFabricatorComponent> ent, ref DropshipFabricatoreRecycleDoafterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		DropshipFabricatorPrintableComponent printable = default(DropshipFabricatorPrintableComponent);
		DropshipFabricatorPointsComponent points = default(DropshipFabricatorPointsComponent);
		if (!args.Cancelled && !((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<DropshipFabricatorPrintableComponent>(args.Used, ref printable) && ((EntitySystem)this).TryComp<DropshipFabricatorPointsComponent>(ent.Comp.Account, ref points))
		{
			((HandledEntityEventArgs)args).Handled = true;
			int refund = printable.Cost;
			DropshipAmmoComponent ammo = default(DropshipAmmoComponent);
			if (((EntitySystem)this).TryComp<DropshipAmmoComponent>(args.Used, ref ammo))
			{
				refund = (int)((float)refund * (float)ammo.Rounds / (float)ammo.MaxRounds);
			}
			points.Points += (int)((float)refund * printable.RecycleMultiplier);
			((EntitySystem)this).Dirty(ent.Comp.Account.Value, (IComponent)(object)points, (MetaDataComponent)null);
			((EntitySystem)this).Del(args.Used);
			_audio.PlayPvs(ent.Comp.RecycleSound, Entity<DropshipFabricatorComponent>.op_Implicit(ent), (AudioParams?)null);
			_powerLoader.TrySyncHands(Entity<PowerLoaderComponent>.op_Implicit(args.User));
		}
	}

	private void OnPrintMsg(Entity<DropshipFabricatorComponent> ent, ref DropshipFabricatorPrintMsg args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype proto = default(EntityPrototype);
		DropshipFabricatorPrintableComponent printable = default(DropshipFabricatorPrintableComponent);
		if (!(args.Id == default(EntProtoId)) && _prototypes.TryIndex(args.Id, ref proto) && proto.TryGetComponent<DropshipFabricatorPrintableComponent>(ref printable, _compFactory))
		{
			EntityUid actor = ((BaseBoundUserInterfaceEvent)args).Actor;
			DropshipFabricatorPointsComponent points = default(DropshipFabricatorPointsComponent);
			if (ent.Comp.Printing.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-dropship-fabricator-busy"), actor, actor, PopupType.SmallCaution);
			}
			else if (((EntitySystem)this).TryComp<DropshipFabricatorPointsComponent>(ent.Comp.Account, ref points) && printable.Cost <= points.Points)
			{
				points.Points -= printable.Cost;
				((EntitySystem)this).Dirty(ent.Comp.Account.Value, (IComponent)(object)points, (MetaDataComponent)null);
				ent.Comp.Points = points.Points;
				ent.Comp.Printing = EntProtoId<DropshipFabricatorPrintableComponent>.op_Implicit(proto.ID);
				ent.Comp.PrintAt = _timing.CurTime + printable.Delay;
				((EntitySystem)this).Dirty<DropshipFabricatorComponent>(ent, (MetaDataComponent)null);
				_appearance.SetData(Entity<DropshipFabricatorComponent>.op_Implicit(ent), (Enum)DropshipFabricatorVisuals.State, (object)DropshipFabricatorState.Fabricating, (AppearanceComponent)null);
			}
		}
	}

	private Entity<DropshipFabricatorPointsComponent> EnsurePoints()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid uid = default(EntityUid);
		DropshipFabricatorPointsComponent comp = default(DropshipFabricatorPointsComponent);
		if (((EntitySystem)this).EntityQueryEnumerator<DropshipFabricatorPointsComponent>().MoveNext(ref uid, ref comp))
		{
			return Entity<DropshipFabricatorPointsComponent>.op_Implicit((uid, comp));
		}
		EntityUid points = ((EntitySystem)this).Spawn((string)null, MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
		DropshipFabricatorPointsComponent pointsComp = ((EntitySystem)this).EnsureComp<DropshipFabricatorPointsComponent>(points);
		pointsComp.Points = _startingPoints;
		return Entity<DropshipFabricatorPointsComponent>.op_Implicit((points, pointsComp));
	}

	private void ReloadPrototypes()
	{
		List<EntityPrototype> printables = new List<EntityPrototype>();
		foreach (EntityPrototype prototype in _prototypes.EnumeratePrototypes<EntityPrototype>())
		{
			if (prototype.HasComponent<DropshipFabricatorPrintableComponent>(_compFactory))
			{
				printables.Add(prototype);
			}
		}
		printables.Sort((EntityPrototype a, EntityPrototype b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
		Printables = printables.Select((EntityPrototype e) => new EntProtoId<DropshipFabricatorPrintableComponent>(e.ID)).ToImmutableArray();
	}

	public void ChangeBudget(int amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<DropshipFabricatorPointsComponent> accountQuery = ((EntitySystem)this).EntityQueryEnumerator<DropshipFabricatorPointsComponent>();
		EntityUid uid = default(EntityUid);
		DropshipFabricatorPointsComponent comp = default(DropshipFabricatorPointsComponent);
		while (accountQuery.MoveNext(ref uid, ref comp))
		{
			comp.Points += amount;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			SendUIStateAll(comp.Points);
		}
	}

	private void SendUIStateAll(int points)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<DropshipFabricatorComponent> fabricatorQuery = ((EntitySystem)this).EntityQueryEnumerator<DropshipFabricatorComponent>();
		EntityUid fabricatorId = default(EntityUid);
		DropshipFabricatorComponent fabricator = default(DropshipFabricatorComponent);
		while (fabricatorQuery.MoveNext(ref fabricatorId, ref fabricator))
		{
			fabricator.Points = points;
			((EntitySystem)this).Dirty(fabricatorId, (IComponent)(object)fabricator, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<DropshipFabricatorComponent, TransformComponent> allFabricatorQuery = ((EntitySystem)this).EntityQueryEnumerator<DropshipFabricatorComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		DropshipFabricatorComponent comp = default(DropshipFabricatorComponent);
		TransformComponent xform = default(TransformComponent);
		while (allFabricatorQuery.MoveNext(ref uid, ref comp, ref xform))
		{
			if (!(time < comp.PrintAt) && comp.Printing.HasValue)
			{
				Angle rotation = _transform.GetWorldRotation(xform);
				EntityCoordinates val = uid.ToCoordinates();
				EntityCoordinates coordinates = ((EntityCoordinates)(ref val)).Offset(Vector2i.op_Implicit(((Vector2i)(ref comp.PrintOffset)).Rotate(rotation)));
				((EntitySystem)this).SpawnAtPosition(EntProtoId<DropshipFabricatorPrintableComponent>.op_Implicit(comp.Printing.Value), coordinates, (ComponentRegistry)null);
				comp.Printing = null;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
				_appearance.SetData(uid, (Enum)DropshipFabricatorVisuals.State, (object)DropshipFabricatorState.Idle, (AppearanceComponent)null);
			}
		}
		EntityQueryEnumerator<DropshipFabricatorPointsComponent> pointsQuery = ((EntitySystem)this).EntityQueryEnumerator<DropshipFabricatorPointsComponent>();
		EntityUid pointsId = default(EntityUid);
		DropshipFabricatorPointsComponent points = default(DropshipFabricatorPointsComponent);
		while (pointsQuery.MoveNext(ref pointsId, ref points))
		{
			if (!(time < points.NextPointsAt))
			{
				points.NextPointsAt = time + _gainEvery;
				points.Points++;
				((EntitySystem)this).Dirty(pointsId, (IComponent)(object)points, (MetaDataComponent)null);
				SendUIStateAll(points.Points);
			}
		}
	}
}
