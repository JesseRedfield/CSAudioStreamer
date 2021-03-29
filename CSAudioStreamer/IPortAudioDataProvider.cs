namespace CSAudioStreamer
{
    public interface AudioStreamDataDelegate
    {
        AudioStreamStatus TryGetData(out byte[] data);

        void StreamComplete();
    }
}