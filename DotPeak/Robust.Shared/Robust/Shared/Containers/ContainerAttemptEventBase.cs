// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Containers.ContainerAttemptEventBase
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Robust.Shared.Containers;

public abstract class ContainerAttemptEventBase : CancellableEntityEventArgs
{
  public readonly BaseContainer Container;
  public readonly EntityUid EntityUid;

  public ContainerAttemptEventBase(BaseContainer container, EntityUid entityUid)
  {
    this.Container = container;
    this.EntityUid = entityUid;
  }
}
