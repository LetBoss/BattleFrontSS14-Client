// Decompiled with JetBrains decompiler
// Type: Content.Shared.CombatMode.Pacification.AttemptPacifiedThrowEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.CombatMode.Pacification;

[ByRefEvent]
public struct AttemptPacifiedThrowEvent(EntityUid itemUid, EntityUid playerUid)
{
  public EntityUid ItemUid = itemUid;
  public EntityUid PlayerUid = playerUid;

  public bool Cancelled { get; private set; } = false;

  public string? CancelReasonMessageId { get; private set; } = (string) null;

  public void Cancel(string? reasonMessageId = null)
  {
    this.Cancelled = true;
    this.CancelReasonMessageId = reasonMessageId;
  }
}
