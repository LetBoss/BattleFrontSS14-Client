// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.Marking
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class Marking : 
  IEquatable<Marking>,
  IComparable<Marking>,
  IComparable<string>,
  ISerializationGenerated<Marking>,
  ISerializationGenerated
{
  [DataField("markingColor", false, 1, false, false, null)]
  private List<Color> _markingColors = new List<Color>();
  [DataField("visible", false, 1, false, false, null)]
  public bool Visible = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool Forced;

  private Marking()
  {
  }

  public Marking(string markingId, List<Color> markingColors)
  {
    this.MarkingId = markingId;
    this._markingColors = markingColors;
  }

  public Marking(string markingId, IReadOnlyList<Color> markingColors)
    : this(markingId, new List<Color>((IEnumerable<Color>) markingColors))
  {
  }

  public Marking(string markingId, int colorCount)
  {
    this.MarkingId = markingId;
    List<Color> colorList = new List<Color>();
    for (int index = 0; index < colorCount; ++index)
      colorList.Add(Color.White);
    this._markingColors = colorList;
  }

  public Marking(Marking other)
  {
    this.MarkingId = other.MarkingId;
    this._markingColors = new List<Color>((IEnumerable<Color>) other.MarkingColors);
    this.Visible = other.Visible;
    this.Forced = other.Forced;
  }

  [DataField("markingId", false, 1, true, false, null)]
  public string MarkingId { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public IReadOnlyList<Color> MarkingColors => (IReadOnlyList<Color>) this._markingColors;

  public void SetColor(int colorIndex, Color color) => this._markingColors[colorIndex] = color;

  public void SetColor(Color color)
  {
    for (int index = 0; index < this._markingColors.Count; ++index)
      this._markingColors[index] = color;
  }

  public int CompareTo(Marking? marking)
  {
    return marking == null ? 1 : string.Compare(this.MarkingId, marking.MarkingId, StringComparison.Ordinal);
  }

  public int CompareTo(string? markingId)
  {
    return markingId == null ? 1 : string.Compare(this.MarkingId, markingId, StringComparison.Ordinal);
  }

  public bool Equals(Marking? other)
  {
    return other != null && this.MarkingId.Equals(other.MarkingId) && this._markingColors.SequenceEqual<Color>((IEnumerable<Color>) other._markingColors) && this.Visible.Equals(other.Visible) && this.Forced.Equals(other.Forced);
  }

  public new string ToString()
  {
    string str = this.MarkingId.Replace('@', '_');
    List<string> values = new List<string>();
    foreach (Color markingColor in this._markingColors)
      values.Add(((Color) ref markingColor).ToHex());
    return $"{str}@{string.Join<string>(',', (IEnumerable<string>) values)}";
  }

  public static Marking? ParseFromDbString(string input)
  {
    if (input.Length == 0)
      return (Marking) null;
    string[] strArray = input.Split('@');
    if (strArray.Length != 2)
      return (Marking) null;
    List<Color> markingColors = new List<Color>();
    foreach (string str in strArray[1].Split(','))
      markingColors.Add(Color.FromHex((ReadOnlySpan<char>) str, new Color?()));
    return new Marking(strArray[0], markingColors);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Marking target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<Marking>(this, ref target, hookCtx, false, context))
      return;
    List<Color> target1 = (List<Color>) null;
    if (this._markingColors == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<Color>>(this._markingColors, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<List<Color>>(this._markingColors, hookCtx, context);
    target._markingColors = target1;
    string target2 = (string) null;
    if (this.MarkingId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.MarkingId, ref target2, hookCtx, false, context))
      target2 = this.MarkingId;
    target.MarkingId = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Visible, ref target3, hookCtx, false, context))
      target3 = this.Visible;
    target.Visible = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Marking target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Marking target1 = (Marking) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public Marking Instantiate() => new Marking();
}
