// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.V2.Moderation.CompoundChatCensor
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chat.V2.Moderation;

public sealed class CompoundChatCensor(IEnumerable<IChatCensor> censors) : IChatCensor
{
  public bool Censor(string input, out string output, char replaceWith = '*')
  {
    bool flag = false;
    foreach (IChatCensor chatCensor in censors)
    {
      if (chatCensor.Censor(input, out output, replaceWith))
        flag = true;
    }
    output = input;
    return flag;
  }
}
