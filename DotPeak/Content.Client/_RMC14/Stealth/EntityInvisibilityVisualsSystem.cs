// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Stealth.EntityInvisibilityVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Stealth;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client._RMC14.Stealth;

public sealed class EntityInvisibilityVisualsSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototypes;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityTurnInvisibleComponent, ComponentStartup>(new EntityEventRefHandler<EntityTurnInvisibleComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<EntityTurnInvisibleComponent, ComponentShutdown>(new EntityEventRefHandler<EntityTurnInvisibleComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(Entity<EntityTurnInvisibleComponent> ent, ref ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<EntityTurnInvisibleComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    spriteComponent.PostShader = this._prototypes.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCInvisible")).InstanceUnique();
  }

  private void OnShutdown(Entity<EntityTurnInvisibleComponent> ent, ref ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (this.TerminatingOrDeleted(Entity<EntityTurnInvisibleComponent>.op_Implicit(ent), (MetaDataComponent) null) || !this.TryComp<SpriteComponent>(Entity<EntityTurnInvisibleComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    spriteComponent.PostShader = (ShaderInstance) null;
  }

  public virtual void Update(float frameTime)
  {
    EntityQueryEnumerator<EntityTurnInvisibleComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<EntityTurnInvisibleComponent, SpriteComponent>();
    EntityUid entityUid;
    EntityTurnInvisibleComponent invisibleComponent1;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref invisibleComponent1, ref spriteComponent))
    {
      EntityActiveInvisibleComponent invisibleComponent2;
      float num = this.TryComp<EntityActiveInvisibleComponent>(entityUid, ref invisibleComponent2) ? invisibleComponent2.Opacity : 1f;
      spriteComponent.PostShader?.SetParameter("visibility", num);
    }
  }
}
