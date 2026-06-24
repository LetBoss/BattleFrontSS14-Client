// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Reagent.ReagentPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chemistry.Reagent;

[Robust.Shared.Prototypes.Prototype(null, 1)]
[DataDefinition]
[Virtual]
public class ReagentPrototype : 
  IPrototype,
  IInheritingPrototype,
  ICMSpecific,
  ISerializationGenerated<ReagentPrototype>,
  ISerializationGenerated
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
  public SoundSpecifier FootstepSound = (SoundSpecifier) new SoundCollectionSpecifier("FootstepPuddle", new AudioParams?());
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

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, true, false, null)]
  private LocId Name { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string LocalizedName => Loc.GetString(LocId.op_Implicit(this.Name));

  [DataField(null, false, 1, false, false, null)]
  public string Group { get; private set; } = nameof (Unknown);

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<ReagentPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [DataField("desc", false, 1, true, false, null)]
  private LocId Description { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string LocalizedDescription => Loc.GetString(LocId.op_Implicit(this.Description));

  [DataField("physicalDesc", false, 1, true, false, null)]
  private LocId PhysicalDescription { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public string LocalizedPhysicalDescription
  {
    get => Loc.GetString(LocId.op_Implicit(this.PhysicalDescription));
  }

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

  public FixedPoint2 ReactionTile(
    TileRef tile,
    FixedPoint2 reactVolume,
    IEntityManager entityManager,
    List<ReagentData>? data)
  {
    FixedPoint2 zero = FixedPoint2.Zero;
    if (((Tile) ref tile.Tile).IsEmpty)
      return zero;
    foreach (ITileReaction tileReaction in this.TileReactions)
    {
      zero += tileReaction.TileReact(tile, this, reactVolume - zero, entityManager, data);
      if (zero > reactVolume)
        throw new Exception("Removed more than we have!");
      if (zero == reactVolume)
        break;
    }
    return zero;
  }

  public void ReactionPlant(EntityUid? plantHolder, ReagentQuantity amount, Solution solution)
  {
    if (!plantHolder.HasValue)
      return;
    IEntityManager entityManager = IoCManager.Resolve<IEntityManager>();
    IRobustRandom random = IoCManager.Resolve<IRobustRandom>();
    EntityEffectReagentArgs args = new EntityEffectReagentArgs(plantHolder.Value, entityManager, new EntityUid?(), solution, amount.Quantity, this, new ReactionMethod?(), (FixedPoint2) 1f);
    foreach (EntityEffect plantMetabolism in this.PlantMetabolisms)
    {
      if (plantMetabolism.ShouldApply((EntityEffectBaseArgs) args, random))
      {
        if (plantMetabolism.ShouldLog)
        {
          EntityUid targetEntity = args.TargetEntity;
          SharedAdminLogSystem sharedAdminLogSystem = entityManager.System<SharedAdminLogSystem>();
          int logImpact = (int) plantMetabolism.LogImpact;
          LogStringHandler logStringHandler = new LogStringHandler(59, 4);
          logStringHandler.AppendLiteral("Plant metabolism effect ");
          logStringHandler.AppendFormatted(plantMetabolism.GetType().Name, format: "effect");
          logStringHandler.AppendLiteral(" of reagent ");
          logStringHandler.AppendFormatted(this.ID, format: "reagent");
          logStringHandler.AppendLiteral(" applied on entity ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(entityManager.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(targetEntity)), "entity", "entMan.ToPrettyString(entity)");
          logStringHandler.AppendLiteral(" at ");
          logStringHandler.AppendFormatted<EntityCoordinates>(entityManager.GetComponent<TransformComponent>(targetEntity).Coordinates, "coordinates", "entMan.GetComponent<TransformComponent>(entity).Coordinates");
          ref LogStringHandler local = ref logStringHandler;
          sharedAdminLogSystem.Add(LogType.ReagentEffect, (LogImpact) logImpact, ref local);
        }
        plantMetabolism.Effect((EntityEffectBaseArgs) args);
      }
    }
  }

  [DataField(null, false, 1, false, false, null)]
  public bool IsCM { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref ReagentPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ReagentPrototype>(this, ref target, hookCtx, false, context))
      return;
    string str1 = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref str1, hookCtx, false, context))
      str1 = this.ID;
    target.ID = str1;
    LocId locId1 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Name, ref locId1, hookCtx, false, context))
      locId1 = serialization.CreateCopy<LocId>(this.Name, hookCtx, context, false);
    target.Name = locId1;
    string str2 = (string) null;
    if (this.Group == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Group, ref str2, hookCtx, false, context))
      str2 = this.Group;
    target.Group = str2;
    string[] strArray = (string[]) null;
    if (!serialization.TryCustomCopy<string[]>(this.Parents, ref strArray, hookCtx, true, context))
      strArray = serialization.CreateCopy<string[]>(this.Parents, hookCtx, context, false);
    target.Parents = strArray;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Abstract, ref flag1, hookCtx, false, context))
      flag1 = this.Abstract;
    target.Abstract = flag1;
    LocId locId2 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Description, ref locId2, hookCtx, false, context))
      locId2 = serialization.CreateCopy<LocId>(this.Description, hookCtx, context, false);
    target.Description = locId2;
    LocId locId3 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.PhysicalDescription, ref locId3, hookCtx, false, context))
      locId3 = serialization.CreateCopy<LocId>(this.PhysicalDescription, hookCtx, context, false);
    target.PhysicalDescription = locId3;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Recognizable, ref flag2, hookCtx, false, context))
      flag2 = this.Recognizable;
    target.Recognizable = flag2;
    ProtoId<FlavorPrototype>? nullable1 = new ProtoId<FlavorPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<FlavorPrototype>?>(this.Flavor, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<ProtoId<FlavorPrototype>?>(this.Flavor, hookCtx, context, false);
    target.Flavor = nullable1;
    FixedPoint2 fixedPoint2_1 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.FlavorMinimum, ref fixedPoint2_1, hookCtx, false, context))
      fixedPoint2_1 = serialization.CreateCopy<FixedPoint2>(this.FlavorMinimum, hookCtx, context, false);
    target.FlavorMinimum = fixedPoint2_1;
    Color color = new Color();
    if (!serialization.TryCustomCopy<Color>(this.SubstanceColor, ref color, hookCtx, false, context))
      color = serialization.CreateCopy<Color>(this.SubstanceColor, hookCtx, context, false);
    target.SubstanceColor = color;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpecificHeat, ref num1, hookCtx, false, context))
      num1 = this.SpecificHeat;
    target.SpecificHeat = num1;
    float? nullable2 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.BoilingPoint, ref nullable2, hookCtx, false, context))
      nullable2 = this.BoilingPoint;
    target.BoilingPoint = nullable2;
    float? nullable3 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.MeltingPoint, ref nullable3, hookCtx, false, context))
      nullable3 = this.MeltingPoint;
    target.MeltingPoint = nullable3;
    SpriteSpecifier spriteSpecifier = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.MetamorphicSprite, ref spriteSpecifier, hookCtx, true, context))
      spriteSpecifier = serialization.CreateCopy<SpriteSpecifier>(this.MetamorphicSprite, hookCtx, context, false);
    target.MetamorphicSprite = spriteSpecifier;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MetamorphicMaxFillLevels, ref num2, hookCtx, false, context))
      num2 = this.MetamorphicMaxFillLevels;
    target.MetamorphicMaxFillLevels = num2;
    string str3 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.MetamorphicFillBaseName, ref str3, hookCtx, false, context))
      str3 = this.MetamorphicFillBaseName;
    target.MetamorphicFillBaseName = str3;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.MetamorphicChangeColor, ref flag3, hookCtx, false, context))
      flag3 = this.MetamorphicChangeColor;
    target.MetamorphicChangeColor = flag3;
    SlipperyEffectEntry slipperyEffectEntry = (SlipperyEffectEntry) null;
    if (!serialization.TryCustomCopy<SlipperyEffectEntry>(this.SlipData, ref slipperyEffectEntry, hookCtx, false, context))
    {
      if (this.SlipData == null)
        slipperyEffectEntry = (SlipperyEffectEntry) null;
      else
        serialization.CopyTo<SlipperyEffectEntry>(this.SlipData, ref slipperyEffectEntry, hookCtx, context, false);
    }
    target.SlipData = slipperyEffectEntry;
    FixedPoint2 fixedPoint2_2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.EvaporationSpeed, ref fixedPoint2_2, hookCtx, false, context))
      fixedPoint2_2 = serialization.CreateCopy<FixedPoint2>(this.EvaporationSpeed, hookCtx, context, false);
    target.EvaporationSpeed = fixedPoint2_2;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Absorbent, ref flag4, hookCtx, false, context))
      flag4 = this.Absorbent;
    target.Absorbent = flag4;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Fizziness, ref num3, hookCtx, false, context))
      num3 = this.Fizziness;
    target.Fizziness = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Viscosity, ref num4, hookCtx, false, context))
      num4 = this.Viscosity;
    target.Viscosity = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Friction, ref num5, hookCtx, false, context))
      num5 = this.Friction;
    target.Friction = num5;
    bool flag5 = false;
    if (!serialization.TryCustomCopy<bool>(this.WorksOnTheDead, ref flag5, hookCtx, false, context))
      flag5 = this.WorksOnTheDead;
    target.WorksOnTheDead = flag5;
    FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry> frozenDictionary = (FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry>) null;
    if (!serialization.TryCustomCopy<FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry>>(this.Metabolisms, ref frozenDictionary, hookCtx, true, context))
      frozenDictionary = serialization.CreateCopy<FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry>>(this.Metabolisms, hookCtx, context, false);
    target.Metabolisms = frozenDictionary;
    Dictionary<ProtoId<ReactiveGroupPrototype>, ReactiveReagentEffectEntry> dictionary = (Dictionary<ProtoId<ReactiveGroupPrototype>, ReactiveReagentEffectEntry>) null;
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<ReactiveGroupPrototype>, ReactiveReagentEffectEntry>>(this.ReactiveEffects, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<ProtoId<ReactiveGroupPrototype>, ReactiveReagentEffectEntry>>(this.ReactiveEffects, hookCtx, context, false);
    target.ReactiveEffects = dictionary;
    List<ITileReaction> tileReactionList = (List<ITileReaction>) null;
    if (this.TileReactions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ITileReaction>>(this.TileReactions, ref tileReactionList, hookCtx, true, context))
      tileReactionList = serialization.CreateCopy<List<ITileReaction>>(this.TileReactions, hookCtx, context, false);
    target.TileReactions = tileReactionList;
    List<EntityEffect> entityEffectList = (List<EntityEffect>) null;
    if (this.PlantMetabolisms == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityEffect>>(this.PlantMetabolisms, ref entityEffectList, hookCtx, true, context))
      entityEffectList = serialization.CreateCopy<List<EntityEffect>>(this.PlantMetabolisms, hookCtx, context, false);
    target.PlantMetabolisms = entityEffectList;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PricePerUnit, ref num6, hookCtx, false, context))
      num6 = this.PricePerUnit;
    target.PricePerUnit = num6;
    SoundSpecifier soundSpecifier = (SoundSpecifier) null;
    if (this.FootstepSound == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FootstepSound, ref soundSpecifier, hookCtx, true, context))
      soundSpecifier = serialization.CreateCopy<SoundSpecifier>(this.FootstepSound, hookCtx, context, false);
    target.FootstepSound = soundSpecifier;
    bool flag6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsCM, ref flag6, hookCtx, false, context))
      flag6 = this.IsCM;
    target.IsCM = flag6;
    bool flag7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Unknown, ref flag7, hookCtx, false, context))
      flag7 = this.Unknown;
    target.Unknown = flag7;
    FixedPoint2? nullable4 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.Overdose, ref nullable4, hookCtx, false, context))
      nullable4 = serialization.CreateCopy<FixedPoint2?>(this.Overdose, hookCtx, context, false);
    target.Overdose = nullable4;
    FixedPoint2? nullable5 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.CriticalOverdose, ref nullable5, hookCtx, false, context))
      nullable5 = serialization.CreateCopy<FixedPoint2?>(this.CriticalOverdose, hookCtx, context, false);
    target.CriticalOverdose = nullable5;
    int num7 = 0;
    if (!serialization.TryCustomCopy<int>(this.Intensity, ref num7, hookCtx, false, context))
      num7 = this.Intensity;
    target.Intensity = num7;
    int num8 = 0;
    if (!serialization.TryCustomCopy<int>(this.Duration, ref num8, hookCtx, false, context))
      num8 = this.Duration;
    target.Duration = num8;
    int num9 = 0;
    if (!serialization.TryCustomCopy<int>(this.Radius, ref num9, hookCtx, false, context))
      num9 = this.Radius;
    target.Radius = num9;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.FireEntity, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.FireEntity, hookCtx, context, false);
    target.FireEntity = entProtoId;
    FixedPoint2 fixedPoint2_3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.IntensityMod, ref fixedPoint2_3, hookCtx, false, context))
      fixedPoint2_3 = serialization.CreateCopy<FixedPoint2>(this.IntensityMod, hookCtx, context, false);
    target.IntensityMod = fixedPoint2_3;
    FixedPoint2 fixedPoint2_4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DurationMod, ref fixedPoint2_4, hookCtx, false, context))
      fixedPoint2_4 = serialization.CreateCopy<FixedPoint2>(this.DurationMod, hookCtx, context, false);
    target.DurationMod = fixedPoint2_4;
    FixedPoint2 fixedPoint2_5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.RadiusMod, ref fixedPoint2_5, hookCtx, false, context))
      fixedPoint2_5 = serialization.CreateCopy<FixedPoint2>(this.RadiusMod, hookCtx, context, false);
    target.RadiusMod = fixedPoint2_5;
    bool flag8 = false;
    if (!serialization.TryCustomCopy<bool>(this.FireSpread, ref flag8, hookCtx, false, context))
      flag8 = this.FireSpread;
    target.FireSpread = flag8;
    bool flag9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Toxin, ref flag9, hookCtx, false, context))
      flag9 = this.Toxin;
    target.Toxin = flag9;
    bool flag10 = false;
    if (!serialization.TryCustomCopy<bool>(this.Alcohol, ref flag10, hookCtx, false, context))
      flag10 = this.Alcohol;
    target.Alcohol = flag10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref ReagentPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ReagentPrototype target1 = (ReagentPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual ReagentPrototype Instantiate() => new ReagentPrototype();
}
