// Decompiled with JetBrains decompiler
// Type: Content.Client.Options.UI.BaseOptionCVar`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Configuration;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Options.UI;

public abstract class BaseOptionCVar<TValue> : BaseOption where TValue : notnull
{
  private readonly IConfigurationManager _cfg;
  private readonly CVarDef<TValue> _cVar;

  public event Action<TValue>? ImmediateValueChanged;

  protected abstract TValue Value { get; set; }

  protected BaseOptionCVar(
    OptionsTabControlRow controller,
    IConfigurationManager cfg,
    CVarDef<TValue> cVar)
    : base(controller)
  {
    this._cfg = cfg;
    this._cVar = cVar;
  }

  public override void LoadValue() => this.Value = this._cfg.GetCVar<TValue>(this._cVar);

  public override void SaveValue() => this._cfg.SetCVar<TValue>(this._cVar, this.Value, false);

  public override void ResetToDefault() => this.Value = this._cVar.DefaultValue;

  public override bool IsModified()
  {
    return !this.IsValueEqual(this.Value, this._cfg.GetCVar<TValue>(this._cVar));
  }

  public override bool IsModifiedFromDefault()
  {
    return !this.IsValueEqual(this.Value, this._cVar.DefaultValue);
  }

  protected virtual bool IsValueEqual(TValue a, TValue b)
  {
    return typeof (TValue) == typeof (float) ? MathHelper.CloseToPercent((float) (object) a, (float) (object) b, 1E-05) : EqualityComparer<TValue>.Default.Equals(a, b);
  }

  protected override void ValueChanged()
  {
    base.ValueChanged();
    Action<TValue> immediateValueChanged = this.ImmediateValueChanged;
    if (immediateValueChanged == null)
      return;
    immediateValueChanged(this.Value);
  }
}
