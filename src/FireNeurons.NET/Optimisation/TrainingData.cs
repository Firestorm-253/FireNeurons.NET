namespace FireNeurons.NET.Optimisation;

using Indexes;
using Objects;

public readonly struct TrainingData
{
    public Data InputData { get; }
    public Data<(object?, Dictionary<NeuronIndex, object>)> LossDerivativeArgs { get; }
    public NeuronIndex[] IgnoreNeurons { get; }

    public TrainingData(Data data)
    {
        this.InputData = new Data();
        this.LossDerivativeArgs = new();
        this.IgnoreNeurons = Array.Empty<NeuronIndex>();
    }
    public TrainingData(Data inputData, Data<(object?, Dictionary<NeuronIndex, object>)> lossDerivativeArgs, NeuronIndex[]? ignoreNeurons = null)
    {
        this.InputData = inputData;
        this.LossDerivativeArgs = lossDerivativeArgs;
        this.IgnoreNeurons = ignoreNeurons ?? Array.Empty<NeuronIndex>();
    }
}
