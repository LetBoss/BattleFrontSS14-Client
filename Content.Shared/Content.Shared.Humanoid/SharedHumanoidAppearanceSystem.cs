using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Content.Shared._RMC14.Humanoid;
using Content.Shared.Corvax.TTS;
using Content.Shared.Examine;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Utility;
using YamlDotNet.RepresentationModel;

namespace Content.Shared.Humanoid;

public abstract class SharedHumanoidAppearanceSystem : EntitySystem
{
	[Dependency]
	private IConfigurationManager _cfgManager;

	[Dependency]
	private INetManager _netManager;

	[Dependency]
	private IPrototypeManager _proto;

	[Dependency]
	private ISerializationManager _serManager;

	[Dependency]
	private MarkingManager _markingManager;

	[Dependency]
	private GrammarSystem _grammarSystem;

	[Dependency]
	private SharedIdentitySystem _identity;

	public static readonly ProtoId<SpeciesPrototype> DefaultSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Human");

	public const string DefaultVoice = "barni";

	public static readonly Dictionary<Sex, string> DefaultSexVoice = new Dictionary<Sex, string>
	{
		{
			Sex.Male,
			"barni"
		},
		{
			Sex.Female,
			"alyx"
		},
		{
			Sex.Unsexed,
			"gman"
		}
	};

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HumanoidAppearanceComponent, ComponentInit>((ComponentEventHandler<HumanoidAppearanceComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HumanoidAppearanceComponent, ExaminedEvent>((ComponentEventHandler<HumanoidAppearanceComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	public DataNode ToDataNode(HumanoidCharacterProfile profile)
	{
		HumanoidProfileExport export = new HumanoidProfileExport
		{
			ForkId = _cfgManager.GetCVar<string>(CVars.BuildForkId),
			Profile = profile
		};
		return _serManager.WriteValue<HumanoidProfileExport>(export, true, (ISerializationContext)null, true);
	}

	public HumanoidCharacterProfile FromStream(Stream stream, ICommonSession session)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		using StreamReader reader = new StreamReader(stream, EncodingHelpers.UTF8);
		YamlStream val = new YamlStream();
		val.Load((TextReader)reader);
		YamlNode root = val.Documents[0].RootNode;
		HumanoidCharacterProfile profile = _serManager.Read<HumanoidProfileExport>(YamlNodeHelpers.ToDataNode(root), (ISerializationContext)null, false, (InstantiationDelegate<HumanoidProfileExport>)null, true).Profile;
		IDependencyCollection collection = IoCManager.Instance;
		profile.EnsureValid(session, collection);
		return profile;
	}

	private void OnInit(EntityUid uid, HumanoidAppearanceComponent humanoid, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(ProtoId<SpeciesPrototype>.op_Implicit(humanoid.Species)) || (_netManager.IsClient && !((EntitySystem)this).IsClientSide(uid, (MetaDataComponent)null)))
		{
			return;
		}
		ProtoId<HumanoidProfilePrototype>? initial = humanoid.Initial;
		HumanoidProfilePrototype startingSet = default(HumanoidProfilePrototype);
		if (string.IsNullOrEmpty(initial.HasValue ? ProtoId<HumanoidProfilePrototype>.op_Implicit(initial.GetValueOrDefault()) : null) || !_proto.TryIndex<HumanoidProfilePrototype>(humanoid.Initial, ref startingSet))
		{
			LoadProfile(uid, HumanoidCharacterProfile.DefaultWithSpecies(ProtoId<SpeciesPrototype>.op_Implicit(humanoid.Species)), humanoid);
			return;
		}
		foreach (var (layer, info) in startingSet.CustomBaseLayers)
		{
			humanoid.CustomBaseLayers.Add(layer, info);
		}
		LoadProfile(uid, startingSet.Profile, humanoid);
	}

	private void OnExamined(EntityUid uid, HumanoidAppearanceComponent component, ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		EntityUid identity = Identity.Entity(uid, (IEntityManager)(object)base.EntityManager);
		string species = GetSpeciesRepresentation(ProtoId<SpeciesPrototype>.op_Implicit(component.Species)).ToLower();
		string age = GetAgeRepresentation(ProtoId<SpeciesPrototype>.op_Implicit(component.Species), component.Age);
		RMCHumanoidRepresentationOverrideComponent humanoidRepComp = default(RMCHumanoidRepresentationOverrideComponent);
		if (((EntitySystem)this).TryComp<RMCHumanoidRepresentationOverrideComponent>(uid, ref humanoidRepComp))
		{
			if (humanoidRepComp.Species.HasValue)
			{
				ILocalizationManager loc = base.Loc;
				LocId? species2 = humanoidRepComp.Species;
				species = loc.GetString(species2.HasValue ? LocId.op_Implicit(species2.GetValueOrDefault()) : null).ToLower();
			}
			if (humanoidRepComp.Age.HasValue)
			{
				ILocalizationManager loc2 = base.Loc;
				LocId? species2 = humanoidRepComp.Age;
				age = loc2.GetString(species2.HasValue ? LocId.op_Implicit(species2.GetValueOrDefault()) : null).ToLower();
			}
		}
		args.PushText(base.Loc.GetString("humanoid-appearance-component-examine", new(string, object)[3]
		{
			("user", identity),
			("age", age),
			("species", species)
		}));
	}

	public void SetLayerVisibility(Entity<HumanoidAppearanceComponent?> ent, HumanoidVisualLayers layer, bool visible, SlotFlags? source = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(ent.Owner, ref ent.Comp, false))
		{
			bool dirty = false;
			SetLayerVisibility(ent, layer, visible, source, ref dirty);
			if (dirty)
			{
				((EntitySystem)this).Dirty<HumanoidAppearanceComponent>(ent, (MetaDataComponent)null);
			}
		}
	}

	public void CloneAppearance(EntityUid source, EntityUid target, HumanoidAppearanceComponent? sourceHumanoid = null, HumanoidAppearanceComponent? targetHumanoid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(source, ref sourceHumanoid, false) && ((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(target, ref targetHumanoid, false))
		{
			targetHumanoid.Species = sourceHumanoid.Species;
			targetHumanoid.SkinColor = sourceHumanoid.SkinColor;
			targetHumanoid.EyeColor = sourceHumanoid.EyeColor;
			targetHumanoid.Age = sourceHumanoid.Age;
			SetSex(target, sourceHumanoid.Sex, sync: false, targetHumanoid);
			targetHumanoid.CustomBaseLayers = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>(sourceHumanoid.CustomBaseLayers);
			targetHumanoid.MarkingSet = new MarkingSet(sourceHumanoid.MarkingSet);
			SetTTSVoice(target, ProtoId<TTSVoicePrototype>.op_Implicit(sourceHumanoid.Voice), targetHumanoid);
			targetHumanoid.Gender = sourceHumanoid.Gender;
			GrammarComponent grammar = default(GrammarComponent);
			if (((EntitySystem)this).TryComp<GrammarComponent>(target, ref grammar))
			{
				_grammarSystem.SetGender(Entity<GrammarComponent>.op_Implicit((target, grammar)), (Gender?)sourceHumanoid.Gender);
			}
			_identity.QueueIdentityUpdate(target);
			((EntitySystem)this).Dirty(target, (IComponent)(object)targetHumanoid, (MetaDataComponent)null);
		}
	}

	public void SetLayersVisibility(Entity<HumanoidAppearanceComponent?> ent, IEnumerable<HumanoidVisualLayers> layers, bool visible)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(ent.Owner, ref ent.Comp, false))
		{
			return;
		}
		bool dirty = false;
		foreach (HumanoidVisualLayers layer in layers)
		{
			SetLayerVisibility(ent, layer, visible, null, ref dirty);
		}
		if (dirty)
		{
			((EntitySystem)this).Dirty<HumanoidAppearanceComponent>(ent, (MetaDataComponent)null);
		}
	}

