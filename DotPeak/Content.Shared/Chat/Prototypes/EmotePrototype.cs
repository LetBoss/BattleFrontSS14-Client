// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.Prototypes.EmotePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chat.Prototypes;

[Prototype(null, 1)]
public sealed class EmotePrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public string Name;
  [DataField(null, false, 1, false, false, null)]
  public bool Available = true;
  [DataField(null, false, 1, false, false, null)]
  public EmoteCategory Category = EmoteCategory.General;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/Actions/scream.png"));
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public List<string> ChatMessages = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<string> ChatTriggers = new HashSet<string>();

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
