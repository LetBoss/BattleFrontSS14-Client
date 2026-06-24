// Decompiled with JetBrains decompiler
// Type: Content.Shared.Destructible.SharedDestructibleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Destructible;

public abstract class SharedDestructibleSystem : EntitySystem
{
  public bool DestroyEntity(EntityUid owner)
  {
    DestructionAttemptEvent destructionAttemptEvent = new DestructionAttemptEvent();
    this.RaiseLocalEvent<DestructionAttemptEvent>(owner, destructionAttemptEvent, false);
    if (destructionAttemptEvent.Cancelled)
      return false;
    DestructionEventArgs destructionEventArgs = new DestructionEventArgs();
    this.RaiseLocalEvent<DestructionEventArgs>(owner, destructionEventArgs, false);
    this.QueueDel(new EntityUid?(owner));
    return true;
  }

  public void BreakEntity(EntityUid owner)
  {
    BreakageEventArgs breakageEventArgs = new BreakageEventArgs();
    this.RaiseLocalEvent<BreakageEventArgs>(owner, breakageEventArgs, false);
  }
}
