// Decompiled with JetBrains decompiler
// Type: Content.Client.Clickable.IClickMapManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Shared.Graphics.RSI;
using Robust.Shared.Maths;

#nullable enable
namespace Content.Client.Clickable;

public interface IClickMapManager
{
  bool IsOccluding(Texture texture, Vector2i pos);

  bool IsOccluding(Robust.Client.Graphics.RSI rsi, Robust.Client.Graphics.RSI.StateId state, RsiDirection dir, int frame, Vector2i pos);
}
