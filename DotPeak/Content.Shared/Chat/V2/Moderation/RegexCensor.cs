// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.V2.Moderation.RegexCensor
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System.Text.RegularExpressions;

#nullable enable
namespace Content.Shared.Chat.V2.Moderation;

public sealed class RegexCensor(Regex censorInstruction) : IChatCensor
{
  private readonly Regex _censorInstruction = censorInstruction;

  public bool Censor(string input, out string output, char replaceWith = '*')
  {
    output = this._censorInstruction.Replace(input, replaceWith.ToString());
    return !string.Equals(input, output);
  }
}
