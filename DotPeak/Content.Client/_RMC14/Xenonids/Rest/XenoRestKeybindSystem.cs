// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Rest.XenoRestKeybindSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Actions;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Input;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Rest;

public sealed class XenoRestKeybindSystem : EntitySystem
{
  [Dependency]
  private ActionsSystem _actionsSystem;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(CMKeyFunctions.RMCXenoRest, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__4_0)), (StateInputCmdDelegate) null, true, true)).Register<XenoRestKeybindSystem>();
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<XenoRestKeybindSystem>();
  }
}
