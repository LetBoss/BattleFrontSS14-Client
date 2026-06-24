using System;
using System.Collections.Generic;
using Content.Shared.Construction.Prototypes;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences;

[Serializable]
[NetSerializable]
public sealed class PlayerPreferences
{
	private Dictionary<int, ICharacterProfile> _characters;

	public IReadOnlyDictionary<int, ICharacterProfile> Characters => _characters;

	public int SelectedCharacterIndex { get; }

	public ICharacterProfile SelectedCharacter => Characters[SelectedCharacterIndex];

	public Color AdminOOCColor { get; set; }

	public List<ProtoId<ConstructionPrototype>> ConstructionFavorites { get; set; } = new List<ProtoId<ConstructionPrototype>>();

	public PlayerPreferences(IEnumerable<KeyValuePair<int, ICharacterProfile>> characters, int selectedCharacterIndex, Color adminOOCColor, List<ProtoId<ConstructionPrototype>> constructionFavorites)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		_characters = new Dictionary<int, ICharacterProfile>(characters);
		SelectedCharacterIndex = selectedCharacterIndex;
		AdminOOCColor = adminOOCColor;
		ConstructionFavorites = constructionFavorites;
	}

	public ICharacterProfile GetProfile(int index)
	{
		return _characters[index];
	}

	public int IndexOfCharacter(ICharacterProfile profile)
	{
		return Extensions.FirstOrNull<KeyValuePair<int, ICharacterProfile>>((IEnumerable<KeyValuePair<int, ICharacterProfile>>)_characters, (Func<KeyValuePair<int, ICharacterProfile>, bool>)((KeyValuePair<int, ICharacterProfile> p) => p.Value == profile))?.Key ?? (-1);
	}

	public bool TryIndexOfCharacter(ICharacterProfile profile, out int index)
	{
		return (index = IndexOfCharacter(profile)) != -1;
	}
}
