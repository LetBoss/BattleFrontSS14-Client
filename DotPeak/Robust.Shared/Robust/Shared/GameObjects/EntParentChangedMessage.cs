// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntParentChangedMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly struct EntParentChangedMessage(
  EntityUid entity,
  EntityUid? oldParent,
  EntityUid? oldMapId,
  TransformComponent xform)
{
  public readonly EntityUid? OldMapId = oldMapId;

  public EntityUid Entity { get; } = entity;

  public EntityUid? OldParent { get; } = oldParent;

  public TransformComponent Transform { get; } = xform;
}
