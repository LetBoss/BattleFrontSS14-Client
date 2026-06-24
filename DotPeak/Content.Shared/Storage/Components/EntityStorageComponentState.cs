// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.Components.EntityStorageComponentState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Storage.Components;

[NetSerializable]
[Serializable]
public sealed class EntityStorageComponentState : ComponentState
{
  public bool Open;
  public int Capacity;
  public bool IsCollidableWhenOpen;
  public bool OpenOnMove;
  public float EnteringRange;
  public TimeSpan NextInternalOpenAttempt;

  public EntityStorageComponentState(
    bool open,
    int capacity,
    bool isCollidableWhenOpen,
    bool openOnMove,
    float enteringRange,
    TimeSpan nextInternalOpenAttempt)
  {
    this.Open = open;
    this.Capacity = capacity;
    this.IsCollidableWhenOpen = isCollidableWhenOpen;
    this.OpenOnMove = openOnMove;
    this.EnteringRange = enteringRange;
    this.NextInternalOpenAttempt = nextInternalOpenAttempt;
  }
}
