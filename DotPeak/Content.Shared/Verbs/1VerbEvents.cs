// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.VerbsResponseEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Verbs;

[NetSerializable]
[Serializable]
public sealed class VerbsResponseEvent : EntityEventArgs
{
  public readonly List<Verb>? Verbs;
  public readonly NetEntity Entity;

  public VerbsResponseEvent(NetEntity entity, SortedSet<Verb>? verbs)
  {
    this.Entity = entity;
    if (verbs == null)
      return;
    this.Verbs = new List<Verb>((IEnumerable<Verb>) verbs);
  }
}
