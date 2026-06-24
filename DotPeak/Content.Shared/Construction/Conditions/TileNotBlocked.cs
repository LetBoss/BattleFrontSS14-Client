// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Conditions.TileNotBlocked
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Content.Shared.Physics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class TileNotBlocked : 
  IConstructionCondition,
  ISerializationGenerated<TileNotBlocked>,
  ISerializationGenerated
{
  [DataField("filterMobs", false, 1, false, false, null)]
  private bool _filterMobs;
  [DataField("failIfSpace", false, 1, false, false, null)]
  private bool _failIfSpace = true;
  [DataField("failIfNotSturdy", false, 1, false, false, null)]
  private bool _failIfNotSturdy = true;

  public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
  {
    TurfSystem turfSystem;
    TileRef? tile;
    return IoCManager.Resolve<IEntityManager>().TrySystem<TurfSystem>(ref turfSystem) && turfSystem.TryGetTileRef(location, out tile) && (!turfSystem.IsSpace(tile.Value) || !this._failIfSpace) && (turfSystem.GetContentTileDefinition(tile.Value).Sturdy || !this._failIfNotSturdy) && !turfSystem.IsTileBlocked(tile.Value, this._filterMobs ? CollisionGroup.MobMask : CollisionGroup.Impassable);
  }

  public ConstructionGuideEntry GenerateGuideEntry()
  {
    return new ConstructionGuideEntry()
    {
      Localization = "construction-step-condition-tile-not-blocked"
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TileNotBlocked target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<TileNotBlocked>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this._filterMobs, ref flag1, hookCtx, false, context))
      flag1 = this._filterMobs;
    target._filterMobs = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this._failIfSpace, ref flag2, hookCtx, false, context))
      flag2 = this._failIfSpace;
    target._failIfSpace = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this._failIfNotSturdy, ref flag3, hookCtx, false, context))
      flag3 = this._failIfNotSturdy;
    target._failIfNotSturdy = flag3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TileNotBlocked target,
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
    TileNotBlocked target1 = (TileNotBlocked) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public TileNotBlocked Instantiate() => new TileNotBlocked();
}
