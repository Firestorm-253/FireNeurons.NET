using System.Diagnostics.CodeAnalysis;

namespace NeuralNetwork.NET.Indexes;

public readonly struct NeuronIndex : IEquatable<NeuronIndex>
{
    public int Index { get; init; }
    public LayerIndex LayerIndex { get; init; }

    public NeuronIndex(int index, LayerIndex layerIndex)
    {
        this.Index = index;
        this.LayerIndex = layerIndex;
    }

    public override bool Equals([NotNullWhen(true)] object? value) => (value is NeuronIndex index) && (this.Index.Equals(index.Index) && this.LayerIndex.Equals(index.LayerIndex));
    public bool Equals(NeuronIndex other) => this.Index == other.Index;
    public override int GetHashCode() => HashCode.Combine(this.Index, this.LayerIndex);

    public static bool operator ==(NeuronIndex a, NeuronIndex b) => a.Equals(b);
    public static bool operator !=(NeuronIndex a, NeuronIndex b) => !a.Equals(b);

    public static implicit operator NeuronIndex((int, LayerIndex) index) => new(index.Item1, index.Item2);
}
