using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

namespace Content.Shared.Preferences;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class HumanoidCharacterProfile : ICharacterProfile, ISerializationGenerated<HumanoidCharacterProfile>, ISerializationGenerated
{
	private static readonly Regex RestrictedNameRegex = new Regex("[^A-Za-zА-Яа-яЁё0-9 '\\-]");

	private static readonly Regex ICNameCaseRegex = new Regex("^(?<word>\\w)|\\b(?<word>\\w)(?=\\w*$)");

	private static readonly ProtoId<DatasetPrototype> BlockedNameWordsDataset = ProtoId<DatasetPrototype>.op_Implicit("PubgBlockedNameWords");

	private static readonly ProtoId<SpeciesPrototype> FelinidSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Felinid");

	[DataField(null, false, 1, false, false, null)]
	private Dictionary<ProtoId<JobPrototype>, JobPriority> _jobPriorities = new Dictionary<ProtoId<JobPrototype>, JobPriority> { 
	{
		SharedGameTicker.FallbackOverflowJob,
		JobPriority.High
	} };

	[DataField(null, false, 1, false, false, null)]
	private HashSet<ProtoId<AntagPrototype>> _antagPreferences = new HashSet<ProtoId<AntagPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	private HashSet<ProtoId<TraitPrototype>> _traitPreferences = new HashSet<ProtoId<TraitPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	private Dictionary<string, RoleLoadout> _loadouts = new Dictionary<string, RoleLoadout>();

	public IReadOnlyDictionary<string, RoleLoadout> Loadouts => _loadouts;

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
	public Gender Gender { get; private set; } = (Gender)3;

	public ICharacterAppearance CharacterAppearance => Appearance;

	[DataField(null, false, 1, false, false, null)]
	public HumanoidCharacterAppearance Appearance { get; set; } = new HumanoidCharacterAppearance();

	[DataField(null, false, 1, false, false, null)]
	public SpawnPriorityPreference SpawnPriority { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public ArmorPreference ArmorPreference { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId<SquadTeamComponent>? SquadPreference { get; private set; }

	public IReadOnlyDictionary<ProtoId<JobPrototype>, JobPriority> JobPriorities => _jobPriorities;

	public IReadOnlySet<ProtoId<AntagPrototype>> AntagPreferences => _antagPreferences;

	public IReadOnlySet<ProtoId<TraitPrototype>> TraitPreferences => _traitPreferences;

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

	public string Summary => Loc.GetString("humanoid-character-profile-summary", new(string, object)[3]
	{
		("name", Name),
		("gender", ((object)Gender/*cast due to constrained. prefix*/).ToString().ToLowerInvariant()),
		("age", Age)
	});

	private static bool ContainsBlockedNameWord(string name, IPrototypeManager prototypeManager)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DatasetPrototype dataset = default(DatasetPrototype);
		if (!prototypeManager.TryIndex<DatasetPrototype>(BlockedNameWordsDataset, ref dataset))
		{
			return false;
		}
		string lower = name.ToLowerInvariant();
		foreach (string word in dataset.Values)
		{
			if (!string.IsNullOrEmpty(word) && lower.Contains(word.ToLowerInvariant()))
			{
				return true;
			}
		}
		return false;
	}

	public HumanoidCharacterProfile(string name, string flavortext, string species, string voice, int age, Sex sex, Gender gender, HumanoidCharacterAppearance appearance, SpawnPriorityPreference spawnPriority, ArmorPreference armorPreference, EntProtoId<SquadTeamComponent>? squadPreference, Dictionary<ProtoId<JobPrototype>, JobPriority> jobPriorities, PreferenceUnavailableMode preferenceUnavailable, HashSet<ProtoId<AntagPrototype>> antagPreferences, HashSet<ProtoId<TraitPrototype>> traitPreferences, Dictionary<string, RoleLoadout> loadouts, SharedRMCNamedItems namedItems, bool playtimePerks, string xenoPrefix, string xenoPostfix)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		Name = name;
		FlavorText = flavortext;
		Species = ProtoId<SpeciesPrototype>.op_Implicit(species);
		Voice = voice;
		Age = age;
		Sex = sex;
		Gender = gender;
		Appearance = appearance;
		SpawnPriority = spawnPriority;
		ArmorPreference = armorPreference;
		SquadPreference = squadPreference;
		_jobPriorities = jobPriorities;
		PreferenceUnavailable = preferenceUnavailable;
		_antagPreferences = antagPreferences;
		_traitPreferences = traitPreferences;
		_loadouts = loadouts;
		bool hasHighPrority = false;
		foreach (var (key, value) in _jobPriorities)
		{
			if (value == JobPriority.Never)
			{
				_jobPriorities.Remove(key);
			}
			else if (value != JobPriority.High)
			{
				continue;
			}
			if (hasHighPrority)
			{
				_jobPriorities[key] = JobPriority.Medium;
			}
			hasHighPrority = true;
		}
		NamedItems = namedItems;
		PlaytimePerks = playtimePerks;
		XenoPrefix = xenoPrefix;
		XenoPostfix = xenoPostfix;
	}

