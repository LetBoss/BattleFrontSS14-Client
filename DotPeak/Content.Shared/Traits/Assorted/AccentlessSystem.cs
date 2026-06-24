// Decompiled with JetBrains decompiler
// Type: Content.Shared.Traits.Assorted.AccentlessSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared.Traits.Assorted;

public sealed class AccentlessSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<AccentlessComponent, ComponentStartup>(new ComponentEventHandler<AccentlessComponent, ComponentStartup>(this.RemoveAccents));
  }

  private void RemoveAccents(EntityUid uid, AccentlessComponent component, ComponentStartup args)
  {
    foreach (EntityPrototype.ComponentRegistryEntry componentRegistryEntry in component.RemovedAccents.Values)
    {
      IComponent component1 = componentRegistryEntry.Component;
      this.RemComp(uid, component1.GetType());
    }
  }
}
