// Decompiled with JetBrains decompiler
// Type: Content.Shared.Throwing.ThrowEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Throwing;

public abstract class ThrowEvent : HandledEntityEventArgs
{
  public readonly EntityUid Thrown;
  public readonly EntityUid Target;
  public ThrownItemComponent Component;

  public ThrowEvent(EntityUid thrown, EntityUid target, ThrownItemComponent component)
  {
    this.Thrown = thrown;
    this.Target = target;
    this.Component = component;
  }
}
