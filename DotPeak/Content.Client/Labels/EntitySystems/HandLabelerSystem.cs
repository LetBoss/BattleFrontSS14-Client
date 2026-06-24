// Decompiled with JetBrains decompiler
// Type: Content.Client.Labels.EntitySystems.HandLabelerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Labels.UI;
using Content.Shared.Labels;
using Content.Shared.Labels.Components;
using Content.Shared.Labels.EntitySystems;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Labels.EntitySystems;

public sealed class HandLabelerSystem : SharedHandLabelerSystem
{
  protected override void UpdateUI(Entity<HandLabelerComponent> ent)
  {
    BoundUserInterface boundUserInterface1;
    if (!this.UserInterfaceSystem.TryGetOpenUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) HandLabelerUiKey.Key, ref boundUserInterface1) || !(boundUserInterface1 is HandLabelerBoundUserInterface boundUserInterface2))
      return;
    boundUserInterface2.Reload();
  }
}