	public virtual void SetLayerVisibility(Entity<HumanoidAppearanceComponent> ent, HumanoidVisualLayers layer, bool visible, SlotFlags? source, ref bool dirty)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (visible)
		{
			if (source.HasValue)
			{
				SlotFlags slot = source.GetValueOrDefault();
				if (ent.Comp.HiddenLayers.TryGetValue(layer, out var oldSlots))
				{
					ent.Comp.HiddenLayers[layer] = ~slot & oldSlots;
					if (ent.Comp.HiddenLayers[layer] == SlotFlags.NONE)
					{
						ent.Comp.HiddenLayers.Remove(layer);
					}
					dirty |= (oldSlots & slot) != 0;
				}
			}
			else
			{
				dirty |= ent.Comp.PermanentlyHidden.Remove(layer);
			}
		}
		else if (source.HasValue)
		{
			SlotFlags slot2 = source.GetValueOrDefault();
			SlotFlags oldSlots2 = ent.Comp.HiddenLayers.GetValueOrDefault(layer);
			ent.Comp.HiddenLayers[layer] = slot2 | oldSlots2;
			dirty |= (oldSlots2 & slot2) != slot2;
		}
		else
		{
			dirty |= ent.Comp.PermanentlyHidden.Add(layer);
		}
	}

	public void SetSpecies(EntityUid uid, string species, bool sync = true, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		SpeciesPrototype prototype = default(SpeciesPrototype);
		if (((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) && _proto.TryIndex<SpeciesPrototype>(species, ref prototype))
		{
			humanoid.Species = ProtoId<SpeciesPrototype>.op_Implicit(species);
			humanoid.MarkingSet.EnsureSpecies(species, humanoid.SkinColor, _markingManager);
			List<Marking> oldMarkings = humanoid.MarkingSet.GetForwardEnumerator().ToList();
			humanoid.MarkingSet = new MarkingSet(oldMarkings, ProtoId<MarkingPointsPrototype>.op_Implicit(prototype.MarkingPoints), _markingManager, _proto);
			if (sync)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)humanoid, (MetaDataComponent)null);
			}
		}
	}

	public virtual void SetSkinColor(EntityUid uid, Color skinColor, bool sync = true, bool verify = true, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		SpeciesPrototype species = default(SpeciesPrototype);
		if (((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) && _proto.TryIndex<SpeciesPrototype>(humanoid.Species, ref species))
		{
			if (verify && !SkinColor.VerifySkinColor(species.SkinColoration, skinColor))
			{
				skinColor = SkinColor.ValidSkinTone(species.SkinColoration, skinColor);
			}
			humanoid.SkinColor = skinColor;
			if (sync)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)humanoid, (MetaDataComponent)null);
			}
		}
	}

	public void SetBaseLayerId(EntityUid uid, HumanoidVisualLayers layer, string? id, bool sync = true, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
		{
			if (humanoid.CustomBaseLayers.TryGetValue(layer, out var info))
			{
				Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> customBaseLayers = humanoid.CustomBaseLayers;
				CustomBaseLayerInfo value = info;
				value.Id = ProtoId<HumanoidSpeciesSpriteLayer>.op_Implicit(id);
				customBaseLayers[layer] = value;
			}
			else
			{
				humanoid.CustomBaseLayers[layer] = new CustomBaseLayerInfo(id);
			}
			if (sync)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)humanoid, (MetaDataComponent)null);
			}
		}
	}

	public void SetBaseLayerColor(EntityUid uid, HumanoidVisualLayers layer, Color? color, bool sync = true, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
		{
			if (humanoid.CustomBaseLayers.TryGetValue(layer, out var info))
			{
				Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> customBaseLayers = humanoid.CustomBaseLayers;
				CustomBaseLayerInfo value = info;
				value.Color = color;
				customBaseLayers[layer] = value;
			}
			else
			{
				humanoid.CustomBaseLayers[layer] = new CustomBaseLayerInfo(null, color);
			}
			if (sync)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)humanoid, (MetaDataComponent)null);
			}
		}
	}

	public void SetSex(EntityUid uid, Sex sex, bool sync = true, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) && humanoid.Sex != sex)
		{
			Sex oldSex = humanoid.Sex;
			humanoid.Sex = sex;
			humanoid.MarkingSet.EnsureSexes(sex, _markingManager);
			((EntitySystem)this).RaiseLocalEvent<SexChangedEvent>(uid, new SexChangedEvent(oldSex, sex), false);
			if (sync)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)humanoid, (MetaDataComponent)null);
			}
		}
	}

	public virtual void LoadProfile(EntityUid uid, HumanoidCharacterProfile? profile, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		if (profile == null || !((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
		{
			return;
		}
		SetSpecies(uid, ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), sync: false, humanoid);
		SetSex(uid, profile.Sex, sync: false, humanoid);
		humanoid.EyeColor = profile.Appearance.EyeColor;
		SetSkinColor(uid, profile.Appearance.SkinColor, sync: false);
		humanoid.MarkingSet.Clear();
		Dictionary<Marking, MarkingPrototype> markingFColored = new Dictionary<Marking, MarkingPrototype>();
		foreach (Marking marking in profile.Appearance.Markings)
		{
			if (_markingManager.TryGetMarking(marking, out MarkingPrototype prototype))
			{
				if (!prototype.ForcedColoring)
				{
					AddMarking(uid, marking.MarkingId, marking.MarkingColors, sync: false);
				}
				else
				{
					markingFColored.Add(marking, prototype);
				}
			}
		}
		Color val;
		Color skinColor;
		if (!_markingManager.MustMatchSkin(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), HumanoidVisualLayers.Hair, out var hairAlpha, _proto))
		{
			val = profile.Appearance.HairColor;
		}
		else
		{
			skinColor = profile.Appearance.SkinColor;
			val = ((Color)(ref skinColor)).WithAlpha(hairAlpha);
		}
		Color hairColor = val;
		Color val2;
		if (!_markingManager.MustMatchSkin(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), HumanoidVisualLayers.FacialHair, out var facialHairAlpha, _proto))
		{
			val2 = profile.Appearance.FacialHairColor;
		}
		else
		{
			skinColor = profile.Appearance.SkinColor;
			val2 = ((Color)(ref skinColor)).WithAlpha(facialHairAlpha);
		}
		Color facialHairColor = val2;
		if (_markingManager.Markings.TryGetValue(profile.Appearance.HairStyleId, out MarkingPrototype hairPrototype) && _markingManager.CanBeApplied(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), profile.Sex, hairPrototype, _proto))
		{
			AddMarking(uid, profile.Appearance.HairStyleId, hairColor, sync: false);
		}
		if (_markingManager.Markings.TryGetValue(profile.Appearance.FacialHairStyleId, out MarkingPrototype facialHairPrototype) && _markingManager.CanBeApplied(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), profile.Sex, facialHairPrototype, _proto))
		{
			AddMarking(uid, profile.Appearance.FacialHairStyleId, facialHairColor, sync: false);
		}
		humanoid.MarkingSet.EnsureSpecies(ProtoId<SpeciesPrototype>.op_Implicit(profile.Species), profile.Appearance.SkinColor, _markingManager, _proto);
		foreach (KeyValuePair<Marking, MarkingPrototype> item in markingFColored)
		{
			item.Deconstruct(out var key, out var value);
			Marking marking2 = key;
			List<Color> markingColors = MarkingColoring.GetMarkingLayerColors(value, profile.Appearance.SkinColor, profile.Appearance.EyeColor, humanoid.MarkingSet);
			AddMarking(uid, marking2.MarkingId, markingColors, sync: false);
		}
		EnsureDefaultMarkings(uid, humanoid);
		SetTTSVoice(uid, profile.Voice, humanoid);
		humanoid.Gender = profile.Gender;
		GrammarComponent grammar = default(GrammarComponent);
		if (((EntitySystem)this).TryComp<GrammarComponent>(uid, ref grammar))
		{
			_grammarSystem.SetGender(Entity<GrammarComponent>.op_Implicit((uid, grammar)), (Gender?)profile.Gender);
		}
		humanoid.Age = profile.Age;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)humanoid, (MetaDataComponent)null);
	}

	public void AddMarking(EntityUid uid, string marking, Color? color = null, bool sync = true, bool forced = false, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) || !_markingManager.Markings.TryGetValue(marking, out MarkingPrototype prototype))
		{
			return;
		}
		Marking markingObject = prototype.AsMarking();
		markingObject.Forced = forced;
		if (color.HasValue)
		{
			for (int i = 0; i < prototype.Sprites.Count; i++)
			{
				markingObject.SetColor(i, color.Value);
			}
		}
		humanoid.MarkingSet.AddBack(prototype.MarkingCategory, markingObject);
		if (sync)
		{
			((EntitySystem)this).Dirty(uid, (IComponent)(object)humanoid, (MetaDataComponent)null);
		}
	}

	private void EnsureDefaultMarkings(EntityUid uid, HumanoidAppearanceComponent? humanoid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true))
		{
			humanoid.MarkingSet.EnsureDefault(humanoid.SkinColor, humanoid.EyeColor, _markingManager);
		}
	}

	public void AddMarking(EntityUid uid, string marking, IReadOnlyList<Color> colors, bool sync = true, bool forced = false, HumanoidAppearanceComponent? humanoid = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HumanoidAppearanceComponent>(uid, ref humanoid, true) && _markingManager.Markings.TryGetValue(marking, out MarkingPrototype prototype))
		{
			Marking markingObject = new Marking(marking, colors);
			markingObject.Forced = forced;
			humanoid.MarkingSet.AddBack(prototype.MarkingCategory, markingObject);
			if (sync)
			{
				((EntitySystem)this).Dirty(uid, (IComponent)(object)humanoid, (MetaDataComponent)null);
			}
		}
	}

	public void SetTTSVoice(EntityUid uid, string voiceId, HumanoidAppearanceComponent humanoid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		TTSComponent comp = default(TTSComponent);
		if (((EntitySystem)this).TryComp<TTSComponent>(uid, ref comp))
		{
			humanoid.Voice = ProtoId<TTSVoicePrototype>.op_Implicit(voiceId);
			comp.VoicePrototypeId = voiceId;
		}
	}

	public string GetSpeciesRepresentation(string speciesId)
	{
		SpeciesPrototype species = default(SpeciesPrototype);
		if (_proto.TryIndex<SpeciesPrototype>(speciesId, ref species))
		{
			return base.Loc.GetString(species.Name);
		}
		((EntitySystem)this).Log.Error("Tried to get representation of unknown species: {speciesId}");
		return base.Loc.GetString("humanoid-appearance-component-unknown-species");
	}

	public string GetAgeRepresentation(string species, int age)
	{
		SpeciesPrototype speciesPrototype = default(SpeciesPrototype);
		if (!_proto.TryIndex<SpeciesPrototype>(species, ref speciesPrototype))
		{
			((EntitySystem)this).Log.Error("Tried to get age representation of species that couldn't be indexed: " + species);
			return base.Loc.GetString("identity-age-young");
		}
		if (age < speciesPrototype.YoungAge)
		{
			return base.Loc.GetString("identity-age-young");
		}
		if (age < speciesPrototype.OldAge)
		{
			return base.Loc.GetString("identity-age-middle-aged");
		}
		return base.Loc.GetString("identity-age-old");
	}
}
