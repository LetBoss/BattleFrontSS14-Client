// Decompiled with JetBrains decompiler
// Type: Content.Client.Configurable.ConfigurationSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Configurable.UI;
using Content.Shared.Configurable;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Configurable;

public sealed class ConfigurationSystem : SharedConfigurationSystem
{
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ConfigurationComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<ConfigurationComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnConfigurationState)), (Type[]) null, (Type[]) null);
  }

  private void OnConfigurationState(
    Entity<ConfigurationComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    ConfigurationBoundUserInterface boundUserInterface;
    if (!this._uiSystem.TryGetOpenUi<ConfigurationBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum) ConfigurationComponent.ConfigurationUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Refresh(ent);
  }
}
