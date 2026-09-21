using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace AppBlockerSecurity.Services;

public class MathPuzzleService : IDisposable
{
    private InferenceSession? _session;
    private bool _isInitialized;

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        using var stream = await FileSystem.OpenAppPackageFileAsync("math_puzzle_model.onnx");
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        var options = new SessionOptions();
        try
        {
            // tenta usar a NPU do celular para poupar bateria
            options.AppendExecutionProvider_Nnapi();
        }
        catch { /* Fallback automático para a CPU */ }

        _session = new InferenceSession(memoryStream.ToArray(), options);
        _isInitialized = true;
    }

    public bool TryUnlock(float numberA, float numberB, float userAnswer)
    {
        if (!_isInitialized || _session == null) return false;

        var tensor = new DenseTensor<float>(new[] { numberA, numberB, userAnswer }, new[] { 1, 3 });
        var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("puzzle_input", tensor) };

        using var results = _session.Run(inputs);
        var output = results.FirstOrDefault(r => r.Name == "unlock_score")?.AsTensor<float>();

        // retorna true se a IA validar o padrão com confiança
        return output != null && output[0, 0] > 0.80f;
    }

    public void Dispose() => _session?.Dispose();
}