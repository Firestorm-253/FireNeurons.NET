using System.Diagnostics.CodeAnalysis;

namespace NeuralNetwork.NET.Indexes;

public struct LayerIndex : IEquatable<LayerIndex>
{
    public int Index { get; init; }

    public LayerIndex(int index)
    {
        this.Index = index;
    }

    public override bool Equals([NotNullWhen(true)] object? value) => (value is LayerIndex index) && (this.Index.Equals(index.Index) && this.Index.Equals(index.Index));

    public bool Equals(LayerIndex other) => this.Index == other.Index;

    public static implicit operator LayerIndex(int index) => new(index);
}
