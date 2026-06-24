// Decompiled with JetBrains decompiler
// Type: Content.Client.Alerts.GenericCounterAlertSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Alert.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Alerts;

public sealed class GenericCounterAlertSystem : EntitySystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GenericCounterAlertComponent, UpdateAlertSpriteEvent>(new EntityEventRefHandler<GenericCounterAlertComponent, UpdateAlertSpriteEvent>((object) this, __methodptr(OnUpdateAlertSprite)), (Type[]) null, (Type[]) null);
  }

  private void OnUpdateAlertSprite(
    Entity<GenericCounterAlertComponent> ent,
    ref UpdateAlertSpriteEvent args)
  {
    SpriteComponent comp = args.SpriteViewEnt.Comp;
    GetGenericAlertCounterAmountEvent counterAmountEvent = new GetGenericAlertCounterAmountEvent(args.Alert);
    this.RaiseLocalEvent<GetGenericAlertCounterAmountEvent>(args.ViewerEnt, ref counterAmountEvent, false);
    if (!counterAmountEvent.Handled)
      return;
    int? amount = counterAmountEvent.Amount;
    if (!amount.HasValue)
      return;
    int maxDigitCount = this.GetMaxDigitCount(Entity<GenericCounterAlertComponent, SpriteComponent>.op_Implicit((Entity<GenericCounterAlertComponent>.op_Implicit(ent), Entity<GenericCounterAlertComponent>.op_Implicit(ent), comp)));
    amount = counterAmountEvent.Amount;
    int num1 = (int) Math.Clamp((double) amount.Value, 0.0, Math.Pow(10.0, (double) maxDigitCount) - 1.0);
    int num2 = ent.Comp.HideLeadingZeroes ? num1.ToString().Length : maxDigitCount;
    if (ent.Comp.HideLeadingZeroes)
    {
      for (int index = 0; index < ent.Comp.DigitKeys.Count; ++index)
      {
        int num3;
        if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(ent.Owner), ent.Comp.DigitKeys[index], ref num3, false))
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(ent.Owner), num3, index <= num2 - 1);
      }
    }
    float num4 = (float) ((ent.Comp.AlertSize.X - num2 * ent.Comp.GlyphWidth) / 2) * (1f / 32f);
    for (int index = 0; index < ent.Comp.DigitKeys.Count; ++index)
    {
      int num5;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(ent.Owner), ent.Comp.DigitKeys[index], ref num5, false))
      {
        int num6 = num1 / (int) Math.Pow(10.0, (double) index) % 10;
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit(ent.Owner), num5, RSI.StateId.op_Implicit(num6.ToString()));
        if (ent.Comp.CenterGlyph)
        {
          float x = num4 + (float) ((num2 - 1 - index) * ent.Comp.GlyphWidth) * (1f / 32f);
          this._sprite.LayerSetOffset(Entity<SpriteComponent>.op_Implicit(ent.Owner), num5, new Vector2(x, 0.0f));
        }
      }
    }
  }

  private int GetMaxDigitCount(
    Entity<GenericCounterAlertComponent, SpriteComponent> ent)
  {
    for (int index = ent.Comp1.DigitKeys.Count - 1; index >= 0; --index)
    {
      if (this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((ent.Owner, ent.Comp2)), ent.Comp1.DigitKeys[index]))
        return index + 1;
    }
    return 0;
  }
}
