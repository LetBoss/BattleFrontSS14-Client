// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Systems.GetEyeProtectionEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System;

#nullable disable
namespace Content.Shared.Eye.Blinding.Systems;

public sealed class GetEyeProtectionEvent : EntityEventArgs, IInventoryRelayEvent
{
  public TimeSpan Protection;

  public SlotFlags TargetSlots => SlotFlags.HEAD | SlotFlags.EYES | SlotFlags.MASK;
}
