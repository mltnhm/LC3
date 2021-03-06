﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LC3.IO;

namespace LC3 {
    public static class Program {
        public static bool Disassemble;
        public static bool SuppressOutput;
        public static IIOAdapter IoAdapter;

        public static bool Output => !SuppressOutput;

        private static int Main(string[] args) {
            if (args.Length > 0) {
                var filename = args.TakeLast(1);
                var bytes = File.ReadAllBytes(filename.First());
                var flags = string.Join("", args.ToList().Where(a => a.StartsWith('-')))
                    .Replace("-", "");

                Disassemble = flags.Contains("d");
                SuppressOutput = flags.Contains("s");
                IoAdapter = new ConsoleAdapter();

                Start(bytes);
                return 0;
            }

            Console.WriteLine("Usage: LC3 [-ds] filename");
            // TODO: Write description of flags
            return 1;
        }

        private static IEnumerable<ushort> GetShorts(byte[] bytes) {
            return bytes.InSetsOf(2)
                .Select(i => {
                    i.Reverse();
                    return BitConverter.ToUInt16(i.ToArray());
                });
        }

        private static void Start(byte[] bytes) {
            var proc = new Processor();
            var data = GetShorts(bytes).ToArray();

            var origin = data[0];

            Debug.WriteLine($"Origin: {origin}");
            var payload = data.Skip(1).ToArray();
            for (ushort i = 0; i < payload.Length; i++) {
                var location = (ushort) (origin + i);
                proc.Memory.Put(location, payload[i]);
            }

            proc.SetRegister((int)Register.PC ,origin);

            while (true) {
                // Fetch & Execute
                proc.Decode(proc.Fetch()).Call(proc);
            }
        }
    }
}