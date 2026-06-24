// Decompiled with JetBrains decompiler
// Type: Content.Shared.Guidebook.GuideEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Guidebook;

[Virtual]
public class GuideEntry
{
  [DataField(null, false, 1, true, false, null)]
  public ResPath Text;
  [IdDataField(1, null)]
  public string Id;
  [DataField(null, false, 1, true, false, null)]
  public string Name;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<GuideEntryPrototype>> Children = new List<ProtoId<GuideEntryPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public bool FilterEnabled;
  [DataField(null, false, 1, false, false, null)]
  public bool RuleEntry;
  [DataField(null, false, 1, false, false, null)]
  public int Priority;
}
