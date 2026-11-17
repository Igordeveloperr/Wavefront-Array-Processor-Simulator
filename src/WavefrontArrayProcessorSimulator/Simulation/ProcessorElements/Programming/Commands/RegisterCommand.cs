using WaveVS.Memory;

namespace WaveVS.WaveFrontVS.PE.program.commandPattern
{
    internal abstract class RegisterCommand : Command
    {
        public override void Execute(params object[] parameters)
        {
            if (parameters.Length != 2 || !(parameters[Operand1Index] is Register destination))
                return;

            if (parameters[Operand2Index] is Register register)
                destination.WriteData(Evaluate(destination.Value, register.Value));
            else if (parameters[Operand2Index] is InputBuffer buffer)
                destination.WriteData(Evaluate(destination.Value, buffer.Value));
        }

        protected abstract float? Evaluate(float? destination, float? source);
    }
}
