// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.AlertLevel.RMCAlertLevelDisplayVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.AlertLevel;
using Content.Shared.Clock;
using Content.Shared.GameTicking;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.AlertLevel;

public sealed class RMCAlertLevelDisplayVisualizerSystem : EntitySystem
{
  [Dependency]
  private SharedGameTicker _ticker;
  [Dependency]
  private RMCAlertLevelSystem _alertLevel;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    RMCAlertLevels? nullable = this._alertLevel.Get();
    RMCAlertLevels rmcAlertLevels = RMCAlertLevels.Green;
    if (nullable.GetValueOrDefault() > rmcAlertLevels & nullable.HasValue)
      return;
    EntityQueryEnumerator<RMCAlertLevelDisplayComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCAlertLevelDisplayComponent, SpriteComponent>();
    EntityUid entityUid;
    RMCAlertLevelDisplayComponent displayComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref displayComponent, ref spriteComponent))
    {
      int num1;
      int num2;
      int num3;
      int num4;
      int num5;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) RMCAlertLevelDisplayVisualLayers.HourTens, ref num1, false) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) RMCAlertLevelDisplayVisualLayers.HourOnes, ref num2, false) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) RMCAlertLevelDisplayVisualLayers.Separator, ref num3, false) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) RMCAlertLevelDisplayVisualLayers.MinuteTens, ref num4, false) && this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) RMCAlertLevelDisplayVisualLayers.MinuteOnes, ref num5, false))
      {
        GlobalTimeManagerComponent managerComponent = this.EntityQuery<GlobalTimeManagerComponent>(false).FirstOrDefault<GlobalTimeManagerComponent>();
        string str1 = ((managerComponent != null ? managerComponent.TimeOffset : TimeSpan.Zero) + this._ticker.RoundDuration()).ToString("hh\\:mm");
        string str2 = $"{str1[0]}";
        string str3 = $"{str1[1]}";
        string str4 = "~";
        string str5 = $"{str1[3]}";
        string str6 = $"{str1[4]}";
        this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num1, new Vector2(0.11f, -7f / 16f));
        this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num2, new Vector2(0.28f, -7f / 16f));
        this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num3, new Vector2(0.406f, -7f / 16f));
        this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num4, new Vector2(0.56f, -7f / 16f));
        this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num5, new Vector2(0.73f, -7f / 16f));
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num1, RSI.StateId.op_Implicit(str2));
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num2, RSI.StateId.op_Implicit(str3));
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num3, RSI.StateId.op_Implicit(str4));
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num4, RSI.StateId.op_Implicit(str5));
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), num5, RSI.StateId.op_Implicit(str6));
      }
    }
  }
}
