// Decompiled with JetBrains decompiler
// Type: Content.Shared.CharacterInfo.CharacterInfoEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Objectives;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.CharacterInfo;

[NetSerializable]
[Serializable]
public sealed class CharacterInfoEvent : EntityEventArgs
{
  public readonly NetEntity NetEntity;
  public readonly string JobTitle;
  public readonly Dictionary<string, List<ObjectiveInfo>> Objectives;
  public readonly string? Briefing;

  public CharacterInfoEvent(
    NetEntity netEntity,
    string jobTitle,
    Dictionary<string, List<ObjectiveInfo>> objectives,
    string? briefing)
  {
    this.NetEntity = netEntity;
    this.JobTitle = jobTitle;
    this.Objectives = objectives;
    this.Briefing = briefing;
  }
}
