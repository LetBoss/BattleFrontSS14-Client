using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Damage;

public sealed class DamageVisualsSystem : VisualizerSystem<DamageVisualsComponent>
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageVisualsComponent, ComponentInit>((ComponentEventHandler<DamageVisualsComponent, ComponentInit>)InitializeEntity, (Type[])null, (Type[])null);
	}

	private void InitializeEntity(EntityUid entity, DamageVisualsComponent comp, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		VerifyVisualizerSetup(entity, comp);
		if (!comp.Valid)
		{
			((EntitySystem)this).RemCompDeferred<DamageVisualsComponent>(entity);
		}
		else
		{
			InitializeVisualizer(entity, comp);
		}
	}

	private void VerifyVisualizerSetup(EntityUid entity, DamageVisualsComponent damageVisComp)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		if (damageVisComp.Thresholds.Count < 1)
		{
			((EntitySystem)this).Log.Error($"ThresholdsLookup were invalid for entity {entity}. ThresholdsLookup: {damageVisComp.Thresholds}");
			damageVisComp.Valid = false;
			return;
		}
		if (damageVisComp.Divisor == 0f)
		{
			((EntitySystem)this).Log.Error($"Divisor for {entity} is set to zero.");
			damageVisComp.Valid = false;
			return;
		}
		if (damageVisComp.Overlay)
		{
			if (damageVisComp.DamageOverlayGroups == null && damageVisComp.DamageOverlay == null)
			{
				((EntitySystem)this).Log.Error($"Enabled overlay without defined damage overlay sprites on {entity}.");
				damageVisComp.Valid = false;
				return;
			}
			if (damageVisComp.TrackAllDamage && damageVisComp.DamageOverlay == null)
			{
				((EntitySystem)this).Log.Error($"Enabled all damage tracking without a damage overlay sprite on {entity}.");
				damageVisComp.Valid = false;
				return;
			}
			if (!damageVisComp.TrackAllDamage && damageVisComp.DamageOverlay != null)
			{
				((EntitySystem)this).Log.Warning($"Disabled all damage tracking with a damage overlay sprite on {entity}.");
				damageVisComp.Valid = false;
				return;
			}
			if (damageVisComp.TrackAllDamage && damageVisComp.DamageOverlayGroups != null)
			{
				((EntitySystem)this).Log.Warning($"Enabled all damage tracking with damage overlay groups on {entity}.");
				damageVisComp.Valid = false;
				return;
			}
		}
		else if (!damageVisComp.Overlay)
		{
			if (damageVisComp.TargetLayers == null)
			{
				((EntitySystem)this).Log.Error($"Disabled overlay without target layers on {entity}.");
				damageVisComp.Valid = false;
				return;
			}
			if (damageVisComp.DamageOverlayGroups != null || damageVisComp.DamageOverlay != null)
			{
				((EntitySystem)this).Log.Error($"Disabled overlay with defined damage overlay sprites on {entity}.");
				damageVisComp.Valid = false;
				return;
			}
			if (damageVisComp.DamageGroup == null)
			{
				((EntitySystem)this).Log.Error($"Disabled overlay without defined damage group on {entity}.");
				damageVisComp.Valid = false;
				return;
			}
		}
		if (damageVisComp.DamageOverlayGroups != null && damageVisComp.DamageGroup != null)
		{
			((EntitySystem)this).Log.Warning($"Damage overlay sprites and damage group are both defined on {entity}.");
		}
		if (damageVisComp.DamageOverlay != null && damageVisComp.DamageGroup != null)
		{
			((EntitySystem)this).Log.Warning($"Damage overlay sprites and damage group are both defined on {entity}.");
		}
	}

	private void InitializeVisualizer(EntityUid entity, DamageVisualsComponent damageVisComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		DamageableComponent damageableComponent = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(entity, ref val) || !((EntitySystem)this).TryComp<DamageableComponent>(entity, ref damageableComponent) || !((EntitySystem)this).HasComp<AppearanceComponent>(entity))
		{
			return;
		}
		damageVisComp.Thresholds.Add(FixedPoint2.Zero);
		damageVisComp.Thresholds.Sort();
		if (damageVisComp.Thresholds[0] != 0)
		{
			((EntitySystem)this).Log.Error($"ThresholdsLookup were invalid for entity {entity}. ThresholdsLookup: {damageVisComp.Thresholds}");
			damageVisComp.Valid = false;
			return;
		}
		DamageContainerPrototype damageContainerPrototype = default(DamageContainerPrototype);
		if (damageableComponent.DamageContainerID.HasValue && _prototypeManager.TryIndex<DamageContainerPrototype>(damageableComponent.DamageContainerID, ref damageContainerPrototype))
		{
			if (damageVisComp.DamageOverlayGroups != null)
			{
				foreach (string key2 in damageVisComp.DamageOverlayGroups.Keys)
				{
					if (!damageContainerPrototype.SupportedGroups.Contains(key2))
					{
						((EntitySystem)this).Log.Error($"Damage key {key2} was invalid for entity {entity}.");
						damageVisComp.Valid = false;
						return;
					}
					damageVisComp.LastThresholdPerGroup.Add(key2, FixedPoint2.Zero);
				}
			}
			else if (!damageVisComp.Overlay && damageVisComp.DamageGroup != null)
			{
				if (!damageContainerPrototype.SupportedGroups.Contains(damageVisComp.DamageGroup))
				{
					((EntitySystem)this).Log.Error($"Damage keys were invalid for entity {entity}.");
					damageVisComp.Valid = false;
					return;
				}
				damageVisComp.LastThresholdPerGroup.Add(damageVisComp.DamageGroup, FixedPoint2.Zero);
			}
		}
		else
		{
			List<string> list = _prototypeManager.EnumeratePrototypes<DamageGroupPrototype>().Select((DamageGroupPrototype p, int _) => p.ID).ToList();
			if (damageVisComp.DamageOverlayGroups != null)
			{
				foreach (string key3 in damageVisComp.DamageOverlayGroups.Keys)
				{
					if (!list.Contains(key3))
					{
						((EntitySystem)this).Log.Error($"Damage keys were invalid for entity {entity}.");
						damageVisComp.Valid = false;
						return;
					}
					damageVisComp.LastThresholdPerGroup.Add(key3, FixedPoint2.Zero);
				}
			}
			else if (damageVisComp.DamageGroup != null)
			{
				if (!list.Contains(damageVisComp.DamageGroup))
				{
					((EntitySystem)this).Log.Error($"Damage keys were invalid for entity {entity}.");
					damageVisComp.Valid = false;
					return;
				}
				damageVisComp.LastThresholdPerGroup.Add(damageVisComp.DamageGroup, FixedPoint2.Zero);
			}
		}
		List<Enum> targetLayers = damageVisComp.TargetLayers;
		string key;
		DamageVisualizerSprite value;
		if (targetLayers != null && targetLayers.Count > 0)
		{
			int num = default(int);
			foreach (Enum targetLayer in damageVisComp.TargetLayers)
			{
				if (!base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, val)), targetLayer, ref num, false))
				{
					((EntitySystem)this).Log.Warning($"Layer at key {targetLayer} was invalid for entity {entity}.");
				}
				else
				{
					damageVisComp.TargetLayerMapKeys.Add(targetLayer);
				}
			}
			if (damageVisComp.TargetLayerMapKeys.Count == 0)
			{
				((EntitySystem)this).Log.Error($"Target layers were invalid for entity {entity}.");
				damageVisComp.Valid = false;
				return;
			}
			{
				foreach (Enum targetLayerMapKey in damageVisComp.TargetLayerMapKeys)
				{
					int num2 = val.AllLayers.Count();
					int num3 = base.SpriteSystem.LayerMapGet(Entity<SpriteComponent>.op_Implicit((entity, val)), targetLayerMapKey);
					if (num3 + 1 != num2)
					{
						num3++;
					}
					damageVisComp.LayerMapKeyStates.Add(targetLayerMapKey, targetLayerMapKey.ToString());
					if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null)
					{
						foreach (KeyValuePair<string, DamageVisualizerSprite> damageOverlayGroup in damageVisComp.DamageOverlayGroups)
						{
							damageOverlayGroup.Deconstruct(out key, out value);
							string value2 = key;
							DamageVisualizerSprite sprite = value;
							AddDamageLayerToSprite(Entity<SpriteComponent>.op_Implicit((entity, val)), sprite, $"{targetLayerMapKey}_{value2}_{damageVisComp.Thresholds[1]}", $"{targetLayerMapKey}{value2}", num3);
						}
						damageVisComp.DisabledLayers.Add(targetLayerMapKey, value: false);
					}
					else if (damageVisComp.DamageOverlay != null)
					{
						AddDamageLayerToSprite(Entity<SpriteComponent>.op_Implicit((entity, val)), damageVisComp.DamageOverlay, $"{targetLayerMapKey}_{damageVisComp.Thresholds[1]}", $"{targetLayerMapKey}trackDamage", num3);
						damageVisComp.DisabledLayers.Add(targetLayerMapKey, value: false);
					}
				}
				return;
			}
		}
		if (damageVisComp.DamageOverlayGroups != null)
		{
			foreach (KeyValuePair<string, DamageVisualizerSprite> damageOverlayGroup2 in damageVisComp.DamageOverlayGroups)
			{
				damageOverlayGroup2.Deconstruct(out key, out value);
				string text = key;
				DamageVisualizerSprite sprite2 = value;
				AddDamageLayerToSprite(Entity<SpriteComponent>.op_Implicit((entity, val)), sprite2, $"DamageOverlay_{text}_{damageVisComp.Thresholds[1]}", "DamageOverlay" + text);
				damageVisComp.TopMostLayerKey = "DamageOverlay" + text;
			}
			return;
		}
		if (damageVisComp.DamageOverlay != null)
		{
			AddDamageLayerToSprite(Entity<SpriteComponent>.op_Implicit((entity, val)), damageVisComp.DamageOverlay, $"DamageOverlay_{damageVisComp.Thresholds[1]}", "DamageOverlay");
			damageVisComp.TopMostLayerKey = "DamageOverlay";
		}
	}

	private void AddDamageLayerToSprite(Entity<SpriteComponent?> spriteEnt, DamageVisualizerSprite sprite, string state, string mapKey, int? index = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		int num = base.SpriteSystem.AddLayer(spriteEnt, (SpriteSpecifier)new Rsi(new ResPath(sprite.Sprite), state), index);
		base.SpriteSystem.LayerMapSet(spriteEnt, mapKey, num);
		if (sprite.Color != null)
		{
			base.SpriteSystem.LayerSetColor(spriteEnt, num, Color.FromHex((ReadOnlySpan<char>)sprite.Color, (Color?)null));
		}
		base.SpriteSystem.LayerSetVisible(spriteEnt, num, false);
	}

	protected override void OnAppearanceChange(EntityUid uid, DamageVisualsComponent damageVisComp, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (damageVisComp.Valid)
		{
			bool disabled = default(bool);
			if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)DamageVisualizerKeys.Disabled, ref disabled, args.Component))
			{
				damageVisComp.Disabled = disabled;
			}
			if (!damageVisComp.Disabled)
			{
				HandleDamage(uid, args.Component, damageVisComp);
			}
		}
	}

	private void HandleDamage(EntityUid uid, AppearanceComponent component, DamageVisualsComponent damageVisComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		DamageableComponent item = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val) || !((EntitySystem)this).TryComp<DamageableComponent>(uid, ref item))
		{
			return;
		}
		if (damageVisComp.TargetLayers != null && damageVisComp.DamageOverlayGroups != null)
		{
			UpdateDisabledLayers(uid, val, component, damageVisComp);
		}
		if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null && damageVisComp.TargetLayers == null)
		{
			CheckOverlayOrdering(Entity<SpriteComponent>.op_Implicit((uid, val)), damageVisComp);
		}
		bool flag = default(bool);
		if (((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)DamageVisualizerKeys.ForceUpdate, ref flag, component) && flag)
		{
			ForceUpdateLayers(Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit((uid, item, val, damageVisComp)));
			return;
		}
		if (damageVisComp.TrackAllDamage)
		{
			UpdateDamageVisuals(Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit((uid, item, val, damageVisComp)));
			return;
		}
		DamageVisualizerGroupData damageVisualizerGroupData = default(DamageVisualizerGroupData);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<DamageVisualizerGroupData>(uid, (Enum)DamageVisualizerKeys.DamageUpdateGroups, ref damageVisualizerGroupData, component))
		{
			damageVisualizerGroupData = new DamageVisualizerGroupData(((EntitySystem)this).Comp<DamageableComponent>(uid).DamagePerGroup.Keys.ToList());
		}
		UpdateDamageVisuals(damageVisualizerGroupData.GroupList, Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit((uid, item, val, damageVisComp)));
	}

	private void UpdateDisabledLayers(EntityUid uid, SpriteComponent spriteComponent, AppearanceComponent component, DamageVisualsComponent damageVisComp)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		foreach (Enum targetLayerMapKey in damageVisComp.TargetLayerMapKeys)
		{
			((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, targetLayerMapKey, ref flag, component);
			if (damageVisComp.DisabledLayers[targetLayerMapKey] == flag)
			{
				continue;
			}
			damageVisComp.DisabledLayers[targetLayerMapKey] = flag;
			if (damageVisComp.TrackAllDamage)
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), $"{targetLayerMapKey}trackDamage", !flag);
			}
			else
			{
				if (damageVisComp.DamageOverlayGroups == null)
				{
					continue;
				}
				foreach (string key in damageVisComp.DamageOverlayGroups.Keys)
				{
					base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), $"{targetLayerMapKey}{key}", !flag);
				}
			}
		}
	}

	private void CheckOverlayOrdering(Entity<SpriteComponent> spriteEnt, DamageVisualsComponent damageVisComp)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (spriteEnt.Comp[(object)damageVisComp.TopMostLayerKey] == spriteEnt.Comp[spriteEnt.Comp.AllLayers.Count() - 1])
		{
			return;
		}
		if (!damageVisComp.TrackAllDamage && damageVisComp.DamageOverlayGroups != null)
		{
			foreach (KeyValuePair<string, DamageVisualizerSprite> damageOverlayGroup in damageVisComp.DamageOverlayGroups)
			{
				damageOverlayGroup.Deconstruct(out var key, out var value);
				string text = key;
				DamageVisualizerSprite sprite = value;
				FixedPoint2 threshold = damageVisComp.LastThresholdPerGroup[text];
				ReorderOverlaySprite(spriteEnt, damageVisComp, sprite, "DamageOverlay" + text, "DamageOverlay_" + text, threshold);
			}
			return;
		}
		if (damageVisComp.TrackAllDamage && damageVisComp.DamageOverlay != null)
		{
			ReorderOverlaySprite(spriteEnt, damageVisComp, damageVisComp.DamageOverlay, "DamageOverlay", "DamageOverlay", damageVisComp.LastDamageThreshold);
		}
	}

	private void ReorderOverlaySprite(Entity<SpriteComponent> spriteEnt, DamageVisualsComponent damageVisComp, DamageVisualizerSprite sprite, string key, string statePrefix, FixedPoint2 threshold)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		base.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), key, ref num, false);
		bool visible = spriteEnt.Comp[num].Visible;
		base.SpriteSystem.RemoveLayer(spriteEnt.AsNullable(), num, true);
		if (threshold == FixedPoint2.Zero)
		{
			threshold = damageVisComp.Thresholds[1];
		}
		num = base.SpriteSystem.AddLayer(spriteEnt.AsNullable(), (SpriteSpecifier)new Rsi(new ResPath(sprite.Sprite), $"{statePrefix}_{threshold}"), (int?)num);
		base.SpriteSystem.LayerMapSet(spriteEnt.AsNullable(), key, num);
		base.SpriteSystem.LayerSetVisible(spriteEnt.AsNullable(), num, visible);
		damageVisComp.TopMostLayerKey = key;
	}

	private void UpdateDamageVisuals(Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent comp = entity.Comp1;
		SpriteComponent comp2 = entity.Comp2;
		DamageVisualsComponent comp3 = entity.Comp3;
		if (!CheckThresholdBoundary(comp.TotalDamage, comp3.LastDamageThreshold, comp3, out var threshold))
		{
			return;
		}
		comp3.LastDamageThreshold = threshold;
		if (comp3.TargetLayers != null)
		{
			foreach (Enum targetLayerMapKey in comp3.TargetLayerMapKeys)
			{
				UpdateTargetLayer(Entity<SpriteComponent>.op_Implicit((Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp2)), comp3, targetLayerMapKey, threshold);
			}
			return;
		}
		UpdateOverlay(Entity<SpriteComponent>.op_Implicit((Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp2)), threshold);
	}

	private void UpdateDamageVisuals(List<string> delta, Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent comp = entity.Comp1;
		SpriteComponent comp2 = entity.Comp2;
		DamageVisualsComponent comp3 = entity.Comp3;
		DamageGroupPrototype damageGroupPrototype = default(DamageGroupPrototype);
		foreach (string deltum in delta)
		{
			if ((!comp3.Overlay && deltum != comp3.DamageGroup) || !_prototypeManager.TryIndex<DamageGroupPrototype>(deltum, ref damageGroupPrototype) || !comp.Damage.TryGetDamageInGroup(damageGroupPrototype, out var total) || !comp3.LastThresholdPerGroup.TryGetValue(deltum, out var value) || !CheckThresholdBoundary(total, value, comp3, out var threshold))
			{
				continue;
			}
			comp3.LastThresholdPerGroup[deltum] = threshold;
			if (comp3.TargetLayers != null)
			{
				foreach (Enum targetLayerMapKey in comp3.TargetLayerMapKeys)
				{
					UpdateTargetLayer(Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit((Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp2, comp3)), targetLayerMapKey, deltum, threshold);
				}
			}
			else
			{
				UpdateOverlay(Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit((Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp2, comp3)), deltum, threshold);
			}
		}
	}

	private bool CheckThresholdBoundary(FixedPoint2 damageTotal, FixedPoint2 lastThreshold, DamageVisualsComponent damageVisComp, out FixedPoint2 threshold)
	{
		threshold = FixedPoint2.Zero;
		damageTotal /= damageVisComp.Divisor;
		int num = damageVisComp.Thresholds.BinarySearch(damageTotal);
		if (num < 0)
		{
			num = ~num;
			threshold = damageVisComp.Thresholds[num - 1];
		}
		else
		{
			threshold = damageVisComp.Thresholds[num];
		}
		if (threshold == lastThreshold)
		{
			return false;
		}
		return true;
	}

	private void ForceUpdateLayers(Entity<DamageableComponent, SpriteComponent, DamageVisualsComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		DamageVisualsComponent comp = entity.Comp3;
		if (comp.DamageOverlayGroups != null)
		{
			UpdateDamageVisuals(comp.DamageOverlayGroups.Keys.ToList(), entity);
		}
		else if (comp.DamageGroup != null)
		{
			UpdateDamageVisuals(new List<string> { comp.DamageGroup }, entity);
		}
		else if (comp.DamageOverlay != null)
		{
			UpdateDamageVisuals(entity);
		}
	}

	private void UpdateTargetLayer(Entity<SpriteComponent> spriteEnt, DamageVisualsComponent damageVisComp, object layerMapKey, FixedPoint2 threshold)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (damageVisComp.Overlay && damageVisComp.DamageOverlayGroups != null)
		{
			if (!damageVisComp.DisabledLayers[layerMapKey])
			{
				string text = damageVisComp.LayerMapKeyStates[layerMapKey];
				int spriteLayer = default(int);
				base.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), $"{layerMapKey}trackDamage", ref spriteLayer, false);
				UpdateDamageLayerState(spriteEnt, spriteLayer, text ?? "", threshold);
			}
		}
		else if (!damageVisComp.Overlay)
		{
			string text2 = damageVisComp.LayerMapKeyStates[layerMapKey];
			int spriteLayer2 = default(int);
			base.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), $"{layerMapKey}", ref spriteLayer2, false);
			UpdateDamageLayerState(spriteEnt, spriteLayer2, text2 ?? "", threshold);
		}
	}

	private void UpdateTargetLayer(Entity<SpriteComponent, DamageVisualsComponent> entity, object layerMapKey, string damageGroup, FixedPoint2 threshold)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent comp = entity.Comp1;
		DamageVisualsComponent comp2 = entity.Comp2;
		if (comp2.Overlay && comp2.DamageOverlayGroups != null)
		{
			if (comp2.DamageOverlayGroups.ContainsKey(damageGroup) && !comp2.DisabledLayers[layerMapKey])
			{
				string text = comp2.LayerMapKeyStates[layerMapKey];
				int spriteLayer = default(int);
				base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp)), $"{layerMapKey}{damageGroup}", ref spriteLayer, false);
				UpdateDamageLayerState(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp)), spriteLayer, text + "_" + damageGroup, threshold);
			}
		}
		else if (!comp2.Overlay)
		{
			string text2 = comp2.LayerMapKeyStates[layerMapKey];
			int spriteLayer2 = default(int);
			base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp)), $"{layerMapKey}", ref spriteLayer2, false);
			UpdateDamageLayerState(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp)), spriteLayer2, text2 + "_" + damageGroup, threshold);
		}
	}

	private void UpdateOverlay(Entity<SpriteComponent> spriteEnt, FixedPoint2 threshold)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		int spriteLayer = default(int);
		base.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), "DamageOverlay", ref spriteLayer, false);
		UpdateDamageLayerState(spriteEnt, spriteLayer, "DamageOverlay", threshold);
	}

	private void UpdateOverlay(Entity<SpriteComponent, DamageVisualsComponent> entity, string damageGroup, FixedPoint2 threshold)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent comp = entity.Comp1;
		DamageVisualsComponent comp2 = entity.Comp2;
		if (comp2.DamageOverlayGroups != null && comp2.DamageOverlayGroups.ContainsKey(damageGroup))
		{
			int spriteLayer = default(int);
			base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp)), "DamageOverlay" + damageGroup, ref spriteLayer, false);
			UpdateDamageLayerState(Entity<SpriteComponent>.op_Implicit((Entity<SpriteComponent, DamageVisualsComponent>.op_Implicit(entity), comp)), spriteLayer, "DamageOverlay_" + damageGroup, threshold);
		}
	}

	private void UpdateDamageLayerState(Entity<SpriteComponent> spriteEnt, int spriteLayer, string statePrefix, FixedPoint2 threshold)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (threshold == 0)
		{
			base.SpriteSystem.LayerSetVisible(spriteEnt.AsNullable(), spriteLayer, false);
			return;
		}
		if (!spriteEnt.Comp[spriteLayer].Visible)
		{
			base.SpriteSystem.LayerSetVisible(spriteEnt.AsNullable(), spriteLayer, true);
		}
		base.SpriteSystem.LayerSetRsiState(spriteEnt.AsNullable(), spriteLayer, StateId.op_Implicit($"{statePrefix}_{threshold}"));
	}

	public void ChangeDamageGroupColor(Entity<SpriteComponent> spriteEnt, DamageVisualsComponent damageVisuals, string group, string color)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (damageVisuals.TargetLayers == null || damageVisuals.DamageOverlayGroups == null)
		{
			return;
		}
		int num = default(int);
		foreach (Enum targetLayerMapKey in damageVisuals.TargetLayerMapKeys)
		{
			if (base.SpriteSystem.LayerMapTryGet(spriteEnt.AsNullable(), $"{targetLayerMapKey}{group}", ref num, false))
			{
				base.SpriteSystem.LayerSetColor(spriteEnt.AsNullable(), num, Color.FromHex((ReadOnlySpan<char>)color, (Color?)null));
			}
		}
		damageVisuals.DamageOverlayGroups[group].Color = color;
	}
}
