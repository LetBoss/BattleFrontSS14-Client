// Decompiled with JetBrains decompiler
// Type: Content.Client.Movement.Systems.FloorOcclusionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Movement.Systems;

public sealed class FloorOcclusionSystem : SharedFloorOcclusionSystem
{
  private static readonly ProtoId<ShaderPrototype> HorizontalCut = ProtoId<ShaderPrototype>.op_Implicit(nameof (HorizontalCut));
  [Dependency]
  private IPrototypeManager _proto;
  private readonly Dictionary<EntityUid, ProtoId<ShaderPrototype>> _appliedShaders = new Dictionary<EntityUid, ProtoId<ShaderPrototype>>();
  private EntityQuery<FloorOccluderShaderComponent> _occluderShaderQuery;
  private EntityQuery<SpriteComponent> _spriteQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._occluderShaderQuery = this.GetEntityQuery<FloorOccluderShaderComponent>();
    this._spriteQuery = this.GetEntityQuery<SpriteComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FloorOcclusionComponent, ComponentStartup>(new EntityEventRefHandler<FloorOcclusionComponent, ComponentStartup>((object) this, __methodptr(OnOcclusionStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FloorOcclusionComponent, ComponentShutdown>(new EntityEventRefHandler<FloorOcclusionComponent, ComponentShutdown>((object) this, __methodptr(OnOcclusionShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FloorOcclusionComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<FloorOcclusionComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnOcclusionAuto)), (Type[]) null, (Type[]) null);
  }

  private void OnOcclusionAuto(
    Entity<FloorOcclusionComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    ProtoId<ShaderPrototype> occlusionShader = this.GetOcclusionShader(ent);
    this.SetShader(Entity<SpriteComponent>.op_Implicit(ent.Owner), occlusionShader, ent.Comp.Enabled);
  }

  private void OnOcclusionStartup(Entity<FloorOcclusionComponent> ent, ref ComponentStartup args)
  {
    ProtoId<ShaderPrototype> occlusionShader = this.GetOcclusionShader(ent);
    this.SetShader(Entity<SpriteComponent>.op_Implicit(ent.Owner), occlusionShader, ent.Comp.Enabled);
  }

  private void OnOcclusionShutdown(Entity<FloorOcclusionComponent> ent, ref ComponentShutdown args)
  {
    this.SetShader(Entity<SpriteComponent>.op_Implicit(ent.Owner), FloorOcclusionSystem.HorizontalCut, false);
  }

  protected override void SetEnabled(Entity<FloorOcclusionComponent> entity)
  {
    ProtoId<ShaderPrototype> occlusionShader = this.GetOcclusionShader(entity);
    this.SetShader(Entity<SpriteComponent>.op_Implicit(entity.Owner), occlusionShader, entity.Comp.Enabled);
  }

  private ProtoId<ShaderPrototype> GetOcclusionShader(Entity<FloorOcclusionComponent> entity)
  {
    foreach (EntityUid entityUid in entity.Comp.Colliding)
    {
      FloorOccluderShaderComponent occluderShaderComponent;
      if (this._occluderShaderQuery.TryComp(entityUid, ref occluderShaderComponent))
        return ProtoId<ShaderPrototype>.op_Implicit(occluderShaderComponent.Shader);
    }
    return FloorOcclusionSystem.HorizontalCut;
  }

  private void SetShader(
    Entity<SpriteComponent?> sprite,
    ProtoId<ShaderPrototype> shaderId,
    bool enabled)
  {
    if (!this._spriteQuery.Resolve(sprite.Owner, ref sprite.Comp, false))
      return;
    if (enabled)
    {
      ShaderInstance shaderInstance = this._proto.Index<ShaderPrototype>(shaderId).Instance();
      if (sprite.Comp.PostShader != null && !this._appliedShaders.ContainsKey(sprite.Owner) && sprite.Comp.PostShader != shaderInstance)
        return;
      sprite.Comp.PostShader = shaderInstance;
      this._appliedShaders[sprite.Owner] = shaderId;
    }
    else
    {
      ProtoId<ShaderPrototype> protoId;
      if (!this._appliedShaders.TryGetValue(sprite.Owner, out protoId))
        return;
      if (sprite.Comp.PostShader == this._proto.Index<ShaderPrototype>(protoId).Instance())
        sprite.Comp.PostShader = (ShaderInstance) null;
      this._appliedShaders.Remove(sprite.Owner);
    }
  }
}
