// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedInputSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Input.Binding;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedInputSystem : EntitySystem
{
  private CommandBindRegistry _bindRegistry;

  protected override void PostInject()
  {
    base.PostInject();
    this._bindRegistry = new CommandBindRegistry(this.Log);
  }

  public ICommandBindRegistry BindRegistry => (ICommandBindRegistry) this._bindRegistry;
}
