using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static PortAudioSharp.Native.Interop;

namespace PortAudioSharp
{
    public class PortAudioOutputStream : IDisposable
    {
        // -- Audio Configuration Members --
        private int mNumChannels = 2;
        private int mSampleRate = 44100;
        private uint mFramesPerBuffer = 64;
        private string mAudioDeviceId = string.Empty;
        private int mBitsPerSample = 16;
        private PaSampleFormat mSampleFormat = PaSampleFormat.Int16;

        private void SetBitsPerSample() {
            if((mSampleFormat & PaSampleFormat.Float32) != 0) mBitsPerSample = 32;
            if ((mSampleFormat & PaSampleFormat.Int32) != 0) mBitsPerSample = 32;
            if ((mSampleFormat & PaSampleFormat.Int24) != 0) mBitsPerSample = 24;
            if ((mSampleFormat & PaSampleFormat.Int16) != 0) mBitsPerSample = 16;
            if ((mSampleFormat & PaSampleFormat.UInt8) != 0) mBitsPerSample = 8;
            if ((mSampleFormat & PaSampleFormat.Int8) != 0) mBitsPerSample = 8;
            mBitsPerSample = 16;
        }

        // -- Native Handles and Delegates for PortAudio Native --
        private IntPtr mpStreamHandle = IntPtr.Zero;
        IntPtr mpOutParameters = IntPtr.Zero;
        private PaStreamCallback mStreamDelegate = null;
        IntPtr mpStreamDelegate = IntPtr.Zero;

        // -- State Management Variables
        private object _lock;
        private bool bIsStreamPlaying = false;
        private bool bIsStreamOpen = false;

        public PortAudioOutputStream(int channels = 2, int sampleRate = 44100, PaSampleFormat sampleFormat = PaSampleFormat.Float32, uint framesPerBuffer = 64)
        {
            _lock = new object();

            mNumChannels = channels;
            mSampleRate = sampleRate;
            mFramesPerBuffer = framesPerBuffer;
            mSampleFormat = sampleFormat;
            SetBitsPerSample();

            mStreamDelegate = new PaStreamCallback(DataCallbackFunc);
            mpStreamDelegate = Marshal.GetFunctionPointerForDelegate<PaStreamCallback>(mStreamDelegate);
        }

        #region -- AudioStreamer public APIs --

        public void Stream(AudioStreamDataDelegate dataProvider, CancellationToken cancellation)
        {
            if (bIsStreamPlaying || dataProvider == null)
                return;

            Task.Run(() =>
            {
                bool bDone = false;
                bIsStreamPlaying = true;

                while (true)
                {
                    byte[] readData = null;
                    var status = dataProvider.TryGetData(out readData);

                    //Handle Halt and Cancellation
                    if (status == AudioStreamStatus.Halted || cancellation.IsCancellationRequested)
                    {
                        Pa_AbortStream(mpStreamHandle);
                        break;
                    }

                    //We have data, write it to the queue
                    if (status == AudioStreamStatus.Data && readData != null)
                    {
                        for (int i = 0; i < readData.Length; i++)
                            _PlaybackQueue.Enqueue(readData[i]);

                        if (IsPreBufferFull(status) && !bIsStreamOpen)
                        {
                            if (OpenStream() != PaErrorCode.NoError)
                            {
                                CloseStream();
                                dataProvider.StreamComplete();
                                return;
                            }
                        }

                        continue;
                    }

                    //we don't have data but we are not finished.
                    if (status == AudioStreamStatus.Data && readData == null)
                    {
                        continue;
                    }

                    //Audio Stream is Finished, pad half a second of silence to avoid
                    //violent cutoffs.
                    if (status == AudioStreamStatus.Finished && bDone == false)
                    {
                        System.Int32 frameSize = (mBitsPerSample / 8) * mNumChannels;
                        System.Int32 samplesToWrite = frameSize * (mSampleRate / 2);
                        for (int i = 0; i < samplesToWrite; i++)
                            _PlaybackQueue.Enqueue(0);

                        bDone = true;
                    }

                    //Audio Stream is finished but the streamer is still consuming data.
                    if (status == AudioStreamStatus.Finished && _PlaybackQueue.Count > 0)
                    {
                        if (Pa_IsStreamActive(mpStreamHandle) <= 0)
                            break;

                        continue;
                    }

                    if (status == AudioStreamStatus.Finished && _PlaybackQueue.Count == 0)
                    {
                        break;
                    }
                }

                CloseStream();
                bIsStreamPlaying = false;
                dataProvider.StreamComplete();
            });
        }


