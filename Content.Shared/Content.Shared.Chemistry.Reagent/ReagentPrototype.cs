using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using Content.Shared._RMC14.Prototypes;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Database;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition;
using Content.Shared.Slippery;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Chemistry.Reagent;

[Prototype(null, 1)]
[DataDefinition]
[Virtual]
public class ReagentPrototype : IPrototype, IInheritingPrototype, ICMSpecific, ISerializationGenerated<ReagentPrototype>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Recognizable;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<FlavorPrototype>? Flavor;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 FlavorMinimum = FixedPoint2.New(0.1f);

	[DataField(null, false, 1, false, false, null)]
	public SlipperyEffectEntry? SlipData;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 EvaporationSpeed = FixedPoint2.Zero;

	[DataField(null, false, 1, false, false, null)]
	public bool Absorbent;

	[DataField(null, false, 1, false, false, null)]
	public float Fizziness;

	[DataField(null, false, 1, false, false, null)]
	public float Viscosity;

	[DataField(null, false, 1, false, false, null)]
	public float Friction = 1f;

	[DataField(null, false, 1, false, false, null)]
	public bool WorksOnTheDead;

	[DataField(null, false, 1, false, false, null)]
	public FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry>? Metabolisms;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<ReactiveGroupPrototype>, ReactiveReagentEffectEntry>? ReactiveEffects;

	[DataField(null, false, 1, false, true, null)]
	public List<ITileReaction> TileReactions = new List<ITileReaction>(0);

	[DataField("plantMetabolism", false, 1, false, false, null)]
	public List<EntityEffect> PlantMetabolisms = new List<EntityEffect>(0);

	[DataField(null, false, 1, false, false, null)]
	public float PricePerUnit;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier FootstepSound = (SoundSpecifier)new SoundCollectionSpecifier("FootstepPuddle", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public bool Unknown;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? Overdose;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? CriticalOverdose;

	[DataField(null, false, 1, false, false, null)]
	public int Intensity;

	[DataField(null, false, 1, false, false, null)]
	public int Duration;

	[DataField(null, false, 1, false, false, null)]
	public int Radius;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId FireEntity = EntProtoId.op_Implicit("RMCTileFire");

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 IntensityMod;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 DurationMod;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 RadiusMod;

	[DataField(null, false, 1, false, false, null)]
	public bool FireSpread;

	[DataField(null, false, 1, false, false, null)]
	public bool Toxin;

	[DataField(null, false, 1, false, false, null)]
	public bool Alcohol;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, true, false, null)]
	private LocId Name { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LocalizedName => Loc.GetString(LocId.op_Implicit(Name));

	[DataField(null, false, 1, false, false, null)]
	public string Group { get; private set; } = "Unknown";

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<ReagentPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[DataField("desc", false, 1, true, false, null)]
	private LocId Description { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LocalizedDescription => Loc.GetString(LocId.op_Implicit(Description));

	[DataField("physicalDesc", false, 1, true, false, null)]
	private LocId PhysicalDescription { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LocalizedPhysicalDescription => Loc.GetString(LocId.op_Implicit(PhysicalDescription));

	[DataField("color", false, 1, false, false, null)]
	public Color SubstanceColor { get; private set; } = Color.White;

	[DataField(null, false, 1, false, false, null)]
	public float SpecificHeat { get; private set; } = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float? BoilingPoint { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public float? MeltingPoint { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? MetamorphicSprite { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public int MetamorphicMaxFillLevels { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string? MetamorphicFillBaseName { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool MetamorphicChangeColor { get; private set; } = true;

	[DataField(null, false, 1, false, false, null)]
	public bool IsCM { get; set; }

	public FixedPoint2 ReactionTile(TileRef tile, FixedPoint2 reactVolume, IEntityManager entityManager, List<ReagentData>? data)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 removed = FixedPoint2.Zero;
		if (((Tile)(ref tile.Tile)).IsEmpty)
		{
			return removed;
		}
		foreach (ITileReaction reaction in TileReactions)
		{
			removed += reaction.TileReact(tile, this, reactVolume - removed, entityManager, data);
			if (removed > reactVolume)
			{
				throw new Exception("Removed more than we have!");
			}
			if (removed == reactVolume)
			{
				break;
			}
		}
		return removed;
	}

	public void ReactionPlant(EntityUid? plantHolder, ReagentQuantity amount, Solution solution)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (!plantHolder.HasValue)
		{
			return;
		}
		IEntityManager entMan = IoCManager.Resolve<IEntityManager>();
		IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
		EntityEffectReagentArgs args = new EntityEffectReagentArgs(plantHolder.Value, entMan, null, solution, amount.Quantity, this, null, 1f);
		foreach (EntityEffect plantMetabolizable in PlantMetabolisms)
		{
			if (plantMetabolizable.ShouldApply(args, random))
			{
				if (plantMetabolizable.ShouldLog)
				{
					EntityUid entity = args.TargetEntity;
					SharedAdminLogSystem sharedAdminLogSystem = entMan.System<SharedAdminLogSystem>();
					LogImpact logImpact = plantMetabolizable.LogImpact;
					LogStringHandler handler = new LogStringHandler(59, 4);
					handler.AppendLiteral("Plant metabolism effect ");
					handler.AppendFormatted(plantMetabolizable.GetType().Name, 0, "effect");
					handler.AppendLiteral(" of reagent ");
					handler.AppendFormatted(ID, 0, "reagent");
					handler.AppendLiteral(" applied on entity ");
					handler.AppendFormatted<EntityStringRepresentation>(entMan.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(entity)), "entity", "entMan.ToPrettyString(entity)");
					handler.AppendLiteral(" at ");
					handler.AppendFormatted<EntityCoordinates>(entMan.GetComponent<TransformComponent>(entity).Coordinates, "coordinates", "entMan.GetComponent<TransformComponent>(entity).Coordinates");
					sharedAdminLogSystem.Add(LogType.ReagentEffect, logImpact, ref handler);
				}
				plantMetabolizable.Effect(args);
			}
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref ReagentPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<ReagentPrototype>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string IDTemp = null;
		if (ID == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(ID, ref IDTemp, hookCtx, false, context))
		{
			IDTemp = ID;
		}
		target.ID = IDTemp;
		LocId NameTemp = default(LocId);
		if (!serialization.TryCustomCopy<LocId>(Name, ref NameTemp, hookCtx, false, context))
		{
			NameTemp = serialization.CreateCopy<LocId>(Name, hookCtx, context, false);
		}
		target.Name = NameTemp;
		string GroupTemp = null;
		if (Group == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(Group, ref GroupTemp, hookCtx, false, context))
		{
			GroupTemp = Group;
		}
		target.Group = GroupTemp;
		string[] ParentsTemp = null;
		if (!serialization.TryCustomCopy<string[]>(Parents, ref ParentsTemp, hookCtx, true, context))
		{
			ParentsTemp = serialization.CreateCopy<string[]>(Parents, hookCtx, context, false);
		}
		target.Parents = ParentsTemp;
		bool AbstractTemp = false;
		if (!serialization.TryCustomCopy<bool>(Abstract, ref AbstractTemp, hookCtx, false, context))
		{
			AbstractTemp = Abstract;
		}
		target.Abstract = AbstractTemp;
		LocId DescriptionTemp = default(LocId);
		if (!serialization.TryCustomCopy<LocId>(Description, ref DescriptionTemp, hookCtx, false, context))
		{
			DescriptionTemp = serialization.CreateCopy<LocId>(Description, hookCtx, context, false);
		}
		target.Description = DescriptionTemp;
		LocId PhysicalDescriptionTemp = default(LocId);
		if (!serialization.TryCustomCopy<LocId>(PhysicalDescription, ref PhysicalDescriptionTemp, hookCtx, false, context))
		{
			PhysicalDescriptionTemp = serialization.CreateCopy<LocId>(PhysicalDescription, hookCtx, context, false);
		}
		target.PhysicalDescription = PhysicalDescriptionTemp;
		bool RecognizableTemp = false;
		if (!serialization.TryCustomCopy<bool>(Recognizable, ref RecognizableTemp, hookCtx, false, context))
		{
			RecognizableTemp = Recognizable;
		}
		target.Recognizable = RecognizableTemp;
		ProtoId<FlavorPrototype>? FlavorTemp = null;
		if (!serialization.TryCustomCopy<ProtoId<FlavorPrototype>?>(Flavor, ref FlavorTemp, hookCtx, false, context))
		{
			FlavorTemp = serialization.CreateCopy<ProtoId<FlavorPrototype>?>(Flavor, hookCtx, context, false);
		}
		target.Flavor = FlavorTemp;
		FixedPoint2 FlavorMinimumTemp = default(FixedPoint2);
		if (!serialization.TryCustomCopy<FixedPoint2>(FlavorMinimum, ref FlavorMinimumTemp, hookCtx, false, context))
		{
			FlavorMinimumTemp = serialization.CreateCopy<FixedPoint2>(FlavorMinimum, hookCtx, context, false);
		}
		target.FlavorMinimum = FlavorMinimumTemp;
		Color SubstanceColorTemp = default(Color);
		if (!serialization.TryCustomCopy<Color>(SubstanceColor, ref SubstanceColorTemp, hookCtx, false, context))
		{
			SubstanceColorTemp = serialization.CreateCopy<Color>(SubstanceColor, hookCtx, context, false);
		}
		target.SubstanceColor = SubstanceColorTemp;
		float SpecificHeatTemp = 0f;
		if (!serialization.TryCustomCopy<float>(SpecificHeat, ref SpecificHeatTemp, hookCtx, false, context))
		{
			SpecificHeatTemp = SpecificHeat;
		}
		target.SpecificHeat = SpecificHeatTemp;
		float? BoilingPointTemp = null;
		if (!serialization.TryCustomCopy<float?>(BoilingPoint, ref BoilingPointTemp, hookCtx, false, context))
		{
			BoilingPointTemp = BoilingPoint;
		}
		target.BoilingPoint = BoilingPointTemp;
		float? MeltingPointTemp = null;
		if (!serialization.TryCustomCopy<float?>(MeltingPoint, ref MeltingPointTemp, hookCtx, false, context))
		{
			MeltingPointTemp = MeltingPoint;
		}
		target.MeltingPoint = MeltingPointTemp;
		SpriteSpecifier MetamorphicSpriteTemp = null;
		if (!serialization.TryCustomCopy<SpriteSpecifier>(MetamorphicSprite, ref MetamorphicSpriteTemp, hookCtx, true, context))
		{
			MetamorphicSpriteTemp = serialization.CreateCopy<SpriteSpecifier>(MetamorphicSprite, hookCtx, context, false);
		}
		target.MetamorphicSprite = MetamorphicSpriteTemp;
		int MetamorphicMaxFillLevelsTemp = 0;
		if (!serialization.TryCustomCopy<int>(MetamorphicMaxFillLevels, ref MetamorphicMaxFillLevelsTemp, hookCtx, false, context))
		{
			MetamorphicMaxFillLevelsTemp = MetamorphicMaxFillLevels;
		}
		target.MetamorphicMaxFillLevels = MetamorphicMaxFillLevelsTemp;
		string MetamorphicFillBaseNameTemp = null;
		if (!serialization.TryCustomCopy<string>(MetamorphicFillBaseName, ref MetamorphicFillBaseNameTemp, hookCtx, false, context))
		{
			MetamorphicFillBaseNameTemp = MetamorphicFillBaseName;
		}
		target.MetamorphicFillBaseName = MetamorphicFillBaseNameTemp;
		bool MetamorphicChangeColorTemp = false;
		if (!serialization.TryCustomCopy<bool>(MetamorphicChangeColor, ref MetamorphicChangeColorTemp, hookCtx, false, context))
		{
			MetamorphicChangeColorTemp = MetamorphicChangeColor;
		}
		target.MetamorphicChangeColor = MetamorphicChangeColorTemp;
		SlipperyEffectEntry SlipDataTemp = null;
		if (!serialization.TryCustomCopy<SlipperyEffectEntry>(SlipData, ref SlipDataTemp, hookCtx, false, context))
		{
			if (SlipData == null)
			{
				SlipDataTemp = null;
			}
			else
			{
				serialization.CopyTo<SlipperyEffectEntry>(SlipData, ref SlipDataTemp, hookCtx, context, false);
			}
		}
		target.SlipData = SlipDataTemp;
		FixedPoint2 EvaporationSpeedTemp = default(FixedPoint2);
		if (!serialization.TryCustomCopy<FixedPoint2>(EvaporationSpeed, ref EvaporationSpeedTemp, hookCtx, false, context))
		{
			EvaporationSpeedTemp = serialization.CreateCopy<FixedPoint2>(EvaporationSpeed, hookCtx, context, false);
		}
		target.EvaporationSpeed = EvaporationSpeedTemp;
		bool AbsorbentTemp = false;
		if (!serialization.TryCustomCopy<bool>(Absorbent, ref AbsorbentTemp, hookCtx, false, context))
		{
			AbsorbentTemp = Absorbent;
		}
		target.Absorbent = AbsorbentTemp;
		float FizzinessTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Fizziness, ref FizzinessTemp, hookCtx, false, context))
		{
			FizzinessTemp = Fizziness;
		}
		target.Fizziness = FizzinessTemp;
		float ViscosityTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Viscosity, ref ViscosityTemp, hookCtx, false, context))
		{
			ViscosityTemp = Viscosity;
		}
		target.Viscosity = ViscosityTemp;
		float FrictionTemp = 0f;
		if (!serialization.TryCustomCopy<float>(Friction, ref FrictionTemp, hookCtx, false, context))
		{
			FrictionTemp = Friction;
		}
		target.Friction = FrictionTemp;
		bool WorksOnTheDeadTemp = false;
		if (!serialization.TryCustomCopy<bool>(WorksOnTheDead, ref WorksOnTheDeadTemp, hookCtx, false, context))
		{
			WorksOnTheDeadTemp = WorksOnTheDead;
		}
		target.WorksOnTheDead = WorksOnTheDeadTemp;
		FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry> MetabolismsTemp = null;
		if (!serialization.TryCustomCopy<FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry>>(Metabolisms, ref MetabolismsTemp, hookCtx, true, context))
		{
			MetabolismsTemp = serialization.CreateCopy<FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry>>(Metabolisms, hookCtx, context, false);
		}
		target.Metabolisms = MetabolismsTemp;
		Dictionary<ProtoId<ReactiveGroupPrototype>, ReactiveReagentEffectEntry> ReactiveEffectsTemp = null;
		if (!serialization.TryCustomCopy<Dictionary<ProtoId<ReactiveGroupPrototype>, ReactiveReagentEffectEntry>>(ReactiveEffects, ref ReactiveEffectsTemp, hookCtx, true, context))
		{
			ReactiveEffectsTemp = serialization.CreateCopy<Dictionary<ProtoId<ReactiveGroupPrototype>, ReactiveReagentEffectEntry>>(ReactiveEffects, hookCtx, context, false);
		}
		target.ReactiveEffects = ReactiveEffectsTemp;
		List<ITileReaction> TileReactionsTemp = null;
		if (TileReactions == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<ITileReaction>>(TileReactions, ref TileReactionsTemp, hookCtx, true, context))
		{
			TileReactionsTemp = serialization.CreateCopy<List<ITileReaction>>(TileReactions, hookCtx, context, false);
		}
		target.TileReactions = TileReactionsTemp;
		List<EntityEffect> PlantMetabolismsTemp = null;
		if (PlantMetabolisms == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<List<EntityEffect>>(PlantMetabolisms, ref PlantMetabolismsTemp, hookCtx, true, context))
		{
			PlantMetabolismsTemp = serialization.CreateCopy<List<EntityEffect>>(PlantMetabolisms, hookCtx, context, false);
		}
		target.PlantMetabolisms = PlantMetabolismsTemp;
		float PricePerUnitTemp = 0f;
		if (!serialization.TryCustomCopy<float>(PricePerUnit, ref PricePerUnitTemp, hookCtx, false, context))
		{
			PricePerUnitTemp = PricePerUnit;
		}
		target.PricePerUnit = PricePerUnitTemp;
		SoundSpecifier FootstepSoundTemp = null;
		if (FootstepSound == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<SoundSpecifier>(FootstepSound, ref FootstepSoundTemp, hookCtx, true, context))
		{
			FootstepSoundTemp = serialization.CreateCopy<SoundSpecifier>(FootstepSound, hookCtx, context, false);
		}
		target.FootstepSound = FootstepSoundTemp;
		bool IsCMTemp = false;
		if (!serialization.TryCustomCopy<bool>(IsCM, ref IsCMTemp, hookCtx, false, context))
		{
			IsCMTemp = IsCM;
		}
		target.IsCM = IsCMTemp;
		bool UnknownTemp = false;
		if (!serialization.TryCustomCopy<bool>(Unknown, ref UnknownTemp, hookCtx, false, context))
		{
			UnknownTemp = Unknown;
		}
		target.Unknown = UnknownTemp;
		FixedPoint2? OverdoseTemp = null;
		if (!serialization.TryCustomCopy<FixedPoint2?>(Overdose, ref OverdoseTemp, hookCtx, false, context))
		{
			OverdoseTemp = serialization.CreateCopy<FixedPoint2?>(Overdose, hookCtx, context, false);
		}
		target.Overdose = OverdoseTemp;
		FixedPoint2? CriticalOverdoseTemp = null;
		if (!serialization.TryCustomCopy<FixedPoint2?>(CriticalOverdose, ref CriticalOverdoseTemp, hookCtx, false, context))
		{
			CriticalOverdoseTemp = serialization.CreateCopy<FixedPoint2?>(CriticalOverdose, hookCtx, context, false);
		}
		target.CriticalOverdose = CriticalOverdoseTemp;
		int IntensityTemp = 0;
		if (!serialization.TryCustomCopy<int>(Intensity, ref IntensityTemp, hookCtx, false, context))
		{
			IntensityTemp = Intensity;
		}
		target.Intensity = IntensityTemp;
		int DurationTemp = 0;
		if (!serialization.TryCustomCopy<int>(Duration, ref DurationTemp, hookCtx, false, context))
		{
			DurationTemp = Duration;
		}
		target.Duration = DurationTemp;
		int RadiusTemp = 0;
		if (!serialization.TryCustomCopy<int>(Radius, ref RadiusTemp, hookCtx, false, context))
		{
			RadiusTemp = Radius;
		}
		target.Radius = RadiusTemp;
		EntProtoId FireEntityTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(FireEntity, ref FireEntityTemp, hookCtx, false, context))
		{
			FireEntityTemp = serialization.CreateCopy<EntProtoId>(FireEntity, hookCtx, context, false);
		}
		target.FireEntity = FireEntityTemp;
		FixedPoint2 IntensityModTemp = default(FixedPoint2);
		if (!serialization.TryCustomCopy<FixedPoint2>(IntensityMod, ref IntensityModTemp, hookCtx, false, context))
		{
			IntensityModTemp = serialization.CreateCopy<FixedPoint2>(IntensityMod, hookCtx, context, false);
		}
		target.IntensityMod = IntensityModTemp;
		FixedPoint2 DurationModTemp = default(FixedPoint2);
		if (!serialization.TryCustomCopy<FixedPoint2>(DurationMod, ref DurationModTemp, hookCtx, false, context))
		{
			DurationModTemp = serialization.CreateCopy<FixedPoint2>(DurationMod, hookCtx, context, false);
		}
		target.DurationMod = DurationModTemp;
		FixedPoint2 RadiusModTemp = default(FixedPoint2);
		if (!serialization.TryCustomCopy<FixedPoint2>(RadiusMod, ref RadiusModTemp, hookCtx, false, context))
		{
			RadiusModTemp = serialization.CreateCopy<FixedPoint2>(RadiusMod, hookCtx, context, false);
		}
		target.RadiusMod = RadiusModTemp;
		bool FireSpreadTemp = false;
		if (!serialization.TryCustomCopy<bool>(FireSpread, ref FireSpreadTemp, hookCtx, false, context))
		{
			FireSpreadTemp = FireSpread;
		}
		target.FireSpread = FireSpreadTemp;
		bool ToxinTemp = false;
		if (!serialization.TryCustomCopy<bool>(Toxin, ref ToxinTemp, hookCtx, false, context))
		{
			ToxinTemp = Toxin;
		}
		target.Toxin = ToxinTemp;
		bool AlcoholTemp = false;
		if (!serialization.TryCustomCopy<bool>(Alcohol, ref AlcoholTemp, hookCtx, false, context))
		{
			AlcoholTemp = Alcohol;
		}
		target.Alcohol = AlcoholTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref ReagentPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ReagentPrototype cast = (ReagentPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual ReagentPrototype Instantiate()
	{
		return new ReagentPrototype();
	}
}
