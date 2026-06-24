// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.SharedHumanoidAppearanceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

#nullable enable
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
  public static readonly ProtoId<SpeciesPrototype> DefaultSpecies = (ProtoId<SpeciesPrototype>) "Human";
  public const string DefaultVoice = "barni";
  public static readonly Dictionary<Sex, string> DefaultSexVoice = new Dictionary<Sex, string>()
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
    base.Initialize();
    this.SubscribeLocalEvent<HumanoidAppearanceComponent, ComponentInit>(new ComponentEventHandler<HumanoidAppearanceComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<HumanoidAppearanceComponent, ExaminedEvent>(new ComponentEventHandler<HumanoidAppearanceComponent, ExaminedEvent>(this.OnExamined));
  }

  public DataNode ToDataNode(HumanoidCharacterProfile profile)
  {
    return this._serManager.WriteValue<HumanoidProfileExport>(new HumanoidProfileExport()
    {
      ForkId = this._cfgManager.GetCVar<string>(CVars.BuildForkId),
      Profile = profile
    }, true, notNullableOverride: true);
  }

  public HumanoidCharacterProfile FromStream(Stream stream, ICommonSession session)
  {
    using (StreamReader streamReader = new StreamReader(stream, EncodingHelpers.UTF8))
    {
      YamlStream yamlStream = new YamlStream();
      yamlStream.Load((TextReader) streamReader);
      HumanoidCharacterProfile profile = this._serManager.Read<HumanoidProfileExport>(yamlStream.Documents[0].RootNode.ToDataNode(), notNullableOverride: true).Profile;
      IDependencyCollection instance = IoCManager.Instance;
      profile.EnsureValid(session, instance);
      return profile;
    }
  }

  private void OnInit(EntityUid uid, HumanoidAppearanceComponent humanoid, ComponentInit args)
  {
    if (string.IsNullOrEmpty((string) humanoid.Species) || this._netManager.IsClient && !this.IsClientSide(uid))
      return;
    ProtoId<HumanoidProfilePrototype>? initial = humanoid.Initial;
    HumanoidProfilePrototype prototype;
    if (string.IsNullOrEmpty(initial.HasValue ? (string) initial.GetValueOrDefault() : (string) null) || !this._proto.TryIndex<HumanoidProfilePrototype>(humanoid.Initial, out prototype))
    {
      this.LoadProfile(uid, HumanoidCharacterProfile.DefaultWithSpecies((string) humanoid.Species), humanoid);
    }
    else
    {
      foreach ((HumanoidVisualLayers key, CustomBaseLayerInfo customBaseLayerInfo) in prototype.CustomBaseLayers)
        humanoid.CustomBaseLayers.Add(key, customBaseLayerInfo);
      this.LoadProfile(uid, prototype.Profile, humanoid);
    }
  }

  private void OnExamined(EntityUid uid, HumanoidAppearanceComponent component, ExaminedEvent args)
  {
    EntityUid entityUid = Identity.Entity(uid, (IEntityManager) this.EntityManager);
    string lower = this.GetSpeciesRepresentation((string) component.Species).ToLower();
    string str = this.GetAgeRepresentation((string) component.Species, component.Age);
    RMCHumanoidRepresentationOverrideComponent comp;
    if (this.TryComp<RMCHumanoidRepresentationOverrideComponent>(uid, out comp))
    {
      LocId? nullable;
      if (comp.Species.HasValue)
      {
        ILocalizationManager loc = this.Loc;
        nullable = comp.Species;
        string valueOrDefault = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
        lower = loc.GetString(valueOrDefault).ToLower();
      }
      if (comp.Age.HasValue)
      {
        ILocalizationManager loc = this.Loc;
        nullable = comp.Age;
        string valueOrDefault = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
        str = loc.GetString(valueOrDefault).ToLower();
      }
    }
    args.PushText(this.Loc.GetString("humanoid-appearance-component-examine", ("user", (object) entityUid), ("age", (object) str), ("species", (object) lower)));
  }

  public void SetLayerVisibility(
    Entity<HumanoidAppearanceComponent?> ent,
    HumanoidVisualLayers layer,
    bool visible,
    SlotFlags? source = null)
  {
    if (!this.Resolve<HumanoidAppearanceComponent>(ent.Owner, ref ent.Comp, false))
      return;
    bool dirty = false;
    this.SetLayerVisibility(ent, layer, visible, source, ref dirty);
    if (!dirty)
      return;
    this.Dirty<HumanoidAppearanceComponent>(ent);
  }

  public void CloneAppearance(
    EntityUid source,
    EntityUid target,
    HumanoidAppearanceComponent? sourceHumanoid = null,
    HumanoidAppearanceComponent? targetHumanoid = null)
  {
    if (!this.Resolve<HumanoidAppearanceComponent>(source, ref sourceHumanoid, false) || !this.Resolve<HumanoidAppearanceComponent>(target, ref targetHumanoid, false))
      return;
    targetHumanoid.Species = sourceHumanoid.Species;
    targetHumanoid.SkinColor = sourceHumanoid.SkinColor;
    targetHumanoid.EyeColor = sourceHumanoid.EyeColor;
    targetHumanoid.Age = sourceHumanoid.Age;
    this.SetSex(target, sourceHumanoid.Sex, false, targetHumanoid);
    targetHumanoid.CustomBaseLayers = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>((IDictionary<HumanoidVisualLayers, CustomBaseLayerInfo>) sourceHumanoid.CustomBaseLayers);
    targetHumanoid.MarkingSet = new MarkingSet(sourceHumanoid.MarkingSet);
    this.SetTTSVoice(target, (string) sourceHumanoid.Voice, targetHumanoid);
    targetHumanoid.Gender = sourceHumanoid.Gender;
    GrammarComponent comp;
    if (this.TryComp<GrammarComponent>(target, out comp))
      this._grammarSystem.SetGender((Entity<GrammarComponent>) (target, comp), new Gender?(sourceHumanoid.Gender));
    this._identity.QueueIdentityUpdate(target);
    this.Dirty(target, (IComponent) targetHumanoid);
  }

  public void SetLayersVisibility(
    Entity<HumanoidAppearanceComponent?> ent,
    IEnumerable<HumanoidVisualLayers> layers,
    bool visible)
  {
    if (!this.Resolve<HumanoidAppearanceComponent>(ent.Owner, ref ent.Comp, false))
      return;
    bool dirty = false;
    foreach (HumanoidVisualLayers layer in layers)
      this.SetLayerVisibility(ent, layer, visible, new SlotFlags?(), ref dirty);
    if (!dirty)
      return;
    this.Dirty<HumanoidAppearanceComponent>(ent);
  }

  public virtual void SetLayerVisibility(
    Entity<HumanoidAppearanceComponent> ent,
    HumanoidVisualLayers layer,
    bool visible,
    SlotFlags? source,
    ref bool dirty)
  {
    if (visible)
    {
      if (source.HasValue)
      {
        SlotFlags valueOrDefault = source.GetValueOrDefault();
        SlotFlags slotFlags;
        if (!ent.Comp.HiddenLayers.TryGetValue(layer, out slotFlags))
          return;
        ent.Comp.HiddenLayers[layer] = ~valueOrDefault & slotFlags;
        if (ent.Comp.HiddenLayers[layer] == SlotFlags.NONE)
          ent.Comp.HiddenLayers.Remove(layer);
        dirty |= (slotFlags & valueOrDefault) != 0;
      }
      else
        dirty |= ent.Comp.PermanentlyHidden.Remove(layer);
    }
    else if (source.HasValue)
    {
      SlotFlags valueOrDefault1 = source.GetValueOrDefault();
      SlotFlags valueOrDefault2 = ent.Comp.HiddenLayers.GetValueOrDefault<HumanoidVisualLayers, SlotFlags>(layer);
      ent.Comp.HiddenLayers[layer] = valueOrDefault1 | valueOrDefault2;
      dirty |= (valueOrDefault2 & valueOrDefault1) != valueOrDefault1;
    }
    else
      dirty |= ent.Comp.PermanentlyHidden.Add(layer);
  }

  public void SetSpecies(
    EntityUid uid,
    string species,
    bool sync = true,
    HumanoidAppearanceComponent? humanoid = null)
  {
    SpeciesPrototype prototype;
    if (!this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid) || !this._proto.TryIndex<SpeciesPrototype>(species, out prototype))
      return;
    humanoid.Species = (ProtoId<SpeciesPrototype>) species;
    humanoid.MarkingSet.EnsureSpecies(species, new Color?(humanoid.SkinColor), this._markingManager);
    List<Marking> list = humanoid.MarkingSet.GetForwardEnumerator().ToList<Marking>();
    humanoid.MarkingSet = new MarkingSet(list, (string) prototype.MarkingPoints, this._markingManager, this._proto);
    if (!sync)
      return;
    this.Dirty(uid, (IComponent) humanoid);
  }

  public virtual void SetSkinColor(
    EntityUid uid,
    Color skinColor,
    bool sync = true,
    bool verify = true,
    HumanoidAppearanceComponent? humanoid = null)
  {
    SpeciesPrototype prototype;
    if (!this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid) || !this._proto.TryIndex<SpeciesPrototype>(humanoid.Species, out prototype))
      return;
    if (verify && !SkinColor.VerifySkinColor(prototype.SkinColoration, skinColor))
      skinColor = SkinColor.ValidSkinTone(prototype.SkinColoration, skinColor);
    humanoid.SkinColor = skinColor;
    if (!sync)
      return;
    this.Dirty(uid, (IComponent) humanoid);
  }

  public void SetBaseLayerId(
    EntityUid uid,
    HumanoidVisualLayers layer,
    string? id,
    bool sync = true,
    HumanoidAppearanceComponent? humanoid = null)
  {
    if (!this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid))
      return;
    CustomBaseLayerInfo customBaseLayerInfo;
    if (humanoid.CustomBaseLayers.TryGetValue(layer, out customBaseLayerInfo))
      humanoid.CustomBaseLayers[layer] = customBaseLayerInfo with
      {
        Id = (ProtoId<HumanoidSpeciesSpriteLayer>?) id
      };
    else
      humanoid.CustomBaseLayers[layer] = new CustomBaseLayerInfo(id);
    if (!sync)
      return;
    this.Dirty(uid, (IComponent) humanoid);
  }

  public void SetBaseLayerColor(
    EntityUid uid,
    HumanoidVisualLayers layer,
    Color? color,
    bool sync = true,
    HumanoidAppearanceComponent? humanoid = null)
  {
    if (!this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid))
      return;
    CustomBaseLayerInfo customBaseLayerInfo;
    if (humanoid.CustomBaseLayers.TryGetValue(layer, out customBaseLayerInfo))
      humanoid.CustomBaseLayers[layer] = customBaseLayerInfo with
      {
        Color = color
      };
    else
      humanoid.CustomBaseLayers[layer] = new CustomBaseLayerInfo((string) null, color);
    if (!sync)
      return;
    this.Dirty(uid, (IComponent) humanoid);
  }

  public void SetSex(EntityUid uid, Sex sex, bool sync = true, HumanoidAppearanceComponent? humanoid = null)
  {
    if (!this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid) || humanoid.Sex == sex)
      return;
    Sex sex1 = humanoid.Sex;
    humanoid.Sex = sex;
    humanoid.MarkingSet.EnsureSexes(sex, this._markingManager);
    this.RaiseLocalEvent<SexChangedEvent>(uid, new SexChangedEvent(sex1, sex));
    if (!sync)
      return;
    this.Dirty(uid, (IComponent) humanoid);
  }

  public virtual void LoadProfile(
    EntityUid uid,
    HumanoidCharacterProfile? profile,
    HumanoidAppearanceComponent? humanoid = null)
  {
    if (profile == null || !this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid))
      return;
    this.SetSpecies(uid, (string) profile.Species, false, humanoid);
    this.SetSex(uid, profile.Sex, false, humanoid);
    humanoid.EyeColor = profile.Appearance.EyeColor;
    this.SetSkinColor(uid, profile.Appearance.SkinColor, false);
    humanoid.MarkingSet.Clear();
    Dictionary<Marking, MarkingPrototype> dictionary = new Dictionary<Marking, MarkingPrototype>();
    foreach (Marking marking in profile.Appearance.Markings)
    {
      MarkingPrototype markingResult;
      if (this._markingManager.TryGetMarking(marking, out markingResult))
      {
        if (!markingResult.ForcedColoring)
          this.AddMarking(uid, marking.MarkingId, marking.MarkingColors, false);
        else
          dictionary.Add(marking, markingResult);
      }
    }
    float alpha1;
    Color skinColor;
    Color color1;
    if (!this._markingManager.MustMatchSkin((string) profile.Species, HumanoidVisualLayers.Hair, out alpha1, this._proto))
    {
      color1 = profile.Appearance.HairColor;
    }
    else
    {
      skinColor = profile.Appearance.SkinColor;
      color1 = ((Color) ref skinColor).WithAlpha(alpha1);
    }
    Color color2 = color1;
    float alpha2;
    Color color3;
    if (!this._markingManager.MustMatchSkin((string) profile.Species, HumanoidVisualLayers.FacialHair, out alpha2, this._proto))
    {
      color3 = profile.Appearance.FacialHairColor;
    }
    else
    {
      skinColor = profile.Appearance.SkinColor;
      color3 = ((Color) ref skinColor).WithAlpha(alpha2);
    }
    Color color4 = color3;
    MarkingPrototype prototype1;
    if (this._markingManager.Markings.TryGetValue(profile.Appearance.HairStyleId, out prototype1) && this._markingManager.CanBeApplied((string) profile.Species, profile.Sex, prototype1, this._proto))
      this.AddMarking(uid, profile.Appearance.HairStyleId, new Color?(color2), false);
    MarkingPrototype prototype2;
    if (this._markingManager.Markings.TryGetValue(profile.Appearance.FacialHairStyleId, out prototype2) && this._markingManager.CanBeApplied((string) profile.Species, profile.Sex, prototype2, this._proto))
      this.AddMarking(uid, profile.Appearance.FacialHairStyleId, new Color?(color4), false);
    humanoid.MarkingSet.EnsureSpecies((string) profile.Species, new Color?(profile.Appearance.SkinColor), this._markingManager, this._proto);
    foreach ((Marking key, MarkingPrototype prototype3) in dictionary)
    {
      List<Color> markingLayerColors = MarkingColoring.GetMarkingLayerColors(prototype3, new Color?(profile.Appearance.SkinColor), new Color?(profile.Appearance.EyeColor), humanoid.MarkingSet);
      this.AddMarking(uid, key.MarkingId, (IReadOnlyList<Color>) markingLayerColors, false);
    }
    this.EnsureDefaultMarkings(uid, humanoid);
    this.SetTTSVoice(uid, profile.Voice, humanoid);
    humanoid.Gender = profile.Gender;
    GrammarComponent comp;
    if (this.TryComp<GrammarComponent>(uid, out comp))
      this._grammarSystem.SetGender((Entity<GrammarComponent>) (uid, comp), new Gender?(profile.Gender));
    humanoid.Age = profile.Age;
    this.Dirty(uid, (IComponent) humanoid);
  }

  public void AddMarking(
    EntityUid uid,
    string marking,
    Color? color = null,
    bool sync = true,
    bool forced = false,
    HumanoidAppearanceComponent? humanoid = null)
  {
    MarkingPrototype markingPrototype;
    if (!this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid) || !this._markingManager.Markings.TryGetValue(marking, out markingPrototype))
      return;
    Marking marking1 = markingPrototype.AsMarking();
    marking1.Forced = forced;
    if (color.HasValue)
    {
      for (int colorIndex = 0; colorIndex < markingPrototype.Sprites.Count; ++colorIndex)
        marking1.SetColor(colorIndex, color.Value);
    }
    humanoid.MarkingSet.AddBack(markingPrototype.MarkingCategory, marking1);
    if (!sync)
      return;
    this.Dirty(uid, (IComponent) humanoid);
  }

  private void EnsureDefaultMarkings(EntityUid uid, HumanoidAppearanceComponent? humanoid)
  {
    if (!this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid))
      return;
    humanoid.MarkingSet.EnsureDefault(new Color?(humanoid.SkinColor), new Color?(humanoid.EyeColor), this._markingManager);
  }

  public void AddMarking(
    EntityUid uid,
    string marking,
    IReadOnlyList<Color> colors,
    bool sync = true,
    bool forced = false,
    HumanoidAppearanceComponent? humanoid = null)
  {
    MarkingPrototype markingPrototype;
    if (!this.Resolve<HumanoidAppearanceComponent>(uid, ref humanoid) || !this._markingManager.Markings.TryGetValue(marking, out markingPrototype))
      return;
    humanoid.MarkingSet.AddBack(markingPrototype.MarkingCategory, new Marking(marking, colors)
    {
      Forced = forced
    });
    if (!sync)
      return;
    this.Dirty(uid, (IComponent) humanoid);
  }

  public void SetTTSVoice(EntityUid uid, string voiceId, HumanoidAppearanceComponent humanoid)
  {
    TTSComponent comp;
    if (!this.TryComp<TTSComponent>(uid, out comp))
      return;
    humanoid.Voice = (ProtoId<TTSVoicePrototype>) voiceId;
    comp.VoicePrototypeId = voiceId;
  }

  public string GetSpeciesRepresentation(string speciesId)
  {
    SpeciesPrototype prototype;
    if (this._proto.TryIndex<SpeciesPrototype>(speciesId, out prototype))
      return this.Loc.GetString(prototype.Name);
    this.Log.Error("Tried to get representation of unknown species: {speciesId}");
    return this.Loc.GetString("humanoid-appearance-component-unknown-species");
  }

  public string GetAgeRepresentation(string species, int age)
  {
    SpeciesPrototype prototype;
    if (!this._proto.TryIndex<SpeciesPrototype>(species, out prototype))
    {
      this.Log.Error("Tried to get age representation of species that couldn't be indexed: " + species);
      return this.Loc.GetString("identity-age-young");
    }
    if (age < prototype.YoungAge)
      return this.Loc.GetString("identity-age-young");
    return age < prototype.OldAge ? this.Loc.GetString("identity-age-middle-aged") : this.Loc.GetString("identity-age-old");
  }
}
