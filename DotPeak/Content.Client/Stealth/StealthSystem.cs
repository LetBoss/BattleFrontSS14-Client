// Decompiled with JetBrains decompiler
// Type: Content.Client.Stealth.StealthSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Interactable.Components;
using Content.Shared.Stealth;
using Content.Shared.Stealth.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Stealth;

public sealed class StealthSystem : SharedStealthSystem
{
  private static readonly ProtoId<ShaderPrototype> Shader = ProtoId<ShaderPrototype>.op_Implicit("Stealth");
  [Dependency]
  private IPrototypeManager _protoMan;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  [Dependency]
  private SpriteSystem _sprite;
  private ShaderInstance _shader;

  public override void Initialize()
  {
    base.Initialize();
    this._shader = this._protoMan.Index<ShaderPrototype>(StealthSystem.Shader).InstanceUnique();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StealthComponent, ComponentShutdown>(new ComponentEventHandler<StealthComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StealthComponent, ComponentStartup>(new ComponentEventHandler<StealthComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StealthComponent, BeforePostShaderRenderEvent>(new ComponentEventHandler<StealthComponent, BeforePostShaderRenderEvent>((object) this, __methodptr(OnShaderRender)), (Type[]) null, (Type[]) null);
  }

  public override void SetEnabled(EntityUid uid, bool value, StealthComponent? component = null)
  {
    if (!this.Resolve<StealthComponent>(uid, ref component, true) || component.Enabled == value)
      return;
    base.SetEnabled(uid, value, component);
    this.SetShader(uid, value, component);
  }

  private void SetShader(
    EntityUid uid,
    bool enabled,
    StealthComponent? component = null,
    SpriteComponent? sprite = null)
  {
    if (!this.Resolve<StealthComponent, SpriteComponent>(uid, ref component, ref sprite, false))
      return;
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, sprite)), Color.White);
    sprite.PostShader = enabled ? this._shader : (ShaderInstance) null;
    sprite.GetScreenTexture = enabled;
    sprite.RaiseShaderEvent = enabled;
    if (!enabled)
    {
      if (!component.HadOutline || this.TerminatingOrDeleted(uid, (MetaDataComponent) null))
        return;
      this.EnsureComp<InteractionOutlineComponent>(uid);
    }
    else
    {
      InteractionOutlineComponent outlineComponent;
      if (!this.TryComp<InteractionOutlineComponent>(uid, ref outlineComponent))
        return;
      this.RemCompDeferred(uid, (IComponent) outlineComponent);
      component.HadOutline = true;
    }
  }

  private void OnStartup(EntityUid uid, StealthComponent component, ComponentStartup args)
  {
    this.SetShader(uid, component.Enabled, component);
  }

  private void OnShutdown(EntityUid uid, StealthComponent component, ComponentShutdown args)
  {
    if (this.Terminating(uid, (MetaDataComponent) null))
      return;
    this.SetShader(uid, false, component);
  }

  private void OnShaderRender(
    EntityUid uid,
    StealthComponent component,
    BeforePostShaderRenderEvent args)
  {
    EntityUid parentUid = this.Transform(uid).ParentUid;
    if (!((EntityUid) ref parentUid).IsValid())
      return;
    TransformComponent transformComponent = this.Transform(parentUid);
    Vector2 local = args.Viewport.WorldToLocal(this._transformSystem.GetWorldPosition(transformComponent));
    local.X = -local.X;
    float y = Math.Clamp(this.GetVisibility(uid, component), -1f, 1f);
    this._shader.SetParameter("reference", local);
    this._shader.SetParameter("visibility", y);
    float num = MathF.Max(0.0f, y);
    this._sprite.SetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), new Color(num, num, 1f, 1f));
  }
}
