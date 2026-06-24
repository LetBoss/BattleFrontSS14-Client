// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Prototypes.HumanoidSpeciesSpriteLayer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Humanoid.Prototypes;

[Prototype("humanoidBaseSprite", 1)]
[NetSerializable]
[Serializable]
public sealed class HumanoidSpeciesSpriteLayer : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("baseSprite", false, 1, false, false, null)]
  public SpriteSpecifier? BaseSprite { get; private set; }

  [DataField("layerAlpha", false, 1, false, false, null)]
  public float LayerAlpha { get; private set; } = 1f;

  [DataField("allowsMarkings", false, 1, false, false, null)]
  public bool AllowsMarkings { get; private set; } = true;

  [DataField("matchSkin", false, 1, false, false, null)]
  public bool MatchSkin { get; private set; } = true;

  [DataField("markingsMatchSkin", false, 1, false, false, null)]
  public bool MarkingsMatchSkin { get; private set; }
}
