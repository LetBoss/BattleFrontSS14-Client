// Decompiled with JetBrains decompiler
// Type: Content.Shared.Dice.SharedDiceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Dice;

public abstract class SharedDiceSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DiceComponent, UseInHandEvent>(new EntityEventRefHandler<DiceComponent, UseInHandEvent>((object) this, __methodptr(OnUseInHand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DiceComponent, LandEvent>(new EntityEventRefHandler<DiceComponent, LandEvent>((object) this, __methodptr(OnLand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DiceComponent, ExaminedEvent>(new EntityEventRefHandler<DiceComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
  }

  private void OnUseInHand(Entity<DiceComponent> entity, ref UseInHandEvent args)
  {
    if (args.Handled)
      return;
    this.Roll(entity, new EntityUid?(args.User));
    args.Handled = true;
  }

  private void OnLand(Entity<DiceComponent> entity, ref LandEvent args) => this.Roll(entity);

  private void OnExamined(Entity<DiceComponent> entity, ref ExaminedEvent args)
  {
    using (args.PushGroup("DiceComponent"))
    {
      args.PushMarkup(this.Loc.GetString("dice-component-on-examine-message-part-1", ("sidesAmount", (object) entity.Comp.Sides)));
      args.PushMarkup(this.Loc.GetString("dice-component-on-examine-message-part-2", ("currentSide", (object) entity.Comp.CurrentValue)));
    }
  }

  private void SetCurrentSide(Entity<DiceComponent> entity, int side)
  {
    if (side < 1 || side > entity.Comp.Sides)
    {
      this.Log.Error($"Attempted to set die {this.ToPrettyString(new EntityUid?(Entity<DiceComponent>.op_Implicit(entity)), (MetaDataComponent) null)} to an invalid side ({side}).");
    }
    else
    {
      entity.Comp.CurrentValue = (side - entity.Comp.Offset) * entity.Comp.Multiplier;
      this.Dirty<DiceComponent>(entity, (MetaDataComponent) null);
    }
  }

  public void SetCurrentValue(Entity<DiceComponent> entity, int value)
  {
    if (value % entity.Comp.Multiplier != 0 || value / entity.Comp.Multiplier + entity.Comp.Offset < 1)
      this.Log.Error($"Attempted to set die {this.ToPrettyString(new EntityUid?(Entity<DiceComponent>.op_Implicit(entity)), (MetaDataComponent) null)} to an invalid value ({value}).");
    else
      this.SetCurrentSide(entity, value / entity.Comp.Multiplier + entity.Comp.Offset);
  }

  private void Roll(Entity<DiceComponent> entity, EntityUid? user = null)
  {
    int side = new Random((int) this._timing.CurTick.Value).Next(1, entity.Comp.Sides + 1);
    this.SetCurrentSide(entity, side);
    this._popup.PopupPredicted(this.Loc.GetString("dice-component-on-roll-land", ("die", (object) entity), ("currentSide", (object) entity.Comp.CurrentValue)), Entity<DiceComponent>.op_Implicit(entity), user);
    this._audio.PlayPredicted(entity.Comp.Sound, Entity<DiceComponent>.op_Implicit(entity), user, new AudioParams?());
  }
}
