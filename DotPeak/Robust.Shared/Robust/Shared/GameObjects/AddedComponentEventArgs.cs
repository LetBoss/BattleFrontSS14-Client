// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.AddedComponentEventArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.GameObjects;

public readonly struct AddedComponentEventArgs
{
  public readonly ComponentEventArgs BaseArgs;
  public readonly ComponentRegistration ComponentType;

  internal AddedComponentEventArgs(ComponentEventArgs baseArgs, ComponentRegistration componentType)
  {
    this.BaseArgs = baseArgs;
    this.ComponentType = componentType;
  }
}
