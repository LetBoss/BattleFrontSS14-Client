// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameStates.SharedPvsOverrideSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Player;

#nullable enable
namespace Robust.Shared.GameStates;

public abstract class SharedPvsOverrideSystem : EntitySystem
{
  public virtual void AddGlobalOverride(EntityUid uid)
  {
  }

  public virtual void RemoveGlobalOverride(EntityUid uid)
  {
  }

  public virtual void AddSessionOverride(EntityUid uid, ICommonSession session)
  {
  }

  public virtual void RemoveSessionOverride(EntityUid uid, ICommonSession session)
  {
  }

  public virtual void AddSessionOverrides(EntityUid uid, Filter filter)
  {
  }
}
