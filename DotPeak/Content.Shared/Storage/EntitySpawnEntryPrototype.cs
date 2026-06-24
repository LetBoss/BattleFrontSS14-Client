// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySpawnEntryPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Storage;

[Prototype(null, 1)]
public sealed class EntitySpawnEntryPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public List<EntitySpawnEntry> Entries = new List<EntitySpawnEntry>();

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;
}
