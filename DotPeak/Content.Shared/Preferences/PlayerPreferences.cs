// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.PlayerPreferences
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.Prototypes;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Preferences;

[NetSerializable]
[Serializable]
public sealed class PlayerPreferences
{
  private Dictionary<int, ICharacterProfile> _characters;

  public PlayerPreferences(
    IEnumerable<KeyValuePair<int, ICharacterProfile>> characters,
    int selectedCharacterIndex,
    Color adminOOCColor,
    List<ProtoId<ConstructionPrototype>> constructionFavorites)
  {
    this._characters = new Dictionary<int, ICharacterProfile>(characters);
    this.SelectedCharacterIndex = selectedCharacterIndex;
    this.AdminOOCColor = adminOOCColor;
    this.ConstructionFavorites = constructionFavorites;
  }

  public IReadOnlyDictionary<int, ICharacterProfile> Characters
  {
    get => (IReadOnlyDictionary<int, ICharacterProfile>) this._characters;
  }

  public ICharacterProfile GetProfile(int index) => this._characters[index];

  public int SelectedCharacterIndex { get; }

  public ICharacterProfile SelectedCharacter => this.Characters[this.SelectedCharacterIndex];

  public Color AdminOOCColor { get; set; }

  public List<ProtoId<ConstructionPrototype>> ConstructionFavorites { get; set; } = new List<ProtoId<ConstructionPrototype>>();

  public int IndexOfCharacter(ICharacterProfile profile)
  {
    KeyValuePair<int, ICharacterProfile>? nullable = this._characters.FirstOrNull<KeyValuePair<int, ICharacterProfile>>((Func<KeyValuePair<int, ICharacterProfile>, bool>) (p => p.Value == profile));
    ref KeyValuePair<int, ICharacterProfile>? local = ref nullable;
    return !local.HasValue ? -1 : local.GetValueOrDefault().Key;
  }

  public bool TryIndexOfCharacter(ICharacterProfile profile, out int index)
  {
    return (index = this.IndexOfCharacter(profile)) != -1;
  }
}
