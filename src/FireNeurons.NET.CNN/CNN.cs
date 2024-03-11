using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;
using FireNeurons.NET.Optimisation;
using FireNeurons.NET.Optimisation.Optimisers;

namespace FireNeurons.NET.CNN;

public class CNN
{
    public readonly NeuralNetwork nn;
    private readonly CNN_Layer[] cnn_layers;

    public CNN(NeuralNetwork nn, CNN_Layer[] cnn_layers)
    {
        this.nn = nn;
        this.cnn_layers = cnn_layers;
    }

    private Data Evaluate(Data3D data, out Data preNN, bool isTraining = false, params LayerIndex[] outputLayers)
    {
        for (int i = 0; i < this.cnn_layers.Length; i++)
        {
            data = this.cnn_layers[i].Execute(data);
        }

        preNN = new Data().Add(0, data.Flatten());
        return this.nn.Evaluate(preNN, isTraining, outputLayers);
    }

    public double[] Predict(Data3D data, out Data preNN)
    {
        var eval = this.Evaluate(data, out preNN, false, this.nn.Layers.Count - 1)[this.nn.Layers.Count - 1];
        return Softmax(eval);
    }

    static double[] Softmax(double[] input)
    {
        double sum = 0;
        for (int i = 0; i < input.Length; i++)
        {
            sum += Math.Pow(Math.E, input[i]);
        }

        var output = new double[input.Length];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = Math.Pow(Math.E, input[i]) / sum;
        }
        return output;
    }

    static IOptimiser optimiser = new Adam((neuron, global_args, local_args) =>
    {
        var (neurons, correctIndex) = (Tuple<List<Neuron>, int>)global_args!;

        var inputs = neurons.Select(x => x.GetValue(true)).ToArray();
        var probs = Softmax(inputs);

        if (neuron.Index.Index == correctIndex)
        {
            return (1 - probs[neuron.Index.Index]);
        }
        else
        {
            return -probs[neuron.Index.Index];
        }
    });

    public void Train(List<(Data, int)> labeledData)
    {
        var trainingDataSet = this.GetTrainingDataSet(labeledData);
        this.nn.Train(optimiser, trainingDataSet, miniBatchSize: 4);
    }

    private List<TrainingData> GetTrainingDataSet(List<(Data, int)> labeledData)
    {
        var trainingDataSet = new List<TrainingData>();
        foreach (var (data, correctIndex) in labeledData)
        {
            var neurons = this.nn.Layers.Last().Value.Neurons;
            var neuronsDict = new Dictionary<NeuronIndex, object>();
            foreach (var neuron in neurons)
            {
                neuronsDict.Add(neuron.Index, null);
            }

            var lossDerivativeArgs = new Data<(object?, Dictionary<NeuronIndex, object>)>();
            lossDerivativeArgs.Add(this.nn.Layers.Count - 1, ((neurons, correctIndex).ToTuple(), neuronsDict));

            var trainingData = new TrainingData(data, lossDerivativeArgs);

            trainingDataSet.Add(trainingData);
        }
        return trainingDataSet;
    }
}
