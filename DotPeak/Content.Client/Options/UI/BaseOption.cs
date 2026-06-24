// Decompiled with JetBrains decompiler
// Type: Content.Client.Options.UI.BaseOption
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

#nullable enable
namespace Content.Client.Options.UI;

public abstract class BaseOption(OptionsTabControlRow controller)
{
  protected virtual void ValueChanged() => controller.ValueChanged();

  public abstract void LoadValue();

  public abstract void SaveValue();

  public abstract void ResetToDefault();

  public abstract bool IsModified();

  public abstract bool IsModifiedFromDefault();
}
