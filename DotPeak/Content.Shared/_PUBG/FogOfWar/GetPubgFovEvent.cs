// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.FogOfWar.GetPubgFovEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared._PUBG.FogOfWar;

public sealed class GetPubgFovEvent : EntityEventArgs, IInventoryRelayEvent
{
  public float Modifier;

  public float BaseFov { get; }

  public GetPubgFovEvent(float baseFov) => this.BaseFov = baseFov;

  public SlotFlags TargetSlots => SlotFlags.HEAD;
}
