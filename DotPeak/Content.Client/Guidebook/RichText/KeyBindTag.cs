// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.Richtext.KeyBindTag
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Input;
using Robust.Client.UserInterface.RichText;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Client.Guidebook.Richtext;

public sealed class KeyBindTag : IMarkupTagHandler
{
  [Dependency]
  private IInputManager _inputManager;

  public string Name => "keybind";

  public string TextBefore(MarkupNode node)
  {
    string str;
    if (!((MarkupParameter) ref node.Value).TryGetString(ref str))
      return "";
    IKeyBinding ikeyBinding;
    return !this._inputManager.TryGetKeyBinding(BoundKeyFunction.op_Implicit(str), ref ikeyBinding) ? str : ikeyBinding.GetKeyString();
  }
}
