using System;
using System.Collections.Generic;
using System.Threading;

namespace AudioTest
{
    static class SETTINGS
    {
        public const int CHANNELS = 2;
        public const int SAMPLE_RATE = 44100;
        public const int BITS_PER_SAMPLE = 32;
        public const int FRAMES_PER_SAMPLE = 64;
    }
    class Program
    {

        static void Main(string[] args)
        {
            var stream = new CSAudioStreamer.PortAudioOutputStream(SETTINGS.CHANNELS, SETTINGS.SAMPLE_RATE, CSAudioStreamer.PaSampleFormat.Float32, SETTINGS.FRAMES_PER_SAMPLE);
            CancellationTokenSource cancel = new CancellationTokenSource();
            stream.Stream(new DataProvider(), cancel.Token);

            int iterations = 2;
            while (iterations > 0) { System.Threading.Thread.Sleep(1000); iterations--; }
            cancel.Cancel();
            System.Threading.Thread.Sleep(1000);
            stream.Dispose();
        }
    }

    class DataProvider : CSAudioStreamer.AudioStreamDataDelegate
    {
        private const int tableSize = 200;
        private float[] sine = new float[tableSize];
        private int left_phase;
        private int right_phase;

        public DataProvider()
        {
            for (int i = 0; i < tableSize; i++)
            {
                sine[i] = (float)Math.Sin(((double)i / (double)tableSize) * Math.PI * 2.0f);
            }
        }


        public void StreamComplete()
        {

        }

        public CSAudioStreamer.AudioStreamStatus TryGetData(out byte[] data)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < SETTINGS.FRAMES_PER_SAMPLE; i++)
            {
                bytes.AddRange(BitConverter.GetBytes(sine[left_phase]));  // Use BitConverter to convert floats to byte arrays
                bytes.AddRange(BitConverter.GetBytes(sine[right_phase])); // Use BitConverter to convert floats to byte arrays

                left_phase += 1;
                if (left_phase >= tableSize) left_phase -= tableSize;
                right_phase += 3; /* higher pitch so we can distinguish left and right. */
                if (right_phase >= tableSize) right_phase -= tableSize;
            }

            data = bytes.ToArray();

            return CSAudioStreamer.AudioStreamStatus.Data;
        }
    }
}
