using System;
using System.Numerics;
using Content.Client.Atmos.Components;
using Content.Shared._RMC14.Atmos;
using Content.Shared.Atmos;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Atmos.EntitySystems;

public sealed class FireVisualizerSystem : VisualizerSystem<FireVisualsComponent>
{
	[Dependency]
	private PointLightSystem _lights;

	private EntityQuery<RMCFireColorComponent> _fireColorQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		_fireColorQuery = ((EntitySystem)this).GetEntityQuery<RMCFireColorComponent>();
		((EntitySystem)this).SubscribeLocalEvent<FireVisualsComponent, ComponentInit>((ComponentEventHandler<FireVisualsComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FireVisualsComponent, ComponentShutdown>((ComponentEventHandler<FireVisualsComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnShutdown(EntityUid uid, FireVisualsComponent component, ComponentShutdown args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (component.LightEntity.HasValue)
		{
			((EntitySystem)this).Del((EntityUid?)component.LightEntity.Value);
			component.LightEntity = null;
		}
		SpriteComponent item = default(SpriteComponent);
		int num = default(int);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item) && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)FireVisualLayers.Fire, ref num, false))
		{
			base.SpriteSystem.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, item)), num, true);
		}
	}

	private void OnComponentInit(EntityUid uid, FireVisualsComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val) && ((EntitySystem)this).TryComp<AppearanceComponent>(uid, ref appearance))
		{
			base.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, val)), (Enum)FireVisualLayers.Fire);
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, val)), (Enum)FireVisualLayers.Fire, false);
			val.LayerSetShader((object)FireVisualLayers.Fire, "unshaded");
			if (component.Sprite != null)
			{
				base.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, val)), (Enum)FireVisualLayers.Fire, new ResPath(component.Sprite), (StateId?)null);
			}
			UpdateAppearance(uid, component, val, appearance);
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, FireVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			UpdateAppearance(uid, component, args.Sprite, args.Component);
		}
	}

	private void UpdateAppearance(EntityUid uid, FireVisualsComponent component, SpriteComponent sprite, AppearanceComponent appearance)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (!base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)FireVisualLayers.Fire, ref num, false))
		{
			return;
		}
		bool flag = default(bool);
		((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)FireVisuals.OnFire, ref flag, appearance);
		float num2 = default(float);
		((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<float>(uid, (Enum)FireVisuals.FireStacks, ref num2, appearance);
		base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, flag);
		if (!flag)
		{
			if (component.LightEntity.HasValue)
			{
				((EntitySystem)this).Del((EntityUid?)component.LightEntity.Value);
				component.LightEntity = null;
			}
			return;
		}
		if (num2 > (float)component.FireStackAlternateState && !string.IsNullOrEmpty(component.AlternateState))
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, StateId.op_Implicit(component.AlternateState));
		}
		else
		{
			base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, StateId.op_Implicit(component.NormalState));
		}
		Color val = component.LightColor;
		RMCFireColorComponent rMCFireColorComponent = default(RMCFireColorComponent);
		if (_fireColorQuery.TryComp(uid, ref rMCFireColorComponent))
		{
			val = rMCFireColorComponent.Color;
			base.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, val);
		}
		EntityUid valueOrDefault = component.LightEntity.GetValueOrDefault();
		if (!component.LightEntity.HasValue)
		{
			valueOrDefault = ((EntitySystem)this).Spawn((string)null, new EntityCoordinates(uid, default(Vector2)));
			component.LightEntity = valueOrDefault;
		}
		PointLightComponent val2 = ((EntitySystem)this).EnsureComp<PointLightComponent>(component.LightEntity.Value);
		((SharedPointLightSystem)_lights).SetColor(component.LightEntity.Value, val, (SharedPointLightComponent)(object)val2);
		((SharedPointLightSystem)_lights).SetRadius(component.LightEntity.Value, Math.Clamp(1.5f + component.LightRadiusPerStack * num2, 0f, component.MaxLightRadius), (SharedPointLightComponent)(object)val2, (MetaDataComponent)null);
		((SharedPointLightSystem)_lights).SetEnergy(component.LightEntity.Value, Math.Clamp(1f + component.LightEnergyPerStack * num2, 0f, component.MaxLightEnergy), (SharedPointLightComponent)(object)val2);
	}
}
