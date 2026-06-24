// Decompiled with JetBrains decompiler
// Type: Content.Client.PDA.Ringer.RingerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.PDA;
using Content.Shared.PDA.Ringer;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.PDA.Ringer;

public sealed class RingerSystem : SharedRingerSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RingerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<RingerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnRingerUpdate)), (Type[]) null, (Type[]) null);
  }

  private void OnRingerUpdate(Entity<RingerComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    this.UpdateRingerUi(ent);
  }

  protected override void UpdateRingerUi(Entity<RingerComponent> ent)
  {
    BoundUserInterface boundUserInterface;
    if (!this.UI.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) RingerUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Update();
  }
}
