namespace WaveVS.WaveFrontVS.PE.program.commandPattern
{
    internal sealed class SubCommand : RegisterCommand
    {
        protected override float? Evaluate(float? destination, float? source) => destination - source;
    }
}
