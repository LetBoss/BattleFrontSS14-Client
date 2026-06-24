// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.RadialMenuTextureButtonBase
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Input;

#nullable enable
namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialMenuTextureButtonBase : TextureButton
{
  protected RadialMenuTextureButtonBase() => ((BaseButton) this).EnableAllKeybinds = true;

  protected virtual void KeyBindUp(GUIBoundKeyEventArgs args)
  {
    if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick) && !BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, ContentKeyFunctions.AltActivateItemInWorld))
      return;
    ((BaseButton) this).KeyBindUp(args);
  }
}
