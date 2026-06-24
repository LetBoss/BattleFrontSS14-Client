// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Effect.RMCEffectSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Effect;
using Content.Shared._RMC14.Stealth;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._RMC14.Effect;

public sealed class RMCEffectSystem : SharedRMCEffectSystem
{
  private const int OpacityDivider = 3;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void FrameUpdate(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    EntityQueryEnumerator<EffectAlphaAnimationComponent, SpriteComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<EffectAlphaAnimationComponent, SpriteComponent>();
    EntityUid entityUid1;
    EffectAlphaAnimationComponent animationComponent;
    SpriteComponent spriteComponent1;
    while (entityQueryEnumerator1.MoveNext(ref entityUid1, ref animationComponent, ref spriteComponent1))
    {
      TimeSpan? spawnedAt = animationComponent.SpawnedAt;
      if (spawnedAt.HasValue)
      {
        TimeSpan valueOrDefault = spawnedAt.GetValueOrDefault();
        double num = MathHelper.Lerp((valueOrDefault + animationComponent.Delay).TotalSeconds, valueOrDefault.TotalSeconds, curTime.TotalSeconds);
        SpriteSystem sprite = this._sprite;
        Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid1, spriteComponent1));
        Color color1 = spriteComponent1.Color;
        Color color2 = ((Color) ref color1).WithAlpha((float) num);
        sprite.SetColor(entity, color2);
      }
    }
    EntityQueryEnumerator<RMCEffectComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCEffectComponent>();
    EntityUid entityUid2;
    RMCEffectComponent rmcEffectComponent;
    while (entityQueryEnumerator2.MoveNext(ref entityUid2, ref rmcEffectComponent))
    {
      EntityUid parentUid = this.Transform(entityUid2).ParentUid;
      SpriteComponent spriteComponent2;
      SpriteComponent spriteComponent3;
      if (!this.TryComp<SpriteComponent>(parentUid, ref spriteComponent2) || !this.TryComp<SpriteComponent>(entityUid2, ref spriteComponent3))
        break;
      EntityActiveInvisibleComponent invisibleComponent;
      if (this.TryComp<EntityActiveInvisibleComponent>(parentUid, ref invisibleComponent) && (double) invisibleComponent.Opacity < 1.0)
      {
        SpriteSystem sprite = this._sprite;
        Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent3));
        Color color3 = spriteComponent3.Color;
        Color color4 = ((Color) ref color3).WithAlpha(invisibleComponent.Opacity / 3f);
        sprite.SetColor(entity, color4);
      }
      else if ((double) spriteComponent3.Color.A < 1.0)
      {
        SpriteSystem sprite = this._sprite;
        Entity<SpriteComponent> entity = Entity<SpriteComponent>.op_Implicit((entityUid2, spriteComponent3));
        Color color5 = spriteComponent3.Color;
        Color color6 = ((Color) ref color5).WithAlpha(spriteComponent2.Color.A / 3f);
        sprite.SetColor(entity, color6);
      }
    }
  }
}
