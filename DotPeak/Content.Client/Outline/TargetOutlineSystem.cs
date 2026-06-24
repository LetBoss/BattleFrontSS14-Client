// Decompiled with JetBrains decompiler
// Type: Content.Client.Outline.TargetOutlineSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Interaction;
using Content.Shared.Whitelist;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Outline;

public sealed class TargetOutlineSystem : EntitySystem
{
  private static readonly ProtoId<ShaderPrototype> ShaderTargetValid = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutlineInrange");
  private static readonly ProtoId<ShaderPrototype> ShaderTargetInvalid = ProtoId<ShaderPrototype>.op_Implicit("SelectionOutline");
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private SharedTransformSystem _transformSystem;
  private bool _enabled;
  public EntityWhitelist? Whitelist;
  public EntityWhitelist? Blacklist;
  public Func<EntityUid, bool>? Predicate;
  public CancellableEntityEventArgs? ValidationEvent;
  public float Range = -1f;
  public bool CheckObstruction = true;
  public float LookupSize = 1f;
  private ShaderInstance? _shaderTargetValid;
  private ShaderInstance? _shaderTargetInvalid;
  private readonly HashSet<SpriteComponent> _highlightedSprites = new HashSet<SpriteComponent>();

  private Vector2 LookupVector => new Vector2(this.LookupSize, this.LookupSize);

  public virtual void Initialize()
  {
    base.Initialize();
    this._shaderTargetValid = this._prototypeManager.Index<ShaderPrototype>(TargetOutlineSystem.ShaderTargetValid).InstanceUnique();
    this._shaderTargetInvalid = this._prototypeManager.Index<ShaderPrototype>(TargetOutlineSystem.ShaderTargetInvalid).InstanceUnique();
  }

  public void Disable()
  {
    if (!this._enabled)
      return;
    this._enabled = false;
    this.RemoveHighlights();
  }

  public void Enable(
    float range,
    bool checkObstructions,
    Func<EntityUid, bool>? predicate,
    EntityWhitelist? whitelist,
    EntityWhitelist? blacklist,
    CancellableEntityEventArgs? validationEvent)
  {
    this.Range = range;
    this.CheckObstruction = checkObstructions;
    this.Predicate = predicate;
    this.Whitelist = whitelist;
    this.Blacklist = blacklist;
    this.ValidationEvent = validationEvent;
    this._enabled = this.Predicate != null || this.Whitelist != null || this.Blacklist != null || this.ValidationEvent != null;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._enabled || !this._timing.IsFirstTimePredicted)
      return;
    this.HighlightTargets();
  }

  private void HighlightTargets()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    if (!((EntityUid) ref valueOrDefault).Valid)
      return;
    this.RemoveHighlights();
    Vector2 position = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition).Position;
    Box2 box2;
    // ISSUE: explicit constructor call
    ((Box2) ref box2).\u002Ector(position - this.LookupVector, position + this.LookupVector);
    HashSet<EntityUid> entitiesIntersecting = this._lookup.GetEntitiesIntersecting(this._eyeManager.CurrentEye.Position.MapId, box2, (LookupFlags) 5);
    EntityQuery<SpriteComponent> entityQuery = this.GetEntityQuery<SpriteComponent>();
    foreach (EntityUid uid in entitiesIntersecting)
    {
      SpriteComponent spriteComponent;
      if (entityQuery.TryGetComponent(uid, ref spriteComponent) && spriteComponent.Visible)
      {
        Func<EntityUid, bool> predicate = this.Predicate;
        bool flag = predicate == null || predicate(uid);
        if (flag && this.Whitelist != null)
          flag = this._whitelistSystem.IsWhitelistPass(this.Whitelist, uid);
        if (flag && this.ValidationEvent != null)
        {
          this.ValidationEvent.Uncancel();
          this.RaiseLocalEvent(uid, (object) this.ValidationEvent, false);
          flag = !this.ValidationEvent.Cancelled;
        }
        if (!flag)
        {
          if (this._highlightedSprites.Remove(spriteComponent) && (spriteComponent.PostShader == this._shaderTargetValid || spriteComponent.PostShader == this._shaderTargetInvalid))
          {
            spriteComponent.PostShader = (ShaderInstance) null;
            spriteComponent.RenderOrder = 0U;
          }
        }
        else
        {
          if (this.CheckObstruction)
            flag = this._interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(valueOrDefault), Entity<TransformComponent>.op_Implicit(uid), this.Range);
          else if ((double) this.Range >= 0.0)
            flag = (double) (this._transformSystem.GetWorldPosition(valueOrDefault) - this._transformSystem.GetWorldPosition(uid)).LengthSquared() <= (double) this.Range;
          if (spriteComponent.PostShader != null && spriteComponent.PostShader != this._shaderTargetValid && spriteComponent.PostShader != this._shaderTargetInvalid)
            break;
          spriteComponent.PostShader = flag ? this._shaderTargetValid : this._shaderTargetInvalid;
          spriteComponent.RenderOrder = this.EntityManager.CurrentTick.Value;
          this._highlightedSprites.Add(spriteComponent);
        }
      }
    }
  }

  private void RemoveHighlights()
  {
    foreach (SpriteComponent highlightedSprite in this._highlightedSprites)
    {
      if (highlightedSprite.PostShader == this._shaderTargetValid || highlightedSprite.PostShader == this._shaderTargetInvalid)
      {
        highlightedSprite.PostShader = (ShaderInstance) null;
        highlightedSprite.RenderOrder = 0U;
      }
    }
    this._highlightedSprites.Clear();
  }
}
