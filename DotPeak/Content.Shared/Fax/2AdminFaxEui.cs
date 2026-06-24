// Decompiled with JetBrains decompiler
// Type: Content.Shared.Fax.AdminFaxEuiMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Fax;

public static class AdminFaxEuiMsg
{
  [NetSerializable]
  [Serializable]
  public sealed class Close : EuiMessageBase
  {
  }

  [NetSerializable]
  [Serializable]
  public sealed class Follow : EuiMessageBase
  {
    public NetEntity TargetFax { get; }

    public Follow(NetEntity targetFax) => this.TargetFax = targetFax;
  }

  [NetSerializable]
  [Serializable]
  public sealed class Send : EuiMessageBase
  {
    public NetEntity Target { get; }

    public string Title { get; }

    public string From { get; }

    public string Content { get; }

    public string StampState { get; }

    public Color StampColor { get; }

    public bool Locked { get; }

    public Send(
      NetEntity target,
      string title,
      string from,
      string content,
      string stamp,
      Color stampColor,
      bool locked)
    {
      this.Target = target;
      this.Title = title;
      this.From = from;
      this.Content = content;
      this.StampState = stamp;
      this.StampColor = stampColor;
      this.Locked = locked;
    }
  }
}
