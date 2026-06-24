// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Conditions.EmptyOrWindowValidInTile
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Construction.Conditions;

[DataDefinition]
public sealed class EmptyOrWindowValidInTile : 
  IConstructionCondition,
  ISerializationGenerated<EmptyOrWindowValidInTile>,
  ISerializationGenerated
{
  [DataField("tileNotBlocked", false, 1, false, false, null)]
  private TileNotBlocked _tileNotBlocked = new TileNotBlocked();

  public bool Condition(EntityUid user, EntityCoordinates location, Direction direction)
  {
    IEntityManager ientityManager = IoCManager.Resolve<IEntityManager>();
    EntityLookupSystem entityLookupSystem = ientityManager.System<EntityLookupSystem>();
    bool flag = false;
    EntityCoordinates entityCoordinates = location;
    foreach (EntityUid entityUid in entityLookupSystem.GetEntitiesIntersecting(entityCoordinates, (LookupFlags) 5))
    {
      if (ientityManager.HasComponent<SharedCanBuildWindowOnTopComponent>(entityUid))
        flag = true;
    }
    if (!flag)
      flag = this._tileNotBlocked.Condition(user, location, direction);
    return flag;
  }

  public ConstructionGuideEntry GenerateGuideEntry()
  {
    return new ConstructionGuideEntry()
    {
      Localization = "construction-guide-condition-empty-or-window-valid-in-tile"
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmptyOrWindowValidInTile target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<EmptyOrWindowValidInTile>(this, ref target, hookCtx, false, context))
      return;
    TileNotBlocked tileNotBlocked = (TileNotBlocked) null;
    if (this._tileNotBlocked == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<TileNotBlocked>(this._tileNotBlocked, ref tileNotBlocked, hookCtx, false, context))
    {
      if (this._tileNotBlocked == null)
        tileNotBlocked = (TileNotBlocked) null;
      else
        serialization.CopyTo<TileNotBlocked>(this._tileNotBlocked, ref tileNotBlocked, hookCtx, context, true);
    }
    target._tileNotBlocked = tileNotBlocked;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmptyOrWindowValidInTile target,
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
    EmptyOrWindowValidInTile target1 = (EmptyOrWindowValidInTile) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public EmptyOrWindowValidInTile Instantiate() => new EmptyOrWindowValidInTile();
}
