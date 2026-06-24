// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.StationAi.StationAiCustomizationMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Silicons.StationAi;

[NetSerializable]
[Serializable]
public sealed class StationAiCustomizationMessage : BoundUserInterfaceMessage
{
  public readonly ProtoId<StationAiCustomizationGroupPrototype> GroupProtoId;
  public readonly ProtoId<StationAiCustomizationPrototype> CustomizationProtoId;

  public StationAiCustomizationMessage(
    ProtoId<StationAiCustomizationGroupPrototype> groupProtoId,
    ProtoId<StationAiCustomizationPrototype> customizationProtoId)
  {
    this.GroupProtoId = groupProtoId;
    this.CustomizationProtoId = customizationProtoId;
  }
}
