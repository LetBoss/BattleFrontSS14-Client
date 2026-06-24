// Decompiled with JetBrains decompiler
// Type: Content.Client.Verbs.UI.ConfirmationMenuElement
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.ContextMenu.UI;
using Content.Shared.Verbs;
using Robust.Client.UserInterface;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Client.Verbs.UI;

public sealed class ConfirmationMenuElement : ContextMenuElement
{
  public const string StyleClassConfirmationContextMenuButton = "confirmationContextMenuButton";
  public readonly Verb Verb;

  public override string Text
  {
    set
    {
      FormattedMessage formattedMessage = new FormattedMessage();
      formattedMessage.PushColor(Color.White);
      formattedMessage.AddMarkupPermissive(value.Trim());
      this.Label.SetMessage(formattedMessage, new Color?());
    }
  }

  public ConfirmationMenuElement(Verb verb, string? text)
    : base(text)
  {
    this.Verb = verb;
    ((Control) this.Icon).Visible = false;
    ((Control) this).SetOnlyStyleClass("confirmationContextMenuButton");
  }
}
