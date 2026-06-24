// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.InputCoalescer`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Client.UserInterface;

public struct InputCoalescer<T>
{
  public bool IsModified;
  public T LastValue;

  public void Set(T value)
  {
    this.LastValue = value;
    this.IsModified = true;
  }

  public bool CheckIsModified([MaybeNullWhen(false)] out T value)
  {
    if (this.IsModified)
    {
      value = this.LastValue;
      this.IsModified = false;
      return true;
    }
    value = default (T);
    return this.IsModified;
  }
}
