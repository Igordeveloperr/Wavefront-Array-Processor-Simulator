namespace WaveVS.WaveFrontVS.PE.program.commandPattern
{
    internal sealed class MovCommand : RegisterCommand
    {
        protected override float? Evaluate(float? destination, float? source) => source;
    }
}
