// Decompiled with JetBrains decompiler
// Type: Content.Client.Interactable.Components.InteractionOutlineComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Interactable.Components;

[RegisterComponent]
public sealed class InteractionOutlineComponent : 
  Component,
  ISerializationGenerated<InteractionOutlineComponent>,
  ISerializationGenerated
{
  private static readonly ProtoId<ShaderPrototype> ShaderInRange = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutlineInrange");
  private static readonly ProtoId<ShaderPrototype> ShaderOutOfRange = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutline");
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IEntityManager _entMan;
  private const float DefaultWidth = 1f;
  private bool _inRange;
  private ShaderInstance? _shader;
  private int _lastRenderScale;

  public void OnMouseEnter(EntityUid uid, bool inInteractionRange, int renderScale)
  {
    this._lastRenderScale = renderScale;
    this._inRange = inInteractionRange;
    SpriteComponent spriteComponent;
    if (!this._entMan.TryGetComponent<SpriteComponent>(uid, ref spriteComponent) || spriteComponent.PostShader != null)
      return;
    this._shader = this.MakeNewShader(inInteractionRange, renderScale);
    spriteComponent.PostShader = this._shader;
  }

  public void OnMouseLeave(EntityUid uid)
  {
    SpriteComponent spriteComponent;
    if (this._entMan.TryGetComponent<SpriteComponent>(uid, ref spriteComponent))
    {
      if (spriteComponent.PostShader == this._shader)
        spriteComponent.PostShader = (ShaderInstance) null;
      spriteComponent.RenderOrder = 0U;
    }
    this._shader?.Dispose();
    this._shader = (ShaderInstance) null;
  }

  public void UpdateInRange(EntityUid uid, bool inInteractionRange, int renderScale)
  {
    SpriteComponent spriteComponent;
    if (!this._entMan.TryGetComponent<SpriteComponent>(uid, ref spriteComponent) || spriteComponent.PostShader != this._shader || inInteractionRange == this._inRange && this._lastRenderScale == renderScale)
      return;
    this._inRange = inInteractionRange;
    this._lastRenderScale = renderScale;
    this._shader = this.MakeNewShader(this._inRange, this._lastRenderScale);
    spriteComponent.PostShader = this._shader;
  }

  private ShaderInstance MakeNewShader(bool inRange, int renderScale)
  {
    ShaderInstance shaderInstance = this._prototypeManager.Index<ShaderPrototype>(inRange ? InteractionOutlineComponent.ShaderInRange : InteractionOutlineComponent.ShaderOutOfRange).InstanceUnique();
    shaderInstance.SetParameter("outline_width", 1f * (float) renderScale);
    return shaderInstance;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InteractionOutlineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (InteractionOutlineComponent) component;
    serialization.TryCustomCopy<InteractionOutlineComponent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InteractionOutlineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InteractionOutlineComponent target1 = (InteractionOutlineComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InteractionOutlineComponent target1 = (InteractionOutlineComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    InteractionOutlineComponent target1 = (InteractionOutlineComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual InteractionOutlineComponent Component.Instantiate() => new InteractionOutlineComponent();
}
