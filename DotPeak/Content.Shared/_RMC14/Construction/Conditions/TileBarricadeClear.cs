// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Construction.Conditions.TileBarricadeClear
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared.Construction;
using Content.Shared.Construction.Conditions;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Construction.Conditions;

[DataDefinition]
public sealed class TileBarricadeClear : 
  IConstructionCondition,
  ISerializationGenerated<TileBarricadeClear>,
  ISerializationGenerated
{
  public ConstructionGuideEntry GenerateGuideEntry()
  {
    return new ConstructionGuideEntry()
    {
      Localization = "rmc-construction-not-barricade-clear"
    };
  }

  public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
  {
    RMCMapSystem rmcMapSystem = IoCManager.Resolve<IEntityManager>().System<RMCMapSystem>();
    EntityCoordinates coords = location;
    DirectionFlag directionFlag = DirectionExtensions.AsFlag(direction);
    Direction? offset = new Direction?();
    DirectionFlag facing = directionFlag;
    return !rmcMapSystem.HasAnchoredEntityEnumerator<BarricadeComponent>(coords, offset, facing);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TileBarricadeClear target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<TileBarricadeClear>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TileBarricadeClear target,
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
    TileBarricadeClear target1 = (TileBarricadeClear) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public TileBarricadeClear Instantiate() => new TileBarricadeClear();
}
