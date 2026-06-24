// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.FoodSequenceVisualLayer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[DataRecord]
[NetSerializable]
[Serializable]
public record struct FoodSequenceVisualLayer
{
  public ProtoId<FoodSequenceElementPrototype> Proto;

  public SpriteSpecifier? Sprite { get; set; }

  public Vector2 Scale { get; set; }

  public Vector2 LocalOffset { get; set; }

  public FoodSequenceVisualLayer(
    ProtoId<FoodSequenceElementPrototype> proto,
    SpriteSpecifier? sprite,
    Vector2 scale,
    Vector2 offset)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CSprite\u003Ek__BackingField = SpriteSpecifier.Invalid;
    // ISSUE: reference to a compiler-generated field
    this.\u003CScale\u003Ek__BackingField = Vector2.One;
    // ISSUE: reference to a compiler-generated field
    this.\u003CLocalOffset\u003Ek__BackingField = Vector2.Zero;
    this.Proto = proto;
    this.Sprite = sprite;
    this.Scale = scale;
    this.LocalOffset = offset;
  }
}