	public HumanoidCharacterProfile(HumanoidCharacterProfile other)
		: this(other.Name, other.FlavorText, ProtoId<SpeciesPrototype>.op_Implicit(other.Species), other.Voice, other.Age, other.Sex, other.Gender, other.Appearance.Clone(), other.SpawnPriority, other.ArmorPreference, other.SquadPreference, new Dictionary<ProtoId<JobPrototype>, JobPriority>(other.JobPriorities), other.PreferenceUnavailable, new HashSet<ProtoId<AntagPrototype>>(other.AntagPreferences), new HashSet<ProtoId<TraitPrototype>>(other.TraitPreferences), new Dictionary<string, RoleLoadout>(other.Loadouts), other.NamedItems, other.PlaytimePerks, other.XenoPrefix, other.XenoPostfix)
	{
	}//IL_000e: Unknown result type (might be due to invalid IL or missing references)
	//IL_002b: Unknown result type (might be due to invalid IL or missing references)


	public HumanoidCharacterProfile()
	{
	}//IL_0007: Unknown result type (might be due to invalid IL or missing references)
	//IL_004f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0054: Unknown result type (might be due to invalid IL or missing references)
	//IL_006e: Unknown result type (might be due to invalid IL or missing references)


	public static HumanoidCharacterProfile DefaultWithSpecies(string? species = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		if (species == null)
		{
			species = ProtoId<SpeciesPrototype>.op_Implicit(SharedHumanoidAppearanceSystem.DefaultSpecies);
		}
		return new HumanoidCharacterProfile
		{
			Species = ProtoId<SpeciesPrototype>.op_Implicit(species)
		};
	}

