// Decompiled with JetBrains decompiler
// Type: Content.Client.Sprite.SpriteFadeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Shared.Sprite;
using Robust.Client.GameObjects;
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
namespace Content.Client.Sprite;

public sealed class SpriteFadeSystem : EntitySystem
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
  private EntityLookupSystem _lookup;
  private List<(MapCoordinates Point, bool ExcludeBoundingBox)> _points = new List<(MapCoordinates, bool)>();
  private readonly HashSet<FadingSpriteComponent> _comps = new HashSet<FadingSpriteComponent>();
  private readonly HashSet<Entity<SpriteFadeComponent>> _fadeEntities = new HashSet<Entity<SpriteFadeComponent>>();
  private EntityQuery<SpriteComponent> _spriteQuery;
  private EntityQuery<SpriteFadeComponent> _fadeQuery;
  private EntityQuery<FadingSpriteComponent> _fadingQuery;
  private EntityQuery<FixturesComponent> _fixturesQuery;
  private const float TargetAlpha = 0.4f;
  private const float ChangeRate = 1f;
  private const float MouseFadeRadius = 3f;

  public virtual void Initialize()
  {
    base.Initialize();
    this._spriteQuery = this.GetEntityQuery<SpriteComponent>();
    this._fadeQuery = this.GetEntityQuery<SpriteFadeComponent>();
    this._fadingQuery = this.GetEntityQuery<FadingSpriteComponent>();
    this._fixturesQuery = this.GetEntityQuery<FixturesComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FadingSpriteComponent, ComponentShutdown>(new ComponentEventHandler<FadingSpriteComponent, ComponentShutdown>((object) this, __methodptr(OnFadingShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnFadingShutdown(
    EntityUid uid,
    FadingSpriteComponent component,
    ComponentShutdown args)
  {
    SpriteComponent spriteComponent;
    if (this.MetaData(uid).EntityLifeStage >= 4 || !this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    SpriteSystem sprite = this._sprite;
    Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((uid, spriteComponent));
    Color color1 = spriteComponent.Color;
    Color color2 = ((Color) ref color1).WithAlpha(component.OriginalAlpha);
    sprite.SetColor(entity, color2);
  }

  private void FadeIn(float change)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    this._points.Clear();
    MapCoordinates? nullable1 = new MapCoordinates?();
    if (this._uiManager.CurrentlyHovered is IViewportControl currentlyHovered)
    {
      ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
      if (((ScreenCoordinates) ref mouseScreenPosition).IsValid)
        nullable1 = new MapCoordinates?(currentlyHovered.PixelToMap(this._inputManager.MouseScreenPosition.Position));
    }
    TransformComponent transformComponent1;
    EntityUid? nullable2;
    if (this.TryComp(localEntity, ref transformComponent1))
    {
      List<(MapCoordinates, bool)> points = this._points;
      SharedTransformSystem transform = this._transform;
      nullable2 = ((ISharedPlayerManager) this._playerManager).LocalEntity;
      EntityUid entityUid = nullable2.Value;
      TransformComponent transformComponent2 = transformComponent1;
      (MapCoordinates, bool) valueTuple = (transform.GetMapCoordinates(entityUid, transformComponent2), false);
      points.Add(valueTuple);
    }
    SpriteComponent spriteComponent1;
    if (!(this._stateManager.CurrentState is GameplayState currentState) || !this._spriteQuery.TryGetComponent(localEntity, ref spriteComponent1))
      return;
    this._fadeEntities.Clear();
    if (nullable1.HasValue)
      this._lookup.GetEntitiesInRange<SpriteFadeComponent>(nullable1.Value, 3f, this._fadeEntities, (LookupFlags) 110);
    EntityUid entityUid1;
    foreach ((MapCoordinates mapCoordinates, bool ExcludeBoundingBox) in this._points)
    {
      foreach (EntityUid clickableEntity in currentState.GetClickableEntities(mapCoordinates, false))
      {
        entityUid1 = clickableEntity;
        nullable2 = localEntity;
        SpriteComponent spriteComponent2;
        if ((nullable2.HasValue ? (EntityUid.op_Equality(entityUid1, nullable2.GetValueOrDefault()) ? 1 : 0) : 0) == 0 && this._fadeQuery.HasComponent(clickableEntity) && this._spriteQuery.TryGetComponent(clickableEntity, ref spriteComponent2) && spriteComponent2.DrawDepth >= spriteComponent1.DrawDepth)
        {
          FixturesComponent fixturesComponent;
          if (ExcludeBoundingBox && this._fixturesQuery.TryComp(clickableEntity, ref fixturesComponent))
          {
            Transform physicsTransform = this._physics.GetPhysicsTransform(clickableEntity, (TransformComponent) null);
            bool flag = false;
            foreach (Fixture fixture in fixturesComponent.Fixtures.Values)
            {
              if (fixture.Hard && this._fixtures.TestPoint<IPhysShape>(fixture.Shape, physicsTransform, mapCoordinates.Position))
              {
                flag = true;
                break;
              }
            }
            if (flag)
              continue;
          }
          FadingSpriteComponent fadingSpriteComponent;
          if (!this._fadingQuery.TryComp(clickableEntity, ref fadingSpriteComponent))
          {
            fadingSpriteComponent = this.AddComp<FadingSpriteComponent>(clickableEntity);
            fadingSpriteComponent.OriginalAlpha = spriteComponent2.Color.A;
          }
          this._comps.Add(fadingSpriteComponent);
          float num = Math.Max(spriteComponent2.Color.A - change, 0.4f);
          if (!spriteComponent2.Color.A.Equals(num))
          {
            SpriteSystem sprite = this._sprite;
            Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((clickableEntity, spriteComponent2));
            Color color1 = spriteComponent2.Color;
            Color color2 = ((Color) ref color1).WithAlpha(num);
            sprite.SetColor(entity, color2);
          }
        }
      }
    }
    foreach (Entity<SpriteFadeComponent> fadeEntity in this._fadeEntities)
    {
      SpriteFadeComponent spriteFadeComponent;
      fadeEntity.Deconstruct(ref entityUid1, ref spriteFadeComponent);
      EntityUid entityUid2 = entityUid1;
      entityUid1 = entityUid2;
      EntityUid? nullable3 = localEntity;
      SpriteComponent spriteComponent3;
      if ((nullable3.HasValue ? (EntityUid.op_Equality(entityUid1, nullable3.GetValueOrDefault()) ? 1 : 0) : 0) == 0 && this._spriteQuery.TryGetComponent(entityUid2, ref spriteComponent3) && spriteComponent3.DrawDepth >= spriteComponent1.DrawDepth)
      {
        FadingSpriteComponent fadingSpriteComponent;
        if (!this._fadingQuery.TryComp(entityUid2, ref fadingSpriteComponent))
        {
          fadingSpriteComponent = this.AddComp<FadingSpriteComponent>(entityUid2);
          fadingSpriteComponent.OriginalAlpha = spriteComponent3.Color.A;
        }
        this._comps.Add(fadingSpriteComponent);
        float num = Math.Max(spriteComponent3.Color.A - change, 0.4f);
        Color color3 = spriteComponent3.Color;
        if (!color3.A.Equals(num))
        {
          SpriteSystem sprite = this._sprite;
          Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent3));
          color3 = spriteComponent3.Color;
          Color color4 = ((Color) ref color3).WithAlpha(num);
          sprite.SetColor(entity, color4);
        }
      }
    }
  }

  private void FadeOut(float change)
  {
    AllEntityQueryEnumerator<FadingSpriteComponent> entityQueryEnumerator = this.AllEntityQuery<FadingSpriteComponent>();
    EntityUid entityUid;
    FadingSpriteComponent fadingSpriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref fadingSpriteComponent))
    {
      SpriteComponent spriteComponent;
      if (!this._comps.Contains(fadingSpriteComponent) && this._spriteQuery.TryGetComponent(entityUid, ref spriteComponent))
      {
        float num = Math.Min(spriteComponent.Color.A + change, fadingSpriteComponent.OriginalAlpha);
        if (!num.Equals(spriteComponent.Color.A))
        {
          SpriteSystem sprite = this._sprite;
          Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent));
          Color color1 = spriteComponent.Color;
          Color color2 = ((Color) ref color1).WithAlpha(num);
          sprite.SetColor(entity, color2);
        }
        else
          this.RemCompDeferred<FadingSpriteComponent>(entityUid);
      }
    }
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    float change = 1f * frameTime;
    this.FadeIn(change);
    this.FadeOut(change);
    this._comps.Clear();
  }
}
