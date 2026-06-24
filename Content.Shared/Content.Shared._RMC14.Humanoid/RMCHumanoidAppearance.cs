using System;
using System.Collections.Generic;
using Content.Shared.DisplacementMap;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Robust.Shared.Enums;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared._RMC14.Humanoid;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class RMCHumanoidAppearance : IRMCHumanoidAppearance, ISerializationGenerated<RMCHumanoidAppearance>, ISerializationGenerated
{
	public MarkingSet ClientOldMarkings { get; set; } = new MarkingSet();

	[DataField(null, false, 1, false, false, null)]
	public MarkingSet MarkingSet { get; set; } = new MarkingSet();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> BaseLayers { get; set; } = new Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<HumanoidVisualLayers> PermanentlyHidden { get; set; } = new HashSet<HumanoidVisualLayers>();

	[DataField(null, false, 1, false, false, null)]
	public Gender Gender { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public int Age { get; set; } = 18;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers { get; set; } = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>();

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<SpeciesPrototype> Species { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<HumanoidProfilePrototype>? Initial { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public Color SkinColor { get; set; } = Color.FromHex((ReadOnlySpan<char>)"#C0967F", (Color?)null);

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<HumanoidVisualLayers, SlotFlags> HiddenLayers { get; set; } = new Dictionary<HumanoidVisualLayers, SlotFlags>();

	[DataField(null, false, 1, false, false, null)]
	public Sex Sex { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public Color EyeColor { get; set; } = Color.Brown;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Color? CachedHairColor { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Color? CachedFacialHairColor { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public HashSet<HumanoidVisualLayers> HideLayersOnEquip { get; set; } = new HashSet<HumanoidVisualLayers> { HumanoidVisualLayers.Hair };

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<MarkingPrototype>? UndergarmentTop { get; set; } = new ProtoId<MarkingPrototype>("UndergarmentTopTanktop");

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<MarkingPrototype>? UndergarmentBottom { get; set; } = new ProtoId<MarkingPrototype>("UndergarmentBottomBoxers");

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<HumanoidVisualLayers, DisplacementData> MarkingsDisplacement { get; set; } = new Dictionary<HumanoidVisualLayers, DisplacementData>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCHumanoidAppearance target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<RMCHumanoidAppearance>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		MarkingSet MarkingSetTemp = null;
		if (MarkingSet == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<MarkingSet>(MarkingSet, ref MarkingSetTemp, hookCtx, false, context))
		{
			if (MarkingSet == null)
			{
				MarkingSetTemp = null;
			}
			else
			{
				serialization.CopyTo<MarkingSet>(MarkingSet, ref MarkingSetTemp, hookCtx, context, true);
			}
		}
		target.MarkingSet = MarkingSetTemp;
		Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> BaseLayersTemp = null;
		if (BaseLayers == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>>(BaseLayers, ref BaseLayersTemp, hookCtx, true, context))
		{
			BaseLayersTemp = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>>(BaseLayers, hookCtx, context, false);
		}
		target.BaseLayers = BaseLayersTemp;
		HashSet<HumanoidVisualLayers> PermanentlyHiddenTemp = null;
		if (PermanentlyHidden == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(PermanentlyHidden, ref PermanentlyHiddenTemp, hookCtx, true, context))
		{
			PermanentlyHiddenTemp = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(PermanentlyHidden, hookCtx, context, false);
		}
		target.PermanentlyHidden = PermanentlyHiddenTemp;
		Gender GenderTemp = (Gender)0;
		if (!serialization.TryCustomCopy<Gender>(Gender, ref GenderTemp, hookCtx, false, context))
		{
			GenderTemp = Gender;
		}
		target.Gender = GenderTemp;
		int AgeTemp = 0;
		if (!serialization.TryCustomCopy<int>(Age, ref AgeTemp, hookCtx, false, context))
		{
			AgeTemp = Age;
		}
		target.Age = AgeTemp;
		Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayersTemp = null;
		if (CustomBaseLayers == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(CustomBaseLayers, ref CustomBaseLayersTemp, hookCtx, true, context))
		{
			CustomBaseLayersTemp = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(CustomBaseLayers, hookCtx, context, false);
		}
		target.CustomBaseLayers = CustomBaseLayersTemp;
		ProtoId<SpeciesPrototype> SpeciesTemp = default(ProtoId<SpeciesPrototype>);
		if (!serialization.TryCustomCopy<ProtoId<SpeciesPrototype>>(Species, ref SpeciesTemp, hookCtx, false, context))
		{
			SpeciesTemp = serialization.CreateCopy<ProtoId<SpeciesPrototype>>(Species, hookCtx, context, false);
		}
		target.Species = SpeciesTemp;
		ProtoId<HumanoidProfilePrototype>? InitialTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<HumanoidProfilePrototype>?>(Initial, ref InitialTemp, hookCtx, false, context))
		{
			InitialTemp = serialization.CreateCopy<ProtoId<HumanoidProfilePrototype>?>(Initial, hookCtx, context, false);
		}
		target.Initial = InitialTemp;
		Color SkinColorTemp = default(Color);
		if (!serialization.TryCustomCopy<Color>(SkinColor, ref SkinColorTemp, hookCtx, false, context))
		{
			SkinColorTemp = serialization.CreateCopy<Color>(SkinColor, hookCtx, context, false);
		}
		target.SkinColor = SkinColorTemp;
		Dictionary<HumanoidVisualLayers, SlotFlags> HiddenLayersTemp = null;
		if (HiddenLayers == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(HiddenLayers, ref HiddenLayersTemp, hookCtx, true, context))
		{
			HiddenLayersTemp = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(HiddenLayers, hookCtx, context, false);
		}
		target.HiddenLayers = HiddenLayersTemp;
		Sex SexTemp = Sex.Male;
		if (!serialization.TryCustomCopy<Sex>(Sex, ref SexTemp, hookCtx, false, context))
		{
			SexTemp = Sex;
		}
		target.Sex = SexTemp;
		Color EyeColorTemp = default(Color);
		if (!serialization.TryCustomCopy<Color>(EyeColor, ref EyeColorTemp, hookCtx, false, context))
		{
			EyeColorTemp = serialization.CreateCopy<Color>(EyeColor, hookCtx, context, false);
		}
		target.EyeColor = EyeColorTemp;
		HashSet<HumanoidVisualLayers> HideLayersOnEquipTemp = null;
		if (HideLayersOnEquip == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(HideLayersOnEquip, ref HideLayersOnEquipTemp, hookCtx, true, context))
		{
			HideLayersOnEquipTemp = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(HideLayersOnEquip, hookCtx, context, false);
		}
		target.HideLayersOnEquip = HideLayersOnEquipTemp;
		ProtoId<MarkingPrototype>? UndergarmentTopTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<MarkingPrototype>?>(UndergarmentTop, ref UndergarmentTopTemp, hookCtx, false, context))
		{
			UndergarmentTopTemp = serialization.CreateCopy<ProtoId<MarkingPrototype>?>(UndergarmentTop, hookCtx, context, false);
		}
		target.UndergarmentTop = UndergarmentTopTemp;
		ProtoId<MarkingPrototype>? UndergarmentBottomTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<MarkingPrototype>?>(UndergarmentBottom, ref UndergarmentBottomTemp, hookCtx, false, context))
		{
			UndergarmentBottomTemp = serialization.CreateCopy<ProtoId<MarkingPrototype>?>(UndergarmentBottom, hookCtx, context, false);
		}
		target.UndergarmentBottom = UndergarmentBottomTemp;
		Dictionary<HumanoidVisualLayers, DisplacementData> MarkingsDisplacementTemp = null;
		if (MarkingsDisplacement == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, DisplacementData>>(MarkingsDisplacement, ref MarkingsDisplacementTemp, hookCtx, true, context))
		{
			MarkingsDisplacementTemp = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, DisplacementData>>(MarkingsDisplacement, hookCtx, context, false);
		}
		target.MarkingsDisplacement = MarkingsDisplacementTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCHumanoidAppearance target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCHumanoidAppearance cast = (RMCHumanoidAppearance)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCHumanoidAppearance Instantiate()
	{
		return new RMCHumanoidAppearance();
	}
}
