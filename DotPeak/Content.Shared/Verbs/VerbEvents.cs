// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.RequestServerVerbsEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

#nullable enable
namespace Content.Shared.Verbs;

[NetSerializable]
[Serializable]
public sealed class RequestServerVerbsEvent : EntityEventArgs
{
  public readonly NetEntity EntityUid;
  public readonly List<string> VerbTypes = new List<string>();
  public readonly NetEntity? SlotOwner;
  public readonly bool AdminRequest;

  public RequestServerVerbsEvent(
    NetEntity entityUid,
    IEnumerable<Type> verbTypes,
    NetEntity? slotOwner = null,
    bool adminRequest = false)
  {
    this.EntityUid = entityUid;
    this.SlotOwner = slotOwner;
    this.AdminRequest = adminRequest;
    foreach (MemberInfo verbType in verbTypes)
      this.VerbTypes.Add(verbType.Name);
  }
}
