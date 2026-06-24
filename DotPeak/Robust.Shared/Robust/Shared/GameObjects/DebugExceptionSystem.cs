// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.DebugExceptionSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.GameObjects;

public sealed class DebugExceptionSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DebugExceptionOnAddComponent, ComponentAdd>(new ComponentEventHandler<DebugExceptionOnAddComponent, ComponentAdd>(this.OnCompAdd));
    this.SubscribeLocalEvent<DebugExceptionInitializeComponent, ComponentInit>((ComponentEventHandler<DebugExceptionInitializeComponent, ComponentInit>) ((_1, _2, _3) =>
    {
      throw new NotSupportedException();
    }));
    this.SubscribeLocalEvent<DebugExceptionStartupComponent, ComponentStartup>((ComponentEventHandler<DebugExceptionStartupComponent, ComponentStartup>) ((_4, _5, _6) =>
    {
      throw new NotSupportedException();
    }));
  }

  private void OnCompAdd(EntityUid uid, DebugExceptionOnAddComponent component, ComponentAdd args)
  {
    throw new NotSupportedException();
  }
}
