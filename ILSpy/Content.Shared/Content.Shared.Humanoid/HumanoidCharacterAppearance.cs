using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Content.Shared.Humanoid;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class HumanoidCharacterAppearance : ICharacterAppearance, IEquatable<HumanoidCharacterAppearance>, ISerializationGenerated<HumanoidCharacterAppearance>, ISerializationGenerated
{
	private static IReadOnlyList<Color> RealisticEyeColors = new List<Color>
	{
		Color.Brown,
		Color.Gray,
		Color.Azure,
		Color.SteelBlue,
		Color.Black
	};

	[DataField("hair", false, 1, false, false, null)]
	public string HairStyleId { get; set; } = ProtoId<MarkingPrototype>.op_Implicit(HairStyles.DefaultHairStyle);

	[DataField(null, false, 1, false, false, null)]
	public Color HairColor { get; set; } = Color.Black;

	[DataField("facialHair", false, 1, false, false, null)]
	public string FacialHairStyleId { get; set; } = ProtoId<MarkingPrototype>.op_Implicit(HairStyles.DefaultFacialHairStyle);

	[DataField(null, false, 1, false, false, null)]
	public Color FacialHairColor { get; set; } = Color.Black;

	[DataField(null, false, 1, false, false, null)]
	public Color EyeColor { get; set; } = Color.Black;

	[DataField(null, false, 1, false, false, null)]
	public Color SkinColor { get; set; } = Content.Shared.Humanoid.SkinColor.ValidHumanSkinTone;

	[DataField(null, false, 1, false, false, null)]
	public List<Marking> Markings { get; set; } = new List<Marking>();

	public HumanoidCharacterAppearance(string hairStyleId, Color hairColor, string facialHairStyleId, Color facialHairColor, Color eyeColor, Color skinColor, List<Marking> markings)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		HairStyleId = hairStyleId;
		HairColor = ClampColor(hairColor);
		FacialHairStyleId = facialHairStyleId;
		FacialHairColor = ClampColor(facialHairColor);
		EyeColor = ClampColor(eyeColor);
		SkinColor = ClampColor(skinColor);
		Markings = markings;
	}

	public HumanoidCharacterAppearance(HumanoidCharacterAppearance other)
		: this(other.HairStyleId, other.HairColor, other.FacialHairStyleId, other.FacialHairColor, other.EyeColor, other.SkinColor, new List<Marking>(other.Markings))
	{
	}//IL_0008: Unknown result type (might be due to invalid IL or missing references)
	//IL_0014: Unknown result type (might be due to invalid IL or missing references)
	//IL_001a: Unknown result type (might be due to invalid IL or missing references)
	//IL_0020: Unknown result type (might be due to invalid IL or missing references)


	public HumanoidCharacterAppearance WithHairStyleName(string newName)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new HumanoidCharacterAppearance(newName, HairColor, FacialHairStyleId, FacialHairColor, EyeColor, SkinColor, Markings);
	}

	public HumanoidCharacterAppearance WithHairColor(Color newColor)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new HumanoidCharacterAppearance(HairStyleId, newColor, FacialHairStyleId, FacialHairColor, EyeColor, SkinColor, Markings);
	}

	public HumanoidCharacterAppearance WithFacialHairStyleName(string newName)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new HumanoidCharacterAppearance(HairStyleId, HairColor, newName, FacialHairColor, EyeColor, SkinColor, Markings);
	}

	public HumanoidCharacterAppearance WithFacialHairColor(Color newColor)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new HumanoidCharacterAppearance(HairStyleId, HairColor, FacialHairStyleId, newColor, EyeColor, SkinColor, Markings);
	}

	public HumanoidCharacterAppearance WithEyeColor(Color newColor)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new HumanoidCharacterAppearance(HairStyleId, HairColor, FacialHairStyleId, FacialHairColor, newColor, SkinColor, Markings);
	}

	public HumanoidCharacterAppearance WithSkinColor(Color newColor)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new HumanoidCharacterAppearance(HairStyleId, HairColor, FacialHairStyleId, FacialHairColor, EyeColor, newColor, Markings);
	}

	public HumanoidCharacterAppearance WithMarkings(List<Marking> newMarkings)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return new HumanoidCharacterAppearance(HairStyleId, HairColor, FacialHairStyleId, FacialHairColor, EyeColor, SkinColor, newMarkings);
	}

	public static HumanoidCharacterAppearance DefaultWithSpecies(string species)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		SpeciesPrototype speciesPrototype = IoCManager.Resolve<IPrototypeManager>().Index<SpeciesPrototype>(species);
		Color skinColor = (Color)(speciesPrototype.SkinColoration switch
		{
			HumanoidSkinColor.HumanToned => Content.Shared.Humanoid.SkinColor.HumanSkinTone(speciesPrototype.DefaultHumanSkinTone), 
			HumanoidSkinColor.Hues => speciesPrototype.DefaultSkinTone, 
			HumanoidSkinColor.TintedHues => Content.Shared.Humanoid.SkinColor.TintedHues(speciesPrototype.DefaultSkinTone), 
			HumanoidSkinColor.VoxFeathers => Content.Shared.Humanoid.SkinColor.ClosestVoxColor(speciesPrototype.DefaultSkinTone), 
			_ => Content.Shared.Humanoid.SkinColor.ValidHumanSkinTone, 
		});
		return new HumanoidCharacterAppearance(ProtoId<MarkingPrototype>.op_Implicit(HairStyles.DefaultHairStyle), Color.Black, ProtoId<MarkingPrototype>.op_Implicit(HairStyles.DefaultFacialHairStyle), Color.Black, Color.Black, skinColor, new List<Marking>());
	}

	public static HumanoidCharacterAppearance Random(string species, Sex sex)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
		MarkingManager markingManager = IoCManager.Resolve<MarkingManager>();
		List<string> hairStyles = markingManager.MarkingsByCategoryAndSpecies(MarkingCategories.Hair, species).Keys.ToList();
		List<string> facialHairStyles = markingManager.MarkingsByCategoryAndSpecies(MarkingCategories.FacialHair, species).Keys.ToList();
		string newHairStyle = ((hairStyles.Count > 0) ? RandomExtensions.Pick<string>(random, (IReadOnlyList<string>)hairStyles) : HairStyles.DefaultHairStyle.Id);
		string newFacialHairStyle = ((facialHairStyles.Count == 0 || sex == Sex.Female) ? HairStyles.DefaultFacialHairStyle.Id : RandomExtensions.Pick<string>(random, (IReadOnlyList<string>)facialHairStyles));
		Color newHairColor = RandomExtensions.Pick<Color>(random, HairStyles.RealisticHairColors);
		Color val = ((Color)(ref newHairColor)).WithRed(RandomizeColor(newHairColor.R));
		val = ((Color)(ref val)).WithGreen(RandomizeColor(newHairColor.G));
		newHairColor = ((Color)(ref val)).WithBlue(RandomizeColor(newHairColor.B));
		Color newEyeColor = RandomExtensions.Pick<Color>(random, RealisticEyeColors);
		HumanoidSkinColor skinType = IoCManager.Resolve<IPrototypeManager>().Index<SpeciesPrototype>(species).SkinColoration;
		Color newSkinColor = default(Color);
		((Color)(ref newSkinColor))._002Ector(random.NextFloat(1f), random.NextFloat(1f), random.NextFloat(1f), 1f);
		switch (skinType)
		{
		case HumanoidSkinColor.HumanToned:
			newSkinColor = Content.Shared.Humanoid.SkinColor.HumanSkinTone(random.Next(0, 101));
			break;
		case HumanoidSkinColor.TintedHues:
			newSkinColor = Content.Shared.Humanoid.SkinColor.ValidTintedHuesSkinTone(newSkinColor);
			break;
		case HumanoidSkinColor.VoxFeathers:
			newSkinColor = Content.Shared.Humanoid.SkinColor.ProportionalVoxColor(newSkinColor);
			break;
		}
		return new HumanoidCharacterAppearance(newHairStyle, newHairColor, newFacialHairStyle, newHairColor, newEyeColor, newSkinColor, new List<Marking>());
		float RandomizeColor(float channel)
		{
			return MathHelper.Clamp01(channel + (float)random.Next(-25, 25) / 100f);
		}
	}

	public static Color ClampColor(Color color)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new Color(((Color)(ref color)).RByte, ((Color)(ref color)).GByte, ((Color)(ref color)).BByte, byte.MaxValue);
	}

	public static HumanoidCharacterAppearance EnsureValid(HumanoidCharacterAppearance appearance, string species, Sex sex)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		string hairStyleId = appearance.HairStyleId;
		string facialHairStyleId = appearance.FacialHairStyleId;
		Color hairColor = ClampColor(appearance.HairColor);
		Color facialHairColor = ClampColor(appearance.FacialHairColor);
		Color eyeColor = ClampColor(appearance.EyeColor);
		IPrototypeManager proto = IoCManager.Resolve<IPrototypeManager>();
		MarkingManager markingManager = IoCManager.Resolve<MarkingManager>();
		if (!markingManager.MarkingsByCategory(MarkingCategories.Hair).ContainsKey(hairStyleId))
		{
			hairStyleId = ProtoId<MarkingPrototype>.op_Implicit(HairStyles.DefaultHairStyle);
		}
		if (!markingManager.MarkingsByCategory(MarkingCategories.FacialHair).ContainsKey(facialHairStyleId))
		{
			facialHairStyleId = ProtoId<MarkingPrototype>.op_Implicit(HairStyles.DefaultFacialHairStyle);
		}
		MarkingSet markingSet = new MarkingSet();
		Color skinColor = appearance.SkinColor;
		SpeciesPrototype speciesProto = default(SpeciesPrototype);
		if (proto.TryIndex<SpeciesPrototype>(species, ref speciesProto))
		{
			markingSet = new MarkingSet(appearance.Markings, ProtoId<MarkingPointsPrototype>.op_Implicit(speciesProto.MarkingPoints), markingManager, proto);
			markingSet.EnsureValid(markingManager);
			if (!Content.Shared.Humanoid.SkinColor.VerifySkinColor(speciesProto.SkinColoration, skinColor))
			{
				skinColor = Content.Shared.Humanoid.SkinColor.ValidSkinTone(speciesProto.SkinColoration, skinColor);
			}
			markingSet.EnsureSpecies(species, skinColor, markingManager);
			markingSet.EnsureSexes(sex, markingManager);
		}
		return new HumanoidCharacterAppearance(hairStyleId, hairColor, facialHairStyleId, facialHairColor, eyeColor, skinColor, markingSet.GetForwardEnumerator().ToList());
	}

	public bool MemberwiseEquals(ICharacterAppearance maybeOther)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (!(maybeOther is HumanoidCharacterAppearance other))
		{
			return false;
		}
		if (HairStyleId != other.HairStyleId)
		{
			return false;
		}
		Color val = HairColor;
		if (!((Color)(ref val)).Equals(other.HairColor))
		{
			return false;
		}
		if (FacialHairStyleId != other.FacialHairStyleId)
		{
			return false;
		}
		val = FacialHairColor;
		if (!((Color)(ref val)).Equals(other.FacialHairColor))
		{
			return false;
		}
		val = EyeColor;
		if (!((Color)(ref val)).Equals(other.EyeColor))
		{
			return false;
		}
		val = SkinColor;
		if (!((Color)(ref val)).Equals(other.SkinColor))
		{
			return false;
		}
		if (!Markings.SequenceEqual(other.Markings))
		{
			return false;
		}
		return true;
	}

	public bool Equals(HumanoidCharacterAppearance? other)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (HairStyleId == other.HairStyleId)
		{
			Color val = HairColor;
			if (((Color)(ref val)).Equals(other.HairColor) && FacialHairStyleId == other.FacialHairStyleId)
			{
				val = FacialHairColor;
				if (((Color)(ref val)).Equals(other.FacialHairColor))
				{
					val = EyeColor;
					if (((Color)(ref val)).Equals(other.EyeColor))
					{
						val = SkinColor;
						if (((Color)(ref val)).Equals(other.SkinColor))
						{
							return Markings.SequenceEqual(other.Markings);
						}
					}
				}
			}
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is HumanoidCharacterAppearance other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return HashCode.Combine<string, Color, string, Color, Color, Color, List<Marking>>(HairStyleId, HairColor, FacialHairStyleId, FacialHairColor, EyeColor, SkinColor, Markings);
	}

	public HumanoidCharacterAppearance Clone()
	{
		return new HumanoidCharacterAppearance(this);
	}

	public HumanoidCharacterAppearance()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	//IL_0016: Unknown result type (might be due to invalid IL or missing references)
	//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	//IL_002c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0031: Unknown result type (might be due to invalid IL or missing references)
	//IL_0037: Unknown result type (might be due to invalid IL or missing references)
	//IL_003c: Unknown result type (might be due to invalid IL or missing references)
	//IL_0042: Unknown result type (might be due to invalid IL or missing references)
	//IL_0047: Unknown result type (might be due to invalid IL or missing references)


	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HumanoidCharacterAppearance target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<HumanoidCharacterAppearance>(this, ref target, hookCtx, false, context))
		{
			string HairStyleIdTemp = null;
			if (HairStyleId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(HairStyleId, ref HairStyleIdTemp, hookCtx, false, context))
			{
				HairStyleIdTemp = HairStyleId;
			}
			target.HairStyleId = HairStyleIdTemp;
			Color HairColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(HairColor, ref HairColorTemp, hookCtx, false, context))
			{
				HairColorTemp = serialization.CreateCopy<Color>(HairColor, hookCtx, context, false);
			}
			target.HairColor = HairColorTemp;
			string FacialHairStyleIdTemp = null;
			if (FacialHairStyleId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FacialHairStyleId, ref FacialHairStyleIdTemp, hookCtx, false, context))
			{
				FacialHairStyleIdTemp = FacialHairStyleId;
			}
			target.FacialHairStyleId = FacialHairStyleIdTemp;
			Color FacialHairColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(FacialHairColor, ref FacialHairColorTemp, hookCtx, false, context))
			{
				FacialHairColorTemp = serialization.CreateCopy<Color>(FacialHairColor, hookCtx, context, false);
			}
			target.FacialHairColor = FacialHairColorTemp;
			Color EyeColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(EyeColor, ref EyeColorTemp, hookCtx, false, context))
			{
				EyeColorTemp = serialization.CreateCopy<Color>(EyeColor, hookCtx, context, false);
			}
			target.EyeColor = EyeColorTemp;
			Color SkinColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(SkinColor, ref SkinColorTemp, hookCtx, false, context))
			{
				SkinColorTemp = serialization.CreateCopy<Color>(SkinColor, hookCtx, context, false);
			}
			target.SkinColor = SkinColorTemp;
			List<Marking> MarkingsTemp = null;
			if (Markings == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<Marking>>(Markings, ref MarkingsTemp, hookCtx, true, context))
			{
				MarkingsTemp = serialization.CreateCopy<List<Marking>>(Markings, hookCtx, context, false);
			}
			target.Markings = MarkingsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HumanoidCharacterAppearance target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HumanoidCharacterAppearance cast = (HumanoidCharacterAppearance)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public HumanoidCharacterAppearance Instantiate()
	{
		return new HumanoidCharacterAppearance();
	}
}
