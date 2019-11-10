using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixFlvMultipleResolutions
{
    class Program
    {
        static void Main(string[] args)
        {
            var search = new byte[] { 0x6F, 0x6E, 0x4D, 0x65, 0x74, 0x61, 0x44, 0x61, 0x74, 0x61 };//onMetaData header, if these are many they our cause problem
            foreach (var arg in args)
            {
                var bytes = File.ReadAllBytes(arg);
                var indexes = new List<int>();
                var index = 0;
                int startindex = 0;
                while ((index = findSequence(bytes, startindex, search)) != -1)
                {
                    indexes.Add(index);
                    startindex += index + search.Length;
                }
                if (indexes.Count == 1) return;
                var newlength = bytes.Length - (indexes.Last() - indexes.First());

                var result = new byte[newlength];
                Array.Copy(bytes, 0, result, 0, indexes.First());
                Array.Copy(bytes, indexes.Last(), result, indexes.First(), bytes.Length - indexes.Last());
                System.IO.File.WriteAllBytes(arg, result);

            }
        }
        private static int findSequence(byte[] array, int start, byte[] sequence)
        {
            int end = array.Length - sequence.Length; // past here no match is possible
            byte firstByte = sequence[0]; // cached to tell compiler there's no aliasing

            while (start <= end)
            {
                // scan for first byte only. compiler-friendly.
                if (array[start] == firstByte)
                {
                    // scan for rest of sequence
                    for (int offset = 1; ; ++offset)
                    {
                        if (offset == sequence.Length)
                        { // full sequence matched?
                            return start;
                        }
                        else if (array[start + offset] != sequence[offset])
                        {
                            break;
                        }
                    }
                }
                ++start;
            }

            // end of array reached without match
            return -1;
        }
    }
}
