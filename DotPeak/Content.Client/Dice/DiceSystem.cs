// Decompiled with JetBrains decompiler
// Type: Content.Client.Dice.DiceSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Dice;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Dice;

public sealed class DiceSystem : SharedDiceSystem
{
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DiceComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<DiceComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnDiceAfterHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnDiceAfterHandleState(
    Entity<DiceComponent> entity,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<DiceComponent>.op_Implicit(entity), ref spriteComponent))
      return;
    string name = this._sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent)), 0).Name;
    if (name == null)
      return;
    string str = name.Substring(0, name.IndexOf('_'));
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entity.Owner, spriteComponent)), 0, RSI.StateId.op_Implicit($"{str}_{entity.Comp.CurrentValue}"));
  }
}
