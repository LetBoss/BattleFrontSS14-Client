// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Conditions.TileType
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Maps;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class TileType : 
  IConstructionCondition,
  ISerializationGenerated<TileType>,
  ISerializationGenerated
{
  [DataField("guideText", false, 1, false, false, null)]
  public string? GuideText;
  [DataField("guideIcon", false, 1, false, false, null)]
  public SpriteSpecifier? GuideIcon;

  [DataField("targets", false, 1, false, false, null)]
  public List<string> TargetTiles { get; private set; } = new List<string>();

  public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
  {
    TurfSystem turfSystem;
    TileRef? tile;
    if (!IoCManager.Resolve<IEntityManager>().TrySystem<TurfSystem>(ref turfSystem) || !turfSystem.TryGetTileRef(location, out tile))
      return false;
    ContentTileDefinition contentTileDefinition = turfSystem.GetContentTileDefinition(tile.Value);
    foreach (string targetTile in this.TargetTiles)
    {
      if (contentTileDefinition.ID == targetTile)
        return true;
    }
    return false;
  }

  public ConstructionGuideEntry? GenerateGuideEntry()
  {
    if (this.GuideText == null)
      return (ConstructionGuideEntry) null;
    return new ConstructionGuideEntry()
    {
      Localization = this.GuideText,
      Icon = this.GuideIcon
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TileType target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<TileType>(this, ref target, hookCtx, false, context))
      return;
    List<string> stringList = (List<string>) null;
    if (this.TargetTiles == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.TargetTiles, ref stringList, hookCtx, true, context))
      stringList = serialization.CreateCopy<List<string>>(this.TargetTiles, hookCtx, context, false);
    target.TargetTiles = stringList;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.GuideText, ref str, hookCtx, false, context))
      str = this.GuideText;
    target.GuideText = str;
    SpriteSpecifier spriteSpecifier = (SpriteSpecifier) null;
    if (!serialization.TryCustomCopy<SpriteSpecifier>(this.GuideIcon, ref spriteSpecifier, hookCtx, true, context))
      spriteSpecifier = serialization.CreateCopy<SpriteSpecifier>(this.GuideIcon, hookCtx, context, false);
    target.GuideIcon = spriteSpecifier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TileType target,
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
    TileType target1 = (TileType) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public TileType Instantiate() => new TileType();
}
