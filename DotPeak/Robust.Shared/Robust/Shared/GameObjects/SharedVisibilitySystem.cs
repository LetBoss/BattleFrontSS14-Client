// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedVisibilitySystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedVisibilitySystem : EntitySystem
{
  public virtual void AddLayer(Entity<VisibilityComponent?> ent, ushort layer, bool refresh = true)
  {
  }

  public virtual void RemoveLayer(Entity<VisibilityComponent?> ent, ushort layer, bool refresh = true)
  {
  }

  public virtual void SetLayer(Entity<VisibilityComponent?> ent, ushort layer, bool refresh = true)
  {
  }

  public virtual void RefreshVisibility(
    EntityUid uid,
    VisibilityComponent? visibilityComponent = null,
    MetaDataComponent? meta = null)
  {
  }

  public virtual void RefreshVisibility(Entity<VisibilityComponent?, MetaDataComponent?> ent)
  {
  }
}
