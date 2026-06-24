// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.HumanoidMarkingModifierBaseLayersSetMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Humanoid;

[NetSerializable]
[Serializable]
public sealed class HumanoidMarkingModifierBaseLayersSetMessage : BoundUserInterfaceMessage
{
  public HumanoidMarkingModifierBaseLayersSetMessage(
    HumanoidVisualLayers layer,
    CustomBaseLayerInfo? info,
    bool resendState)
  {
    this.Layer = layer;
    this.Info = info;
    this.ResendState = resendState;
  }

  public HumanoidVisualLayers Layer { get; }

  public CustomBaseLayerInfo? Info { get; }

  public bool ResendState { get; }
}
