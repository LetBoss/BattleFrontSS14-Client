// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Systems.SharedFlyBySoundSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Systems;

public abstract class SharedFlyBySoundSystem : EntitySystem
{
  [Dependency]
  private FixtureSystem _fixtures;
  public const string FlyByFixture = "fly-by";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FlyBySoundComponent, ComponentStartup>(new ComponentEventHandler<FlyBySoundComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<FlyBySoundComponent, ComponentShutdown>(new ComponentEventHandler<FlyBySoundComponent, ComponentShutdown>(this.OnShutdown));
  }

  private void OnStartup(EntityUid uid, FlyBySoundComponent component, ComponentStartup args)
  {
    if (!this.TryComp<PhysicsComponent>(uid, out PhysicsComponent _))
      return;
    PhysShapeCircle physShapeCircle = new PhysShapeCircle(component.Range);
  }

  private void OnShutdown(EntityUid uid, FlyBySoundComponent component, ComponentShutdown args)
  {
    PhysicsComponent comp;
    if (!this.TryComp<PhysicsComponent>(uid, out comp) || this.MetaData(uid).EntityLifeStage >= EntityLifeStage.Terminating)
      return;
    this._fixtures.DestroyFixture(uid, "fly-by", body: comp);
  }
}
