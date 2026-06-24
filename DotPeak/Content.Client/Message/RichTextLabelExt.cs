// Decompiled with JetBrains decompiler
// Type: Content.Client.Message.RichTextLabelExt
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

#nullable enable
namespace Content.Client.Message;

public static class RichTextLabelExt
{
  public static RichTextLabel SetMarkup(this RichTextLabel label, string markup)
  {
    label.SetMessage(FormattedMessage.FromMarkupOrThrow(markup), new Color?());
    return label;
  }

  public static RichTextLabel SetMarkupPermissive(this RichTextLabel label, string markup)
  {
    label.SetMessage(FormattedMessage.FromMarkupPermissive(markup), new Color?());
    return label;
  }
}
