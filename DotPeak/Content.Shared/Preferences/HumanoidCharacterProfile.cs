// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.HumanoidCharacterProfile
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.NamedItems;
using Content.Shared._RMC14.Xenonids.Name;
using Content.Shared.CCVar;
using Content.Shared.Corvax.TTS;
using Content.Shared.Dataset;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.Traits;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable enable
namespace Content.Shared.Preferences;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class HumanoidCharacterProfile : 
  ICharacterProfile,
  ISerializationGenerated<HumanoidCharacterProfile>,
  ISerializationGenerated
{
  private static readonly Regex RestrictedNameRegex = new Regex("[^A-Za-zА-Яа-яЁё0-9 '\\-]");
  private static readonly Regex ICNameCaseRegex = new Regex("^(?<word>\\w)|\\b(?<word>\\w)(?=\\w*$)");
  private static readonly ProtoId<DatasetPrototype> BlockedNameWordsDataset = (ProtoId<DatasetPrototype>) "PubgBlockedNameWords";
  private static readonly ProtoId<SpeciesPrototype> FelinidSpecies = (ProtoId<SpeciesPrototype>) "Felinid";
  [DataField(null, false, 1, false, false, null)]
  private Dictionary<ProtoId<JobPrototype>, JobPriority> _jobPriorities = new Dictionary<ProtoId<JobPrototype>, JobPriority>()
  {
    {
      SharedGameTicker.FallbackOverflowJob,
      JobPriority.High
    }
  };
  [DataField(null, false, 1, false, false, null)]
  private HashSet<ProtoId<AntagPrototype>> _antagPreferences = new HashSet<ProtoId<AntagPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  private HashSet<ProtoId<TraitPrototype>> _traitPreferences = new HashSet<ProtoId<TraitPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  private Dictionary<string, RoleLoadout> _loadouts = new Dictionary<string, RoleLoadout>();

  private static bool ContainsBlockedNameWord(string name, IPrototypeManager prototypeManager)
  {
    DatasetPrototype prototype;
    if (!prototypeManager.TryIndex<DatasetPrototype>(HumanoidCharacterProfile.BlockedNameWordsDataset, out prototype))
      return false;
    string lowerInvariant = name.ToLowerInvariant();
    foreach (string str in (IEnumerable<string>) prototype.Values)
    {
      if (!string.IsNullOrEmpty(str) && lowerInvariant.Contains(str.ToLowerInvariant()))
        return true;
    }
    return false;
  }

  public IReadOnlyDictionary<string, RoleLoadout> Loadouts
  {
    get => (IReadOnlyDictionary<string, RoleLoadout>) this._loadouts;
  }

  [DataField(null, false, 1, false, false, null)]
  public string Name { get; set; } = "John Doe";

  [DataField(null, false, 1, false, false, null)]
  public string FlavorText { get; set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SpeciesPrototype> Species { get; set; } = SharedHumanoidAppearanceSystem.DefaultSpecies;

  [DataField(null, false, 1, false, false, null)]
  public string Voice { get; set; } = "barni";

  [DataField(null, false, 1, false, false, null)]
  public int Age { get; set; } = 18;

  [DataField(null, false, 1, false, false, null)]
  public Sex Sex { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public Gender Gender { get; private set; } = Gender.Male;

  public ICharacterAppearance CharacterAppearance => (ICharacterAppearance) this.Appearance;

  [DataField(null, false, 1, false, false, null)]
  public HumanoidCharacterAppearance Appearance { get; set; } = new HumanoidCharacterAppearance();

  [DataField(null, false, 1, false, false, null)]
  public SpawnPriorityPreference SpawnPriority { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ArmorPreference ArmorPreference { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<SquadTeamComponent>? SquadPreference { get; private set; }

  public IReadOnlyDictionary<ProtoId<JobPrototype>, JobPriority> JobPriorities
  {
    get => (IReadOnlyDictionary<ProtoId<JobPrototype>, JobPriority>) this._jobPriorities;
  }

  public IReadOnlySet<ProtoId<AntagPrototype>> AntagPreferences
  {
    get => (IReadOnlySet<ProtoId<AntagPrototype>>) this._antagPreferences;
  }

  public IReadOnlySet<ProtoId<TraitPrototype>> TraitPreferences
  {
    get => (IReadOnlySet<ProtoId<TraitPrototype>>) this._traitPreferences;
  }

  [DataField(null, false, 1, false, false, null)]
  public PreferenceUnavailableMode PreferenceUnavailable { get; private set; } = PreferenceUnavailableMode.SpawnAsOverflow;

  [DataField(null, false, 1, false, false, null)]
  public SharedRMCNamedItems NamedItems { get; private set; } = new SharedRMCNamedItems();

  [DataField(null, false, 1, false, false, null)]
  public bool PlaytimePerks { get; private set; } = true;

  [DataField(null, false, 1, false, false, null)]
  public string XenoPrefix { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public string XenoPostfix { get; private set; } = string.Empty;

  public HumanoidCharacterProfile(
    string name,
    string flavortext,
    string species,
    string voice,
    int age,
    Sex sex,
    Gender gender,
    HumanoidCharacterAppearance appearance,
    SpawnPriorityPreference spawnPriority,
    ArmorPreference armorPreference,
    EntProtoId<SquadTeamComponent>? squadPreference,
    Dictionary<ProtoId<JobPrototype>, JobPriority> jobPriorities,
    PreferenceUnavailableMode preferenceUnavailable,
    HashSet<ProtoId<AntagPrototype>> antagPreferences,
    HashSet<ProtoId<TraitPrototype>> traitPreferences,
    Dictionary<string, RoleLoadout> loadouts,
    SharedRMCNamedItems namedItems,
    bool playtimePerks,
    string xenoPrefix,
    string xenoPostfix)
  {
    this.Name = name;
    this.FlavorText = flavortext;
    this.Species = (ProtoId<SpeciesPrototype>) species;
    this.Voice = voice;
    this.Age = age;
    this.Sex = sex;
    this.Gender = gender;
    this.Appearance = appearance;
    this.SpawnPriority = spawnPriority;
    this.ArmorPreference = armorPreference;
    this.SquadPreference = squadPreference;
    this._jobPriorities = jobPriorities;
    this.PreferenceUnavailable = preferenceUnavailable;
    this._antagPreferences = antagPreferences;
    this._traitPreferences = traitPreferences;
    this._loadouts = loadouts;
    bool flag = false;
    foreach ((ProtoId<JobPrototype> key, JobPriority jobPriority) in this._jobPriorities)
    {
      switch (jobPriority)
      {
        case JobPriority.Never:
          this._jobPriorities.Remove(key);
          goto case JobPriority.High;
        case JobPriority.High:
          if (flag)
            this._jobPriorities[key] = JobPriority.Medium;
          flag = true;
          continue;
        default:
          continue;
      }
    }
    this.NamedItems = namedItems;
    this.PlaytimePerks = playtimePerks;
    this.XenoPrefix = xenoPrefix;
    this.XenoPostfix = xenoPostfix;
  }

  public HumanoidCharacterProfile(HumanoidCharacterProfile other)
    : this(other.Name, other.FlavorText, (string) other.Species, other.Voice, other.Age, other.Sex, other.Gender, other.Appearance.Clone(), other.SpawnPriority, other.ArmorPreference, other.SquadPreference, new Dictionary<ProtoId<JobPrototype>, JobPriority>((IEnumerable<KeyValuePair<ProtoId<JobPrototype>, JobPriority>>) other.JobPriorities), other.PreferenceUnavailable, new HashSet<ProtoId<AntagPrototype>>((IEnumerable<ProtoId<AntagPrototype>>) other.AntagPreferences), new HashSet<ProtoId<TraitPrototype>>((IEnumerable<ProtoId<TraitPrototype>>) other.TraitPreferences), new Dictionary<string, RoleLoadout>((IEnumerable<KeyValuePair<string, RoleLoadout>>) other.Loadouts), other.NamedItems, other.PlaytimePerks, other.XenoPrefix, other.XenoPostfix)
  {
  }

  public HumanoidCharacterProfile()
  {
  }

  public static HumanoidCharacterProfile DefaultWithSpecies(string? species = null)
  {
    if (species == null)
      species = (string) SharedHumanoidAppearanceSystem.DefaultSpecies;
    return new HumanoidCharacterProfile()
    {
      Species = (ProtoId<SpeciesPrototype>) species
    };
  }

  public static HumanoidCharacterProfile Random(HashSet<string>? ignoredSpecies = null)
  {
    IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
    IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
    List<string> allowedSpecies = new List<string>()
    {
      "Human"
    };
    if (prototypeManager.HasIndex<SpeciesPrototype>(HumanoidCharacterProfile.FelinidSpecies))
      allowedSpecies.Add("Felinid");
    SpeciesPrototype[] array = prototypeManager.EnumeratePrototypes<SpeciesPrototype>().Where<SpeciesPrototype>((Func<SpeciesPrototype, bool>) (x => allowedSpecies.Contains(x.ID) && (ignoredSpecies == null || !ignoredSpecies.Contains(x.ID)) && x.RoundStart)).ToArray<SpeciesPrototype>();
    return HumanoidCharacterProfile.RandomWithSpecies(array.Length != 0 ? RandomExtensions.Pick<SpeciesPrototype>(random, (IReadOnlyList<SpeciesPrototype>) array).ID : "Human");
  }

  public static HumanoidCharacterProfile RandomWithSpecies(string? species = null)
  {
    if (species == null)
      species = (string) SharedHumanoidAppearanceSystem.DefaultSpecies;
    IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
    IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
    Sex sex = Sex.Unsexed;
    int num = 18;
    SpeciesPrototype prototype;
    if (prototypeManager.TryIndex<SpeciesPrototype>(species, out prototype))
    {
      sex = RandomExtensions.Pick<Sex>(random, (IReadOnlyList<Sex>) prototype.Sexes);
      num = random.Next(prototype.MinAge, prototype.OldAge);
    }
    Gender gender = Gender.Epicene;
    switch (sex)
    {
      case Sex.Male:
        gender = Gender.Male;
        break;
      case Sex.Female:
        gender = Gender.Female;
        break;
    }
    TTSVoicePrototype[] array = prototypeManager.EnumeratePrototypes<TTSVoicePrototype>().Where<TTSVoicePrototype>((Func<TTSVoicePrototype, bool>) (o => HumanoidCharacterProfile.CanHaveVoice(o, sex) && !o.SponsorOnly)).ToArray<TTSVoicePrototype>();
    string str = array.Length != 0 ? RandomExtensions.Pick<TTSVoicePrototype>(random, (IReadOnlyList<TTSVoicePrototype>) array).ID : SharedHumanoidAppearanceSystem.DefaultSexVoice[sex];
    string name = HumanoidCharacterProfile.GetName(species, gender);
    return new HumanoidCharacterProfile()
    {
      Name = name,
      Sex = sex,
      Age = num,
      Gender = gender,
      Species = (ProtoId<SpeciesPrototype>) species,
      Voice = str,
      Appearance = HumanoidCharacterAppearance.Random(species, sex)
    };
  }

  public HumanoidCharacterProfile WithName(string name)
  {
    return new HumanoidCharacterProfile(this)
    {
      Name = name
    };
  }

  public HumanoidCharacterProfile WithFlavorText(string flavorText)
  {
    return new HumanoidCharacterProfile(this)
    {
      FlavorText = flavorText
    };
  }

  public HumanoidCharacterProfile WithAge(int age)
  {
    return new HumanoidCharacterProfile(this) { Age = age };
  }

  public HumanoidCharacterProfile WithSex(Sex sex)
  {
    return new HumanoidCharacterProfile(this) { Sex = sex };
  }

  public HumanoidCharacterProfile WithGender(Gender gender)
  {
    return new HumanoidCharacterProfile(this)
    {
      Gender = gender
    };
  }

  public HumanoidCharacterProfile WithSpecies(string species)
  {
    return new HumanoidCharacterProfile(this)
    {
      Species = (ProtoId<SpeciesPrototype>) species
    };
  }

  public HumanoidCharacterProfile WithVoice(string voice)
  {
    return new HumanoidCharacterProfile(this)
    {
      Voice = voice
    };
  }

  public HumanoidCharacterProfile WithCharacterAppearance(HumanoidCharacterAppearance appearance)
  {
    return new HumanoidCharacterProfile(this)
    {
      Appearance = appearance
    };
  }

  public HumanoidCharacterProfile WithSpawnPriorityPreference(SpawnPriorityPreference spawnPriority)
  {
    return new HumanoidCharacterProfile(this)
    {
      SpawnPriority = spawnPriority
    };
  }

  public HumanoidCharacterProfile WithArmorPreference(ArmorPreference armorPreference)
  {
    return new HumanoidCharacterProfile(this)
    {
      ArmorPreference = armorPreference
    };
  }

  public HumanoidCharacterProfile WithSquadPreference(
    EntProtoId<SquadTeamComponent>? squadPreference)
  {
    return new HumanoidCharacterProfile(this)
    {
      SquadPreference = squadPreference
    };
  }

  public HumanoidCharacterProfile WithPlaytimePerks(bool playtimePerks)
  {
    return new HumanoidCharacterProfile(this)
    {
      PlaytimePerks = playtimePerks
    };
  }

  public HumanoidCharacterProfile WithXenoPrefix(string prefix)
  {
    return new HumanoidCharacterProfile(this)
    {
      XenoPrefix = prefix
    };
  }

  public HumanoidCharacterProfile WithXenoPostfix(string postfix)
  {
    return new HumanoidCharacterProfile(this)
    {
      XenoPostfix = postfix
    };
  }

  public HumanoidCharacterProfile WithoutLoadouts()
  {
    HumanoidCharacterProfile characterProfile = new HumanoidCharacterProfile(this);
    characterProfile._loadouts.Clear();
    return characterProfile;
  }

  public HumanoidCharacterProfile WithJobPriorities(
    IEnumerable<KeyValuePair<ProtoId<JobPrototype>, JobPriority>> jobPriorities)
  {
    Dictionary<ProtoId<JobPrototype>, JobPriority> dictionary = new Dictionary<ProtoId<JobPrototype>, JobPriority>(jobPriorities);
    bool flag = false;
    foreach ((ProtoId<JobPrototype> key, JobPriority jobPriority) in dictionary)
    {
      switch (jobPriority)
      {
        case JobPriority.Never:
          dictionary.Remove(key);
          goto case JobPriority.High;
        case JobPriority.High:
          if (flag)
            dictionary[key] = JobPriority.Medium;
          flag = true;
          continue;
        default:
          continue;
      }
    }
    return new HumanoidCharacterProfile(this)
    {
      _jobPriorities = dictionary
    };
  }

  public HumanoidCharacterProfile WithJobPriority(ProtoId<JobPrototype> jobId, JobPriority priority)
  {
    Dictionary<ProtoId<JobPrototype>, JobPriority> dictionary = new Dictionary<ProtoId<JobPrototype>, JobPriority>((IDictionary<ProtoId<JobPrototype>, JobPriority>) this._jobPriorities);
    switch (priority)
    {
      case JobPriority.Never:
        dictionary.Remove(jobId);
        break;
      case JobPriority.High:
        foreach ((ProtoId<JobPrototype> key, JobPriority jobPriority) in dictionary)
        {
          if (jobPriority == JobPriority.High)
            dictionary[key] = JobPriority.Medium;
        }
        dictionary[jobId] = priority;
        break;
      default:
        dictionary[jobId] = priority;
        break;
    }
    return new HumanoidCharacterProfile(this)
    {
      _jobPriorities = dictionary
    };
  }

  public HumanoidCharacterProfile WithPreferenceUnavailable(PreferenceUnavailableMode mode)
  {
    return new HumanoidCharacterProfile(this)
    {
      PreferenceUnavailable = mode
    };
  }

  public HumanoidCharacterProfile WithAntagPreferences(
    IEnumerable<ProtoId<AntagPrototype>> antagPreferences)
  {
    return new HumanoidCharacterProfile(this)
    {
      _antagPreferences = new HashSet<ProtoId<AntagPrototype>>(antagPreferences)
    };
  }

  public HumanoidCharacterProfile WithAntagPreference(ProtoId<AntagPrototype> antagId, bool pref)
  {
    HashSet<ProtoId<AntagPrototype>> protoIdSet = new HashSet<ProtoId<AntagPrototype>>((IEnumerable<ProtoId<AntagPrototype>>) this._antagPreferences);
    if (pref)
      protoIdSet.Add(antagId);
    else
      protoIdSet.Remove(antagId);
    return new HumanoidCharacterProfile(this)
    {
      _antagPreferences = protoIdSet
    };
  }

  public HumanoidCharacterProfile WithTraitPreference(
    ProtoId<TraitPrototype> traitId,
    IPrototypeManager protoManager)
  {
    TraitPrototype prototype1;
    if (!protoManager.TryIndex<TraitPrototype>(traitId, out prototype1))
      return new HumanoidCharacterProfile(this);
    ProtoId<TraitCategoryPrototype>? category1 = prototype1.Category;
    TraitCategoryPrototype prototype2 = (TraitCategoryPrototype) null;
    if (category1.HasValue && !protoManager.TryIndex<TraitCategoryPrototype>(category1, out prototype2))
      return new HumanoidCharacterProfile(this);
    HashSet<ProtoId<TraitPrototype>> protoIdSet = new HashSet<ProtoId<TraitPrototype>>((IEnumerable<ProtoId<TraitPrototype>>) this._traitPreferences)
    {
      traitId
    };
    if (prototype2 != null)
    {
      int? maxTraitPoints1 = prototype2.MaxTraitPoints;
      int num1 = 0;
      if (!(maxTraitPoints1.GetValueOrDefault() < num1 & maxTraitPoints1.HasValue))
      {
        int num2 = 0;
        foreach (ProtoId<TraitPrototype> id in protoIdSet)
        {
          TraitPrototype prototype3;
          if (protoManager.TryIndex<TraitPrototype>(id, out prototype3))
          {
            ProtoId<TraitCategoryPrototype>? category2 = prototype3.Category;
            ProtoId<TraitCategoryPrototype> protoId = (ProtoId<TraitCategoryPrototype>) prototype2;
            if ((category2.HasValue ? (category2.GetValueOrDefault() != protoId ? 1 : 0) : 1) == 0)
              num2 += prototype3.Cost;
          }
        }
        int num3 = num2;
        int? maxTraitPoints2 = prototype2.MaxTraitPoints;
        int valueOrDefault = maxTraitPoints2.GetValueOrDefault();
        if (num3 > valueOrDefault & maxTraitPoints2.HasValue && prototype1.Cost != 0)
          return new HumanoidCharacterProfile(this);
        return new HumanoidCharacterProfile(this)
        {
          _traitPreferences = protoIdSet
        };
      }
    }
    return new HumanoidCharacterProfile(this)
    {
      _traitPreferences = protoIdSet
    };
  }

  public HumanoidCharacterProfile WithoutTraitPreference(
    ProtoId<TraitPrototype> traitId,
    IPrototypeManager protoManager)
  {
    HashSet<ProtoId<TraitPrototype>> protoIdSet = new HashSet<ProtoId<TraitPrototype>>((IEnumerable<ProtoId<TraitPrototype>>) this._traitPreferences);
    protoIdSet.Remove(traitId);
    return new HumanoidCharacterProfile(this)
    {
      _traitPreferences = protoIdSet
    };
  }

  public string Summary
  {
    get
    {
      return Loc.GetString("humanoid-character-profile-summary", ("name", (object) this.Name), ("gender", (object) this.Gender.ToString().ToLowerInvariant()), ("age", (object) this.Age));
    }
  }

  public bool MemberwiseEquals(ICharacterProfile maybeOther)
  {
    if (!(maybeOther is HumanoidCharacterProfile characterProfile) || this.Name != characterProfile.Name || this.Age != characterProfile.Age || this.Sex != characterProfile.Sex || this.Gender != characterProfile.Gender || this.Species != characterProfile.Species || this.Voice != characterProfile.Voice || this.PreferenceUnavailable != characterProfile.PreferenceUnavailable || this.SpawnPriority != characterProfile.SpawnPriority)
      return false;
    EntProtoId<SquadTeamComponent>? squadPreference1 = this.SquadPreference;
    EntProtoId<SquadTeamComponent>? squadPreference2 = characterProfile.SquadPreference;
    return (squadPreference1.HasValue == squadPreference2.HasValue ? (squadPreference1.HasValue ? (squadPreference1.GetValueOrDefault() != squadPreference2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0 && this._jobPriorities.SequenceEqual<KeyValuePair<ProtoId<JobPrototype>, JobPriority>>((IEnumerable<KeyValuePair<ProtoId<JobPrototype>, JobPriority>>) characterProfile._jobPriorities) && this._antagPreferences.SequenceEqual<ProtoId<AntagPrototype>>((IEnumerable<ProtoId<AntagPrototype>>) characterProfile._antagPreferences) && this._traitPreferences.SequenceEqual<ProtoId<TraitPrototype>>((IEnumerable<ProtoId<TraitPrototype>>) characterProfile._traitPreferences) && this.Loadouts.SequenceEqual<KeyValuePair<string, RoleLoadout>>((IEnumerable<KeyValuePair<string, RoleLoadout>>) characterProfile.Loadouts) && !(this.FlavorText != characterProfile.FlavorText) && !(this.NamedItems != characterProfile.NamedItems) && this.ArmorPreference == characterProfile.ArmorPreference && this.PlaytimePerks == characterProfile.PlaytimePerks && !(this.XenoPrefix != characterProfile.XenoPrefix) && !(this.XenoPostfix != characterProfile.XenoPostfix) && this.Appearance.MemberwiseEquals((ICharacterAppearance) characterProfile.Appearance);
  }

  public void EnsureValid(ICommonSession session, IDependencyCollection collection)
  {
    IConfigurationManager configurationManager = collection.Resolve<IConfigurationManager>();
    IPrototypeManager prototypeManager = collection.Resolve<IPrototypeManager>();
    IComponentFactory factory = collection.Resolve<IComponentFactory>();
    SpeciesPrototype prototype1;
    if (!prototypeManager.TryIndex<SpeciesPrototype>(this.Species, out prototype1) || !prototype1.RoundStart)
    {
      this.Species = SharedHumanoidAppearanceSystem.DefaultSpecies;
      prototype1 = prototypeManager.Index<SpeciesPrototype>(this.Species);
    }
    if (this.Species != (ProtoId<SpeciesPrototype>) "Human" && this.Species != (ProtoId<SpeciesPrototype>) "Felinid")
    {
      this.Species = SharedHumanoidAppearanceSystem.DefaultSpecies;
      prototype1 = prototypeManager.Index<SpeciesPrototype>(this.Species);
    }
    Sex sex;
    switch (this.Sex)
    {
      case Sex.Male:
        sex = Sex.Male;
        break;
      case Sex.Female:
        sex = Sex.Female;
        break;
      case Sex.Unsexed:
        sex = Sex.Unsexed;
        break;
      default:
        sex = Sex.Male;
        break;
    }
    Sex key1 = sex;
    if (!prototype1.Sexes.Contains(key1))
      key1 = prototype1.Sexes[0];
    int num = Math.Clamp(this.Age, prototype1.MinAge, prototype1.MaxAge);
    Gender gender1;
    switch (this.Gender)
    {
      case Gender.Neuter:
        gender1 = Gender.Neuter;
        break;
      case Gender.Epicene:
        gender1 = Gender.Epicene;
        break;
      case Gender.Female:
        gender1 = Gender.Female;
        break;
      case Gender.Male:
        gender1 = Gender.Male;
        break;
      default:
        gender1 = Gender.Epicene;
        break;
    }
    Gender gender2 = gender1;
    int cvar1 = configurationManager.GetCVar<int>(CCVars.MaxNameLength);
    string str1 = (!string.IsNullOrEmpty(this.Name) ? (this.Name.Length <= cvar1 ? this.Name : this.Name.Substring(0, cvar1)) : HumanoidCharacterProfile.GetName((string) this.Species, gender2)).Trim();
    if (configurationManager.GetCVar<bool>(CCVars.RestrictedNames))
    {
      str1 = HumanoidCharacterProfile.RestrictedNameRegex.Replace(str1, string.Empty);
      if (HumanoidCharacterProfile.ContainsBlockedNameWord(str1, prototypeManager))
        str1 = string.Empty;
    }
    if (configurationManager.GetCVar<bool>(CCVars.ICNameCase))
      str1 = HumanoidCharacterProfile.ICNameCaseRegex.Replace(str1, (MatchEvaluator) (m => m.Groups["word"].Value.ToUpper()));
    if (string.IsNullOrEmpty(str1))
      str1 = HumanoidCharacterProfile.GetName((string) this.Species, gender2);
    int cvar2 = configurationManager.GetCVar<int>(CCVars.MaxFlavorTextLength);
    string str2 = this.FlavorText.Length <= cvar2 ? FormattedMessage.RemoveMarkupOrThrow(this.FlavorText) : FormattedMessage.RemoveMarkupOrThrow(this.FlavorText).Substring(0, cvar2);
    HumanoidCharacterAppearance characterAppearance = HumanoidCharacterAppearance.EnsureValid(this.Appearance, (string) this.Species, this.Sex);
    TTSVoicePrototype prototype2;
    prototypeManager.TryIndex<TTSVoicePrototype>(this.Voice, out prototype2);
    if (prototype2 == null || !HumanoidCharacterProfile.CanHaveVoice(prototype2, this.Sex))
      this.Voice = SharedHumanoidAppearanceSystem.DefaultSexVoice[key1];
    PreferenceUnavailableMode preferenceUnavailableMode1;
    switch (this.PreferenceUnavailable)
    {
      case PreferenceUnavailableMode.StayInLobby:
        preferenceUnavailableMode1 = PreferenceUnavailableMode.StayInLobby;
        break;
      case PreferenceUnavailableMode.SpawnAsOverflow:
        preferenceUnavailableMode1 = PreferenceUnavailableMode.SpawnAsOverflow;
        break;
      default:
        preferenceUnavailableMode1 = PreferenceUnavailableMode.StayInLobby;
        break;
    }
    PreferenceUnavailableMode preferenceUnavailableMode2 = preferenceUnavailableMode1;
    SpawnPriorityPreference priorityPreference1;
    switch (this.SpawnPriority)
    {
      case SpawnPriorityPreference.None:
        priorityPreference1 = SpawnPriorityPreference.None;
        break;
      case SpawnPriorityPreference.Arrivals:
        priorityPreference1 = SpawnPriorityPreference.Arrivals;
        break;
      case SpawnPriorityPreference.Cryosleep:
        priorityPreference1 = SpawnPriorityPreference.Cryosleep;
        break;
      default:
        priorityPreference1 = SpawnPriorityPreference.None;
        break;
    }
    SpawnPriorityPreference priorityPreference2 = priorityPreference1;
    Dictionary<ProtoId<JobPrototype>, JobPriority> dictionary = new Dictionary<ProtoId<JobPrototype>, JobPriority>(this.JobPriorities.Where<KeyValuePair<ProtoId<JobPrototype>, JobPriority>>((Func<KeyValuePair<ProtoId<JobPrototype>, JobPriority>, bool>) (p =>
    {
      JobPrototype prototype3;
      bool flag1 = prototypeManager.TryIndex<JobPrototype>(p.Key, out prototype3) && prototype3.SetPreference;
      if (flag1)
      {
        bool flag2;
        switch (p.Value)
        {
          case JobPriority.Never:
            flag2 = false;
            break;
          case JobPriority.Low:
            flag2 = true;
            break;
          case JobPriority.Medium:
            flag2 = true;
            break;
          case JobPriority.High:
            flag2 = true;
            break;
          default:
            flag2 = false;
            break;
        }
        flag1 = flag2;
      }
      return flag1;
    })));
    bool flag = false;
    foreach ((ProtoId<JobPrototype> key4, JobPriority jobPriority) in dictionary)
    {
      ProtoId<JobPrototype> key3 = key4;
      if (jobPriority == JobPriority.High)
      {
        if (flag)
          dictionary[key3] = JobPriority.Medium;
        flag = true;
      }
    }
    AntagPrototype prototype4;
    List<ProtoId<AntagPrototype>> list1 = this.AntagPreferences.Where<ProtoId<AntagPrototype>>((Func<ProtoId<AntagPrototype>, bool>) (id => prototypeManager.TryIndex<AntagPrototype>(id, out prototype4) && prototype4.SetPreference)).ToList<ProtoId<AntagPrototype>>();
    List<ProtoId<TraitPrototype>> list2 = this.TraitPreferences.Where<ProtoId<TraitPrototype>>(new Func<ProtoId<TraitPrototype>, bool>(prototypeManager.HasIndex<TraitPrototype>)).ToList<ProtoId<TraitPrototype>>();
    this.Name = str1;
    this.FlavorText = str2;
    this.Age = num;
    this.Sex = key1;
    this.Gender = gender2;
    this.Appearance = characterAppearance;
    this.SpawnPriority = priorityPreference2;
    ArmorPreference armorPreference;
    switch (this.ArmorPreference)
    {
      case ArmorPreference.Random:
        armorPreference = ArmorPreference.Random;
        break;
      case ArmorPreference.Padded:
        armorPreference = ArmorPreference.Padded;
        break;
      case ArmorPreference.Padless:
        armorPreference = ArmorPreference.Padless;
        break;
      case ArmorPreference.Ridged:
        armorPreference = ArmorPreference.Ridged;
        break;
      case ArmorPreference.Carrier:
        armorPreference = ArmorPreference.Carrier;
        break;
      case ArmorPreference.Skull:
        armorPreference = ArmorPreference.Skull;
        break;
      case ArmorPreference.Smooth:
        armorPreference = ArmorPreference.Smooth;
        break;
      default:
        armorPreference = ArmorPreference.Random;
        break;
    }
    this.ArmorPreference = armorPreference;
    IPrototypeManager prototypeManager1 = prototypeManager;
    EntProtoId<SquadTeamComponent>? squadPreference = this.SquadPreference;
    EntProtoId? id1 = squadPreference.HasValue ? new EntProtoId?((EntProtoId) squadPreference.GetValueOrDefault()) : new EntProtoId?();
    Robust.Shared.Prototypes.EntityPrototype entityPrototype;
    ref Robust.Shared.Prototypes.EntityPrototype local = ref entityPrototype;
    SquadTeamComponent component;
    if (!prototypeManager1.TryIndex(id1, out local) || !entityPrototype.TryGetComponent<SquadTeamComponent>(out component, factory) || !component.RoundStart)
      this.SquadPreference = new EntProtoId<SquadTeamComponent>?();
    this._jobPriorities.Clear();
    foreach ((key4, jobPriority) in dictionary)
      this._jobPriorities.Add(key4, jobPriority);
    this.PreferenceUnavailable = preferenceUnavailableMode2;
    this._antagPreferences.Clear();
    this._antagPreferences.UnionWith((IEnumerable<ProtoId<AntagPrototype>>) list1);
    this._traitPreferences.Clear();
    this._traitPreferences.UnionWith((IEnumerable<ProtoId<TraitPrototype>>) this.GetValidTraits((IEnumerable<ProtoId<TraitPrototype>>) list2, prototypeManager));
    ValueList<string> valueList = new ValueList<string>();
    foreach ((string str3, RoleLoadout roleLoadout) in this._loadouts)
    {
      if (!prototypeManager.HasIndex<RoleLoadoutPrototype>(str3))
        valueList.Add(str3);
      else
        roleLoadout.EnsureValid(this, session, collection);
    }
    foreach (string key5 in valueList)
      this._loadouts.Remove(key5);
    this.NamedItems = new SharedRMCNamedItems()
    {
      PrimaryGunName = ValidateNamedItem(this.NamedItems.PrimaryGunName),
      SidearmName = ValidateNamedItem(this.NamedItems.SidearmName),
      HelmetName = ValidateNamedItem(this.NamedItems.HelmetName),
      ArmorName = ValidateNamedItem(this.NamedItems.ArmorName),
      SentryName = ValidateNamedItem(this.NamedItems.SentryName)
    };
    this.XenoPrefix = this.XenoPrefix.Trim();
    this.XenoPostfix = this.XenoPostfix.Trim();
    SharedXenoNameSystem sharedXenoNameSystem = collection.Resolve<IEntityManager>().System<SharedXenoNameSystem>();
    int xenoPrefixLength = sharedXenoNameSystem.GetMaxXenoPrefixLength(session);
    int xenoPostfixLength = sharedXenoNameSystem.GetMaxXenoPostfixLength(session);
    if (this.XenoPrefix.Length > xenoPrefixLength)
      this.XenoPrefix = this.XenoPrefix.Substring(0, xenoPrefixLength);
    this.XenoPrefix = ValidateXenoName(this.XenoPrefix, false);
    if (this.XenoPrefix.Length > 2)
    {
      this.XenoPostfix = string.Empty;
    }
    else
    {
      if (this.XenoPostfix.Length > xenoPostfixLength)
        this.XenoPostfix = this.XenoPostfix.Substring(0, xenoPostfixLength);
      this.XenoPostfix = ValidateXenoName(this.XenoPostfix, true);
    }

    static string? ValidateNamedItem(string? itemName)
    {
      return itemName == null || itemName.Length <= 20 ? itemName : itemName.Substring(0, 20);
    }

    static string ValidateXenoName(string xenoName, bool numberEndingAllowed)
    {
      xenoName = xenoName.ToUpperInvariant();
      for (int index = 0; index < xenoName.Length; ++index)
      {
        char ch = xenoName[index];
        if ((!(index > 0 & numberEndingAllowed) || ch <= '0' && ch >= '9') && (ch < 'A' || ch > 'Z'))
          return string.Empty;
      }
      return xenoName;
    }
  }

  public List<ProtoId<TraitPrototype>> GetValidTraits(
    IEnumerable<ProtoId<TraitPrototype>> traits,
    IPrototypeManager protoManager)
  {
    Dictionary<string, int> dict = new Dictionary<string, int>();
    List<ProtoId<TraitPrototype>> validTraits = new List<ProtoId<TraitPrototype>>();
    foreach (ProtoId<TraitPrototype> trait in traits)
    {
      TraitPrototype prototype1;
      if (protoManager.TryIndex<TraitPrototype>(trait, out prototype1))
      {
        if (!prototype1.Category.HasValue)
        {
          validTraits.Add(trait);
        }
        else
        {
          TraitCategoryPrototype prototype2;
          if (protoManager.TryIndex<TraitCategoryPrototype>(prototype1.Category, out prototype2))
          {
            int num1 = dict.GetOrNew<string, int>(prototype2.ID) + prototype1.Cost;
            int num2 = num1;
            int? maxTraitPoints = prototype2.MaxTraitPoints;
            int valueOrDefault = maxTraitPoints.GetValueOrDefault();
            if (!(num2 > valueOrDefault & maxTraitPoints.HasValue))
            {
              dict[prototype2.ID] = num1;
              validTraits.Add(trait);
            }
          }
        }
      }
    }
    return validTraits;
  }

  public ICharacterProfile Validated(ICommonSession session, IDependencyCollection collection)
  {
    HumanoidCharacterProfile characterProfile = new HumanoidCharacterProfile(this);
    characterProfile.EnsureValid(session, collection);
    return (ICharacterProfile) characterProfile;
  }

  public static bool CanHaveVoice(TTSVoicePrototype voice, Sex sex)
  {
    return voice.RoundStart && sex == Sex.Unsexed || voice.Sex == sex || voice.Sex == Sex.Unsexed;
  }

  public static string GetName(string species, Gender gender)
  {
    return IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<NamingSystem>().GetName(species, new Gender?(gender));
  }

  public override bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is HumanoidCharacterProfile characterProfile && this.Equals((object) characterProfile);
  }

  public override int GetHashCode()
  {
    HashCode hashCode = new HashCode();
    hashCode.Add<Dictionary<ProtoId<JobPrototype>, JobPriority>>(this._jobPriorities);
    hashCode.Add<HashSet<ProtoId<AntagPrototype>>>(this._antagPreferences);
    hashCode.Add<HashSet<ProtoId<TraitPrototype>>>(this._traitPreferences);
    hashCode.Add<Dictionary<string, RoleLoadout>>(this._loadouts);
    hashCode.Add<string>(this.Name);
    hashCode.Add<string>(this.FlavorText);
    hashCode.Add<ProtoId<SpeciesPrototype>>(this.Species);
    hashCode.Add<string>(this.Voice);
    hashCode.Add<int>(this.Age);
    hashCode.Add<int>((int) this.Sex);
    hashCode.Add<int>((int) this.Gender);
    hashCode.Add<HumanoidCharacterAppearance>(this.Appearance);
    hashCode.Add<int>((int) this.SpawnPriority);
    hashCode.Add<int>((int) this.ArmorPreference);
    hashCode.Add<EntProtoId<SquadTeamComponent>?>(this.SquadPreference);
    hashCode.Add<int>((int) this.PreferenceUnavailable);
    hashCode.Add<SharedRMCNamedItems>(this.NamedItems);
    hashCode.Add<bool>(this.PlaytimePerks);
    hashCode.Add<string>(this.XenoPrefix);
    hashCode.Add<string>(this.XenoPostfix);
    return hashCode.ToHashCode();
  }

  public void SetLoadout(RoleLoadout loadout) => this._loadouts[loadout.Role.Id] = loadout;

  public HumanoidCharacterProfile WithLoadout(RoleLoadout loadout)
  {
    Dictionary<string, RoleLoadout> dictionary = new Dictionary<string, RoleLoadout>();
    foreach (KeyValuePair<string, RoleLoadout> loadout1 in this._loadouts)
    {
      if (!((ProtoId<RoleLoadoutPrototype>) loadout1.Key == loadout.Role))
        dictionary[loadout1.Key] = loadout1.Value.Clone();
    }
    dictionary[(string) loadout.Role] = loadout.Clone();
    HumanoidCharacterProfile characterProfile = this.Clone();
    characterProfile._loadouts = dictionary;
    return characterProfile;
  }

  public RoleLoadout GetLoadoutOrDefault(
    string id,
    ICommonSession? session,
    ProtoId<SpeciesPrototype>? species,
    IEntityManager entManager,
    IPrototypeManager protoManager)
  {
    RoleLoadout loadoutOrDefault;
    if (!this._loadouts.TryGetValue(id, out loadoutOrDefault))
    {
      loadoutOrDefault = new RoleLoadout((ProtoId<RoleLoadoutPrototype>) id);
      loadoutOrDefault.SetDefault(this, session, protoManager, true);
    }
    loadoutOrDefault.SetDefault(this, session, protoManager);
    return loadoutOrDefault;
  }

  public HumanoidCharacterProfile WithNamedItems(SharedRMCNamedItems named)
  {
    HumanoidCharacterProfile characterProfile = this.Clone();
    characterProfile.NamedItems = named;
    return characterProfile;
  }

  public HumanoidCharacterProfile Clone() => new HumanoidCharacterProfile(this);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HumanoidCharacterProfile target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<HumanoidCharacterProfile>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<ProtoId<JobPrototype>, JobPriority> target1 = (Dictionary<ProtoId<JobPrototype>, JobPriority>) null;
    if (this._jobPriorities == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, JobPriority>>(this._jobPriorities, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, JobPriority>>(this._jobPriorities, hookCtx, context);
    target._jobPriorities = target1;
    HashSet<ProtoId<AntagPrototype>> target2 = (HashSet<ProtoId<AntagPrototype>>) null;
    if (this._antagPreferences == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AntagPrototype>>>(this._antagPreferences, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<AntagPrototype>>>(this._antagPreferences, hookCtx, context);
    target._antagPreferences = target2;
    HashSet<ProtoId<TraitPrototype>> target3 = (HashSet<ProtoId<TraitPrototype>>) null;
    if (this._traitPreferences == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<TraitPrototype>>>(this._traitPreferences, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<TraitPrototype>>>(this._traitPreferences, hookCtx, context);
    target._traitPreferences = target3;
    Dictionary<string, RoleLoadout> target4 = (Dictionary<string, RoleLoadout>) null;
    if (this._loadouts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, RoleLoadout>>(this._loadouts, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<string, RoleLoadout>>(this._loadouts, hookCtx, context);
    target._loadouts = target4;
    string target5 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target5, hookCtx, false, context))
      target5 = this.Name;
    target.Name = target5;
    string target6 = (string) null;
    if (this.FlavorText == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FlavorText, ref target6, hookCtx, false, context))
      target6 = this.FlavorText;
    target.FlavorText = target6;
    ProtoId<SpeciesPrototype> target7 = new ProtoId<SpeciesPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SpeciesPrototype>>(this.Species, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<SpeciesPrototype>>(this.Species, hookCtx, context);
    target.Species = target7;
    string target8 = (string) null;
    if (this.Voice == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Voice, ref target8, hookCtx, false, context))
      target8 = this.Voice;
    target.Voice = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.Age, ref target9, hookCtx, false, context))
      target9 = this.Age;
    target.Age = target9;
    Sex target10 = Sex.Male;
    if (!serialization.TryCustomCopy<Sex>(this.Sex, ref target10, hookCtx, false, context))
      target10 = this.Sex;
    target.Sex = target10;
    Gender target11 = Gender.Neuter;
    if (!serialization.TryCustomCopy<Gender>(this.Gender, ref target11, hookCtx, false, context))
      target11 = this.Gender;
    target.Gender = target11;
    HumanoidCharacterAppearance target12 = (HumanoidCharacterAppearance) null;
    if (this.Appearance == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HumanoidCharacterAppearance>(this.Appearance, ref target12, hookCtx, false, context))
    {
      if (this.Appearance == null)
        target12 = (HumanoidCharacterAppearance) null;
      else
        serialization.CopyTo<HumanoidCharacterAppearance>(this.Appearance, ref target12, hookCtx, context, true);
    }
    target.Appearance = target12;
    SpawnPriorityPreference target13 = SpawnPriorityPreference.None;
    if (!serialization.TryCustomCopy<SpawnPriorityPreference>(this.SpawnPriority, ref target13, hookCtx, false, context))
      target13 = this.SpawnPriority;
    target.SpawnPriority = target13;
    ArmorPreference target14 = ArmorPreference.Random;
    if (!serialization.TryCustomCopy<ArmorPreference>(this.ArmorPreference, ref target14, hookCtx, false, context))
      target14 = this.ArmorPreference;
    target.ArmorPreference = target14;
    EntProtoId<SquadTeamComponent>? target15 = new EntProtoId<SquadTeamComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<SquadTeamComponent>?>(this.SquadPreference, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntProtoId<SquadTeamComponent>?>(this.SquadPreference, hookCtx, context);
    target.SquadPreference = target15;
    PreferenceUnavailableMode target16 = PreferenceUnavailableMode.StayInLobby;
    if (!serialization.TryCustomCopy<PreferenceUnavailableMode>(this.PreferenceUnavailable, ref target16, hookCtx, false, context))
      target16 = this.PreferenceUnavailable;
    target.PreferenceUnavailable = target16;
    SharedRMCNamedItems target17 = (SharedRMCNamedItems) null;
    if (this.NamedItems == (SharedRMCNamedItems) null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SharedRMCNamedItems>(this.NamedItems, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SharedRMCNamedItems>(this.NamedItems, hookCtx, context);
    target.NamedItems = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.PlaytimePerks, ref target18, hookCtx, false, context))
      target18 = this.PlaytimePerks;
    target.PlaytimePerks = target18;
    string target19 = (string) null;
    if (this.XenoPrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.XenoPrefix, ref target19, hookCtx, false, context))
      target19 = this.XenoPrefix;
    target.XenoPrefix = target19;
    string target20 = (string) null;
    if (this.XenoPostfix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.XenoPostfix, ref target20, hookCtx, false, context))
      target20 = this.XenoPostfix;
    target.XenoPostfix = target20;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HumanoidCharacterProfile target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HumanoidCharacterProfile target1 = (HumanoidCharacterProfile) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public HumanoidCharacterProfile Instantiate() => new HumanoidCharacterProfile();
}
