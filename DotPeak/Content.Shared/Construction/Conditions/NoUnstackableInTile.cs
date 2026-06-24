// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Conditions.NoUnstackableInTile
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.EntitySystems;
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
public sealed class NoUnstackableInTile : 
  IConstructionCondition,
  ISerializationGenerated<NoUnstackableInTile>,
  ISerializationGenerated
{
  public const string GuidebookString = "construction-step-condition-no-unstackable-in-tile";

  public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
  {
    return !IoCManager.Resolve<IEntitySystemManager>().GetEntitySystem<AnchorableSystem>().AnyUnstackablesAnchoredAt(location);
  }

  public ConstructionGuideEntry GenerateGuideEntry()
  {
    return new ConstructionGuideEntry()
    {
      Localization = "construction-step-condition-no-unstackable-in-tile"
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NoUnstackableInTile target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<NoUnstackableInTile>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NoUnstackableInTile target,
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
    NoUnstackableInTile target1 = (NoUnstackableInTile) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public NoUnstackableInTile Instantiate() => new NoUnstackableInTile();
}
