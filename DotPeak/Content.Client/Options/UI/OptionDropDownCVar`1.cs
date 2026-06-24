// Decompiled with JetBrains decompiler
// Type: Content.Client.Options.UI.OptionDropDownCVar`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Options.UI;

public sealed class OptionDropDownCVar<T> : BaseOptionCVar<T> where T : notnull
{
  private readonly OptionDropDown _dropDown;
  private readonly OptionDropDownCVar<
  #nullable disable
  T>.ItemEntry[] _entries;

  protected override 
  #nullable enable
  T Value
  {
    get => (T) this._dropDown.Button.SelectedMetadata;
    set => this._dropDown.Button.SelectId(this.FindValueId(value));
  }

  public OptionDropDownCVar(
    OptionsTabControlRow controller,
    IConfigurationManager cfg,
    CVarDef<T> cVar,
    OptionDropDown dropDown,
    IReadOnlyCollection<OptionDropDownCVar<
    #nullable disable
    T>.ValueOption> options)
    : base(controller, cfg, cVar)
  {
    OptionDropDownCVar<T> optionDropDownCvar = this;
    if (options.Count == 0)
      throw new ArgumentException("Need at least one option!");
    this._dropDown = dropDown;
    this._entries = new OptionDropDownCVar<T>.ItemEntry[options.Count];
    OptionButton button = dropDown.Button;
    int index = 0;
    foreach (OptionDropDownCVar<T>.ValueOption option in (IEnumerable<OptionDropDownCVar<T>.ValueOption>) options)
    {
      this._entries[index] = new OptionDropDownCVar<T>.ItemEntry()
      {
        Key = option.Key
      };
      button.AddItem(option.Label, new int?(index));
      button.SetItemMetadata(button.GetIdx(index), (object) option.Key);
      ++index;
    }
    dropDown.Button.OnItemSelected += (Action<OptionButton.ItemSelectedEventArgs>) (args =>
    {
      dropDown.Button.SelectId(args.Id);
      optionDropDownCvar.ValueChanged();
    });
  }

  private int FindValueId(
  #nullable enable
  T value)
  {
    for (int valueId = 0; valueId < this._entries.Length; ++valueId)
    {
      if (this.IsValueEqual(this._entries[valueId].Key, value))
        return valueId;
    }
    return 0;
  }

  public sealed class ValueOption(T key, string label)
  {
    public readonly T Key = key;
    public readonly string Label = label;
  }

  private struct ItemEntry
  {
    public T Key;
  }
}
