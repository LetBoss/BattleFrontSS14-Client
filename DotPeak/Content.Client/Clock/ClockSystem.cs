// Decompiled with JetBrains decompiler
// Type: Content.Client.Clock.ClockSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Clock;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Clock;

public sealed class ClockSystem : SharedClockSystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<ClockComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ClockComponent, SpriteComponent>();
    EntityUid entityUid;
    ClockComponent clockComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref clockComponent, ref spriteComponent))
    {
      int num1;
      int num2;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) ClockVisualLayers.HourHand, ref num1, false) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) ClockVisualLayers.MinuteHand, ref num2, false))
      {
        TimeSpan clockTime = this.GetClockTime(Entity<ClockComponent>.op_Implicit((entityUid, clockComponent)));
        string str1 = $"{clockComponent.HoursBase}{clockTime.Hours % 12}";
        string str2 = $"{clockComponent.MinutesBase}{clockTime.Minutes / 5}";
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num1, RSI.StateId.op_Implicit(str1));
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num2, RSI.StateId.op_Implicit(str2));
      }
    }
  }
}