	public static HumanoidCharacterProfile Random(HashSet<string>? ignoredSpecies = null)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		IPrototypeManager obj = IoCManager.Resolve<IPrototypeManager>();
		IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
		List<string> allowedSpecies = new List<string> { "Human" };
		if (obj.HasIndex<SpeciesPrototype>(FelinidSpecies))
		{
			allowedSpecies.Add("Felinid");
		}
		SpeciesPrototype[] species = (from x in obj.EnumeratePrototypes<SpeciesPrototype>()
			where allowedSpecies.Contains(x.ID) && (ignoredSpecies == null || !ignoredSpecies.Contains(x.ID)) && x.RoundStart
			select x).ToArray();
		return RandomWithSpecies((species.Length != 0) ? RandomExtensions.Pick<SpeciesPrototype>(random, (IReadOnlyList<SpeciesPrototype>)species).ID : "Human");
	}

	public static HumanoidCharacterProfile RandomWithSpecies(string? species = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		if (species == null)
		{
			species = ProtoId<SpeciesPrototype>.op_Implicit(SharedHumanoidAppearanceSystem.DefaultSpecies);
		}
		IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
		IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
		Sex sex = Sex.Unsexed;
		int age = 18;
		SpeciesPrototype speciesPrototype = default(SpeciesPrototype);
		if (prototypeManager.TryIndex<SpeciesPrototype>(species, ref speciesPrototype))
		{
			sex = RandomExtensions.Pick<Sex>(random, (IReadOnlyList<Sex>)speciesPrototype.Sexes);
			age = random.Next(speciesPrototype.MinAge, speciesPrototype.OldAge);
		}
		Gender gender = (Gender)1;
		switch (sex)
		{
		case Sex.Male:
			gender = (Gender)3;
			break;
		case Sex.Female:
			gender = (Gender)2;
			break;
		}
		TTSVoicePrototype[] voices = (from o in prototypeManager.EnumeratePrototypes<TTSVoicePrototype>()
			where CanHaveVoice(o, sex) && !o.SponsorOnly
			select o).ToArray();
		string voiceId = ((voices.Length != 0) ? RandomExtensions.Pick<TTSVoicePrototype>(random, (IReadOnlyList<TTSVoicePrototype>)voices).ID : SharedHumanoidAppearanceSystem.DefaultSexVoice[sex]);
		string name = GetName(species, gender);
		return new HumanoidCharacterProfile
		{
			Name = name,
			Sex = sex,
			Age = age,
			Gender = gender,
			Species = ProtoId<SpeciesPrototype>.op_Implicit(species),
			Voice = voiceId,
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
		return new HumanoidCharacterProfile(this)
		{
			Age = age
		};
	}

	public HumanoidCharacterProfile WithSex(Sex sex)
	{
		return new HumanoidCharacterProfile(this)
		{
			Sex = sex
		};
	}

	public HumanoidCharacterProfile WithGender(Gender gender)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new HumanoidCharacterProfile(this)
		{
			Gender = gender
		};
	}

	public HumanoidCharacterProfile WithSpecies(string species)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return new HumanoidCharacterProfile(this)
		{
			Species = ProtoId<SpeciesPrototype>.op_Implicit(species)
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

	public HumanoidCharacterProfile WithSquadPreference(EntProtoId<SquadTeamComponent>? squadPreference)
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
		HumanoidCharacterProfile humanoidCharacterProfile = new HumanoidCharacterProfile(this);
		humanoidCharacterProfile._loadouts.Clear();
		return humanoidCharacterProfile;
	}

	public HumanoidCharacterProfile WithJobPriorities(IEnumerable<KeyValuePair<ProtoId<JobPrototype>, JobPriority>> jobPriorities)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ProtoId<JobPrototype>, JobPriority> dictionary = new Dictionary<ProtoId<JobPrototype>, JobPriority>(jobPriorities);
		bool hasHighPrority = false;
		foreach (var (key, value) in dictionary)
		{
			if (value == JobPriority.Never)
			{
				dictionary.Remove(key);
			}
			else if (value != JobPriority.High)
			{
				continue;
			}
			if (hasHighPrority)
			{
				dictionary[key] = JobPriority.Medium;
			}
			hasHighPrority = true;
		}
		return new HumanoidCharacterProfile(this)
		{
			_jobPriorities = dictionary
		};
	}

	public HumanoidCharacterProfile WithJobPriority(ProtoId<JobPrototype> jobId, JobPriority priority)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ProtoId<JobPrototype>, JobPriority> dictionary = new Dictionary<ProtoId<JobPrototype>, JobPriority>(_jobPriorities);
		switch (priority)
		{
		case JobPriority.Never:
			dictionary.Remove(jobId);
			break;
		case JobPriority.High:
			foreach (var (job, jobPriority2) in dictionary)
			{
				if (jobPriority2 == JobPriority.High)
				{
					dictionary[job] = JobPriority.Medium;
				}
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

	public HumanoidCharacterProfile WithAntagPreferences(IEnumerable<ProtoId<AntagPrototype>> antagPreferences)
	{
		return new HumanoidCharacterProfile(this)
		{
			_antagPreferences = new HashSet<ProtoId<AntagPrototype>>(antagPreferences)
		};
	}

	public HumanoidCharacterProfile WithAntagPreference(ProtoId<AntagPrototype> antagId, bool pref)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		HashSet<ProtoId<AntagPrototype>> list = new HashSet<ProtoId<AntagPrototype>>(_antagPreferences);
		if (pref)
		{
			list.Add(antagId);
		}
		else
		{
			list.Remove(antagId);
		}
		return new HumanoidCharacterProfile(this)
		{
			_antagPreferences = list
		};
	}

	public HumanoidCharacterProfile WithTraitPreference(ProtoId<TraitPrototype> traitId, IPrototypeManager protoManager)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		TraitPrototype traitProto = default(TraitPrototype);
		if (!protoManager.TryIndex<TraitPrototype>(traitId, ref traitProto))
		{
			return new HumanoidCharacterProfile(this);
		}
		ProtoId<TraitCategoryPrototype>? category = traitProto.Category;
		TraitCategoryPrototype traitCategory = null;
		if (category.HasValue && !protoManager.TryIndex<TraitCategoryPrototype>(category, ref traitCategory))
		{
			return new HumanoidCharacterProfile(this);
		}
		HashSet<ProtoId<TraitPrototype>> list = new HashSet<ProtoId<TraitPrototype>>(_traitPreferences) { traitId };
		if (traitCategory == null || traitCategory.MaxTraitPoints < 0)
		{
			return new HumanoidCharacterProfile(this)
			{
				_traitPreferences = list
			};
		}
		int count = 0;
		TraitPrototype otherProto = default(TraitPrototype);
		foreach (ProtoId<TraitPrototype> trait in list)
		{
			if (protoManager.TryIndex<TraitPrototype>(trait, ref otherProto))
			{
				ProtoId<TraitCategoryPrototype>? category2 = otherProto.Category;
				ProtoId<TraitCategoryPrototype> val = ProtoId<TraitCategoryPrototype>.op_Implicit(traitCategory);
				if (category2.HasValue && !(category2.GetValueOrDefault() != val))
				{
					count += otherProto.Cost;
				}
			}
		}
		if (count > traitCategory.MaxTraitPoints && traitProto.Cost != 0)
		{
			return new HumanoidCharacterProfile(this);
		}
		return new HumanoidCharacterProfile(this)
		{
			_traitPreferences = list
		};
	}

	public HumanoidCharacterProfile WithoutTraitPreference(ProtoId<TraitPrototype> traitId, IPrototypeManager protoManager)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		HashSet<ProtoId<TraitPrototype>> list = new HashSet<ProtoId<TraitPrototype>>(_traitPreferences);
		list.Remove(traitId);
		return new HumanoidCharacterProfile(this)
		{
			_traitPreferences = list
		};
	}

	public bool MemberwiseEquals(ICharacterProfile maybeOther)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (!(maybeOther is HumanoidCharacterProfile other))
		{
			return false;
		}
		if (Name != other.Name)
		{
			return false;
		}
		if (Age != other.Age)
		{
			return false;
		}
		if (Sex != other.Sex)
		{
			return false;
		}
		if (Gender != other.Gender)
		{
			return false;
		}
		if (Species != other.Species)
		{
			return false;
		}
		if (Voice != other.Voice)
		{
			return false;
		}
		if (PreferenceUnavailable != other.PreferenceUnavailable)
		{
			return false;
		}
		if (SpawnPriority != other.SpawnPriority)
		{
			return false;
		}
		EntProtoId<SquadTeamComponent>? squadPreference = SquadPreference;
		EntProtoId<SquadTeamComponent>? squadPreference2 = other.SquadPreference;
		if (squadPreference.HasValue != squadPreference2.HasValue || (squadPreference.HasValue && squadPreference.GetValueOrDefault() != squadPreference2.GetValueOrDefault()))
		{
			return false;
		}
		if (!_jobPriorities.SequenceEqual(other._jobPriorities))
		{
			return false;
		}
		if (!_antagPreferences.SequenceEqual(other._antagPreferences))
		{
			return false;
		}
		if (!_traitPreferences.SequenceEqual(other._traitPreferences))
		{
			return false;
		}
		if (!Loadouts.SequenceEqual(other.Loadouts))
		{
			return false;
		}
		if (FlavorText != other.FlavorText)
		{
			return false;
		}
		if (NamedItems != other.NamedItems)
		{
			return false;
		}
		if (ArmorPreference != other.ArmorPreference)
		{
			return false;
		}
		if (PlaytimePerks != other.PlaytimePerks)
		{
			return false;
		}
		if (XenoPrefix != other.XenoPrefix)
		{
			return false;
		}
		if (XenoPostfix != other.XenoPostfix)
		{
			return false;
		}
		return Appearance.MemberwiseEquals(other.Appearance);
	}

	public void EnsureValid(ICommonSession session, IDependencyCollection collection)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected I4, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		IConfigurationManager configManager = collection.Resolve<IConfigurationManager>();
		IPrototypeManager prototypeManager = collection.Resolve<IPrototypeManager>();
		IComponentFactory compFactory = collection.Resolve<IComponentFactory>();
		SpeciesPrototype speciesPrototype = default(SpeciesPrototype);
		if (!prototypeManager.TryIndex<SpeciesPrototype>(Species, ref speciesPrototype) || !speciesPrototype.RoundStart)
		{
			Species = SharedHumanoidAppearanceSystem.DefaultSpecies;
			speciesPrototype = prototypeManager.Index<SpeciesPrototype>(Species);
		}
		if (Species != ProtoId<SpeciesPrototype>.op_Implicit("Human") && Species != ProtoId<SpeciesPrototype>.op_Implicit("Felinid"))
		{
			Species = SharedHumanoidAppearanceSystem.DefaultSpecies;
			speciesPrototype = prototypeManager.Index<SpeciesPrototype>(Species);
		}
		Sex sex = Sex switch
		{
			Sex.Male => Sex.Male, 
			Sex.Female => Sex.Female, 
			Sex.Unsexed => Sex.Unsexed, 
			_ => Sex.Male, 
		};
		if (!speciesPrototype.Sexes.Contains(sex))
		{
			sex = speciesPrototype.Sexes[0];
		}
		int age = Math.Clamp(Age, speciesPrototype.MinAge, speciesPrototype.MaxAge);
		Gender gender = Gender;
		Gender gender2 = (Gender)((int)gender switch
		{
			1 => 1, 
			2 => 2, 
			3 => 3, 
			0 => 0, 
			_ => 1, 
		});
		int maxNameLength = configManager.GetCVar<int>(CCVars.MaxNameLength);
		string name = (string.IsNullOrEmpty(Name) ? GetName(ProtoId<SpeciesPrototype>.op_Implicit(Species), gender2) : ((Name.Length <= maxNameLength) ? Name : Name.Substring(0, maxNameLength)));
		name = name.Trim();
		if (configManager.GetCVar<bool>(CCVars.RestrictedNames))
		{
			name = RestrictedNameRegex.Replace(name, string.Empty);
			if (ContainsBlockedNameWord(name, prototypeManager))
			{
				name = string.Empty;
			}
		}
		if (configManager.GetCVar<bool>(CCVars.ICNameCase))
		{
			name = ICNameCaseRegex.Replace(name, (Match m) => m.Groups["word"].Value.ToUpper());
		}
		if (string.IsNullOrEmpty(name))
		{
			name = GetName(ProtoId<SpeciesPrototype>.op_Implicit(Species), gender2);
		}
		int maxFlavorTextLength = configManager.GetCVar<int>(CCVars.MaxFlavorTextLength);
		string flavortext = ((FlavorText.Length <= maxFlavorTextLength) ? FormattedMessage.RemoveMarkupOrThrow(FlavorText) : FormattedMessage.RemoveMarkupOrThrow(FlavorText).Substring(0, maxFlavorTextLength));
		HumanoidCharacterAppearance appearance = HumanoidCharacterAppearance.EnsureValid(Appearance, ProtoId<SpeciesPrototype>.op_Implicit(Species), Sex);
		TTSVoicePrototype voice = default(TTSVoicePrototype);
		prototypeManager.TryIndex<TTSVoicePrototype>(Voice, ref voice);
		if (voice == null || !CanHaveVoice(voice, Sex))
		{
			Voice = SharedHumanoidAppearanceSystem.DefaultSexVoice[sex];
		}
		PreferenceUnavailableMode prefsUnavailableMode = PreferenceUnavailable switch
		{
			PreferenceUnavailableMode.StayInLobby => PreferenceUnavailableMode.StayInLobby, 
			PreferenceUnavailableMode.SpawnAsOverflow => PreferenceUnavailableMode.SpawnAsOverflow, 
			_ => PreferenceUnavailableMode.StayInLobby, 
		};
		SpawnPriorityPreference spawnPriority = SpawnPriority switch
		{
			SpawnPriorityPreference.None => SpawnPriorityPreference.None, 
			SpawnPriorityPreference.Arrivals => SpawnPriorityPreference.Arrivals, 
			SpawnPriorityPreference.Cryosleep => SpawnPriorityPreference.Cryosleep, 
			_ => SpawnPriorityPreference.None, 
		};
		Dictionary<ProtoId<JobPrototype>, JobPriority> priorities = new Dictionary<ProtoId<JobPrototype>, JobPriority>(JobPriorities.Where<KeyValuePair<ProtoId<JobPrototype>, JobPriority>>(delegate(KeyValuePair<ProtoId<JobPrototype>, JobPriority> p)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			JobPrototype jobPrototype = default(JobPrototype);
			bool flag = prototypeManager.TryIndex<JobPrototype>(p.Key, ref jobPrototype) && jobPrototype.SetPreference;
			if (flag)
			{
				flag = p.Value switch
				{
					JobPriority.Never => false, 
					JobPriority.Low => true, 
					JobPriority.Medium => true, 
					JobPriority.High => true, 
					_ => false, 
				};
			}
			return flag;
		}));
		bool hasHighPrio = false;
		ProtoId<JobPrototype> key;
		JobPriority value;
		foreach (KeyValuePair<ProtoId<JobPrototype>, JobPriority> item in priorities)
		{
			item.Deconstruct(out key, out value);
			ProtoId<JobPrototype> key2 = key;
			if (value == JobPriority.High)
			{
				if (hasHighPrio)
				{
					priorities[key2] = JobPriority.Medium;
				}
				hasHighPrio = true;
			}
		}
		List<ProtoId<AntagPrototype>> antags = AntagPreferences.Where<ProtoId<AntagPrototype>>((ProtoId<AntagPrototype> id) =>
		{
			AntagPrototype antagPrototype = default(AntagPrototype);
			return prototypeManager.TryIndex<AntagPrototype>(id, ref antagPrototype) && antagPrototype.SetPreference;
		}).ToList();
		List<ProtoId<TraitPrototype>> traits = ((IEnumerable<ProtoId<TraitPrototype>>)TraitPreferences).Where((Func<ProtoId<TraitPrototype>, bool>)prototypeManager.HasIndex<TraitPrototype>).ToList();
		Name = name;
		FlavorText = flavortext;
		Age = age;
		Sex = sex;
		Gender = gender2;
		Appearance = appearance;
		SpawnPriority = spawnPriority;
		ArmorPreference = ArmorPreference switch
		{
			ArmorPreference.Random => ArmorPreference.Random, 
			ArmorPreference.Padded => ArmorPreference.Padded, 
			ArmorPreference.Padless => ArmorPreference.Padless, 
			ArmorPreference.Ridged => ArmorPreference.Ridged, 
			ArmorPreference.Carrier => ArmorPreference.Carrier, 
			ArmorPreference.Skull => ArmorPreference.Skull, 
			ArmorPreference.Smooth => ArmorPreference.Smooth, 
			_ => ArmorPreference.Random, 
		};
		IPrototypeManager obj = prototypeManager;
		EntProtoId<SquadTeamComponent>? squadPreference = SquadPreference;
		EntityPrototype squad = default(EntityPrototype);
		SquadTeamComponent team = default(SquadTeamComponent);
		if (!obj.TryIndex(squadPreference.HasValue ? new EntProtoId?(EntProtoId<SquadTeamComponent>.op_Implicit(squadPreference.GetValueOrDefault())) : ((EntProtoId?)null), ref squad) || !squad.TryGetComponent<SquadTeamComponent>(ref team, compFactory) || !team.RoundStart)
		{
			SquadPreference = null;
		}
		_jobPriorities.Clear();
		foreach (KeyValuePair<ProtoId<JobPrototype>, JobPriority> item2 in priorities)
		{
			item2.Deconstruct(out key, out value);
			ProtoId<JobPrototype> job = key;
			JobPriority priority = value;
			_jobPriorities.Add(job, priority);
		}
		PreferenceUnavailable = prefsUnavailableMode;
		_antagPreferences.Clear();
		_antagPreferences.UnionWith(antags);
		_traitPreferences.Clear();
		_traitPreferences.UnionWith(GetValidTraits(traits, prototypeManager));
		ValueList<string> toRemove = default(ValueList<string>);
		foreach (var (roleName, loadouts) in _loadouts)
		{
			if (!prototypeManager.HasIndex<RoleLoadoutPrototype>(roleName))
			{
				toRemove.Add(roleName);
			}
			else
			{
				loadouts.EnsureValid(this, session, collection);
			}
		}
		Enumerator<string> enumerator3 = toRemove.GetEnumerator();
		try
		{
			while (enumerator3.MoveNext())
			{
				string value2 = enumerator3.Current;
				_loadouts.Remove(value2);
			}
		}
		finally
		{
			((IDisposable)enumerator3/*cast due to constrained. prefix*/).Dispose();
		}
		NamedItems = new SharedRMCNamedItems
		{
			PrimaryGunName = ValidateNamedItem(NamedItems.PrimaryGunName),
			SidearmName = ValidateNamedItem(NamedItems.SidearmName),
			HelmetName = ValidateNamedItem(NamedItems.HelmetName),
			ArmorName = ValidateNamedItem(NamedItems.ArmorName),
			SentryName = ValidateNamedItem(NamedItems.SentryName)
		};
		XenoPrefix = XenoPrefix.Trim();
		XenoPostfix = XenoPostfix.Trim();
		SharedXenoNameSystem sharedXenoNameSystem = collection.Resolve<IEntityManager>().System<SharedXenoNameSystem>();
		int prefixMax = sharedXenoNameSystem.GetMaxXenoPrefixLength(session);
		int postfixMax = sharedXenoNameSystem.GetMaxXenoPostfixLength(session);
		if (XenoPrefix.Length > prefixMax)
		{
			XenoPrefix = XenoPrefix.Substring(0, prefixMax);
		}
		XenoPrefix = ValidateXenoName(XenoPrefix, numberEndingAllowed: false);
		if (XenoPrefix.Length > 2)
		{
			XenoPostfix = string.Empty;
			return;
		}
		if (XenoPostfix.Length > postfixMax)
		{
			XenoPostfix = XenoPostfix.Substring(0, postfixMax);
		}
		XenoPostfix = ValidateXenoName(XenoPostfix, numberEndingAllowed: true);
		static string? ValidateNamedItem(string? itemName)
		{
			if (itemName == null || itemName.Length <= 20)
			{
				return itemName;
			}
			return itemName.Substring(0, 20);
		}
		static string ValidateXenoName(string xenoName, bool numberEndingAllowed)
		{
			xenoName = xenoName.ToUpperInvariant();
			for (int i = 0; i < xenoName.Length; i++)
			{
				char c = xenoName[i];
				if ((!(i > 0 && numberEndingAllowed) || (c <= '0' && c >= '9')) && (c < 'A' || c > 'Z'))
				{
					return string.Empty;
				}
			}
			return xenoName;
		}
	}

	public List<ProtoId<TraitPrototype>> GetValidTraits(IEnumerable<ProtoId<TraitPrototype>> traits, IPrototypeManager protoManager)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, int> groups = new Dictionary<string, int>();
		List<ProtoId<TraitPrototype>> result = new List<ProtoId<TraitPrototype>>();
		TraitPrototype traitProto = default(TraitPrototype);
		TraitCategoryPrototype category = default(TraitCategoryPrototype);
		foreach (ProtoId<TraitPrototype> trait in traits)
		{
			if (!protoManager.TryIndex<TraitPrototype>(trait, ref traitProto))
			{
				continue;
			}
			if (!traitProto.Category.HasValue)
			{
				result.Add(trait);
			}
			else if (protoManager.TryIndex<TraitCategoryPrototype>(traitProto.Category, ref category))
			{
				int existing = Extensions.GetOrNew<string, int>(groups, category.ID);
				existing += traitProto.Cost;
				if (!(existing > category.MaxTraitPoints))
				{
					groups[category.ID] = existing;
					result.Add(trait);
				}
			}
		}
		return result;
	}

	public ICharacterProfile Validated(ICommonSession session, IDependencyCollection collection)
	{
		HumanoidCharacterProfile humanoidCharacterProfile = new HumanoidCharacterProfile(this);
		humanoidCharacterProfile.EnsureValid(session, collection);
		return humanoidCharacterProfile;
	}

	public static bool CanHaveVoice(TTSVoicePrototype voice, Sex sex)
	{
		if (!voice.RoundStart || sex != Sex.Unsexed)
		{
			if (voice.Sex != sex)
			{
				return voice.Sex == Sex.Unsexed;
			}
			return true;
		}
		return true;
	}

	public static string GetName(string species, Gender gender)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<NamingSystem>().GetName(species, gender);
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is HumanoidCharacterProfile other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected I4, but got Unknown
		HashCode hashCode = default(HashCode);
		hashCode.Add(_jobPriorities);
		hashCode.Add(_antagPreferences);
		hashCode.Add(_traitPreferences);
		hashCode.Add(_loadouts);
		hashCode.Add(Name);
		hashCode.Add(FlavorText);
		hashCode.Add<ProtoId<SpeciesPrototype>>(Species);
		hashCode.Add(Voice);
		hashCode.Add(Age);
		hashCode.Add((int)Sex);
		hashCode.Add((int)Gender);
		hashCode.Add(Appearance);
		hashCode.Add((int)SpawnPriority);
		hashCode.Add((int)ArmorPreference);
		hashCode.Add(SquadPreference);
		hashCode.Add((int)PreferenceUnavailable);
		hashCode.Add(NamedItems);
		hashCode.Add(PlaytimePerks);
		hashCode.Add(XenoPrefix);
		hashCode.Add(XenoPostfix);
		return hashCode.ToHashCode();
	}

	public void SetLoadout(RoleLoadout loadout)
	{
		_loadouts[loadout.Role.Id] = loadout;
	}

	public HumanoidCharacterProfile WithLoadout(RoleLoadout loadout)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, RoleLoadout> copied = new Dictionary<string, RoleLoadout>();
		foreach (KeyValuePair<string, RoleLoadout> proto in _loadouts)
		{
			if (!(ProtoId<RoleLoadoutPrototype>.op_Implicit(proto.Key) == loadout.Role))
			{
				copied[proto.Key] = proto.Value.Clone();
			}
		}
		copied[ProtoId<RoleLoadoutPrototype>.op_Implicit(loadout.Role)] = loadout.Clone();
		HumanoidCharacterProfile humanoidCharacterProfile = Clone();
		humanoidCharacterProfile._loadouts = copied;
		return humanoidCharacterProfile;
	}

	public RoleLoadout GetLoadoutOrDefault(string id, ICommonSession? session, ProtoId<SpeciesPrototype>? species, IEntityManager entManager, IPrototypeManager protoManager)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!_loadouts.TryGetValue(id, out RoleLoadout loadout))
		{
			loadout = new RoleLoadout(ProtoId<RoleLoadoutPrototype>.op_Implicit(id));
			loadout.SetDefault(this, session, protoManager, force: true);
		}
		loadout.SetDefault(this, session, protoManager);
		return loadout;
	}

	public HumanoidCharacterProfile WithNamedItems(SharedRMCNamedItems named)
	{
		HumanoidCharacterProfile humanoidCharacterProfile = Clone();
		humanoidCharacterProfile.NamedItems = named;
		return humanoidCharacterProfile;
	}

	public HumanoidCharacterProfile Clone()
	{
		return new HumanoidCharacterProfile(this);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HumanoidCharacterProfile target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<HumanoidCharacterProfile>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		Dictionary<ProtoId<JobPrototype>, JobPriority> _jobPrioritiesTemp = null;
		if (_jobPriorities == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<ProtoId<JobPrototype>, JobPriority>>(_jobPriorities, ref _jobPrioritiesTemp, hookCtx, true, context))
		{
			_jobPrioritiesTemp = serialization.CreateCopy<Dictionary<ProtoId<JobPrototype>, JobPriority>>(_jobPriorities, hookCtx, context, false);
		}
		target._jobPriorities = _jobPrioritiesTemp;
		HashSet<ProtoId<AntagPrototype>> _antagPreferencesTemp = null;
		if (_antagPreferences == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<HashSet<ProtoId<AntagPrototype>>>(_antagPreferences, ref _antagPreferencesTemp, hookCtx, true, context))
		{
			_antagPreferencesTemp = serialization.CreateCopy<HashSet<ProtoId<AntagPrototype>>>(_antagPreferences, hookCtx, context, false);
		}
		target._antagPreferences = _antagPreferencesTemp;
		HashSet<ProtoId<TraitPrototype>> _traitPreferencesTemp = null;
		if (_traitPreferences == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<HashSet<ProtoId<TraitPrototype>>>(_traitPreferences, ref _traitPreferencesTemp, hookCtx, true, context))
		{
			_traitPreferencesTemp = serialization.CreateCopy<HashSet<ProtoId<TraitPrototype>>>(_traitPreferences, hookCtx, context, false);
		}
		target._traitPreferences = _traitPreferencesTemp;
		Dictionary<string, RoleLoadout> _loadoutsTemp = null;
		if (_loadouts == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<string, RoleLoadout>>(_loadouts, ref _loadoutsTemp, hookCtx, true, context))
		{
			_loadoutsTemp = serialization.CreateCopy<Dictionary<string, RoleLoadout>>(_loadouts, hookCtx, context, false);
		}
		target._loadouts = _loadoutsTemp;
		string NameTemp = null;
		if (Name == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
		{
			NameTemp = Name;
		}
		target.Name = NameTemp;
		string FlavorTextTemp = null;
		if (FlavorText == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(FlavorText, ref FlavorTextTemp, hookCtx, false, context))
		{
			FlavorTextTemp = FlavorText;
		}
		target.FlavorText = FlavorTextTemp;
		ProtoId<SpeciesPrototype> SpeciesTemp = default(ProtoId<SpeciesPrototype>);
		if (!serialization.TryCustomCopy<ProtoId<SpeciesPrototype>>(Species, ref SpeciesTemp, hookCtx, false, context))
		{
			SpeciesTemp = serialization.CreateCopy<ProtoId<SpeciesPrototype>>(Species, hookCtx, context, false);
		}
		target.Species = SpeciesTemp;
		string VoiceTemp = null;
		if (Voice == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(Voice, ref VoiceTemp, hookCtx, false, context))
		{
			VoiceTemp = Voice;
		}
		target.Voice = VoiceTemp;
		int AgeTemp = 0;
		if (!serialization.TryCustomCopy<int>(Age, ref AgeTemp, hookCtx, false, context))
		{
			AgeTemp = Age;
		}
		target.Age = AgeTemp;
		Sex SexTemp = Sex.Male;
		if (!serialization.TryCustomCopy<Sex>(Sex, ref SexTemp, hookCtx, false, context))
		{
			SexTemp = Sex;
		}
		target.Sex = SexTemp;
		Gender GenderTemp = (Gender)0;
		if (!serialization.TryCustomCopy<Gender>(Gender, ref GenderTemp, hookCtx, false, context))
		{
			GenderTemp = Gender;
		}
		target.Gender = GenderTemp;
		HumanoidCharacterAppearance AppearanceTemp = null;
		if (Appearance == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<HumanoidCharacterAppearance>(Appearance, ref AppearanceTemp, hookCtx, false, context))
		{
			if (Appearance == null)
			{
				AppearanceTemp = null;
			}
			else
			{
				serialization.CopyTo<HumanoidCharacterAppearance>(Appearance, ref AppearanceTemp, hookCtx, context, true);
			}
		}
		target.Appearance = AppearanceTemp;
		SpawnPriorityPreference SpawnPriorityTemp = SpawnPriorityPreference.None;
		if (!serialization.TryCustomCopy<SpawnPriorityPreference>(SpawnPriority, ref SpawnPriorityTemp, hookCtx, false, context))
		{
			SpawnPriorityTemp = SpawnPriority;
		}
		target.SpawnPriority = SpawnPriorityTemp;
		ArmorPreference ArmorPreferenceTemp = ArmorPreference.Random;
		if (!serialization.TryCustomCopy<ArmorPreference>(ArmorPreference, ref ArmorPreferenceTemp, hookCtx, false, context))
		{
			ArmorPreferenceTemp = ArmorPreference;
		}
		target.ArmorPreference = ArmorPreferenceTemp;
		EntProtoId<SquadTeamComponent>? SquadPreferenceTemp = null;
		if (!serialization.TryCustomCopy<EntProtoId<SquadTeamComponent>?>(SquadPreference, ref SquadPreferenceTemp, hookCtx, false, context))
		{
			SquadPreferenceTemp = serialization.CreateCopy<EntProtoId<SquadTeamComponent>?>(SquadPreference, hookCtx, context, false);
		}
		target.SquadPreference = SquadPreferenceTemp;
		PreferenceUnavailableMode PreferenceUnavailableTemp = PreferenceUnavailableMode.StayInLobby;
		if (!serialization.TryCustomCopy<PreferenceUnavailableMode>(PreferenceUnavailable, ref PreferenceUnavailableTemp, hookCtx, false, context))
		{
			PreferenceUnavailableTemp = PreferenceUnavailable;
		}
		target.PreferenceUnavailable = PreferenceUnavailableTemp;
		SharedRMCNamedItems NamedItemsTemp = null;
		if (NamedItems == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SharedRMCNamedItems>(NamedItems, ref NamedItemsTemp, hookCtx, true, context))
		{
			NamedItemsTemp = serialization.CreateCopy<SharedRMCNamedItems>(NamedItems, hookCtx, context, false);
		}
		target.NamedItems = NamedItemsTemp;
		bool PlaytimePerksTemp = false;
		if (!serialization.TryCustomCopy<bool>(PlaytimePerks, ref PlaytimePerksTemp, hookCtx, false, context))
		{
			PlaytimePerksTemp = PlaytimePerks;
		}
		target.PlaytimePerks = PlaytimePerksTemp;
		string XenoPrefixTemp = null;
		if (XenoPrefix == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(XenoPrefix, ref XenoPrefixTemp, hookCtx, false, context))
		{
			XenoPrefixTemp = XenoPrefix;
		}
		target.XenoPrefix = XenoPrefixTemp;
		string XenoPostfixTemp = null;
		if (XenoPostfix == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(XenoPostfix, ref XenoPostfixTemp, hookCtx, false, context))
		{
			XenoPostfixTemp = XenoPostfix;
		}
		target.XenoPostfix = XenoPostfixTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HumanoidCharacterProfile target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HumanoidCharacterProfile cast = (HumanoidCharacterProfile)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public HumanoidCharacterProfile Instantiate()
	{
		return new HumanoidCharacterProfile();
	}
}
