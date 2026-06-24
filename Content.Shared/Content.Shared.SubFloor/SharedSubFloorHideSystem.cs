using System;
using Content.Shared.Audio;
using Content.Shared.Construction.Components;
using Content.Shared.Explosion;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.SubFloor;

public abstract class SharedSubFloorHideSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class ShowSubfloorRequestEvent : EntityEventArgs
	{
		public bool Value;
	}

	[Dependency]
	private ITileDefinitionManager _tileDefinitionManager;

	[Dependency]
	private SharedAmbientSoundSystem _ambientSoundSystem;

	[Dependency]
	protected SharedMapSystem Map;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	private SharedVisibilitySystem _visibility;

	[Dependency]
	protected SharedPopupSystem _popup;

	private EntityQuery<SubFloorHideComponent> _hideQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_hideQuery = ((EntitySystem)this).GetEntityQuery<SubFloorHideComponent>();
		((EntitySystem)this).SubscribeLocalEvent<TileChangedEvent>((EntityEventRefHandler<TileChangedEvent>)OnTileChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubFloorHideComponent, ComponentStartup>((ComponentEventHandler<SubFloorHideComponent, ComponentStartup>)OnSubFloorStarted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubFloorHideComponent, ComponentShutdown>((ComponentEventHandler<SubFloorHideComponent, ComponentShutdown>)OnSubFloorTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubFloorHideComponent, AnchorStateChangedEvent>((ComponentEventRefHandler<SubFloorHideComponent, AnchorStateChangedEvent>)HandleAnchorChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubFloorHideComponent, GettingInteractedWithAttemptEvent>((ComponentEventRefHandler<SubFloorHideComponent, GettingInteractedWithAttemptEvent>)OnInteractionAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubFloorHideComponent, GettingAttackedAttemptEvent>((ComponentEventRefHandler<SubFloorHideComponent, GettingAttackedAttemptEvent>)OnAttackAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubFloorHideComponent, GetExplosionResistanceEvent>((ComponentEventRefHandler<SubFloorHideComponent, GetExplosionResistanceEvent>)OnGetExplosionResistance, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubFloorHideComponent, AnchorAttemptEvent>((ComponentEventHandler<SubFloorHideComponent, AnchorAttemptEvent>)OnAnchorAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SubFloorHideComponent, UnanchorAttemptEvent>((ComponentEventHandler<SubFloorHideComponent, UnanchorAttemptEvent>)OnUnanchorAttempt, (Type[])null, (Type[])null);
	}

	private void OnAnchorAttempt(EntityUid uid, SubFloorHideComponent component, AnchorAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		MapGridComponent grid = default(MapGridComponent);
		if (((EntitySystem)this).TryComp<MapGridComponent>(xform.GridUid, ref grid) && HasFloorCover(xform.GridUid.Value, grid, Map.TileIndicesFor(xform.GridUid.Value, grid, xform.Coordinates)))
		{
			_popup.PopupClient(base.Loc.GetString("subfloor-anchor-failure", (ValueTuple<string, object>)("entity", uid)), args.User);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnUnanchorAttempt(EntityUid uid, SubFloorHideComponent component, UnanchorAttemptEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (component.IsUnderCover)
		{
			_popup.PopupClient(base.Loc.GetString("subfloor-unanchor-failure", (ValueTuple<string, object>)("entity", uid)), args.User);
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnGetExplosionResistance(EntityUid uid, SubFloorHideComponent component, ref GetExplosionResistanceEvent args)
	{
		if (component.BlockInteractions && component.IsUnderCover)
		{
			args.DamageCoefficient = 0f;
		}
	}

	private void OnAttackAttempt(EntityUid uid, SubFloorHideComponent component, ref GettingAttackedAttemptEvent args)
	{
		if (component.BlockInteractions && component.IsUnderCover)
		{
			args.Cancelled = true;
		}
	}

	private void OnInteractionAttempt(EntityUid uid, SubFloorHideComponent component, ref GettingInteractedWithAttemptEvent args)
	{
		if (component.BlockInteractions && component.IsUnderCover)
		{
			args.Cancelled = true;
		}
	}

	private void OnSubFloorStarted(EntityUid uid, SubFloorHideComponent component, ComponentStartup _)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		UpdateFloorCover(uid, component);
		UpdateAppearance(uid, component);
		((EntitySystem)this).EnsureComp<CollideOnAnchorComponent>(uid);
	}

	private void OnSubFloorTerminating(EntityUid uid, SubFloorHideComponent component, ComponentShutdown _)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((EntitySystem)this).Comp<MetaDataComponent>(uid).EntityLifeStage < 4)
		{
			SetUnderCover(Entity<SubFloorHideComponent>.op_Implicit((uid, component)), value: false);
			UpdateAppearance(uid, component);
		}
	}

	private void HandleAnchorChanged(EntityUid uid, SubFloorHideComponent component, ref AnchorStateChangedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (((AnchorStateChangedEvent)(ref args)).Anchored)
		{
			TransformComponent xform = ((EntitySystem)this).Transform(uid);
			UpdateFloorCover(uid, component, xform);
		}
		else if (component.IsUnderCover)
		{
			SetUnderCover(Entity<SubFloorHideComponent>.op_Implicit((uid, component)), value: false);
			UpdateAppearance(uid, component);
		}
	}

	private void OnTileChanged(ref TileChangedEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		TileChangedEntry[] changes = args.Changes;
		for (int i = 0; i < changes.Length; i++)
		{
			TileChangedEntry change = changes[i];
			Tile val = ((TileChangedEntry)(ref change)).OldTile;
			if (!((Tile)(ref val)).IsEmpty)
			{
				val = ((TileChangedEntry)(ref change)).NewTile;
				if (!((Tile)(ref val)).IsEmpty)
				{
					UpdateTile(Entity<MapGridComponent>.op_Implicit(args.Entity), args.Entity.Comp, ((TileChangedEntry)(ref change)).GridIndices);
				}
			}
		}
	}

	private void UpdateFloorCover(EntityUid uid, SubFloorHideComponent? component = null, TransformComponent? xform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SubFloorHideComponent, TransformComponent>(uid, ref component, ref xform, true))
		{
			MapGridComponent grid = default(MapGridComponent);
			if (xform.Anchored && ((EntitySystem)this).TryComp<MapGridComponent>(xform.GridUid, ref grid))
			{
				SetUnderCover(Entity<SubFloorHideComponent>.op_Implicit((uid, component)), HasFloorCover(xform.GridUid.Value, grid, Map.TileIndicesFor(xform.GridUid.Value, grid, xform.Coordinates)));
			}
			else
			{
				SetUnderCover(Entity<SubFloorHideComponent>.op_Implicit((uid, component)), value: false);
			}
			UpdateAppearance(uid, component);
		}
	}

	private void SetUnderCover(Entity<SubFloorHideComponent> entity, bool value)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		_visibility.SetLayer(Entity<VisibilityComponent>.op_Implicit(entity.Owner), (ushort)((!value || entity.Comp.VisibleLayers.Count != 0) ? 1 : 4), true);
		if (entity.Comp.IsUnderCover != value)
		{
			entity.Comp.IsUnderCover = value;
		}
	}

	public bool HasFloorCover(EntityUid gridUid, MapGridComponent grid, Vector2i position)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return !((ContentTileDefinition)(object)_tileDefinitionManager[Map.GetTileRef(gridUid, grid, position).Tile.TypeId]).IsSubFloor;
	}

	private void UpdateTile(EntityUid gridUid, MapGridComponent grid, Vector2i position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		bool covered = HasFloorCover(gridUid, grid, position);
		SubFloorHideComponent hideComp = default(SubFloorHideComponent);
		foreach (EntityUid uid in Map.GetAnchoredEntities(gridUid, grid, position))
		{
			if (_hideQuery.TryComp(uid, ref hideComp) && hideComp.IsUnderCover != covered)
			{
				SetUnderCover(Entity<SubFloorHideComponent>.op_Implicit((uid, hideComp)), covered);
				UpdateAppearance(uid, hideComp);
			}
		}
	}

	public void UpdateAppearance(EntityUid uid, SubFloorHideComponent? hideComp = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SubFloorHideComponent>(uid, ref hideComp, false))
		{
			if (hideComp.BlockAmbience && hideComp.IsUnderCover)
			{
				_ambientSoundSystem.SetAmbience(uid, value: false);
			}
			else if (hideComp.BlockAmbience && !hideComp.IsUnderCover)
			{
				_ambientSoundSystem.SetAmbience(uid, value: true);
			}
			if (((EntitySystem)this).Resolve<AppearanceComponent>(uid, ref appearance, false))
			{
				Appearance.SetData(uid, (Enum)SubFloorVisuals.Covered, (object)hideComp.IsUnderCover, appearance);
			}
		}
	}
}
