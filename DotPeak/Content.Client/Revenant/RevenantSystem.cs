// Decompiled with JetBrains decompiler
// Type: Content.Client.Revenant.RevenantSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Alert;
using Content.Shared.Alert.Components;
using Content.Shared.Revenant;
using Content.Shared.Revenant.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Revenant;

public sealed class RevenantSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RevenantComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<RevenantComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RevenantComponent, GetGenericAlertCounterAmountEvent>(new EntityEventRefHandler<RevenantComponent, GetGenericAlertCounterAmountEvent>((object) this, __methodptr(OnGetCounterAmount)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    RevenantComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    bool flag1;
    if (this._appearance.TryGetData<bool>(uid, (Enum) RevenantVisuals.Harvesting, ref flag1, args.Component) & flag1)
    {
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(component.HarvestingState));
    }
    else
    {
      bool flag2;
      if (this._appearance.TryGetData<bool>(uid, (Enum) RevenantVisuals.Stunned, ref flag2, args.Component) & flag2)
      {
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(component.StunnedState));
      }
      else
      {
        bool flag3;
        if (!this._appearance.TryGetData<bool>(uid, (Enum) RevenantVisuals.Corporeal, ref flag3, args.Component))
          return;
        if (flag3)
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(component.CorporealState));
        else
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), 0, RSI.StateId.op_Implicit(component.State));
      }
    }
  }

  private void OnGetCounterAmount(
    Entity<RevenantComponent> ent,
    ref GetGenericAlertCounterAmountEvent args)
  {
    if (args.Handled || ProtoId<AlertPrototype>.op_Inequality(ent.Comp.EssenceAlert, ProtoId<AlertPrototype>.op_Implicit(args.Alert)))
      return;
    args.Amount = new int?(ent.Comp.Essence.Int());
  }
}
