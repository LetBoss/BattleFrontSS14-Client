// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Gameplay.GameplayStateLoadController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controllers;
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Gameplay;

public sealed class GameplayStateLoadController : UIController
{
  public Action? OnScreenLoad;
  public Action? OnScreenUnload;

  public void UnloadScreen()
  {
    Action onScreenUnload = this.OnScreenUnload;
    if (onScreenUnload == null)
      return;
    onScreenUnload();
  }

  public void LoadScreen()
  {
    Action onScreenLoad = this.OnScreenLoad;
    if (onScreenLoad == null)
      return;
    onScreenLoad();
  }
}