        private bool IsPreBufferFull(AudioStreamStatus status)
        {
            System.Int32 frameSize = (mBitsPerSample / 8) * mNumChannels;
            System.Int32 prebufferSize = frameSize * (mSampleRate / 5);  //100ms

            //We have pre-buffered the desired number of frames, so we don't lag behind the
            //needs of the underlying audio stream.
            if (_PlaybackQueue.Count >= prebufferSize) return true;

            //Audio is finished, but the buffer is shorter than the desired length
            //we need to open the stream now.
            if (status == AudioStreamStatus.Halted) return true;

            return false;
        }

        public void AddTimelineMarker(TimeSpan mark, Action<object> callback, object tag)
        {
            if (_TimelineMarkers == null)
                _TimelineMarkers = new ConcurrentQueue<TimelineMarker>();

            _TimelineMarkers.Enqueue(new TimelineMarker
            {
                Mark = mark,
                Callback = callback,
                Tag = tag
            });
        }

        #endregion

        #region -- Port Audio Streaming Code --
        private ConcurrentQueue<byte> _PlaybackQueue = new ConcurrentQueue<byte>();

        private PaStreamCallbackResult DataCallbackFunc(IntPtr inputBuffer, IntPtr outputBuffer, System.UInt32 frameCount,
            ref PaStreamCallbackTimeInfo timeInfo, PaStreamCallbackFlags statusFlags, IntPtr userData)
        {
            unsafe
            {
                byte* pOutBuf = (byte*)outputBuffer;
                System.Int32 frameSize = (mBitsPerSample / 8) * mNumChannels;

                for (System.UInt32 i = 0; i < frameSize * frameCount; i++)
                {
                    byte b = 0;
                    if (_PlaybackQueue == null || !_PlaybackQueue.TryDequeue(out b))
                        *pOutBuf++ = 0;
                    else
                        *pOutBuf++ = b;
                }
            }

            //update the time line with the current playback state
            double sampleDuration = (double)frameCount / (double)mSampleRate;
            TimeSpan duration = new TimeSpan((long)(sampleDuration * TimeSpan.TicksPerSecond));
            _TimeLine = UpdateTimeline(_TimeLine, duration);

            return PaStreamCallbackResult.paContinue;
        }

        private PaDeviceInfo? GetDeviceInfo(string deviceId)
        {
            foreach (PaDeviceInfo device in GetPaDeviceInfos())
            {
                //todo come up with an API thing
            }

            return null;
        }

        private IEnumerable<PaDeviceInfo> GetPaDeviceInfos()
        {
            List<PaDeviceInfo> devices = new List<PaDeviceInfo>();

            if (Pa_Initialize() != PaErrorCode.NoError) return devices;

            int defaltHostApiId = Pa_GetDefaultHostApi();

            int numDevices = Pa_GetDeviceCount();

            for (int i = 0; i < numDevices; i++)
            {
                IntPtr ptrStruct = Pa_GetDeviceInfo(i);
                PaDeviceInfo deviceInfo = Marshal.PtrToStructure<PaDeviceInfo>(ptrStruct);

                // only give the user choices of devices in the default API
                if (deviceInfo.hostApi == defaltHostApiId)
                    devices.Add(deviceInfo);
            }

            Pa_Terminate();
            return devices;
        }

