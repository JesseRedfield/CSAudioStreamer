using System;
using System.Runtime.InteropServices;

namespace CSAudioStreamer
{

    [StructLayout(LayoutKind.Sequential)]
    public struct PaStreamParameters
    {
        /// <summary>
        /// A valid device index in the range 0 to (Pa_GetDeviceCount()-1)
        /// specifying the device to be used or the special constant
        /// paUseHostApiSpecificDeviceSpecification which indicates that the actual
        /// device(s) to use are specified in hostApiSpecificStreamInfo.
        /// This field must not be set to paNoDevice.
        /// </summary>
        public System.Int32 DeviceIndex;

        /// <summary>
        /// The number of channels of sound to be delivered to the
        /// stream callback or accessed by Pa_ReadStream() or Pa_WriteStream().
        /// It can range from 1 to the value of maxInputChannels in the
        /// PaDeviceInfo record for the device specified by the device parameter.
        /// </summary>
        public int ChannelCount;

        /// <summary>
        ///The sample format of the buffer provided to the stream callback,
        ///a_ReadStream() or Pa_WriteStream(). It may be any of the formats described
        ///by the PaSampleFormat enumeration.
        /// </summary>
        public PaSampleFormat SampleFormat;

        /// <summary>
        /// The desired latency in seconds. Where practical, implementations should
        /// configure their latency based on these parameters, otherwise they may
        /// choose the closest viable latency instead. Unless the suggested latency
        /// is greater than the absolute upper limit for the device implementations
        /// should round the suggestedLatency up to the next practical value - ie to
        /// provide an equal or higher latency than suggestedLatency wherever possible.
        /// Actual latency values for an open stream may be retrieved using the
        /// inputLatency and outputLatency fields of the PaStreamInfo structure
        /// returned by Pa_GetStreamInfo().
        /// @see default*Latency in PaDeviceInfo, *Latency in PaStreamInfo
        /// </summary>
        public System.Double SuggestedLatency;

        /// <summary>
        /// An optional pointer to a host api specific data structure
        /// containing additional information for device setup and/or stream processing.
        /// hostApiSpecificStreamInfo is never required for correct operation,
        /// if not used it should be set to NULL.
        /// </summary>
        public IntPtr hostApiSpecificStreamInfo;
    }

    /// <summary>
    /// Timing information for the buffers passed to the stream callback.
    /// Time values are expressed in seconds and are synchronised with the time base used by Pa_GetStreamTime() for the associated stream.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PaStreamCallbackTimeInfo
    {
        /// <summary>
        /// The time when the first sample of the input buffer was captured at the ADC input
        /// </summary>
        public System.Double inputBufferAdcTime;

        /// <summary>
        /// The time when the stream callback was invoked
        /// </summary>
        public System.Double currentTime;

        /// <summary>
        /// The time when the first sample of the output buffer will output the DAC
        /// </summary>
        public System.Double outputBufferDacTime;
    }

    /// <summary>
    /// Timing information for the buffers passed to the stream callback.
    ///
    /// Time values are expressed in seconds and are synchronised with the time base used by Pa_GetStreamTime() for the associated stream.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StreamCallbackTimeInfo
    {
        /// <summary>
        /// The time when the first sample of the input buffer was captured at the ADC input
        /// </summary>
        public System.Double inputBufferAdcTime;

        /// <summary>
        /// The time when the stream callback was invoked
        /// </summary>
        public System.Double currentTime;

        /// <summary>
        ///  The time when the first sample of the output buffer will output the DAC
        /// </summary>
        public System.Double outputBufferDacTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PaDeviceInfo
    {
        public int structVersion;

        [MarshalAs(UnmanagedType.LPStr)]
        public string name;

        public int hostApi;

        public int maxInputChannels;
        public int maxOutputChannels;
        /// <summary>
        // Default latency values for interactive performance.
        /// </summary>
        public System.Double defaultLowInputLatency;
        public System.Double defaultLowOutputLatency;

        /// <summary>
        /// Default latency values for robust non-interactive applications (eg. playing sound files).
        /// </summary>
        public System.Double defaultHighInputLatency;
        public System.Double defaultHighOutputLatency;

        public double defaultSampleRate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PaHostApiInfo
    {
        /// <summary>
        /// this is struct version 1
        /// </summary>
        int structVersion;
        /// <summary>
        /// The well known unique identifier of this host API @see PaHostApiTypeId
        /// </summary>
        PaHostApiTypeId type;
        /// <summary>
        /// A textual description of the host API for display on user interfaces.
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr)]
        public string name;

        /// <summary>
        /// The number of devices belonging to this host API. This field may be
        /// used in conjunction with Pa_HostApiDeviceIndexToDeviceIndex() to enumerate
        /// all devices for this host API.
        /// </summary>
        int deviceCount;

        ///<summary> The default input device for this host API. The value will be a
        ///device index ranging from 0 to (Pa_GetDeviceCount()-1), or paNoDevice
        ///if no default input device is available.
        System.Int32 defaultInputDevice;

        ///<summary> The default output device for this host API. The value will be a
        /// device index ranging from 0 to(Pa_GetDeviceCount()-1), or paNoDevice
        /// if no default output device is available.
        ///</summary>
        System.Int32 defaultOutputDevice;

    }
}
