// Decompiled with JetBrains decompiler
// Type: Content.Client.AlertLevel.AlertLevelDisplaySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.AlertLevel;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Linq;

#nullable enable
namespace Content.Client.AlertLevel;

public sealed class AlertLevelDisplaySystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AlertLevelDisplayComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<AlertLevelDisplayComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    AlertLevelDisplayComponent alertLevelDisplay,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    int num = this._sprite.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) AlertLevelDisplay.Layer);
    object obj;
    if (args.AppearanceData.TryGetValue((Enum) AlertLevelDisplay.Powered, out obj))
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, obj is bool flag && flag);
    object key;
    if (!args.AppearanceData.TryGetValue((Enum) AlertLevelDisplay.CurrentLevel, out key))
    {
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(alertLevelDisplay.AlertVisuals.Values.First<string>()));
    }
    else
    {
      string str;
      if (alertLevelDisplay.AlertVisuals.TryGetValue((string) key, out str))
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(str));
      else
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit(alertLevelDisplay.AlertVisuals.Values.First<string>()));
    }
  }
}