        private PaErrorCode BuildPaStreamParameters()
        {
            var err = Pa_Initialize();
            if (err != PaErrorCode.NoError) return err;

            PaStreamParameters outputParameters;
            outputParameters.hostApiSpecificStreamInfo = IntPtr.Zero;
            outputParameters.ChannelCount = mNumChannels;
            outputParameters.SampleFormat = mSampleFormat;
            
            //todo: Device List and Pick default based on default API
            outputParameters.DeviceIndex = Pa_GetDefaultOutputDevice();
            IntPtr ptrStruct = Pa_GetDeviceInfo(outputParameters.DeviceIndex);

            if (ptrStruct != IntPtr.Zero)
            {
                PaDeviceInfo deviceInfo = Marshal.PtrToStructure<PaDeviceInfo>(ptrStruct);
                outputParameters.SuggestedLatency = deviceInfo.defaultHighOutputLatency;
            }
            else
            {
                outputParameters.SuggestedLatency = 1.0;
            }

            if (mpOutParameters == IntPtr.Zero) mpOutParameters = Marshal.AllocHGlobal(Marshal.SizeOf(outputParameters));
            Marshal.StructureToPtr(outputParameters, mpOutParameters, false);

            Pa_Terminate();
            return PaErrorCode.NoError;
        }

        private PaErrorCode OpenStream()
        {
            lock (_lock)
            {
                var err = Pa_Initialize();
                if (err != PaErrorCode.NoError) return err;

                err = BuildPaStreamParameters();
                if (err != PaErrorCode.NoError) return err;

                err = Pa_OpenStream(out mpStreamHandle, IntPtr.Zero, mpOutParameters, mSampleRate, mFramesPerBuffer, PaStreamFlags.NoFlag, mpStreamDelegate, IntPtr.Zero);
                if (err != PaErrorCode.NoError) return err;

                bIsStreamOpen = true;
                return Pa_StartStream(mpStreamHandle);
            }
        }

        private void CloseStream()
        {
            lock (_lock)
            {
                while ((int)Pa_IsStreamActive(mpStreamHandle) > 0 && _PlaybackQueue.Count > 0) Pa_Sleep(100);

                if (mpStreamHandle != IntPtr.Zero)
                    Pa_CloseStream(mpStreamHandle);

                mpStreamHandle = IntPtr.Zero;

                Pa_Terminate();

                _TimeLine = TimeSpan.Zero;
                _TimelineMarkers = null;

                _PlaybackQueue = new ConcurrentQueue<byte>();

                bIsStreamOpen = false;
            }
        }
        #endregion

        #region -- Timeline Helpers --
        private ConcurrentQueue<TimelineMarker> _TimelineMarkers;
        private TimeSpan _TimeLine;

        private class TimelineMarker
        {
            public TimeSpan Mark { get; set; }
            public Action<object> Callback { get; set; }
            public object Tag { get; set; }
        }

        private TimeSpan UpdateTimeline(TimeSpan sampleTime, TimeSpan sampleDuration)
        {
            var startTime = sampleTime;
            var endTime = startTime + sampleDuration;

            NotifyTimeline(startTime, endTime);

            return endTime;
        }

        private void NotifyTimeline(TimeSpan startTime, TimeSpan endTime)
        {
            if (_TimelineMarkers == null || _TimelineMarkers.Count <= 0)
                return;

            TimelineMarker last = null;

            while (true)
            {
                if (!_TimelineMarkers.TryPeek(out TimelineMarker marker))
                    break;

                if (startTime >= marker.Mark || endTime >= marker.Mark)
                {
                    _TimelineMarkers.TryDequeue(out last);
                }
                else
                {
                    break;
                }
            }

            if (last != null)
                last.Callback?.Invoke(last.Tag);
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (mpOutParameters != IntPtr.Zero)
                    Marshal.FreeHGlobal(mpOutParameters);

                if (mpStreamHandle != IntPtr.Zero)
                    CloseStream();

                mpOutParameters = IntPtr.Zero;
                mpStreamHandle = IntPtr.Zero;
                disposedValue = true;
            }
        }

        ~PortAudioOutputStream()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
