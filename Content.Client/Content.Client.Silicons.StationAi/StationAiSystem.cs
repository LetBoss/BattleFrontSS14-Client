using System;
using Content.Shared.Doors.Components;
using Content.Shared.Electrocution;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Light.Components;
using Content.Shared.Silicons.StationAi;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client.Silicons.StationAi;

public sealed class StationAiSystem : SharedStationAiSystem
{
	private readonly ResPath _aiActionsRsi = new ResPath("/Textures/Interface/Actions/actions_ai.rsi");

	[Dependency]
	private IOverlayManager _overlayMgr;

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	private StationAiOverlay? _overlay;

	private void InitializeAirlock()
	{
		((EntitySystem)this).SubscribeLocalEvent<DoorBoltComponent, GetStationAiRadialEvent>((EntityEventRefHandler<DoorBoltComponent, GetStationAiRadialEvent>)OnDoorBoltGetRadial, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AirlockComponent, GetStationAiRadialEvent>((EntityEventRefHandler<AirlockComponent, GetStationAiRadialEvent>)OnEmergencyAccessGetRadial, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ElectrifiedComponent, GetStationAiRadialEvent>((EntityEventRefHandler<ElectrifiedComponent, GetStationAiRadialEvent>)OnDoorElectrifiedGetRadial, (Type[])null, (Type[])null);
	}

	private void OnDoorBoltGetRadial(Entity<DoorBoltComponent> ent, ref GetStationAiRadialEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		args.Actions.Add(new StationAiRadial
		{
			Sprite = (SpriteSpecifier?)(ent.Comp.BoltsDown ? new Rsi(_aiActionsRsi, "unbolt_door") : new Rsi(_aiActionsRsi, "bolt_door")),
			Tooltip = (ent.Comp.BoltsDown ? ((EntitySystem)this).Loc.GetString("bolt-open") : ((EntitySystem)this).Loc.GetString("bolt-close")),
			Event = new StationAiBoltEvent
			{
				Bolted = !ent.Comp.BoltsDown
			}
		});
	}

	private void OnEmergencyAccessGetRadial(Entity<AirlockComponent> ent, ref GetStationAiRadialEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		args.Actions.Add(new StationAiRadial
		{
			Sprite = (SpriteSpecifier?)(ent.Comp.EmergencyAccess ? new Rsi(_aiActionsRsi, "emergency_off") : new Rsi(_aiActionsRsi, "emergency_on")),
			Tooltip = (ent.Comp.EmergencyAccess ? ((EntitySystem)this).Loc.GetString("emergency-access-off") : ((EntitySystem)this).Loc.GetString("emergency-access-on")),
			Event = new StationAiEmergencyAccessEvent
			{
				EmergencyAccess = !ent.Comp.EmergencyAccess
			}
		});
	}

	private void OnDoorElectrifiedGetRadial(Entity<ElectrifiedComponent> ent, ref GetStationAiRadialEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		args.Actions.Add(new StationAiRadial
		{
			Sprite = (SpriteSpecifier?)(ent.Comp.Enabled ? new Rsi(_aiActionsRsi, "door_overcharge_off") : new Rsi(_aiActionsRsi, "door_overcharge_on")),
			Tooltip = (ent.Comp.Enabled ? ((EntitySystem)this).Loc.GetString("electrify-door-off") : ((EntitySystem)this).Loc.GetString("electrify-door-on")),
			Event = new StationAiElectrifiedEvent
			{
				Electrified = !ent.Comp.Enabled
			}
		});
	}

	public override void Initialize()
	{
		base.Initialize();
		InitializeAirlock();
		InitializePowerToggle();
		((EntitySystem)this).SubscribeLocalEvent<StationAiOverlayComponent, LocalPlayerAttachedEvent>((EntityEventRefHandler<StationAiOverlayComponent, LocalPlayerAttachedEvent>)OnAiAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiOverlayComponent, LocalPlayerDetachedEvent>((EntityEventRefHandler<StationAiOverlayComponent, LocalPlayerDetachedEvent>)OnAiDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiOverlayComponent, ComponentInit>((EntityEventRefHandler<StationAiOverlayComponent, ComponentInit>)OnAiOverlayInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiOverlayComponent, ComponentRemove>((EntityEventRefHandler<StationAiOverlayComponent, ComponentRemove>)OnAiOverlayRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StationAiCoreComponent, AppearanceChangeEvent>((EntityEventRefHandler<StationAiCoreComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAiOverlayInit(Entity<StationAiOverlayComponent> ent, ref ComponentInit args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid owner = ent.Owner;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != owner))
		{
			AddOverlay();
		}
	}

	private void OnAiOverlayRemove(Entity<StationAiOverlayComponent> ent, ref ComponentRemove args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		EntityUid owner = ent.Owner;
		if (localEntity.HasValue && !(localEntity.GetValueOrDefault() != owner))
		{
			RemoveOverlay();
		}
	}

	private void AddOverlay()
	{
		if (_overlay == null)
		{
			_overlay = new StationAiOverlay();
			_overlayMgr.AddOverlay((Overlay)(object)_overlay);
		}
	}

	private void RemoveOverlay()
	{
		if (_overlay != null)
		{
			_overlayMgr.RemoveOverlay((Overlay)(object)_overlay);
			_overlay = null;
		}
	}

	private void OnAiAttached(Entity<StationAiOverlayComponent> ent, ref LocalPlayerAttachedEvent args)
	{
		AddOverlay();
	}

	private void OnAiDetached(Entity<StationAiOverlayComponent> ent, ref LocalPlayerDetachedEvent args)
	{
		RemoveOverlay();
	}

	private void OnAppearanceChange(Entity<StationAiCoreComponent> entity, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			PrototypeLayerData val = default(PrototypeLayerData);
			if (_appearance.TryGetData<PrototypeLayerData>(entity.Owner, (Enum)StationAiVisualState.Key, ref val, args.Component))
			{
				_sprite.LayerSetData(Entity<SpriteComponent>.op_Implicit((entity.Owner, args.Sprite)), (Enum)StationAiVisualState.Key, val);
			}
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity.Owner, args.Sprite)), (Enum)StationAiVisualState.Key, val != null);
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlayMgr.RemoveOverlay<StationAiOverlay>();
	}

	private void InitializePowerToggle()
	{
		((EntitySystem)this).SubscribeLocalEvent<ItemTogglePointLightComponent, GetStationAiRadialEvent>((EntityEventRefHandler<ItemTogglePointLightComponent, GetStationAiRadialEvent>)OnLightGetRadial, (Type[])null, (Type[])null);
	}

	private void OnLightGetRadial(Entity<ItemTogglePointLightComponent> ent, ref GetStationAiRadialEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		ItemToggleComponent itemToggleComponent = default(ItemToggleComponent);
		if (((EntitySystem)this).TryComp<ItemToggleComponent>(ent.Owner, ref itemToggleComponent))
		{
			args.Actions.Add(new StationAiRadial
			{
				Tooltip = ((EntitySystem)this).Loc.GetString("toggle-light"),
				Sprite = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/light.svg.192dpi.png")),
				Event = new StationAiLightEvent
				{
					Enabled = !itemToggleComponent.Activated
				}
			});
		}
	}
}
