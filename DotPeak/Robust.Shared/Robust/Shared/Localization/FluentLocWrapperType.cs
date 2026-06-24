// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Localization.FluentLocWrapperType
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Linguini.Shared.Types.Bundle;

#nullable enable
namespace Robust.Shared.Localization;

internal sealed class FluentLocWrapperType : IFluentType
{
  public readonly ILocValue WrappedValue;
  private readonly LocContext _context;

  public FluentLocWrapperType(ILocValue wrappedValue, LocContext context)
  {
    this.WrappedValue = wrappedValue;
    this._context = context;
  }

  public string AsString() => this.WrappedValue.Format(this._context);

  public bool IsError() => false;

  public bool Matches(IFluentType other, IScope scope)
  {
    if (!(other is FluentLocWrapperType fluentLocWrapperType))
      return false;
    ILocValue wrappedValue1 = this.WrappedValue;
    ILocValue wrappedValue2 = fluentLocWrapperType.WrappedValue;
    bool flag;
    if ((object) (wrappedValue1 as LocValueNone) != null)
    {
      if ((object) (wrappedValue2 as LocValueNone) != null)
      {
        flag = true;
        goto label_23;
      }
    }
    else
    {
      LocValueDateTime locValueDateTime1 = wrappedValue1 as LocValueDateTime;
      if ((object) locValueDateTime1 != null)
      {
        LocValueDateTime locValueDateTime2 = wrappedValue2 as LocValueDateTime;
        if ((object) locValueDateTime2 != null)
        {
          flag = locValueDateTime1.Value.Equals(locValueDateTime2.Value);
          goto label_23;
        }
      }
      else
      {
        LocValueTimeSpan locValueTimeSpan1 = wrappedValue1 as LocValueTimeSpan;
        if ((object) locValueTimeSpan1 != null)
        {
          LocValueTimeSpan locValueTimeSpan2 = wrappedValue2 as LocValueTimeSpan;
          if ((object) locValueTimeSpan2 != null)
          {
            flag = locValueTimeSpan1.Value.Equals(locValueTimeSpan2.Value);
            goto label_23;
          }
        }
        else
        {
          LocValueNumber locValueNumber1 = wrappedValue1 as LocValueNumber;
          if ((object) locValueNumber1 != null)
          {
            LocValueNumber locValueNumber2 = wrappedValue2 as LocValueNumber;
            if ((object) locValueNumber2 != null)
            {
              flag = locValueNumber1.Value.Equals(locValueNumber2.Value);
              goto label_23;
            }
          }
          else
          {
            LocValueString locValueString1 = wrappedValue1 as LocValueString;
            if ((object) locValueString1 != null)
            {
              LocValueString locValueString2 = wrappedValue2 as LocValueString;
              if ((object) locValueString2 != null)
              {
                flag = locValueString1.Value.Equals(locValueString2.Value);
                goto label_23;
              }
            }
            else
            {
              LocValueEntity locValueEntity1 = wrappedValue1 as LocValueEntity;
              if ((object) locValueEntity1 != null)
              {
                LocValueEntity locValueEntity2 = wrappedValue2 as LocValueEntity;
                if ((object) locValueEntity2 != null)
                {
                  flag = locValueEntity1.Value.Equals(locValueEntity2.Value);
                  goto label_23;
                }
              }
              else if (wrappedValue1 == null)
                goto label_22;
            }
          }
        }
      }
    }
    if (wrappedValue2 != null)
    {
      flag = object.Equals((object) wrappedValue1, (object) wrappedValue2);
      goto label_23;
    }
label_22:
    flag = false;
label_23:
    return flag;
  }

  public IFluentType Copy() => (IFluentType) this;
}
