// Decompiled with JetBrains decompiler
// Type: Content.Shared.Hands.Components.HandsComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Hands.Components;

[NetSerializable]
[Serializable]
public sealed class HandsComponentState : ComponentState
{
  public readonly Dictionary<string, Hand> Hands;
  public readonly List<string> SortedHands;
  public readonly string? ActiveHandId;

  public HandsComponentState(HandsComponent handComp)
  {
    this.Hands = new Dictionary<string, Hand>((IDictionary<string, Hand>) handComp.Hands);
    this.SortedHands = new List<string>((IEnumerable<string>) handComp.SortedHands);
    this.ActiveHandId = handComp.ActiveHandId;
  }
}
