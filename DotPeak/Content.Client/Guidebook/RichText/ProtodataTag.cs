// Decompiled with JetBrains decompiler
// Type: Content.Client.Guidebook.RichText.ProtodataTag
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.RichText;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Utility;
using System;
using System.Globalization;

#nullable enable
namespace Content.Client.Guidebook.RichText;

public sealed class ProtodataTag : IMarkupTagHandler
{
  [Dependency]
  private ILogManager _logMan;
  [Dependency]
  private IEntityManager _entMan;
  private ISawmill? _log;

  public string Name => "protodata";

  private ISawmill Log => this._log ?? (this._log = this._logMan.GetSawmill("protodata_tag"));

  public string TextBefore(MarkupNode node)
  {
    string prototype;
    MarkupParameter markupParameter1;
    MarkupParameter markupParameter2;
    if (!((MarkupParameter) ref node.Value).TryGetString(ref prototype) || !node.Attributes.TryGetValue("comp", out markupParameter1) || !node.Attributes.TryGetValue("member", out markupParameter2))
      return string.Empty;
    MarkupParameter markupParameter3;
    node.Attributes.TryGetValue("format", out markupParameter3);
    object obj;
    if (!this._entMan.System<GuidebookDataSystem>().TryGetValue(prototype, ((MarkupParameter) ref markupParameter1).StringValue, ((MarkupParameter) ref markupParameter2).StringValue, out obj))
    {
      this.Log.Error($"Failed to find protodata for {markupParameter1}.{markupParameter2} in {prototype}");
      return "???";
    }
    return !string.IsNullOrEmpty(((MarkupParameter) ref markupParameter3).StringValue) && obj is IFormattable formattable ? formattable.ToString(((MarkupParameter) ref markupParameter3).StringValue, (IFormatProvider) CultureInfo.CurrentCulture) : obj?.ToString() ?? "NULL";
  }
}
