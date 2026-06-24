// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Conditions.NoWindowsInTile
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class NoWindowsInTile : 
  IConstructionCondition,
  ISerializationGenerated<NoWindowsInTile>,
  ISerializationGenerated
{
  private static readonly ProtoId<TagPrototype> WindowTag = ProtoId<TagPrototype>.op_Implicit("Window");

  public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
  {
    IEntitySystemManager entitySysManager = IoCManager.Resolve<IEntityManager>().EntitySysManager;
    TagSystem entitySystem = entitySysManager.GetEntitySystem<TagSystem>();
    foreach (EntityUid entityUid in entitySysManager.GetEntitySystem<EntityLookupSystem>().GetEntitiesIntersecting(location, (LookupFlags) 4))
    {
      if (entitySystem.HasTag(entityUid, NoWindowsInTile.WindowTag))
        return false;
    }
    return true;
  }

  public ConstructionGuideEntry GenerateGuideEntry()
  {
    return new ConstructionGuideEntry()
    {
      Localization = "construction-step-condition-no-windows-in-tile"
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NoWindowsInTile target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<NoWindowsInTile>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NoWindowsInTile target,
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
    NoWindowsInTile target1 = (NoWindowsInTile) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public NoWindowsInTile Instantiate() => new NoWindowsInTile();
}
