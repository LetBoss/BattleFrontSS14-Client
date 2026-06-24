// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedViewSubscriberSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Player;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedViewSubscriberSystem : EntitySystem
{
  public virtual void AddViewSubscriber(EntityUid uid, ICommonSession session)
  {
  }

  public virtual void RemoveViewSubscriber(EntityUid uid, ICommonSession session)
  {
  }
}
