// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Hands.ClientRMCHandsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Hands.Systems;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client._RMC14.Hands;

public sealed class ClientRMCHandsSystem : RMCHandsSystem
{
  [Dependency]
  private HandsSystem _hands;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(CMKeyFunctions.RMCInteractWithOtherHand, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003CInitialize\u003Eb__1_0)), (StateInputCmdDelegate) null, true, true)).Register<ClientRMCHandsSystem>();
  }
}
