// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.BaseBoundUIWrapMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[Serializable]
internal abstract class BaseBoundUIWrapMessage(
  NetEntity entity,
  BoundUserInterfaceMessage message,
  Enum uiKey) : EntityEventArgs
{
  public readonly NetEntity Entity = entity;
  public readonly BoundUserInterfaceMessage Message = message;
  public readonly Enum UiKey = uiKey;
}
