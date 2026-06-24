// Decompiled with JetBrains decompiler
// Type: Content.Shared.Arcade.BlockGameMessages
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Arcade;

public static class BlockGameMessages
{
  [NetSerializable]
  [Serializable]
  public sealed class BlockGamePlayerActionMessage : BoundUserInterfaceMessage
  {
    public readonly BlockGamePlayerAction PlayerAction;

    public BlockGamePlayerActionMessage(BlockGamePlayerAction playerAction)
    {
      this.PlayerAction = playerAction;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BlockGameVisualUpdateMessage : BoundUserInterfaceMessage
  {
    public readonly BlockGameMessages.BlockGameVisualType GameVisualType;
    public readonly BlockGameBlock[] Blocks;

    public BlockGameVisualUpdateMessage(
      BlockGameBlock[] blocks,
      BlockGameMessages.BlockGameVisualType gameVisualType)
    {
      this.Blocks = blocks;
      this.GameVisualType = gameVisualType;
    }
  }

  public enum BlockGameVisualType
  {
    GameField,
    HoldBlock,
    NextBlock,
  }

  [NetSerializable]
  [Serializable]
  public sealed class BlockGameScoreUpdateMessage : BoundUserInterfaceMessage
  {
    public readonly int Points;

    public BlockGameScoreUpdateMessage(int points) => this.Points = points;
  }

  [NetSerializable]
  [Serializable]
  public sealed class BlockGameUserStatusMessage : BoundUserInterfaceMessage
  {
    public readonly bool IsPlayer;

    public BlockGameUserStatusMessage(bool isPlayer) => this.IsPlayer = isPlayer;
  }

  [NetSerializable]
  [Virtual]
  [Serializable]
  public class BlockGameSetScreenMessage : BoundUserInterfaceMessage
  {
    public readonly BlockGameMessages.BlockGameScreen Screen;
    public readonly bool IsStarted;

    public BlockGameSetScreenMessage(BlockGameMessages.BlockGameScreen screen, bool isStarted = true)
    {
      this.Screen = screen;
      this.IsStarted = isStarted;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BlockGameGameOverScreenMessage : BlockGameMessages.BlockGameSetScreenMessage
  {
    public readonly int FinalScore;
    public readonly int? LocalPlacement;
    public readonly int? GlobalPlacement;

    public BlockGameGameOverScreenMessage(
      int finalScore,
      int? localPlacement,
      int? globalPlacement)
      : base(BlockGameMessages.BlockGameScreen.Gameover)
    {
      this.FinalScore = finalScore;
      this.LocalPlacement = localPlacement;
      this.GlobalPlacement = globalPlacement;
    }
  }

  [NetSerializable]
  [Serializable]
  public enum BlockGameScreen
  {
    Game,
    Pause,
    Gameover,
    Highscores,
  }

  [NetSerializable]
  [Serializable]
  public sealed class BlockGameHighScoreUpdateMessage : BoundUserInterfaceMessage
  {
    public List<BlockGameMessages.HighScoreEntry> LocalHighscores;
    public List<BlockGameMessages.HighScoreEntry> GlobalHighscores;

    public BlockGameHighScoreUpdateMessage(
      List<BlockGameMessages.HighScoreEntry> localHighscores,
      List<BlockGameMessages.HighScoreEntry> globalHighscores)
    {
      this.LocalHighscores = localHighscores;
      this.GlobalHighscores = globalHighscores;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class HighScoreEntry : IComparable
  {
    public string Name;
    public int Score;

    public HighScoreEntry(string name, int score)
    {
      this.Name = name;
      this.Score = score;
    }

    public int CompareTo(object? obj)
    {
      return !(obj is BlockGameMessages.HighScoreEntry highScoreEntry) ? 0 : this.Score.CompareTo(highScoreEntry.Score);
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BlockGameLevelUpdateMessage : BoundUserInterfaceMessage
  {
    public readonly int Level;

    public BlockGameLevelUpdateMessage(int level) => this.Level = level;
  }
}
