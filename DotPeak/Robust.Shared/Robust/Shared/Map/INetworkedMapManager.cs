// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.INetworkedMapManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Timing;

#nullable disable
namespace Robust.Shared.Map;

internal interface INetworkedMapManager : IMapManagerInternal, IMapManager
{
  void CullDeletionHistory(GameTick upToTick);
}
