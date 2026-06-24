// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.TypingIndicator.BeforeShowTypingIndicatorEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Chat.TypingIndicator;

[NetSerializable]
[Serializable]
public sealed class BeforeShowTypingIndicatorEvent : IInventoryRelayEvent
{
  private ProtoId<TypingIndicatorPrototype>? _overrideIndicator;
  private TimeSpan? _latestEquipTime;

  public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

  public BeforeShowTypingIndicatorEvent()
  {
    this._overrideIndicator = new ProtoId<TypingIndicatorPrototype>?();
    this._latestEquipTime = new TimeSpan?();
  }

  public bool TryUpdateTimeAndIndicator(
    ProtoId<TypingIndicatorPrototype>? indicator,
    TimeSpan? equipTime)
  {
    if (equipTime.HasValue)
    {
      if (this._latestEquipTime.HasValue)
      {
        TimeSpan? latestEquipTime = this._latestEquipTime;
        TimeSpan? nullable = equipTime;
        if ((latestEquipTime.HasValue & nullable.HasValue ? (latestEquipTime.GetValueOrDefault() < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          goto label_4;
      }
      this._latestEquipTime = equipTime;
      this._overrideIndicator = indicator;
      return true;
    }
label_4:
    return false;
  }

  public ProtoId<TypingIndicatorPrototype>? GetMostRecentIndicator() => this._overrideIndicator;
}
