// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.AnimateInsertingEntitiesEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Storage;

[NetSerializable]
[Serializable]
public sealed class AnimateInsertingEntitiesEvent : EntityEventArgs
{
  public readonly NetEntity Storage;
  public readonly List<NetEntity> StoredEntities;
  public readonly List<NetCoordinates> EntityPositions;
  public readonly List<Angle> EntityAngles;

  public AnimateInsertingEntitiesEvent(
    NetEntity storage,
    List<NetEntity> storedEntities,
    List<NetCoordinates> entityPositions,
    List<Angle> entityAngles)
  {
    this.Storage = storage;
    this.StoredEntities = storedEntities;
    this.EntityPositions = entityPositions;
    this.EntityAngles = entityAngles;
  }
}
