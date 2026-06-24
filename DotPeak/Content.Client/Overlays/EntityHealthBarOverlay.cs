// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.EntityHealthBarOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.StatusIcon;
using Content.Client.UserInterface.Systems;
using Content.Shared._RMC14.CrashLand;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.ParaDrop;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Overlays;

public sealed class EntityHealthBarOverlay : Overlay
{
  private readonly IEntityManager _entManager;
  private readonly IPrototypeManager _prototype;
  private readonly SharedTransformSystem _transform;
  private readonly MobStateSystem _mobStateSystem;
  private readonly MobThresholdSystem _mobThresholdSystem;
  private readonly StatusIconSystem _statusIconSystem;
  private readonly SpriteSystem _spriteSystem;
  private readonly ProgressColorSystem _progressColor;
  private readonly EntityQuery<CrashLandingComponent> _crashLandingQuery;
  private readonly EntityQuery<ParaDroppingComponent> _paraDroppingQuery;
  public HashSet<string> DamageContainers = new HashSet<string>();
  public ProtoId<HealthIconPrototype>? StatusIcon;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public EntityHealthBarOverlay(IEntityManager entManager, IPrototypeManager prototype)
  {
    this._entManager = entManager;
    this._prototype = prototype;
    this._transform = this._entManager.System<SharedTransformSystem>();
    this._mobStateSystem = this._entManager.System<MobStateSystem>();
    this._mobThresholdSystem = this._entManager.System<MobThresholdSystem>();
    this._statusIconSystem = this._entManager.System<StatusIconSystem>();
    this._spriteSystem = this._entManager.System<SpriteSystem>();
    this._progressColor = this._entManager.System<ProgressColorSystem>();
    this._crashLandingQuery = this._entManager.GetEntityQuery<CrashLandingComponent>();
    this._paraDroppingQuery = this._entManager.GetEntityQuery<ParaDroppingComponent>();
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IEye eye = args.Viewport.Eye;
    Angle angle = eye != null ? eye.Rotation : Angle.Zero;
    EntityQuery<TransformComponent> entityQuery = this._entManager.GetEntityQuery<TransformComponent>();
    Vector2 vector2_1 = new Vector2(1f, 1f);
    Matrix3x2 scale = Matrix3Helpers.CreateScale(ref vector2_1);
    Matrix3x2 rotation = Matrix3Helpers.CreateRotation(Angle.op_Implicit(Angle.op_UnaryNegation(angle)));
    HealthIconPrototype data;
    this._prototype.TryIndex<HealthIconPrototype>(this.StatusIcon, ref data);
    AllEntityQueryEnumerator<MobThresholdsComponent, MobStateComponent, DamageableComponent, SpriteComponent> entityQueryEnumerator = this._entManager.AllEntityQueryEnumerator<MobThresholdsComponent, MobStateComponent, DamageableComponent, SpriteComponent>();
    EntityUid uid;
    MobThresholdsComponent thresholds;
    MobStateComponent component;
    DamageableComponent dmg;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref uid, ref thresholds, ref component, ref dmg, ref spriteComponent))
    {
      TransformComponent transformComponent;
      if ((data == null || this._statusIconSystem.IsVisible(Entity<MetaDataComponent>.op_Implicit((uid, this._entManager.GetComponent<MetaDataComponent>(uid))), (StatusIconData) data)) && entityQuery.TryGetComponent(uid, ref transformComponent) && !MapId.op_Inequality(transformComponent.MapID, args.MapId) && dmg.DamageContainerID.HasValue)
      {
        HashSet<string> damageContainers = this.DamageContainers;
        ProtoId<DamageContainerPrototype>? damageContainerId = dmg.DamageContainerID;
        string str = damageContainerId.HasValue ? ProtoId<DamageContainerPrototype>.op_Implicit(damageContainerId.GetValueOrDefault()) : (string) null;
        if (damageContainers.Contains(str))
        {
          Box2 box2_1 = (Box2?) EntityManagerExt.GetComponentOrNull<StatusIconComponent>(this._entManager, uid)?.Bounds ?? this._spriteSystem.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)));
          Vector2 worldPosition = this._transform.GetWorldPosition(transformComponent, entityQuery);
          Box2 box2_2 = ((Box2) ref box2_1).Translated(worldPosition);
          if (((Box2) ref box2_2).Intersects(ref args.WorldAABB))
          {
            (float ratio, bool inCrit)? nullable = this.CalcProgress(uid, component, dmg, thresholds);
            if (nullable.HasValue)
            {
              (float ratio, bool inCrit) valueOrDefault = nullable.GetValueOrDefault();
              Matrix3x2 translation = Matrix3Helpers.CreateTranslation(this._transform.GetWorldPosition(transformComponent));
              Matrix3x2 matrix3x2_1 = Matrix3x2.Multiply(scale, translation);
              Matrix3x2 matrix3x2_2 = Matrix3x2.Multiply(rotation, matrix3x2_1);
              ((DrawingHandleBase) worldHandle).SetTransform(ref matrix3x2_2);
              float num = (float) ((double) ((Box2) ref box2_1).Height * 32.0 / 2.0 - 3.0);
              float x1 = ((Box2) ref box2_1).Width * 32f;
              Vector2 vector2_2 = new Vector2((float) (-(double) x1 / 32.0 / 2.0), num / 32f);
              Color progressColor = this.GetProgressColor(valueOrDefault.ratio, valueOrDefault.inCrit);
              if (this._crashLandingQuery.HasComp(uid) || this._paraDroppingQuery.HasComp(uid))
              {
                float y = 0.4f + spriteComponent.Offset.Y;
                x1 = spriteComponent.Offset.X;
                vector2_2 = new Vector2(x1, y);
              }
              float x2 = x1 - 8f;
              float x3 = (float) (((double) x2 - 8.0) * (double) valueOrDefault.ratio + 8.0);
              Box2 box2_3;
              // ISSUE: explicit constructor call
              ((Box2) ref box2_3).\u002Ector(new Vector2(8f, 0.0f) / 32f, new Vector2(x2, 3f) / 32f);
              box2_3 = ((Box2) ref box2_3).Translated(vector2_2);
              DrawingHandleWorld drawingHandleWorld1 = worldHandle;
              Box2 box2_4 = box2_3;
              Color black = Color.Black;
              Color color1 = ((Color) ref black).WithAlpha((byte) 192 /*0xC0*/);
              drawingHandleWorld1.DrawRect(box2_4, color1, true);
              Box2 box2_5;
              // ISSUE: explicit constructor call
              ((Box2) ref box2_5).\u002Ector(new Vector2(8f, 0.0f) / 32f, new Vector2(x3, 3f) / 32f);
              box2_5 = ((Box2) ref box2_5).Translated(vector2_2);
              worldHandle.DrawRect(box2_5, progressColor, true);
              Box2 box2_6;
              // ISSUE: explicit constructor call
              ((Box2) ref box2_6).\u002Ector(new Vector2(8f, 2f) / 32f, new Vector2(x3, 3f) / 32f);
              box2_6 = ((Box2) ref box2_6).Translated(vector2_2);
              DrawingHandleWorld drawingHandleWorld2 = worldHandle;
              Box2 box2_7 = box2_6;
              black = Color.Black;
              Color color2 = ((Color) ref black).WithAlpha((byte) 128 /*0x80*/);
              drawingHandleWorld2.DrawRect(box2_7, color2, true);
            }
          }
        }
      }
    }
    DrawingHandleWorld drawingHandleWorld = worldHandle;
    Matrix3x2 identity = Matrix3x2.Identity;
    ref Matrix3x2 local = ref identity;
    ((DrawingHandleBase) drawingHandleWorld).SetTransform(ref local);
  }

  private (float ratio, bool inCrit)? CalcProgress(
    EntityUid uid,
    MobStateComponent component,
    DamageableComponent dmg,
    MobThresholdsComponent thresholds)
  {
    if (this._mobStateSystem.IsAlive(uid, component))
    {
      FixedPoint2? nullable;
      if (dmg.HealthBarThreshold.HasValue)
      {
        FixedPoint2 totalDamage = dmg.TotalDamage;
        nullable = dmg.HealthBarThreshold;
        if ((nullable.HasValue ? (totalDamage < nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          return new (float, bool)?();
      }
      FixedPoint2? threshold;
      if (!this._mobThresholdSystem.TryGetThresholdForState(uid, MobState.Critical, out threshold, thresholds) && !this._mobThresholdSystem.TryGetThresholdForState(uid, MobState.Dead, out threshold, thresholds))
        return new (float, bool)?((1f, false));
      FixedPoint2 totalDamage1 = dmg.TotalDamage;
      nullable = threshold;
      return new (float, bool)?(((float) (1.0 - (double) (nullable.HasValue ? new FixedPoint2?(totalDamage1 / nullable.GetValueOrDefault()) : new FixedPoint2?()).Value.Float()), false));
    }
    if (!this._mobStateSystem.IsCritical(uid, component))
      return new (float, bool)?((0.0f, true));
    FixedPoint2? threshold1;
    FixedPoint2? threshold2;
    if (!this._mobThresholdSystem.TryGetThresholdForState(uid, MobState.Critical, out threshold1, thresholds) || !this._mobThresholdSystem.TryGetThresholdForState(uid, MobState.Dead, out threshold2, thresholds))
      return new (float, bool)?((1f, true));
    FixedPoint2 totalDamage2 = dmg.TotalDamage;
    FixedPoint2? nullable1 = threshold1;
    FixedPoint2? nullable2 = nullable1.HasValue ? new FixedPoint2?(totalDamage2 - nullable1.GetValueOrDefault()) : new FixedPoint2?();
    nullable1 = threshold2;
    FixedPoint2? nullable3 = threshold1;
    FixedPoint2? nullable4 = nullable1.HasValue & nullable3.HasValue ? new FixedPoint2?(nullable1.GetValueOrDefault() - nullable3.GetValueOrDefault()) : new FixedPoint2?();
    return new (float, bool)?(((float) (1.0 - (double) (nullable2.HasValue & nullable4.HasValue ? new FixedPoint2?(nullable2.GetValueOrDefault() / nullable4.GetValueOrDefault()) : new FixedPoint2?()).Value.Float()), true));
  }

  public Color GetProgressColor(float progress, bool crit)
  {
    if (crit)
      progress = 0.0f;
    return this._progressColor.GetProgressColor(progress);
  }
}
