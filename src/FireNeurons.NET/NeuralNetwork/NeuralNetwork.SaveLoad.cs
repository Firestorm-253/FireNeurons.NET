using System.IO.Compression;
using System.Text;
using System.Text.Json;

using FireNeurons.NET.Dto;
using FireNeurons.NET.Objects;
using FireNeurons.NET.Optimizers;

namespace FireNeurons.NET;

public enum SaveType
{
    Json,
    Binary
}

public partial class NeuralNetwork
{
    public static NeuralNetwork Load(string file, IOptimizer optimizer)
        => new(file, optimizer);
    public NeuralNetwork(string file, IOptimizer optimizer)
        : this(LoadFromJsonFile(file), optimizer) { }

    public NeuralNetwork(NeuralNetworkDto dto, IOptimizer optimizer) : this(optimizer)
    {
        this.Optimizer = optimizer;
        foreach (var layerDto in dto.Layers)
        {
            var layer = new Layer(layerDto, this);
            this.Layers.Add(layer);
        }
    }


    public void Save(string file, SaveType saveType)
    {
        var dto = new NeuralNetworkDto(this);

        file = Path.GetFullPath(file);
        if (Path.HasExtension(file))
        {
            file = file.Replace(Path.GetExtension(file), string.Empty);
        }

        switch (saveType)
        {
            case SaveType.Json:
                {
                    string json = JsonSerializer.Serialize(dto, new JsonSerializerOptions() { WriteIndented = true });
                    File.WriteAllText(file + ".json", json);
                }
                break;
            case SaveType.Binary:
                {
                    string json = JsonSerializer.Serialize(dto, new JsonSerializerOptions() { WriteIndented = false });
                    var zipped = Zip(json);
                    File.WriteAllBytes(file + ".nn", zipped);
                }
                break;
            default: throw new Exception("ERROR: Invalid SaveType!");
        }
    }

    private static NeuralNetworkDto LoadFromJsonFile(string file)
    {
        if (!File.Exists(file))
        {
            throw new Exception("ERROR: File does not exist!");
        }

        NeuralNetworkDto? dto;

        switch (Path.GetExtension(file))
        {
            case ".json":
                {
                    string json = File.ReadAllText(file);
                    dto = JsonSerializer.Deserialize<NeuralNetworkDto>(json, new JsonSerializerOptions() { WriteIndented = true, });
                }
                break;
            case ".nn":
                {
                    var zipped = File.ReadAllBytes(file);
                    string json = UnZip(zipped);
                    dto = JsonSerializer.Deserialize<NeuralNetworkDto>(json, new JsonSerializerOptions() { WriteIndented = false, });
                }
                break;
            default: throw new Exception("ERROR: Invalid file-format!");
        }

        if (dto == null)
        {
            throw new Exception("ERROR: Loading failed!");
        }
        return dto;
    }


    private static byte[] Zip(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);

        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
            {
                gzipStream.Write(bytes, 0, bytes.Length);
            }
            return memoryStream.ToArray();
        }
    }

    private static string UnZip(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream(bytes))
        using (var outputStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(outputStream);
            }
            var output = outputStream.ToArray();
            return Encoding.UTF8.GetString(output, 0, output.Length);
        }
    }
}
