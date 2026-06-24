// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.ExecuteVerbEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Verbs;

[NetSerializable]
[Serializable]
public sealed class ExecuteVerbEvent : EntityEventArgs
{
  public readonly NetEntity Target;
  public readonly Verb RequestedVerb;

  public ExecuteVerbEvent(NetEntity target, Verb requestedVerb)
  {
    this.Target = target;
    this.RequestedVerb = requestedVerb;
  }
}
