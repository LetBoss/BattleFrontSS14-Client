using System;
using System.Collections.Generic;
using Content.Client.DisplacementMap;
using Content.Client.Items.Systems;
using Content.Shared._RMC14.Humanoid;
using Content.Shared.CCVar;
using Content.Shared.DisplacementMap;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Humanoid;

public sealed class HumanoidAppearanceSystem : SharedHumanoidAppearanceSystem
{
	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private MarkingManager _markingManager;

	[Dependency]
	private IConfigurationManager _configurationManager;

	[Dependency]
	private DisplacementMapSystem _displacement;

	[Dependency]
	private SpriteSystem _sprite;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private ItemSystem _item;

	[Dependency]
	private RMCHumanoidAppearanceSystem _rmcHumanoid;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HumanoidAppearanceComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<HumanoidAppearanceComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configurationManager, CCVars.AccessibilityClientCensorNudity, (Action<bool>)OnCvarChanged, true);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _configurationManager, CCVars.AccessibilityServerCensorNudity, (Action<bool>)OnCvarChanged, true);
		((EntitySystem)this).SubscribeLocalEvent<LocalPlayerAttachedEvent>((EntityEventHandler<LocalPlayerAttachedEvent>)UpdateHiddenSprites<LocalPlayerAttachedEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LocalPlayerDetachedEvent>((EntityEventHandler<LocalPlayerDetachedEvent>)UpdateHiddenSprites<LocalPlayerDetachedEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiddenAppearanceComponent, ComponentRemove>((EntityEventRefHandler<HiddenAppearanceComponent, ComponentRemove>)OnHiddenRemove, (Type[])null, (Type[])null);
	}

	private void OnHandleState(EntityUid uid, HumanoidAppearanceComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		UpdateSprite(uid, component, ((EntitySystem)this).Comp<SpriteComponent>(uid));
	}

	private void OnCvarChanged(bool value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<HumanoidAppearanceComponent, SpriteComponent> val = ((EntitySystem)this).AllEntityQuery<HumanoidAppearanceComponent, SpriteComponent>();
		EntityUid entity = default(EntityUid);
		HumanoidAppearanceComponent humanoid = default(HumanoidAppearanceComponent);
		SpriteComponent sprite = default(SpriteComponent);
		while (val.MoveNext(ref entity, ref humanoid, ref sprite))
		{
			UpdateSprite(entity, humanoid, sprite);
		}
	}

	private void UpdateSprite(EntityUid entity, IRMCHumanoidAppearance humanoid, SpriteComponent sprite)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		ClearAllMarkings(entity, humanoid, sprite);
		int num = default(int);
		foreach (KeyValuePair<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> baseLayer in humanoid.BaseLayers)
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum)baseLayer.Key, ref num, false))
			{
				sprite[num].Visible = false;
			}
		}
		IRMCHumanoidAppearance iRMCHumanoidAppearance = humanoid;
		if (_rmcHumanoid.TryGetLocalHiddenAppearance(entity, out IRMCHumanoidAppearance appearance))
		{
			iRMCHumanoidAppearance = appearance;
		}
		if (appearance != null)
		{
			ClearAllMarkings(entity, appearance, sprite);
			int num2 = default(int);
			foreach (KeyValuePair<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> baseLayer2 in appearance.BaseLayers)
			{
				if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum)baseLayer2.Key, ref num2, false))
				{
					sprite[num2].Visible = false;
				}
			}
		}
		UpdateLayers(entity, iRMCHumanoidAppearance, sprite);
		ApplyMarkingSet(entity, iRMCHumanoidAppearance, sprite);
		sprite[_sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum)HumanoidVisualLayers.Eyes)].Color = iRMCHumanoidAppearance.EyeColor;
	}

	private static bool IsHidden(IRMCHumanoidAppearance humanoid, HumanoidVisualLayers layer)
	{
		if (!humanoid.HiddenLayers.ContainsKey(layer))
		{
			return humanoid.PermanentlyHidden.Contains(layer);
		}
		return true;
	}

	private void UpdateLayers(EntityUid entity, IRMCHumanoidAppearance component, SpriteComponent sprite)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		HashSet<HumanoidVisualLayers> hashSet = new HashSet<HumanoidVisualLayers>(component.BaseLayers.Keys);
		component.BaseLayers.Clear();
		SpeciesPrototype speciesPrototype = _prototypeManager.Index<SpeciesPrototype>(component.Species);
		HumanoidVisualLayers key;
		foreach (KeyValuePair<HumanoidVisualLayers, string> sprite2 in _prototypeManager.Index<HumanoidSpeciesBaseSpritesPrototype>(speciesPrototype.SpriteSet).Sprites)
		{
			sprite2.Deconstruct(out key, out var value);
			HumanoidVisualLayers humanoidVisualLayers = key;
			string protoId = value;
			hashSet.Remove(humanoidVisualLayers);
			if (!component.CustomBaseLayers.ContainsKey(humanoidVisualLayers))
			{
				SetLayerData(entity, component, sprite, humanoidVisualLayers, protoId, sexMorph: true);
			}
		}
		foreach (KeyValuePair<HumanoidVisualLayers, CustomBaseLayerInfo> customBaseLayer in component.CustomBaseLayers)
		{
			customBaseLayer.Deconstruct(out key, out var value2);
			HumanoidVisualLayers humanoidVisualLayers2 = key;
			CustomBaseLayerInfo customBaseLayerInfo = value2;
			hashSet.Remove(humanoidVisualLayers2);
			ProtoId<HumanoidSpeciesSpriteLayer>? id = customBaseLayerInfo.Id;
			SetLayerData(entity, component, sprite, humanoidVisualLayers2, id.HasValue ? ProtoId<HumanoidSpeciesSpriteLayer>.op_Implicit(id.GetValueOrDefault()) : null, sexMorph: false, customBaseLayerInfo.Color);
		}
		int num = default(int);
		foreach (HumanoidVisualLayers item in hashSet)
		{
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum)item, ref num, false))
			{
				sprite[num].Visible = false;
			}
		}
	}

	private void SetLayerData(EntityUid entity, IRMCHumanoidAppearance component, SpriteComponent sprite, HumanoidVisualLayers key, string? protoId, bool sexMorph = false, Color? color = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		int num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum)key);
		ISpriteLayer val = sprite[num];
		val.Visible = !IsHidden(component, key);
		if (color.HasValue)
		{
			val.Color = color.Value;
		}
		if (protoId != null)
		{
			if (sexMorph)
			{
				protoId = HumanoidVisualLayersExtension.GetSexMorph(key, component.Sex, protoId);
			}
			HumanoidSpeciesSpriteLayer humanoidSpeciesSpriteLayer = _prototypeManager.Index<HumanoidSpeciesSpriteLayer>(protoId);
			component.BaseLayers[key] = humanoidSpeciesSpriteLayer;
			if (humanoidSpeciesSpriteLayer.MatchSkin)
			{
				Color skinColor = component.SkinColor;
				val.Color = ((Color)(ref skinColor)).WithAlpha(humanoidSpeciesSpriteLayer.LayerAlpha);
			}
			if (humanoidSpeciesSpriteLayer.BaseSprite != null)
			{
				_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((entity, sprite)), num, humanoidSpeciesSpriteLayer.BaseSprite);
			}
		}
	}

	public override void LoadProfile(EntityUid uid, HumanoidCharacterProfile? profile, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		if (profile == null || !((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
		{
			return;
		}
		Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> customBaseLayers = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>();
		MarkingSet markingSet = new MarkingSet(ProtoId<MarkingPointsPrototype>.op_Implicit(_prototypeManager.Index<SpeciesPrototype>(profile.Species).MarkingPoints), _markingManager, _prototypeManager);
		Dictionary<Marking, MarkingPrototype> dictionary = new Dictionary<Marking, MarkingPrototype>();
		foreach (Marking marking4 in profile.Appearance.Markings)
		{
			if (_markingManager.TryGetMarking(marking4, out MarkingPrototype markingResult))
			{
				if (!markingResult.ForcedColoring)
				{
					markingSet.AddBack(markingResult.MarkingCategory, marking4);
				}
				else
				{
					dictionary.Add(marking4, markingResult);
				}
			}
		}
		Color val;
		Color skinColor;
		if (!_markingManager.MustMatchSkin(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), HumanoidVisualLayers.Hair, out var alpha, _prototypeManager))
		{
			val = profile.Appearance.HairColor;
		}
		else
		{
			skinColor = profile.Appearance.SkinColor;
			val = ((Color)(ref skinColor)).WithAlpha(alpha);
		}
		Color val2 = val;
		Marking marking = new Marking(profile.Appearance.HairStyleId, (IReadOnlyList<Color>)(object)new Color[1] { val2 });
		Color val3;
		if (!_markingManager.MustMatchSkin(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), HumanoidVisualLayers.FacialHair, out var alpha2, _prototypeManager))
		{
			val3 = profile.Appearance.FacialHairColor;
		}
		else
		{
			skinColor = profile.Appearance.SkinColor;
			val3 = ((Color)(ref skinColor)).WithAlpha(alpha2);
		}
		Color val4 = val3;
		Marking marking2 = new Marking(profile.Appearance.FacialHairStyleId, (IReadOnlyList<Color>)(object)new Color[1] { val4 });
		if (_markingManager.CanBeApplied(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), profile.Sex, marking, _prototypeManager))
		{
			markingSet.AddBack(MarkingCategories.Hair, marking);
		}
		if (_markingManager.CanBeApplied(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), profile.Sex, marking2, _prototypeManager))
		{
			markingSet.AddBack(MarkingCategories.FacialHair, marking2);
		}
		foreach (KeyValuePair<Marking, MarkingPrototype> item in dictionary)
		{
			item.Deconstruct(out var key, out var value);
			Marking marking3 = key;
			MarkingPrototype markingPrototype = value;
			List<Color> markingLayerColors = MarkingColoring.GetMarkingLayerColors(markingPrototype, profile.Appearance.SkinColor, profile.Appearance.EyeColor, markingSet);
			markingSet.AddBack(markingPrototype.MarkingCategory, new Marking(marking3.MarkingId, markingLayerColors));
		}
		markingSet.EnsureSpecies(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), profile.Appearance.SkinColor, _markingManager, _prototypeManager);
		markingSet.EnsureSexes(profile.Sex, _markingManager);
		markingSet.EnsureDefault(profile.Appearance.SkinColor, profile.Appearance.EyeColor, _markingManager);
		humanoid.MarkingSet = markingSet;
		humanoid.PermanentlyHidden = new HashSet<HumanoidVisualLayers>();
		humanoid.HiddenLayers = new Dictionary<HumanoidVisualLayers, SlotFlags>();
		humanoid.CustomBaseLayers = customBaseLayers;
		humanoid.Sex = profile.Sex;
		humanoid.Gender = profile.Gender;
		humanoid.Age = profile.Age;
		humanoid.Species = profile.Species;
		humanoid.SkinColor = profile.Appearance.SkinColor;
		humanoid.EyeColor = profile.Appearance.EyeColor;
		UpdateSprite(uid, humanoid, ((EntitySystem)this).Comp<SpriteComponent>(uid));
	}

	private void ApplyMarkingSet(EntityUid entity, IRMCHumanoidAppearance humanoid, SpriteComponent sprite)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		ClearAllMarkings(entity, humanoid, sprite);
		bool undergarmentTop;
		bool undergarmentBottom = (undergarmentTop = _configurationManager.GetCVar<bool>(CCVars.AccessibilityClientCensorNudity) || _configurationManager.GetCVar<bool>(CCVars.AccessibilityServerCensorNudity));
		foreach (List<Marking> value in humanoid.MarkingSet.Markings.Values)
		{
			foreach (Marking item in value)
			{
				if (_markingManager.TryGetMarking(item, out MarkingPrototype markingResult))
				{
					ApplyMarking(markingResult, item.MarkingColors, item.Visible, entity, humanoid, sprite);
					if (markingResult.BodyPart == HumanoidVisualLayers.UndergarmentTop)
					{
						undergarmentTop = false;
					}
					else if (markingResult.BodyPart == HumanoidVisualLayers.UndergarmentBottom)
					{
						undergarmentBottom = false;
					}
				}
			}
		}
		humanoid.ClientOldMarkings = new MarkingSet(humanoid.MarkingSet);
		AddUndergarments(entity, humanoid, sprite, undergarmentTop, undergarmentBottom);
	}

	private void ClearAllMarkings(EntityUid entity, IRMCHumanoidAppearance humanoid, SpriteComponent sprite)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		foreach (List<Marking> value in humanoid.ClientOldMarkings.Markings.Values)
		{
			foreach (Marking item in value)
			{
				RemoveMarking(item, Entity<SpriteComponent>.op_Implicit((entity, sprite)));
			}
		}
		humanoid.ClientOldMarkings.Clear();
		foreach (List<Marking> value2 in humanoid.MarkingSet.Markings.Values)
		{
			foreach (Marking item2 in value2)
			{
				RemoveMarking(item2, Entity<SpriteComponent>.op_Implicit((entity, sprite)));
			}
		}
	}

	private void RemoveMarking(Marking marking, Entity<SpriteComponent> spriteEnt)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (!_markingManager.TryGetMarking(marking, out MarkingPrototype markingResult))
		{
			return;
		}
		int num = default(int);
		foreach (SpriteSpecifier sprite in markingResult.Sprites)
		{
			Rsi val = (Rsi)(object)((sprite is Rsi) ? sprite : null);
			if (val != null)
			{
				string text = marking.MarkingId + "-" + val.RsiState;
				if (_sprite.LayerMapTryGet(spriteEnt.AsNullable(), text, ref num, false))
				{
					_sprite.LayerMapRemove(spriteEnt.AsNullable(), text);
					_sprite.RemoveLayer(spriteEnt.AsNullable(), num, true);
				}
			}
		}
	}

	private void AddUndergarments(EntityUid entity, IRMCHumanoidAppearance humanoid, SpriteComponent sprite, bool undergarmentTop, bool undergarmentBottom)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (undergarmentTop && humanoid.UndergarmentTop.HasValue)
		{
			ProtoId<MarkingPrototype>? undergarmentTop2 = humanoid.UndergarmentTop;
			Marking marking = new Marking(undergarmentTop2.HasValue ? ProtoId<MarkingPrototype>.op_Implicit(undergarmentTop2.GetValueOrDefault()) : null, new List<Color> { default(Color) });
			if (_markingManager.TryGetMarking(marking, out MarkingPrototype markingResult))
			{
				humanoid.ClientOldMarkings.Markings.Add(MarkingCategories.UndergarmentTop, new List<Marking> { marking });
				ApplyMarking(markingResult, null, visible: true, entity, humanoid, sprite);
			}
		}
		if (undergarmentBottom && humanoid.UndergarmentBottom.HasValue)
		{
			ProtoId<MarkingPrototype>? undergarmentTop2 = humanoid.UndergarmentBottom;
			Marking marking2 = new Marking(undergarmentTop2.HasValue ? ProtoId<MarkingPrototype>.op_Implicit(undergarmentTop2.GetValueOrDefault()) : null, new List<Color> { default(Color) });
			if (_markingManager.TryGetMarking(marking2, out MarkingPrototype markingResult2))
			{
				humanoid.ClientOldMarkings.Markings.Add(MarkingCategories.UndergarmentBottom, new List<Marking> { marking2 });
				ApplyMarking(markingResult2, null, visible: true, entity, humanoid, sprite);
			}
		}
	}

	private void ApplyMarking(MarkingPrototype markingPrototype, IReadOnlyList<Color>? colors, bool visible, EntityUid entity, IRMCHumanoidAppearance humanoid, SpriteComponent sprite)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), (Enum)markingPrototype.BodyPart, ref num, false))
		{
			return;
		}
		visible &= !IsHidden(humanoid, markingPrototype.BodyPart);
		visible &= humanoid.BaseLayers.TryGetValue(markingPrototype.BodyPart, out HumanoidSpeciesSpriteLayer value) && value.AllowsMarkings;
		int num2 = default(int);
		for (int i = 0; i < markingPrototype.Sprites.Count; i++)
		{
			SpriteSpecifier val = markingPrototype.Sprites[i];
			Rsi val2 = (Rsi)(object)((val is Rsi) ? val : null);
			if (val2 == null)
			{
				continue;
			}
			string text = markingPrototype.ID + "-" + val2.RsiState;
			if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), text, ref num2, false))
			{
				int num3 = _sprite.AddLayer(Entity<SpriteComponent>.op_Implicit((entity, sprite)), val, (int?)(num + i + 1));
				_sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((entity, sprite)), text, num3);
				_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((entity, sprite)), text, (SpriteSpecifier)(object)val2);
			}
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((entity, sprite)), text, visible);
			if (visible && value != null)
			{
				if (colors != null && i < colors.Count)
				{
					_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((entity, sprite)), text, colors[i]);
				}
				else
				{
					_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((entity, sprite)), text, Color.White);
				}
				if (humanoid.MarkingsDisplacement.TryGetValue(markingPrototype.BodyPart, out DisplacementData value2) && markingPrototype.CanBeDisplaced)
				{
					_displacement.TryAddDisplacement(value2, Entity<SpriteComponent>.op_Implicit((entity, sprite)), num + i + 1, text, out string _);
				}
			}
		}
	}

	public override void SetSkinColor(EntityUid uid, Color skinColor, bool sync = true, bool verify = true, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || humanoid.SkinColor == skinColor)
		{
			return;
		}
		base.SetSkinColor(uid, skinColor, sync: false, verify, humanoid);
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(uid, ref val))
		{
			return;
		}
		foreach (var (humanoidVisualLayers2, humanoidSpeciesSpriteLayer2) in humanoid.BaseLayers)
		{
			if (humanoidSpeciesSpriteLayer2.MatchSkin)
			{
				int num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, val)), (Enum)humanoidVisualLayers2);
				val[num].Color = ((Color)(ref skinColor)).WithAlpha(humanoidSpeciesSpriteLayer2.LayerAlpha);
			}
		}
	}

	public override void SetLayerVisibility(Entity<HumanoidAppearanceComponent> ent, HumanoidVisualLayers layer, bool visible, SlotFlags? slot, ref bool dirty)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		base.SetLayerVisibility(ent, layer, visible, slot, ref dirty);
		SpriteComponent val = ((EntitySystem)this).Comp<SpriteComponent>(Entity<HumanoidAppearanceComponent>.op_Implicit(ent));
		int num = default(int);
		if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), (Enum)layer, ref num, false))
		{
			if (!visible)
			{
				return;
			}
			num = _sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((ent.Owner, val)), (Enum)layer);
		}
		ISpriteLayer val2 = val[num];
		if (val2.Visible == visible)
		{
			return;
		}
		val2.Visible = visible;
		IRMCHumanoidAppearance humanoid = ent.Comp;
		if (_rmcHumanoid.TryGetLocalHiddenAppearance(Entity<HumanoidAppearanceComponent>.op_Implicit(ent), out IRMCHumanoidAppearance appearance))
		{
			humanoid = appearance;
		}
		foreach (List<Marking> value in ent.Comp.MarkingSet.Markings.Values)
		{
			foreach (Marking item in value)
			{
				if (_markingManager.TryGetMarking(item, out MarkingPrototype markingResult) && markingResult.BodyPart == layer)
				{
					ApplyMarking(markingResult, item.MarkingColors, item.Visible, Entity<HumanoidAppearanceComponent>.op_Implicit(ent), humanoid, val);
				}
			}
		}
	}

	private void UpdateHiddenSprites<T>(T ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		AllEntityQueryEnumerator<HiddenAppearanceComponent, HumanoidAppearanceComponent, SpriteComponent> val = ((EntitySystem)this).AllEntityQuery<HiddenAppearanceComponent, HumanoidAppearanceComponent, SpriteComponent>();
		EntityUid val2 = default(EntityUid);
		HiddenAppearanceComponent hiddenAppearanceComponent = default(HiddenAppearanceComponent);
		HumanoidAppearanceComponent humanoid = default(HumanoidAppearanceComponent);
		SpriteComponent sprite = default(SpriteComponent);
		while (val.MoveNext(ref val2, ref hiddenAppearanceComponent, ref humanoid, ref sprite))
		{
			UpdateSprite(val2, humanoid, sprite);
			UpdatePlayerMedals(val2);
		}
	}

	private void OnHiddenRemove(Entity<HiddenAppearanceComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		HumanoidAppearanceComponent humanoid = default(HumanoidAppearanceComponent);
		SpriteComponent sprite = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<HumanoidAppearanceComponent>(Entity<HiddenAppearanceComponent>.op_Implicit(ent), ref humanoid) && ((EntitySystem)this).TryComp<SpriteComponent>(Entity<HiddenAppearanceComponent>.op_Implicit(ent), ref sprite))
		{
			UpdateSprite(Entity<HiddenAppearanceComponent>.op_Implicit(ent), humanoid, sprite);
			UpdatePlayerMedals(Entity<HiddenAppearanceComponent>.op_Implicit(ent));
		}
	}

	private void UpdatePlayerMedals(EntityUid player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		InventorySystem.InventorySlotEnumerator slotEnumerator = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(player));
		ContainerSlot container;
		while (slotEnumerator.MoveNext(out container))
		{
			EntityUid? containedEntity = container.ContainedEntity;
			if (containedEntity.HasValue)
			{
				EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
				_item.VisualsChanged(valueOrDefault);
			}
		}
	}
}
