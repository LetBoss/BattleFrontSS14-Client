using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Vehicle;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Client._RMC14.Vehicle;

public sealed class VehicleHardpointVisualizerSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleHardpointVisualsComponent, ComponentStartup>((ComponentEventRefHandler<VehicleHardpointVisualsComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleHardpointVisualsComponent, ComponentHandleState>((ComponentEventRefHandler<VehicleHardpointVisualsComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
	}

	private void OnStartup(EntityUid uid, VehicleHardpointVisualsComponent component, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ApplyLayers(uid, component);
	}

	private void OnHandleState(EntityUid uid, VehicleHardpointVisualsComponent component, ref ComponentHandleState args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is VehicleHardpointVisualsComponentState vehicleHardpointVisualsComponentState)
		{
			component.Layers = new List<VehicleHardpointLayerState>(vehicleHardpointVisualsComponentState.Layers);
			ApplyLayers(uid, component);
		}
	}

	private void ApplyLayers(EntityUid uid, VehicleHardpointVisualsComponent component)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(uid, ref sprite))
		{
			return;
		}
		foreach (VehicleHardpointLayerState layer in component.Layers)
		{
			UpdateLayer(sprite, layer.Layer, layer.State);
		}
	}

	private void UpdateLayer(SpriteComponent sprite, string layerMap, string state)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (sprite.LayerMapTryGet((object)layerMap, ref num, false))
		{
			if (string.IsNullOrWhiteSpace(state))
			{
				sprite.LayerSetVisible(num, false);
				return;
			}
			sprite.LayerSetState(num, StateId.op_Implicit(state));
			sprite.LayerSetVisible(num, true);
		}
	}
}
