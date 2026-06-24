// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Sprite.RMCSpriteFadeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Shared._RMC14.Sprite;
using Content.Shared.Ghost;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Sprite;

public sealed class RMCSpriteFadeSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IStateManager _stateManager;
  [Dependency]
  private FixtureSystem _fixtures;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private IEyeManager _eyeManager;
  private List<(MapCoordinates Point, bool ExcludeBoundingBox)> _points = new List<(MapCoordinates, bool)>();
  private readonly HashSet<RMCFadingSpriteComponent> _comps = new HashSet<RMCFadingSpriteComponent>();
  private EntityQuery<SpriteComponent> _spriteQuery;
  private EntityQuery<RMCSpriteFadeComponent> _fadeQuery;
  private EntityQuery<RMCFadingSpriteComponent> _fadingQuery;
  private EntityQuery<FixturesComponent> _fixturesQuery;

  public virtual void Initialize()
  {
    base.Initialize();
    this._spriteQuery = this.GetEntityQuery<SpriteComponent>();
    this._fadeQuery = this.GetEntityQuery<RMCSpriteFadeComponent>();
    this._fadingQuery = this.GetEntityQuery<RMCFadingSpriteComponent>();
    this._fixturesQuery = this.GetEntityQuery<FixturesComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCFadingSpriteComponent, ComponentRemove>(new EntityEventRefHandler<RMCFadingSpriteComponent, ComponentRemove>((object) this, __methodptr(OnFadingRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnFadingRemove(Entity<RMCFadingSpriteComponent> entity, ref ComponentRemove args)
  {
    SpriteComponent spriteComponent;
    if (this.MetaData(Entity<RMCFadingSpriteComponent>.op_Implicit(entity)).EntityLifeStage >= 4 || !this.TryComp<SpriteComponent>(Entity<RMCFadingSpriteComponent>.op_Implicit(entity), ref spriteComponent))
      return;
    SpriteSystem sprite1 = this._sprite;
    Entity<SpriteComponent> entity1 = Entity<SpriteComponent>.op_Implicit((Entity<RMCFadingSpriteComponent>.op_Implicit(entity), spriteComponent));
    Color color1 = spriteComponent.Color;
    Color color2 = ((Color) ref color1).WithAlpha(entity.Comp.OriginalAlpha);
    sprite1.SetColor(entity1, color2);
    foreach ((string key, float num1) in entity.Comp.OriginalLayerAlphas)
    {
      int num2;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<RMCFadingSpriteComponent>.op_Implicit(entity), spriteComponent)), key, ref num2, true))
      {
        ISpriteLayer ispriteLayer = spriteComponent[num2];
        SpriteSystem sprite2 = this._sprite;
        Entity<SpriteComponent> entity2 = Entity<SpriteComponent>.op_Implicit((Entity<RMCFadingSpriteComponent>.op_Implicit(entity), spriteComponent));
        int num3 = num2;
        Color color3 = ispriteLayer.Color;
        Color color4 = ((Color) ref color3).WithAlpha(num1);
        sprite2.LayerSetColor(entity2, num3, color4);
      }
    }
  }

  private void FadeIn(float frameTime)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    this._points.Clear();
    if (this._uiManager.CurrentlyHovered is IViewportControl currentlyHovered)
    {
      ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
      if (((ScreenCoordinates) ref mouseScreenPosition).IsValid)
        this._points.Add((currentlyHovered.PixelToMap(this._inputManager.MouseScreenPosition.Position), true));
    }
    TransformComponent transformComponent;
    if (this.TryComp(localEntity, ref transformComponent))
      this._points.Add((this._transform.GetMapCoordinates(((ISharedPlayerManager) this._playerManager).LocalEntity.Value, transformComponent), false));
    SpriteComponent spriteComponent1;
    if (!(this._stateManager.CurrentState is GameplayState currentState) || !this._spriteQuery.TryGetComponent(localEntity, ref spriteComponent1))
      return;
    bool flag1 = localEntity.HasValue && this.HasComp<GhostComponent>(localEntity.Value);
    foreach ((MapCoordinates mapCoordinates, bool ExcludeBoundingBox) in this._points)
    {
      foreach (EntityUid clickableEntity in currentState.GetClickableEntities(mapCoordinates, this._eyeManager.CurrentEye, false, true))
      {
        EntityUid entityUid = clickableEntity;
        EntityUid? nullable = localEntity;
        SpriteComponent spriteComponent2;
        if ((nullable.HasValue ? (EntityUid.op_Equality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0 && this._fadeQuery.HasComponent(clickableEntity) && this._spriteQuery.TryGetComponent(clickableEntity, ref spriteComponent2) && (flag1 || spriteComponent2.DrawDepth >= spriteComponent1.DrawDepth))
        {
          FixturesComponent fixturesComponent;
          if (ExcludeBoundingBox && this._fixturesQuery.TryComp(clickableEntity, ref fixturesComponent))
          {
            Transform physicsTransform = this._physics.GetPhysicsTransform(clickableEntity, (TransformComponent) null);
            bool flag2 = false;
            foreach (Fixture fixture in fixturesComponent.Fixtures.Values)
            {
              if (fixture.Hard && this._fixtures.TestPoint<IPhysShape>(fixture.Shape, physicsTransform, mapCoordinates.Position))
              {
                flag2 = true;
                break;
              }
            }
            if (flag2)
              continue;
          }
          RMCSpriteFadeComponent component = this._fadeQuery.GetComponent(clickableEntity);
          if (!ExcludeBoundingBox || component.ReactToMouse)
          {
            RMCFadingSpriteComponent fadingSpriteComponent;
            if (!this._fadingQuery.TryComp(clickableEntity, ref fadingSpriteComponent))
            {
              fadingSpriteComponent = this.AddComp<RMCFadingSpriteComponent>(clickableEntity);
              fadingSpriteComponent.OriginalAlpha = spriteComponent2.Color.A;
            }
            this._comps.Add(fadingSpriteComponent);
            float targetAlpha = component.TargetAlpha;
            float num1 = component.ChangeRate * frameTime;
            if (component.FadeLayers.Count > 0)
            {
              foreach (string fadeLayer in component.FadeLayers)
              {
                int num2;
                if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((clickableEntity, spriteComponent2)), fadeLayer, ref num2, true))
                {
                  ISpriteLayer ispriteLayer = spriteComponent2[num2];
                  if (!fadingSpriteComponent.OriginalLayerAlphas.ContainsKey(fadeLayer))
                    fadingSpriteComponent.OriginalLayerAlphas[fadeLayer] = ispriteLayer.Color.A;
                  float num3 = Math.Max(ispriteLayer.Color.A - num1, targetAlpha);
                  Color color1 = ispriteLayer.Color;
                  if (!color1.A.Equals(num3))
                  {
                    SpriteSystem sprite = this._sprite;
                    Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((clickableEntity, spriteComponent2));
                    int num4 = num2;
                    color1 = ispriteLayer.Color;
                    Color color2 = ((Color) ref color1).WithAlpha(num3);
                    sprite.LayerSetColor(entity, num4, color2);
                  }
                }
              }
            }
            else
            {
              float num5 = Math.Max(spriteComponent2.Color.A - num1, targetAlpha);
              Color color3 = spriteComponent2.Color;
              if (!color3.A.Equals(num5))
              {
                SpriteSystem sprite = this._sprite;
                Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((clickableEntity, spriteComponent2));
                color3 = spriteComponent2.Color;
                Color color4 = ((Color) ref color3).WithAlpha(num5);
                sprite.SetColor(entity, color4);
              }
            }
          }
        }
      }
    }
  }

  private void FadeOut(float frameTime)
  {
    AllEntityQueryEnumerator<RMCFadingSpriteComponent> entityQueryEnumerator = this.AllEntityQuery<RMCFadingSpriteComponent>();
    EntityUid entityUid;
    RMCFadingSpriteComponent fadingSpriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref fadingSpriteComponent))
    {
      SpriteComponent spriteComponent;
      RMCSpriteFadeComponent spriteFadeComponent;
      if (!this._comps.Contains(fadingSpriteComponent) && this._spriteQuery.TryGetComponent(entityUid, ref spriteComponent) && this._fadeQuery.TryComp(entityUid, ref spriteFadeComponent))
      {
        float num1 = spriteFadeComponent.ChangeRate * frameTime;
        if (spriteFadeComponent.FadeLayers.Count > 0)
        {
          bool flag = true;
          foreach ((string key, float val2) in fadingSpriteComponent.OriginalLayerAlphas)
          {
            int num2;
            if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), key, ref num2, true))
            {
              ISpriteLayer ispriteLayer = spriteComponent[num2];
              float num3 = Math.Min(ispriteLayer.Color.A + num1, val2);
              if (!num3.Equals(ispriteLayer.Color.A))
              {
                SpriteSystem sprite = this._sprite;
                Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent));
                int num4 = num2;
                Color color1 = ispriteLayer.Color;
                Color color2 = ((Color) ref color1).WithAlpha(num3);
                sprite.LayerSetColor(entity, num4, color2);
                flag = false;
              }
            }
          }
          if (flag)
            this.RemCompDeferred<RMCFadingSpriteComponent>(entityUid);
        }
        else
        {
          float num5 = Math.Min(spriteComponent.Color.A + num1, fadingSpriteComponent.OriginalAlpha);
          if (!num5.Equals(spriteComponent.Color.A))
          {
            SpriteSystem sprite = this._sprite;
            Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent));
            Color color3 = spriteComponent.Color;
            Color color4 = ((Color) ref color3).WithAlpha(num5);
            sprite.SetColor(entity, color4);
          }
          else
            this.RemCompDeferred<RMCFadingSpriteComponent>(entityUid);
        }
      }
    }
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    this.FadeIn(frameTime);
    this.FadeOut(frameTime);
    this._comps.Clear();
  }
}
