// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.AccessReaderComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Access.Components;

[NetSerializable]
[Serializable]
public sealed class AccessReaderComponentState : ComponentState
{
  public bool Enabled;
  public HashSet<ProtoId<AccessLevelPrototype>> DenyTags;
  public List<HashSet<ProtoId<AccessLevelPrototype>>> AccessLists;
  public List<(NetEntity, uint)> AccessKeys;
  public Queue<AccessRecord> AccessLog;
  public int AccessLogLimit;

  public AccessReaderComponentState(
    bool enabled,
    HashSet<ProtoId<AccessLevelPrototype>> denyTags,
    List<HashSet<ProtoId<AccessLevelPrototype>>> accessLists,
    List<(NetEntity, uint)> accessKeys,
    Queue<AccessRecord> accessLog,
    int accessLogLimit)
  {
    this.Enabled = enabled;
    this.DenyTags = denyTags;
    this.AccessLists = accessLists;
    this.AccessKeys = accessKeys;
    this.AccessLog = accessLog;
    this.AccessLogLimit = accessLogLimit;
  }
}
