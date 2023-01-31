using System.Diagnostics.CodeAnalysis;

namespace NeuralNetwork.NET.Indexes;

public struct LevelIndex : IEquatable<LevelIndex>
{
    public int Index { get; init; }

    public LevelIndex(int index)
    {
        this.Index = index;
    }

    public override bool Equals([NotNullWhen(true)] object? value) => (value is LevelIndex index) && this.Index.Equals(index.Index);
    public bool Equals(LevelIndex other) => this.Index == other.Index;

    public static implicit operator LevelIndex((int, LayerIndex) index) => new(index.Item1, index.Item2);
}
