// Decompiled with JetBrains decompiler
// Type: Content.Client.Stylesheets.ResCacheExtension
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using System;

#nullable enable
namespace Content.Client.Stylesheets;

public static class ResCacheExtension
{
  public static Font NotoStack(
    this IResourceCache resCache,
    string variation = "Regular",
    int size = 10,
    bool display = false)
  {
    string str1 = display ? "Display" : "";
    string str2 = variation.StartsWith("Bold", StringComparison.Ordinal) ? "Bold" : "Regular";
    return resCache.GetFont(new string[3]
    {
      $"/Fonts/NotoSans{str1}/NotoSans{str1}-{variation}.ttf",
      $"/Fonts/NotoSans/NotoSansSymbols-{str2}.ttf",
      "/Fonts/NotoSans/NotoSansSymbols2-Regular.ttf"
    }, size);
  }
}
