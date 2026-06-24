// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.Manager.EntityDiffContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Serialization.Manager;

public sealed class EntityDiffContext : ISerializationContext
{
  public SerializationManager.SerializerProvider SerializerProvider { get; }

  public bool WritingReadingPrototypes { get; set; } = true;

  public EntityDiffContext()
  {
    this.SerializerProvider = new SerializationManager.SerializerProvider();
    this.SerializerProvider.RegisterSerializer((object) this);
  }
}
