// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.RichText.ScrambleTag
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.RichText;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Text;

#nullable enable
namespace Content.Client.UserInterface.RichText;

public sealed class ScrambleTag : IMarkupTag, IMarkupTagHandler
{
  [Dependency]
  private IGameTiming _timing;
  private const int MaxScrambleLength = 32 /*0x20*/;

  public string Name => "scramble";

  public string TextBefore(MarkupNode node)
  {
    MarkupParameter markupParameter1;
    long? nullable1;
    MarkupParameter markupParameter2;
    long? nullable2;
    MarkupParameter markupParameter3;
    string str;
    if (!node.Attributes.TryGetValue("rate", out markupParameter1) || !((MarkupParameter) ref markupParameter1).TryGetLong(ref nullable1) || !node.Attributes.TryGetValue("length", out markupParameter2) || !((MarkupParameter) ref markupParameter2).TryGetLong(ref nullable2) || !node.Attributes.TryGetValue("chars", out markupParameter3) || !((MarkupParameter) ref markupParameter3).TryGetString(ref str))
      return string.Empty;
    double totalMilliseconds = this._timing.CurTime.TotalMilliseconds;
    long? nullable3 = nullable1;
    double? nullable4 = nullable3.HasValue ? new double?((double) nullable3.GetValueOrDefault()) : new double?();
    Random random = new Random((int) (nullable4.HasValue ? new double?(totalMilliseconds / nullable4.GetValueOrDefault()) : new double?()).Value + node.GetHashCode());
    char[] charArray = str.ToCharArray();
    float num = MathF.Min((float) nullable2.Value, 32f);
    StringBuilder stringBuilder = new StringBuilder();
    for (int index1 = 0; (double) index1 < (double) num; ++index1)
    {
      int index2 = random.Next() % charArray.Length;
      stringBuilder.Append(charArray[index2]);
    }
    return stringBuilder.ToString();
  }
}
