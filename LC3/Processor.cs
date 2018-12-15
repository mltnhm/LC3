using System.Diagnostics;

namespace LC3 {
    public class Processor {
        private ushort[] _registers = new ushort[12];
        public Memory Memory = new Memory();

        public ushort GetRegister(int register) => _registers[register];

        public void SetRegister(int register, ushort value) => _registers[register] = value;

        public ushort this[int key] {
            get => GetRegister(key);
            set => SetRegister(key, value);
        }
        
        public ushort this[Register key] {
            get => this[(int)key];
            set => this[(int)key] = value;
        }

        public void Increment(Register register) {
            this[register]++;
        }

        public ushort Fetch() {
            this[Register.MemoryAddress] = this[Register.ProgramCounter];
            Increment(Register.ProgramCounter);
            this[Register.MemoryData] = Memory[this[Register.MemoryAddress]];
            return this[Register.MemoryData];
        }

        public void Decode(ushort instruction) {
            var opcode = instruction >> 12;
            Debug.WriteLine(opcode);
        }
    }
}