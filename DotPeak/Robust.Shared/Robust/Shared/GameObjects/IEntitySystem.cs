// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.IEntitySystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IEntitySystem : IEntityEventSubscriber
{
  IEnumerable<Type> UpdatesAfter { get; }

  IEnumerable<Type> UpdatesBefore { get; }

  bool UpdatesOutsidePrediction { get; }

  void Initialize();

  void Shutdown();

  void Update(float frameTime);

  void FrameUpdate(float frameTime);
}
