// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.HumanoidCharacterAppearance
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Humanoid;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class HumanoidCharacterAppearance : 
  ICharacterAppearance,
  IEquatable<HumanoidCharacterAppearance>,
  ISerializationGenerated<HumanoidCharacterAppearance>,
  ISerializationGenerated
{
  private static IReadOnlyList<Color> RealisticEyeColors = (IReadOnlyList<Color>) new List<Color>()
  {
    Color.Brown,
    Color.Gray,
    Color.Azure,
    Color.SteelBlue,
    Color.Black
  };

  [DataField("hair", false, 1, false, false, null)]
  public string HairStyleId { get; set; } = (string) HairStyles.DefaultHairStyle;

  [DataField(null, false, 1, false, false, null)]
  public Color HairColor { get; set; } = Color.Black;

  [DataField("facialHair", false, 1, false, false, null)]
  public string FacialHairStyleId { get; set; } = (string) HairStyles.DefaultFacialHairStyle;

  [DataField(null, false, 1, false, false, null)]
  public Color FacialHairColor { get; set; } = Color.Black;

  [DataField(null, false, 1, false, false, null)]
  public Color EyeColor { get; set; } = Color.Black;

  [DataField(null, false, 1, false, false, null)]
  public Color SkinColor { get; set; } = Content.Shared.Humanoid.SkinColor.ValidHumanSkinTone;

  [DataField(null, false, 1, false, false, null)]
  public List<Marking> Markings { get; set; } = new List<Marking>();

  public HumanoidCharacterAppearance(
    string hairStyleId,
    Color hairColor,
    string facialHairStyleId,
    Color facialHairColor,
    Color eyeColor,
    Color skinColor,
    List<Marking> markings)
  {
    this.HairStyleId = hairStyleId;
    this.HairColor = HumanoidCharacterAppearance.ClampColor(hairColor);
    this.FacialHairStyleId = facialHairStyleId;
    this.FacialHairColor = HumanoidCharacterAppearance.ClampColor(facialHairColor);
    this.EyeColor = HumanoidCharacterAppearance.ClampColor(eyeColor);
    this.SkinColor = HumanoidCharacterAppearance.ClampColor(skinColor);
    this.Markings = markings;
  }

  public HumanoidCharacterAppearance(HumanoidCharacterAppearance other)
    : this(other.HairStyleId, other.HairColor, other.FacialHairStyleId, other.FacialHairColor, other.EyeColor, other.SkinColor, new List<Marking>((IEnumerable<Marking>) other.Markings))
  {
  }

  public HumanoidCharacterAppearance WithHairStyleName(string newName)
  {
    return new HumanoidCharacterAppearance(newName, this.HairColor, this.FacialHairStyleId, this.FacialHairColor, this.EyeColor, this.SkinColor, this.Markings);
  }

  public HumanoidCharacterAppearance WithHairColor(Color newColor)
  {
    return new HumanoidCharacterAppearance(this.HairStyleId, newColor, this.FacialHairStyleId, this.FacialHairColor, this.EyeColor, this.SkinColor, this.Markings);
  }

  public HumanoidCharacterAppearance WithFacialHairStyleName(string newName)
  {
    return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, newName, this.FacialHairColor, this.EyeColor, this.SkinColor, this.Markings);
  }

  public HumanoidCharacterAppearance WithFacialHairColor(Color newColor)
  {
    return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, this.FacialHairStyleId, newColor, this.EyeColor, this.SkinColor, this.Markings);
  }

  public HumanoidCharacterAppearance WithEyeColor(Color newColor)
  {
    return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, this.FacialHairStyleId, this.FacialHairColor, newColor, this.SkinColor, this.Markings);
  }

  public HumanoidCharacterAppearance WithSkinColor(Color newColor)
  {
    return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, this.FacialHairStyleId, this.FacialHairColor, this.EyeColor, newColor, this.Markings);
  }

  public HumanoidCharacterAppearance WithMarkings(List<Marking> newMarkings)
  {
    return new HumanoidCharacterAppearance(this.HairStyleId, this.HairColor, this.FacialHairStyleId, this.FacialHairColor, this.EyeColor, this.SkinColor, newMarkings);
  }

  public static HumanoidCharacterAppearance DefaultWithSpecies(string species)
  {
    SpeciesPrototype speciesPrototype = IoCManager.Resolve<IPrototypeManager>().Index<SpeciesPrototype>(species);
    Color color;
    switch (speciesPrototype.SkinColoration)
    {
      case HumanoidSkinColor.HumanToned:
        color = Content.Shared.Humanoid.SkinColor.HumanSkinTone(speciesPrototype.DefaultHumanSkinTone);
        break;
      case HumanoidSkinColor.Hues:
        color = speciesPrototype.DefaultSkinTone;
        break;
      case HumanoidSkinColor.VoxFeathers:
        color = Content.Shared.Humanoid.SkinColor.ClosestVoxColor(speciesPrototype.DefaultSkinTone);
        break;
      case HumanoidSkinColor.TintedHues:
        color = Content.Shared.Humanoid.SkinColor.TintedHues(speciesPrototype.DefaultSkinTone);
        break;
      default:
        color = Content.Shared.Humanoid.SkinColor.ValidHumanSkinTone;
        break;
    }
    Color skinColor = color;
    return new HumanoidCharacterAppearance((string) HairStyles.DefaultHairStyle, Color.Black, (string) HairStyles.DefaultFacialHairStyle, Color.Black, Color.Black, skinColor, new List<Marking>());
  }

  public static HumanoidCharacterAppearance Random(string species, Sex sex)
  {
    IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
    MarkingManager markingManager = IoCManager.Resolve<MarkingManager>();
    List<string> list1 = markingManager.MarkingsByCategoryAndSpecies(MarkingCategories.Hair, species).Keys.ToList<string>();
    List<string> list2 = markingManager.MarkingsByCategoryAndSpecies(MarkingCategories.FacialHair, species).Keys.ToList<string>();
    string hairStyleId = list1.Count > 0 ? RandomExtensions.Pick<string>(random, (IReadOnlyList<string>) list1) : HairStyles.DefaultHairStyle.Id;
    string facialHairStyleId = list2.Count == 0 || sex == Sex.Female ? HairStyles.DefaultFacialHairStyle.Id : RandomExtensions.Pick<string>(random, (IReadOnlyList<string>) list2);
    Color color1 = RandomExtensions.Pick<Color>(random, HairStyles.RealisticHairColors);
    Color color2 = ((Color) ref color1).WithRed(RandomizeColor(color1.R));
    Color color3 = ((Color) ref color2).WithGreen(RandomizeColor(color1.G));
    Color color4 = ((Color) ref color3).WithBlue(RandomizeColor(color1.B));
    Color eyeColor = RandomExtensions.Pick<Color>(random, HumanoidCharacterAppearance.RealisticEyeColors);
    HumanoidSkinColor skinColoration = IoCManager.Resolve<IPrototypeManager>().Index<SpeciesPrototype>(species).SkinColoration;
    Color color5;
    // ISSUE: explicit constructor call
    ((Color) ref color5).\u002Ector(random.NextFloat(1f), random.NextFloat(1f), random.NextFloat(1f), 1f);
    switch (skinColoration)
    {
      case HumanoidSkinColor.HumanToned:
        color5 = Content.Shared.Humanoid.SkinColor.HumanSkinTone(random.Next(0, 101));
        break;
      case HumanoidSkinColor.VoxFeathers:
        color5 = Content.Shared.Humanoid.SkinColor.ProportionalVoxColor(color5);
        break;
      case HumanoidSkinColor.TintedHues:
        color5 = Content.Shared.Humanoid.SkinColor.ValidTintedHuesSkinTone(color5);
        break;
    }
    return new HumanoidCharacterAppearance(hairStyleId, color4, facialHairStyleId, color4, eyeColor, color5, new List<Marking>());

    float RandomizeColor(float channel)
    {
      return MathHelper.Clamp01(channel + (float) random.Next(-25, 25) / 100f);
    }
  }

  public static Color ClampColor(Color color)
  {
    return new Color(((Color) ref color).RByte, ((Color) ref color).GByte, ((Color) ref color).BByte, byte.MaxValue);
  }

  public static HumanoidCharacterAppearance EnsureValid(
    HumanoidCharacterAppearance appearance,
    string species,
    Sex sex)
  {
    string str1 = appearance.HairStyleId;
    string str2 = appearance.FacialHairStyleId;
    Color hairColor = HumanoidCharacterAppearance.ClampColor(appearance.HairColor);
    Color facialHairColor = HumanoidCharacterAppearance.ClampColor(appearance.FacialHairColor);
    Color eyeColor = HumanoidCharacterAppearance.ClampColor(appearance.EyeColor);
    IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
    MarkingManager markingManager = IoCManager.Resolve<MarkingManager>();
    if (!markingManager.MarkingsByCategory(MarkingCategories.Hair).ContainsKey(str1))
      str1 = (string) HairStyles.DefaultHairStyle;
    if (!markingManager.MarkingsByCategory(MarkingCategories.FacialHair).ContainsKey(str2))
      str2 = (string) HairStyles.DefaultFacialHairStyle;
    MarkingSet markingSet = new MarkingSet();
    Color color = appearance.SkinColor;
    SpeciesPrototype prototype;
    if (prototypeManager.TryIndex<SpeciesPrototype>(species, out prototype))
    {
      markingSet = new MarkingSet(appearance.Markings, (string) prototype.MarkingPoints, markingManager, prototypeManager);
      markingSet.EnsureValid(markingManager);
      if (!Content.Shared.Humanoid.SkinColor.VerifySkinColor(prototype.SkinColoration, color))
        color = Content.Shared.Humanoid.SkinColor.ValidSkinTone(prototype.SkinColoration, color);
      markingSet.EnsureSpecies(species, new Color?(color), markingManager);
      markingSet.EnsureSexes(sex, markingManager);
    }
    return new HumanoidCharacterAppearance(str1, hairColor, str2, facialHairColor, eyeColor, color, markingSet.GetForwardEnumerator().ToList<Marking>());
  }

  public bool MemberwiseEquals(ICharacterAppearance maybeOther)
  {
    if (!(maybeOther is HumanoidCharacterAppearance characterAppearance) || this.HairStyleId != characterAppearance.HairStyleId)
      return false;
    Color hairColor = this.HairColor;
    if (!((Color) ref hairColor).Equals(characterAppearance.HairColor) || this.FacialHairStyleId != characterAppearance.FacialHairStyleId)
      return false;
    Color facialHairColor = this.FacialHairColor;
    if (!((Color) ref facialHairColor).Equals(characterAppearance.FacialHairColor))
      return false;
    Color eyeColor = this.EyeColor;
    if (!((Color) ref eyeColor).Equals(characterAppearance.EyeColor))
      return false;
    Color skinColor = this.SkinColor;
    return ((Color) ref skinColor).Equals(characterAppearance.SkinColor) && this.Markings.SequenceEqual<Marking>((IEnumerable<Marking>) characterAppearance.Markings);
  }

  public bool Equals(HumanoidCharacterAppearance? other)
  {
    if (other == null)
      return false;
    if (this == other)
      return true;
    if (this.HairStyleId == other.HairStyleId)
    {
      Color color = this.HairColor;
      if (((Color) ref color).Equals(other.HairColor) && this.FacialHairStyleId == other.FacialHairStyleId)
      {
        color = this.FacialHairColor;
        if (((Color) ref color).Equals(other.FacialHairColor))
        {
          color = this.EyeColor;
          if (((Color) ref color).Equals(other.EyeColor))
          {
            color = this.SkinColor;
            if (((Color) ref color).Equals(other.SkinColor))
              return this.Markings.SequenceEqual<Marking>((IEnumerable<Marking>) other.Markings);
          }
        }
      }
    }
    return false;
  }

  public override bool Equals(object? obj)
  {
    if (this == obj)
      return true;
    return obj is HumanoidCharacterAppearance other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<string, Color, string, Color, Color, Color, List<Marking>>(this.HairStyleId, this.HairColor, this.FacialHairStyleId, this.FacialHairColor, this.EyeColor, this.SkinColor, this.Markings);
  }

  public HumanoidCharacterAppearance Clone() => new HumanoidCharacterAppearance(this);

  public HumanoidCharacterAppearance()
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HumanoidCharacterAppearance target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<HumanoidCharacterAppearance>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.HairStyleId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HairStyleId, ref target1, hookCtx, false, context))
      target1 = this.HairStyleId;
    target.HairStyleId = target1;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.HairColor, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.HairColor, hookCtx, context);
    target.HairColor = target2;
    string target3 = (string) null;
    if (this.FacialHairStyleId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FacialHairStyleId, ref target3, hookCtx, false, context))
      target3 = this.FacialHairStyleId;
    target.FacialHairStyleId = target3;
    Color target4 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.FacialHairColor, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Color>(this.FacialHairColor, hookCtx, context);
    target.FacialHairColor = target4;
    Color target5 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.EyeColor, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Color>(this.EyeColor, hookCtx, context);
    target.EyeColor = target5;
    Color target6 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.SkinColor, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Color>(this.SkinColor, hookCtx, context);
    target.SkinColor = target6;
    List<Marking> target7 = (List<Marking>) null;
    if (this.Markings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Marking>>(this.Markings, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<List<Marking>>(this.Markings, hookCtx, context);
    target.Markings = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HumanoidCharacterAppearance target,
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
    HumanoidCharacterAppearance target1 = (HumanoidCharacterAppearance) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public HumanoidCharacterAppearance Instantiate() => new HumanoidCharacterAppearance();
}
