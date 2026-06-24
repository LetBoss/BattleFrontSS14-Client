// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntitySessionMessage`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.GameObjects;

internal readonly struct EntitySessionMessage<T>(EntitySessionEventArgs eventArgs, T message)
{
  public EntitySessionEventArgs EventArgs { get; } = eventArgs;

  public T Message { get; } = message;

  public void Deconstruct(out EntitySessionEventArgs eventArgs, out T message)
  {
    eventArgs = this.EventArgs;
    message = this.Message;
  }
}
