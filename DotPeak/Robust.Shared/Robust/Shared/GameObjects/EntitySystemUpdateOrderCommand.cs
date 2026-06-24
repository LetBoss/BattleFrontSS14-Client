// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntitySystemUpdateOrderCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Toolshed;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.GameObjects;

[ToolshedCommand]
internal sealed class EntitySystemUpdateOrderCommand : ToolshedCommand
{
  [Dependency]
  private readonly IEntitySystemManager _entitySystemManager;

  [CommandImplementation("tick")]
  public IEnumerable<Type> Tick()
  {
    return ((Robust.Shared.GameObjects.EntitySystemManager) this._entitySystemManager).TickUpdateOrder;
  }

  [CommandImplementation("frame")]
  public IEnumerable<Type> Frame()
  {
    return ((Robust.Shared.GameObjects.EntitySystemManager) this._entitySystemManager).FrameUpdateOrder;
  }
}
