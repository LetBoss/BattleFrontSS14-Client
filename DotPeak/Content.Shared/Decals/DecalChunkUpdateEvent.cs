// Decompiled with JetBrains decompiler
// Type: Content.Shared.Decals.DecalChunkUpdateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Decals;

[NetSerializable]
[Serializable]
public sealed class DecalChunkUpdateEvent : EntityEventArgs
{
  public Dictionary<NetEntity, Dictionary<Vector2i, DecalGridComponent.DecalChunk>> Data = new Dictionary<NetEntity, Dictionary<Vector2i, DecalGridComponent.DecalChunk>>();
  public Dictionary<NetEntity, HashSet<Vector2i>> RemovedChunks = new Dictionary<NetEntity, HashSet<Vector2i>>();
}
