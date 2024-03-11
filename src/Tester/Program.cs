using PacketManager.dotnet;
using FireNeurons.NET.CNN;
using FireNeurons.NET;


var cnn = ConfigureCNN();


var training = LoadData("CNN DataSets\\training.Data3D");
var testing = LoadData("CNN DataSets\\testing.Data3D");

var labeledData = new List<(FireNeurons.NET.Objects.Data, int)>();
for (int c = 0; c < training.Count; c++)
{
    for (int i = 0; i < training[c].Length / 20; i++)
    {
        var probs = cnn.Predict(training[c][i], out FireNeurons.NET.Objects.Data preNN);

        labeledData.Add((preNN, c));
    }
}

for (int i = 0; i < 200; i++)
{
    int counter = 0;
    double l1_loss = 0;
    double l2_loss = 0;
    for (int l = 1; l < cnn.nn.Layers.Count; l++)
    {
        foreach (var neuron in cnn.nn.Layers.ToArray()[l].Value.Neurons)
        {
            l1_loss += neuron.Options.L1_Reg * Math.Abs(neuron.Bias);
            l2_loss += neuron.Options.L2_Reg * Math.Pow(neuron.Bias, 2);
            counter++;

            foreach (var connection in neuron.Connections)
            {
                l1_loss += neuron.Options.L1_Reg * Math.Abs(connection.Weight);
                l2_loss += neuron.Options.L2_Reg * Math.Pow(connection.Weight, 2);
                counter++;
            }
        }
    }

    var l_loss = TestingLoss(out double avg_prob);

    Console.WriteLine($"{l_loss + l1_loss + l2_loss} ({Math.Round(avg_prob * 100, 2)}% -> (L: {l_loss} | L1: {l1_loss} | L2: {l2_loss})");

    cnn.Train(labeledData);
}





Console.ReadKey();


double TestingLoss(out double avg_prob)
{
    double prob_sum = 0;
    double loss = 0;
    for (int c = 0; c < testing.Count; c++)
    {
        double _loss = 0;
        for (int i = 0; i < testing[c].Length; i++)
        {
            var probs = cnn.Predict(testing[c][i], out var _);
            prob_sum += probs[c] / testing[c].Length;

            _loss += Loss(probs[c]);
        }

        loss += _loss / testing[c].Length;
    }

    avg_prob = prob_sum / testing.Count;
    return loss / testing.Count;
}


double Loss(double correctLabelProb)
{
    return -Math.Log(correctLabelProb);
}

CNN ConfigureCNN()
{
    var cnn_layers = new CNN_Layer[]
    {
        new ConvolutionLayer(10, 3, 1, 1),
        new ConvolutionLayer(10, 3, 10, 1),
        new PoolingLayer(2),
        new ConvolutionLayer(20, 3, 10, 1),
        new ConvolutionLayer(20, 3, 20, 1),
        new PoolingLayer(2),
    };

    var nn = new NeuralNetwork();
    nn.Add(28 * 28, 0, new FireNeurons.NET.Objects.Options() { Activation = FireNeurons.NET.Objects.Activation.LeakyRelu, L2_Reg = 0.0001 });
    nn.Add(320, 1, new FireNeurons.NET.Objects.Options() { Activation = FireNeurons.NET.Objects.Activation.LeakyRelu, L2_Reg = 0.0001 }, 0);
    nn.Add(100, 2, new FireNeurons.NET.Objects.Options() { Activation = FireNeurons.NET.Objects.Activation.LeakyRelu, L2_Reg = 0.0001 }, 1);
    nn.Add(10, 3, new FireNeurons.NET.Objects.Options() { Activation = FireNeurons.NET.Objects.Activation.Identity, L2_Reg = 0.0001 }, 2);
    nn.Randomize();

    return new CNN(nn, cnn_layers);
}


//var trainingFiles = new string[10][];
//for (int i = 0; i < trainingFiles.Length; i++)
//{
//    trainingFiles[i] = Directory.GetFiles($"CNN DataSets\\training (10x4000)\\{i}");
//}
//var testingFiles = new string[10][];
//for (int i = 0; i < testingFiles.Length; i++)
//{
//    testingFiles[i] = Directory.GetFiles($"CNN DataSets\\testing (10x60)\\{i}");
//}

//Data3D LoadData3D(string file)
//{
//    var img = new Bitmap(Image.FromFile(file));
//    var output = new double[img.Height][];

//    for (int y = 0; y < img.Height; y++)
//    {
//        output[y] = new double[img.Width];

//        for (int x = 0; x < img.Width; x++)
//        {
//            output[y][x] = img.GetPixel(x, y).GetBrightness();
//        }
//    }

//    return new Data3D(new double[][][] { output });
//}


Dictionary<int, Data3D[]> LoadData(string file)
{
    using var fs = new FileStream(file, FileMode.Open);
    var buffer = new byte[fs.Length];
    fs.Read(buffer, 0, buffer.Length);

    fs.Dispose();
    fs.Close();

    var pr = new PacketReader(buffer);
    return pr.ReadObject<Dictionary<int, Data3D[]>>();
}