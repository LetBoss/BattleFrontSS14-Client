// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Containers.ContainerGettingInsertedAttemptEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;

#nullable enable
namespace Robust.Shared.Containers;

public sealed class ContainerGettingInsertedAttemptEvent : ContainerAttemptEventBase
{
  public bool AssumeEmpty { get; set; }

  public ContainerGettingInsertedAttemptEvent(
    BaseContainer container,
    EntityUid entityUid,
    bool assumeEmpty)
    : base(container, entityUid)
  {
    this.AssumeEmpty = assumeEmpty;
  }
}
