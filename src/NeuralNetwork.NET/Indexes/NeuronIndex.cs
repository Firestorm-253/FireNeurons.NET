using System.Diagnostics.CodeAnalysis;

namespace NeuralNetwork.NET.Indexes;

public struct NeuronIndex : IEquatable<NeuronIndex>
{
    public int Index { get; init; }
    public LevelIndex LevelIndex { get; init; }

    public NeuronIndex(int index, LevelIndex levelIndex)
    {
        this.Index = index;
        this.LevelIndex = levelIndex;
    }

    public override bool Equals([NotNullWhen(true)] object? value) => (value is NeuronIndex index) && (this.Index.Equals(index.Index) && this.LevelIndex.Equals(index.LevelIndex));

    public bool Equals(NeuronIndex other) => this.Index == other.Index;

    public static bool operator ==(NeuronIndex a, NeuronIndex b) => a.Equals(b);
    public static bool operator !=(NeuronIndex a, NeuronIndex b) => !a.Equals(b);
    public static implicit operator NeuronIndex((int, LevelIndex) index) => new(index.Item1, index.Item2);
    public static implicit operator NeuronIndex((int, int, LayerIndex) index) => new(index.Item1, (index.Item2, index.Item3));
}
