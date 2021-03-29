namespace PortAudioSharp
{
    public interface AudioStreamDataDelegate
    {
        AudioStreamStatus TryGetData(out byte[] data);

        void StreamComplete();
    }
}