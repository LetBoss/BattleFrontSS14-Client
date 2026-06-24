using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Barricade.Components;
using Content.Shared._RMC14.Construction;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Timing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Entrenching;

public sealed class BarricadeSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private SharedMapSystem _mapSystem;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private RMCConstructionSystem _rmcConstruction;

	[Dependency]
	private SharedStackSystem _stack;

	[Dependency]
	private ITileDefinitionManager _tiles;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private UseDelaySystem _useDelay;

	private EntityQuery<BarricadeComponent> _barricadeQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_barricadeQuery = ((EntitySystem)this).GetEntityQuery<BarricadeComponent>();
		((EntitySystem)this).SubscribeLocalEvent<EntrenchingToolComponent, AfterInteractEvent>((EntityEventRefHandler<EntrenchingToolComponent, AfterInteractEvent>)OnAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntrenchingToolComponent, EntrenchingToolDoAfterEvent>((EntityEventRefHandler<EntrenchingToolComponent, EntrenchingToolDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntrenchingToolComponent, ItemToggledEvent>((EntityEventRefHandler<EntrenchingToolComponent, ItemToggledEvent>)OnItemToggled, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntrenchingToolComponent, SandbagFillDoAfterEvent>((EntityEventRefHandler<EntrenchingToolComponent, SandbagFillDoAfterEvent>)OnSandbagFillDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntrenchingToolComponent, SandbagDismantleDoAfterEvent>((EntityEventRefHandler<EntrenchingToolComponent, SandbagDismantleDoAfterEvent>)OnSandbagDismantleDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EmptySandbagComponent, InteractUsingEvent>((EntityEventRefHandler<EmptySandbagComponent, InteractUsingEvent>)OnEmptyInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FullSandbagComponent, ActivateInWorldEvent>((EntityEventRefHandler<FullSandbagComponent, ActivateInWorldEvent>)OnFullActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FullSandbagComponent, AfterInteractEvent>((EntityEventRefHandler<FullSandbagComponent, AfterInteractEvent>)OnFullAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FullSandbagComponent, SandbagBuildDoAfterEvent>((EntityEventRefHandler<FullSandbagComponent, SandbagBuildDoAfterEvent>)OnFullBuildDoAfter, (Type[])null, (Type[])null);
	}

	private void OnAfterInteract(Entity<EntrenchingToolComponent> tool, ref AfterInteractEvent args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach)
		{
			if (((EntitySystem)this).HasComp<BarricadeSandbagComponent>(args.Target))
			{
				DismantleSandbagBaricade(tool, ref args);
				((HandledEntityEventArgs)args).Handled = true;
			}
			else
			{
				StartDigging(tool, args.User, args.ClickLocation);
				((HandledEntityEventArgs)args).Handled = true;
			}
		}
	}

	private void DismantleSandbagBaricade(Entity<EntrenchingToolComponent> tool, ref AfterInteractEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		ItemToggleComponent toggle = default(ItemToggleComponent);
		if (!((EntitySystem)this).TryComp<ItemToggleComponent>(Entity<EntrenchingToolComponent>.op_Implicit(tool), ref toggle) || toggle.Activated)
		{
			_popup.PopupClient(base.Loc.GetString("cm-entrenching-dismantle"), args.User, args.User);
			SandbagDismantleDoAfterEvent ev = new SandbagDismantleDoAfterEvent(((EntitySystem)this).GetNetCoordinates(args.ClickLocation, (MetaDataComponent)null));
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, tool.Comp.DigDelay, ev, Entity<EntrenchingToolComponent>.op_Implicit(tool), args.Target, Entity<EntrenchingToolComponent>.op_Implicit(tool))
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnSandbagDismantleDoAfter(Entity<EntrenchingToolComponent> tool, ref SandbagDismantleDoAfterEvent args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		BarricadeSandbagComponent barricade = default(BarricadeSandbagComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryComp<BarricadeSandbagComponent>(args.Target, ref barricade))
		{
			EntityUid full = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(barricade.Material), ((EntitySystem)this).GetCoordinates(args.Coordinates));
			int bagsSalvaged = barricade.MaxMaterial;
			FullSandbagComponent fullSandbag = default(FullSandbagComponent);
			if (bagsSalvaged <= 0 && ((EntitySystem)this).TryComp<FullSandbagComponent>(full, ref fullSandbag))
			{
				bagsSalvaged = fullSandbag.StackRequired;
			}
			DamageableComponent damageable = default(DamageableComponent);
			if (((EntitySystem)this).TryComp<DamageableComponent>(args.Target, ref damageable))
			{
				bagsSalvaged -= Math.Max((int)damageable.TotalDamage / barricade.MaterialLossDamageInterval - 1, 0);
			}
			BarbedComponent barbed = default(BarbedComponent);
			if (((EntitySystem)this).TryComp<BarbedComponent>(args.Target, ref barbed) && barbed.IsBarbed)
			{
				((EntitySystem)this).Spawn(EntProtoId.op_Implicit(barbed.Spawn), ((EntitySystem)this).GetCoordinates(args.Coordinates));
			}
			((EntitySystem)this).Del(args.Target);
			StackComponent fullStack = default(StackComponent);
			if (bagsSalvaged <= 0)
			{
				((EntitySystem)this).Del((EntityUid?)full);
			}
			else if (((EntitySystem)this).TryComp<StackComponent>(full, ref fullStack))
			{
				_stack.SetCount(full, bagsSalvaged, fullStack);
			}
		}
	}

	private void OnDoAfter(Entity<EntrenchingToolComponent> tool, ref EntrenchingToolDoAfterEvent args)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		if (args.Cancelled)
		{
			_popup.PopupClient(base.Loc.GetString("cm-entrenching-stop-digging"), args.User, args.User);
			return;
		}
		EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		if (!CanDig(tool, args.User, coordinates, checkUseDelay: false, out Entity<MapGridComponent> _, out TileRef _))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		tool.Comp.TotalLayers = tool.Comp.LayersPerDig;
		((EntitySystem)this).Dirty<EntrenchingToolComponent>(tool, (MetaDataComponent)null);
		EntityCoordinates userCoordinates = _transform.GetMoverCoordinates(args.User);
		using HashSet<Entity<EmptySandbagComponent>>.Enumerator enumerator = _lookup.GetEntitiesInRange<EmptySandbagComponent>(userCoordinates, 1.5f, (LookupFlags)110).GetEnumerator();
		if (enumerator.MoveNext())
		{
			Entity<EmptySandbagComponent> empty = enumerator.Current;
			SandbagFillDoAfterEvent ev = new SandbagFillDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, tool.Comp.FillDelay, ev, Entity<EntrenchingToolComponent>.op_Implicit(tool), Entity<EmptySandbagComponent>.op_Implicit(empty), Entity<EntrenchingToolComponent>.op_Implicit(tool))
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfter);
			_popup.PopupClient(base.Loc.GetString("cm-entrenching-begin-filling"), args.User, args.User);
		}
	}

	private void OnItemToggled(Entity<EntrenchingToolComponent> tool, ref ItemToggledEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		tool.Comp.TotalLayers = 0;
		((EntitySystem)this).Dirty<EntrenchingToolComponent>(tool, (MetaDataComponent)null);
	}

	private void OnSandbagFillDoAfter(Entity<EntrenchingToolComponent> tool, ref SandbagFillDoAfterEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityCoordinates userCoordinates = _transform.GetMoverCoordinates(args.User);
		HashSet<Entity<EmptySandbagComponent>> entitiesInRange = _lookup.GetEntitiesInRange<EmptySandbagComponent>(userCoordinates, 1.5f, (LookupFlags)110);
		bool filled = false;
		foreach (Entity<EmptySandbagComponent> empty in entitiesInRange)
		{
			if (filled)
			{
				args.Repeat = true;
				break;
			}
			filled = true;
			Fill(tool, empty, args.User, tool.Comp.TotalLayers);
			if (!((EntitySystem)this).TerminatingOrDeleted(Entity<EmptySandbagComponent>.op_Implicit(empty), (MetaDataComponent)null))
			{
				args.Repeat = true;
				break;
			}
		}
		if (tool.Comp.TotalLayers <= 0)
		{
			args.Repeat = false;
			StartDigging(tool, args.User, tool.Comp.LastDigLocation);
		}
	}

	private void OnEmptyInteractUsing(Entity<EmptySandbagComponent> empty, ref InteractUsingEvent args)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		EntrenchingToolComponent toolComp = default(EntrenchingToolComponent);
		if (!_net.IsClient && !((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<EntrenchingToolComponent>(args.Used, ref toolComp))
		{
			((HandledEntityEventArgs)args).Handled = true;
			Entity<EntrenchingToolComponent> tool = default(Entity<EntrenchingToolComponent>);
			tool._002Ector(args.Used, toolComp);
			SandbagFillDoAfterEvent ev = new SandbagFillDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, tool.Comp.FillDelay, ev, Entity<EntrenchingToolComponent>.op_Implicit(tool), Entity<EmptySandbagComponent>.op_Implicit(empty), Entity<EntrenchingToolComponent>.op_Implicit(tool))
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfter);
			_popup.PopupClient(base.Loc.GetString("cm-entrenching-begin-filling"), args.User, args.User);
		}
	}

	private void OnFullActivateInWorld(Entity<FullSandbagComponent> full, ref ActivateInWorldEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = default(TransformComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp(args.User, ref transform))
		{
			EntityCoordinates coordinates = _transform.GetMoverCoordinates(args.User, transform);
			Angle localRotation = transform.LocalRotation;
			Direction direction = ((Angle)(ref localRotation)).GetCardinalDir();
			if (Build(full, args.User, coordinates, direction, out var handled))
			{
				((HandledEntityEventArgs)args).Handled = handled;
			}
		}
	}

	private void OnFullAfterInteract(Entity<FullSandbagComponent> full, ref AfterInteractEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent transform = default(TransformComponent);
		if (!((HandledEntityEventArgs)args).Handled && args.CanReach && ((EntitySystem)this).TryComp(args.User, ref transform))
		{
			Angle localRotation = transform.LocalRotation;
			Direction direction = ((Angle)(ref localRotation)).GetCardinalDir();
			if (Build(full, args.User, args.ClickLocation, direction, out var handled))
			{
				((HandledEntityEventArgs)args).Handled = handled;
			}
		}
	}

	private void OnFullBuildDoAfter(Entity<FullSandbagComponent> full, ref SandbagBuildDoAfterEvent args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(args.Coordinates);
		EntityUid gridId = default(EntityUid);
		MapGridComponent gridComp = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(_transform.ToMapCoordinates(coordinates, true), ref gridId, ref gridComp) || !_interaction.InRangeUnobstructed(Entity<FullSandbagComponent>.op_Implicit(full), coordinates) || !_turf.TryGetTileRef(coordinates, out var turf) || !CanBuild(full, Entity<MapGridComponent>.op_Implicit((gridId, gridComp)), args.User, turf.Value, args.Direction))
		{
			return;
		}
		if (full.Comp.StackRequired > 1)
		{
			int count = _stack.GetCount(Entity<FullSandbagComponent>.op_Implicit(full));
			if (count < full.Comp.StackRequired)
			{
				return;
			}
			StackComponent fullStack = default(StackComponent);
			if (((EntitySystem)this).TryComp<StackComponent>(Entity<FullSandbagComponent>.op_Implicit(full), ref fullStack))
			{
				_stack.SetCount(Entity<FullSandbagComponent>.op_Implicit(full), count - full.Comp.StackRequired, fullStack);
			}
			else
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<FullSandbagComponent>.op_Implicit(full));
			}
		}
		EntityUid built = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(full.Comp.Builds), coordinates, (ComponentRegistry)null);
		_transform.SetLocalRotation(built, DirectionExtensions.ToAngle(args.Direction), (TransformComponent)null);
		((HandledEntityEventArgs)args).Handled = true;
	}

	private bool StartDigging(Entity<EntrenchingToolComponent> tool, EntityUid user, EntityCoordinates clicked)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if (!CanDig(tool, user, clicked, checkUseDelay: true, out Entity<MapGridComponent> grid, out TileRef tile))
		{
			return false;
		}
		EntityCoordinates coordinates = _mapSystem.GridTileToLocal(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), tile.GridIndices);
		tool.Comp.LastDigLocation = coordinates;
		((EntitySystem)this).Dirty<EntrenchingToolComponent>(tool, (MetaDataComponent)null);
		EntrenchingToolDoAfterEvent ev = new EntrenchingToolDoAfterEvent(((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null));
		EntityManager entityManager = base.EntityManager;
		TimeSpan digDelay = tool.Comp.DigDelay;
		EntityUid? eventTarget = Entity<EntrenchingToolComponent>.op_Implicit(tool);
		EntityUid? used = Entity<EntrenchingToolComponent>.op_Implicit(tool);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)entityManager, user, digDelay, ev, eventTarget, null, used)
		{
			BreakOnMove = true,
			NeedHand = true,
			BreakOnHandChange = true
		};
		_doAfter.TryStartDoAfter(doAfter);
		_popup.PopupClient(base.Loc.GetString("cm-entrenching-start-digging"), user, user);
		_audio.PlayPredicted(tool.Comp.DigSound, user, (EntityUid?)user, (AudioParams?)null);
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<EntrenchingToolComponent>.op_Implicit(tool), ref useDelay))
		{
			_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((Entity<EntrenchingToolComponent>.op_Implicit(tool), useDelay)));
		}
		return true;
	}

	private bool Fill(Entity<EntrenchingToolComponent> tool, Entity<EmptySandbagComponent> empty, EntityUid user, int amount)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (tool.Comp.TotalLayers < amount)
		{
			return false;
		}
		int toRemove = amount;
		EntityCoordinates coordinates = _transform.GetMoverCoordinates(Entity<EmptySandbagComponent>.op_Implicit(empty));
		StackComponent stack = default(StackComponent);
		if (((EntitySystem)this).TryComp<StackComponent>(Entity<EmptySandbagComponent>.op_Implicit(empty), ref stack))
		{
			int stackCount = _stack.GetCount(Entity<EmptySandbagComponent>.op_Implicit(empty), stack);
			toRemove = Math.Min(toRemove, stackCount);
			_stack.SetCount(Entity<EmptySandbagComponent>.op_Implicit(empty), stackCount - toRemove, stack);
			if (_net.IsServer)
			{
				EntityUid filled = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(empty.Comp.Filled), coordinates);
				StackComponent filledStack = ((EntitySystem)this).EnsureComp<StackComponent>(filled);
				_stack.SetCount(filled, toRemove, filledStack);
			}
		}
		else if (_net.IsServer)
		{
			((EntitySystem)this).Del((EntityUid?)Entity<EmptySandbagComponent>.op_Implicit(empty));
		}
		tool.Comp.TotalLayers -= amount;
		((EntitySystem)this).Dirty<EntrenchingToolComponent>(tool, (MetaDataComponent)null);
		_audio.PlayPredicted(tool.Comp.FillSound, user, (EntityUid?)user, (AudioParams?)null);
		return true;
	}

	private bool Build(Entity<FullSandbagComponent> full, EntityUid user, EntityCoordinates coordinates, Direction direction, out bool handled)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		handled = false;
		EntityUid gridId = default(EntityUid);
		MapGridComponent gridComp = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(_transform.ToMapCoordinates(coordinates, true), ref gridId, ref gridComp) || !_turf.TryGetTileRef(coordinates, out var tile))
		{
			return false;
		}
		handled = true;
		if (!CanBuild(full, Entity<MapGridComponent>.op_Implicit((gridId, gridComp)), user, tile.Value, direction))
		{
			return false;
		}
		SandbagBuildDoAfterEvent ev = new SandbagBuildDoAfterEvent(((EntitySystem)this).GetNetCoordinates(coordinates, (MetaDataComponent)null), direction);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, full.Comp.BuildDelay, ev, Entity<FullSandbagComponent>.op_Implicit(full), Entity<FullSandbagComponent>.op_Implicit(full))
		{
			BreakOnMove = true
		};
		_doAfter.TryStartDoAfter(doAfter);
		return true;
	}

	private bool CanDig(Entity<EntrenchingToolComponent> tool, EntityUid user, EntityCoordinates coordinates, bool checkUseDelay, out Entity<MapGridComponent> grid, out TileRef tileRef)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		grid = default(Entity<MapGridComponent>);
		tileRef = default(TileRef);
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (checkUseDelay && ((EntitySystem)this).TryComp<UseDelayComponent>(Entity<EntrenchingToolComponent>.op_Implicit(tool), ref useDelay) && _useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<EntrenchingToolComponent>.op_Implicit(tool), useDelay))))
		{
			return false;
		}
		if (!_interaction.InRangeUnobstructed(user, coordinates))
		{
			return false;
		}
		ItemToggleComponent toggle = default(ItemToggleComponent);
		if (((EntitySystem)this).TryComp<ItemToggleComponent>(Entity<EntrenchingToolComponent>.op_Implicit(tool), ref toggle) && !toggle.Activated)
		{
			return false;
		}
		EntityUid gridId = default(EntityUid);
		MapGridComponent gridComp = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(_transform.ToMapCoordinates(coordinates, true), ref gridId, ref gridComp))
		{
			return false;
		}
		tileRef = _mapSystem.GetTileRef(gridId, gridComp, coordinates);
		if (!((ContentTileDefinition)(object)_tiles[tileRef.Tile.TypeId]).CanDig)
		{
			return false;
		}
		if (!TileSolidAndNotBlocked(tileRef))
		{
			return false;
		}
		grid = Entity<MapGridComponent>.op_Implicit((gridId, gridComp));
		return true;
	}

	private bool TileSolidAndNotBlocked(TileRef tile)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!_turf.IsSpace(tile) && _turf.GetContentTileDefinition(tile).Sturdy)
		{
			return !_turf.IsTileBlocked(tile, CollisionGroup.Impassable);
		}
		return false;
	}

	private bool CanBuild(Entity<FullSandbagComponent> full, Entity<MapGridComponent> grid, EntityUid user, TileRef tile, Direction direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		StackComponent stack = default(StackComponent);
		if (!((EntitySystem)this).TryComp<StackComponent>(Entity<FullSandbagComponent>.op_Implicit(full), ref stack) || stack.Count < 5)
		{
			return false;
		}
		EntityCoordinates val = new EntityCoordinates(tile.GridUid, (float)((TileRef)(ref tile)).X, (float)((TileRef)(ref tile)).Y);
		EntityCoordinates coordinates = ((EntityCoordinates)(ref val)).Offset(grid.Comp.TileSizeHalfVector);
		CollisionGroup mask = CollisionGroup.TableMask | CollisionGroup.InteractImpassable;
		bool popup = _net.IsClient;
		if (!_interaction.InRangeUnobstructed(user, coordinates, 1.5f, mask, null, popup))
		{
			return false;
		}
		if (!TileSolidAndNotBlocked(tile))
		{
			return false;
		}
		AnchoredEntitiesEnumerator anchored = _mapSystem.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), tile.GridIndices);
		EntityUid? uid = default(EntityUid?);
		TransformComponent transform = default(TransformComponent);
		while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref uid))
		{
			if (((EntitySystem)this).HasComp<BarricadeComponent>(uid) && ((EntitySystem)this).TryComp(uid, ref transform))
			{
				Angle localRotation = transform.LocalRotation;
				if (((Angle)(ref localRotation)).GetCardinalDir() == direction)
				{
					return false;
				}
			}
		}
		EntityPrototype builds = default(EntityPrototype);
		string name = (_prototype.TryIndex(full.Comp.Builds, ref builds) ? builds.Name : ((EntitySystem)this).Name(Entity<FullSandbagComponent>.op_Implicit(full), (MetaDataComponent)null));
		if (!_rmcConstruction.CanBuildAt(coordinates, name, out string popupStr, anchoring: false, (Direction)(-1)))
		{
			if (popup)
			{
				_popup.PopupClient(popupStr, user, user, PopupType.SmallCaution);
			}
			return false;
		}
		return true;
	}

	public bool HasBarricadeFacing(EntityCoordinates coordinates, Direction direction)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				Vector2i indices = _mapSystem.TileIndicesFor(gridId, grid2, coordinates);
				AnchoredEntitiesEnumerator anchored = _mapSystem.GetAnchoredEntitiesEnumerator(gridId, grid2, indices);
				EntityUid? uid = default(EntityUid?);
				while (((AnchoredEntitiesEnumerator)(ref anchored)).MoveNext(ref uid))
				{
					if (_barricadeQuery.HasComp(uid))
					{
						Angle worldRotation = _transform.GetWorldRotation(uid.Value);
						if (((Angle)(ref worldRotation)).GetCardinalDir() == direction)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		return false;
	}

	public bool HasBarricadeNearbyPopup(Entity<MapGridComponent> grid, EntityUid user, EntityCoordinates coordinates, int range)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		Vector2i position = _mapSystem.LocalToTile(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), coordinates);
		Box2 checkArea = default(Box2);
		((Box2)(ref checkArea))._002Ector((float)(position.X - range), (float)(position.Y - range), (float)(position.X + range), (float)(position.Y + range));
		foreach (EntityUid anchored in _mapSystem.GetLocalAnchoredEntities(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), checkArea))
		{
			if (((EntitySystem)this).HasComp<BarricadeComponent>(anchored))
			{
				string msg = base.Loc.GetString("barricade-anchored-too-close", (ValueTuple<string, object>)("barricade", anchored));
				_popup.PopupClient(msg, user, user, PopupType.SmallCaution);
				return true;
			}
		}
		return false;
	}
}
