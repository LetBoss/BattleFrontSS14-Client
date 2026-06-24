// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chat.V2.Moderation.ChatCensorFactory
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Chat.V2.Moderation;

public sealed class ChatCensorFactory
{
  private List<IChatCensor> _censors = new List<IChatCensor>();

  public void With(IChatCensor censor) => this._censors.Add(censor);

  public IChatCensor Build()
  {
    return (IChatCensor) new CompoundChatCensor((IEnumerable<IChatCensor>) this._censors.ToArray());
  }

  public bool Reset()
  {
    int num = this._censors.Count > 0 ? 1 : 0;
    this._censors = new List<IChatCensor>();
    return num != 0;
  }
}
