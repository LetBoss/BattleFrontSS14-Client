// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Lights.PointLightRotationSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Lights;

public sealed class PointLightRotationSystem : EntitySystem
{
  [Dependency]
  private SharedPointLightSystem _pointLight;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PointLightRotationComponent, ComponentStartup>(new EntityEventRefHandler<PointLightRotationComponent, ComponentStartup>(this.OnSetRotation<ComponentStartup>));
    this.SubscribeLocalEvent<PointLightRotationComponent, MapInitEvent>(new EntityEventRefHandler<PointLightRotationComponent, MapInitEvent>(this.OnSetRotation<MapInitEvent>));
    this.SubscribeLocalEvent<PointLightRotationComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<PointLightRotationComponent, AfterAutoHandleStateEvent>(this.OnSetRotation<AfterAutoHandleStateEvent>));
  }

  private void OnSetRotation<T>(Entity<PointLightRotationComponent> ent, ref T args)
  {
    SharedPointLightComponent component;
    if (this._pointLight.TryGetLight((EntityUid) ent, out component))
      component.Rotation = ent.Comp.Rotation;
    this.Dirty<PointLightRotationComponent>(ent);
  }
}
