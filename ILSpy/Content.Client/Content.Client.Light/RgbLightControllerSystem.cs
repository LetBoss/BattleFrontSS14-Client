using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Client.Items.Systems;
using Content.Shared.Clothing;
using Content.Shared.Hands;
using Content.Shared.Inventory.Events;
using Content.Shared.Light;
using Content.Shared.Light.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Light;

public sealed class RgbLightControllerSystem : SharedRgbLightControllerSystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private ItemSystem _itemSystem;

	[Dependency]
	private SharedPointLightSystem _lights;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RgbLightControllerComponent, ComponentHandleState>((ComponentEventRefHandler<RgbLightControllerComponent, ComponentHandleState>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RgbLightControllerComponent, ComponentShutdown>((ComponentEventHandler<RgbLightControllerComponent, ComponentShutdown>)OnComponentShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RgbLightControllerComponent, ComponentStartup>((ComponentEventHandler<RgbLightControllerComponent, ComponentStartup>)OnComponentStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RgbLightControllerComponent, GotUnequippedEvent>((ComponentEventHandler<RgbLightControllerComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RgbLightControllerComponent, EquipmentVisualsUpdatedEvent>((ComponentEventHandler<RgbLightControllerComponent, EquipmentVisualsUpdatedEvent>)OnEquipmentVisualsUpdated, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RgbLightControllerComponent, HeldVisualsUpdatedEvent>((ComponentEventHandler<RgbLightControllerComponent, HeldVisualsUpdatedEvent>)OnHeldVisualsUpdated, (Type[])null, (Type[])null);
	}

	private void OnComponentStart(EntityUid uid, RgbLightControllerComponent rgb, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		GetOriginalColors(uid, rgb);
		_itemSystem.VisualsChanged(uid);
	}

	private void OnComponentShutdown(EntityUid uid, RgbLightControllerComponent rgb, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((EntitySystem)this).LifeStage(uid, (MetaDataComponent)null) < 4)
		{
			ResetOriginalColors(uid, rgb);
			_itemSystem.VisualsChanged(uid);
		}
	}

	private void OnGotUnequipped(EntityUid uid, RgbLightControllerComponent rgb, GotUnequippedEvent args)
	{
		rgb.Holder = null;
		rgb.HolderLayers = null;
	}

	private void OnHeldVisualsUpdated(EntityUid uid, RgbLightControllerComponent rgb, HeldVisualsUpdatedEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (args.RevealedLayers.Count == 0)
		{
			rgb.Holder = null;
			rgb.HolderLayers = null;
			return;
		}
		rgb.Holder = args.User;
		rgb.HolderLayers = new List<string>();
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(args.User, ref val))
		{
			return;
		}
		int num = default(int);
		foreach (string revealedLayer in args.RevealedLayers)
		{
			if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((args.User, val)), revealedLayer, ref num, false))
			{
				continue;
			}
			ISpriteLayer obj = val[num];
			Layer val2 = (Layer)(object)((obj is Layer) ? obj : null);
			if (val2 != null)
			{
				ProtoId<ShaderPrototype>? shaderPrototype = val2.ShaderPrototype;
				ProtoId<ShaderPrototype>? val3 = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
				if (shaderPrototype.HasValue == val3.HasValue && (!shaderPrototype.HasValue || shaderPrototype.GetValueOrDefault() == val3.GetValueOrDefault()))
				{
					rgb.HolderLayers.Add(revealedLayer);
				}
			}
		}
	}

	private void OnEquipmentVisualsUpdated(EntityUid uid, RgbLightControllerComponent rgb, EquipmentVisualsUpdatedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		rgb.Holder = args.Equipee;
		rgb.HolderLayers = new List<string>();
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(args.Equipee, ref val))
		{
			return;
		}
		int num = default(int);
		foreach (string revealedLayer in args.RevealedLayers)
		{
			if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((args.Equipee, val)), revealedLayer, ref num, false))
			{
				continue;
			}
			ISpriteLayer obj = val[num];
			Layer val2 = (Layer)(object)((obj is Layer) ? obj : null);
			if (val2 != null)
			{
				ProtoId<ShaderPrototype>? shaderPrototype = val2.ShaderPrototype;
				ProtoId<ShaderPrototype>? val3 = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
				if (shaderPrototype.HasValue == val3.HasValue && (!shaderPrototype.HasValue || shaderPrototype.GetValueOrDefault() == val3.GetValueOrDefault()))
				{
					rgb.HolderLayers.Add(revealedLayer);
				}
			}
		}
	}

	private void OnHandleState(EntityUid uid, RgbLightControllerComponent rgb, ref ComponentHandleState args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (((ComponentHandleState)(ref args)).Current is RgbLightControllerState rgbLightControllerState)
		{
			ResetOriginalColors(uid, rgb);
			rgb.CycleRate = rgbLightControllerState.CycleRate;
			rgb.Layers = rgbLightControllerState.Layers;
			GetOriginalColors(uid, rgb);
		}
	}

	private void GetOriginalColors(EntityUid uid, RgbLightControllerComponent? rgb = null, PointLightComponent? light = null, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RgbLightControllerComponent, SpriteComponent, PointLightComponent>(uid, ref rgb, ref sprite, ref light, true))
		{
			return;
		}
		rgb.OriginalLightColor = ((SharedPointLightComponent)light).Color;
		rgb.OriginalLayerColors = new Dictionary<int, Color>();
		int num = sprite.AllLayers.Count();
		if (rgb.Layers == null)
		{
			rgb.Layers = new List<int>();
			for (int i = 0; i < num; i++)
			{
				ISpriteLayer obj = sprite[i];
				Layer val = (Layer)(object)((obj is Layer) ? obj : null);
				if (val != null)
				{
					ProtoId<ShaderPrototype>? shaderPrototype = val.ShaderPrototype;
					ProtoId<ShaderPrototype>? val2 = ProtoId<ShaderPrototype>.op_Implicit("unshaded");
					if (shaderPrototype.HasValue == val2.HasValue && (!shaderPrototype.HasValue || shaderPrototype.GetValueOrDefault() == val2.GetValueOrDefault()))
					{
						rgb.Layers.Add(i);
						rgb.OriginalLayerColors[i] = val.Color;
					}
				}
			}
			return;
		}
		int[] array = rgb.Layers.ToArray();
		foreach (int num2 in array)
		{
			if (num2 < num)
			{
				rgb.OriginalLayerColors[num2] = sprite[num2].Color;
				continue;
			}
			((EntitySystem)this).Log.Warning($"RGB light attempted to use invalid sprite index {num2} on entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			rgb.Layers.Remove(num2);
		}
	}

	private void ResetOriginalColors(EntityUid uid, RgbLightControllerComponent? rgb = null, PointLightComponent? light = null, SpriteComponent? sprite = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RgbLightControllerComponent, SpriteComponent, PointLightComponent>(uid, ref rgb, ref sprite, ref light, false))
		{
			return;
		}
		_lights.SetColor(uid, rgb.OriginalLightColor, (SharedPointLightComponent)(object)light);
		if (rgb.Layers == null || rgb.OriginalLayerColors == null)
		{
			return;
		}
		foreach (var (num2, val2) in rgb.OriginalLayerColors)
		{
			_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, val2);
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RgbLightControllerComponent, PointLightComponent, SpriteComponent> val = ((EntitySystem)this).EntityQueryEnumerator<RgbLightControllerComponent, PointLightComponent, SpriteComponent>();
		EntityUid val2 = default(EntityUid);
		RgbLightControllerComponent rgbLightControllerComponent = default(RgbLightControllerComponent);
		PointLightComponent val3 = default(PointLightComponent);
		SpriteComponent item = default(SpriteComponent);
		Layer val4 = default(Layer);
		SpriteComponent item2 = default(SpriteComponent);
		int num = default(int);
		while (val.MoveNext(ref val2, ref rgbLightControllerComponent, ref val3, ref item))
		{
			Color currentRgbColor = GetCurrentRgbColor(_gameTiming.RealTime, ((Component)rgbLightControllerComponent).CreationTick.Value * _gameTiming.TickPeriod, Entity<RgbLightControllerComponent>.op_Implicit((val2, rgbLightControllerComponent)));
			_lights.SetColor(val2, currentRgbColor, (SharedPointLightComponent)(object)val3);
			if (rgbLightControllerComponent.Layers != null)
			{
				foreach (int layer in rgbLightControllerComponent.Layers)
				{
					if (_sprite.TryGetLayer(Entity<SpriteComponent>.op_Implicit((val2, item)), layer, ref val4, false))
					{
						val4.Color = currentRgbColor;
					}
				}
			}
			if (rgbLightControllerComponent.HolderLayers == null || !((EntitySystem)this).TryComp<SpriteComponent>(rgbLightControllerComponent.Holder, ref item2))
			{
				continue;
			}
			foreach (string holderLayer in rgbLightControllerComponent.HolderLayers)
			{
				if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((rgbLightControllerComponent.Holder.Value, item2)), holderLayer, ref num, false))
				{
					_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((rgbLightControllerComponent.Holder.Value, item2)), num, currentRgbColor);
				}
			}
		}
		EntityQueryEnumerator<MapLightComponent, RgbLightControllerComponent> val5 = ((EntitySystem)this).EntityQueryEnumerator<MapLightComponent, RgbLightControllerComponent>();
		EntityUid item3 = default(EntityUid);
		MapLightComponent val6 = default(MapLightComponent);
		RgbLightControllerComponent rgbLightControllerComponent2 = default(RgbLightControllerComponent);
		while (val5.MoveNext(ref item3, ref val6, ref rgbLightControllerComponent2))
		{
			Color currentRgbColor2 = GetCurrentRgbColor(_gameTiming.RealTime, ((Component)rgbLightControllerComponent2).CreationTick.Value * _gameTiming.TickPeriod, Entity<RgbLightControllerComponent>.op_Implicit((item3, rgbLightControllerComponent2)));
			val6.AmbientLightColor = currentRgbColor2;
		}
	}

	public static Color GetCurrentRgbColor(TimeSpan curTime, TimeSpan offset, Entity<RgbLightControllerComponent> rgb)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)(curTime - offset).TotalSeconds;
		float num2 = Math.Abs((float)rgb.Owner.Id * 0.09817f);
		return Color.FromHsv(new Vector4(MathF.Abs((num * rgb.Comp.CycleRate + num2) % 1f), 1f, 1f, 1f));
	}
}
