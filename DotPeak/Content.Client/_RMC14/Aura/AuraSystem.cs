// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Aura.AuraSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Stealth;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client._RMC14.Aura;

public sealed class AuraSystem : SharedAuraSystem
{
  [Dependency]
  private IPrototypeManager _prototypes;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AuraComponent, ComponentStartup>(new EntityEventRefHandler<AuraComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AuraComponent, ComponentShutdown>(new EntityEventRefHandler<AuraComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(Entity<AuraComponent> ent, ref ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<AuraComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    spriteComponent.PostShader = this._prototypes.Index<ShaderPrototype>(new ProtoId<ShaderPrototype>("RMCAuraOutline")).InstanceUnique();
  }

  private void OnShutdown(Entity<AuraComponent> ent, ref ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (this.TerminatingOrDeleted(Entity<AuraComponent>.op_Implicit(ent), (MetaDataComponent) null) || this.HasComp<EntityActiveInvisibleComponent>(Entity<AuraComponent>.op_Implicit(ent)) || !this.TryComp<SpriteComponent>(Entity<AuraComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    spriteComponent.PostShader = (ShaderInstance) null;
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<AuraComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AuraComponent, SpriteComponent>();
    EntityUid entityUid;
    AuraComponent auraComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref auraComponent, ref spriteComponent))
    {
      spriteComponent.PostShader?.SetParameter("outline_color", auraComponent.Color);
      spriteComponent.PostShader?.SetParameter("outline_width", auraComponent.OutlineWidth);
    }
  }
}
