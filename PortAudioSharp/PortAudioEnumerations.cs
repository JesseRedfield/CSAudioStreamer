namespace PortAudioSharp
{
    /// <summary>
    /// Error codes returned by PortAudio functions.
    /// Note that with the exception of paNoError, all PaErrorCodes are negative.
    /// </summary>
    public enum PaErrorCode
    {
        NoError = 0,

        NotInitialized = -10000,
        UnanticitedHostError,
        InvalidChannelCount,
        InvalidSampleRate,
        InvalidDevice,
        InvalidFlag,
        SampleFormatNotSupported,
        BadIODeviceCombination,
        InsufficientMemory,
        BufferTooBig,
        BufferTooSmall,
        NullCallback,
        BadStreamPtr,
        TimedOut,
        InternalError,
        DeviceUnavailable,
        IncomtibleHostApiSpecificStreamInfo,
        StreamIsStopped,
        StreamIsNotStopped,
        InputOverflowed,
        OutputUnderflowed,
        HostApiNotFound,
        InvalidHostApi,
        CanNotReadFromACallbackStream,
        CanNotWriteToACallbackStream,
        CanNotReadFromAnOutputOnlyStream,
        CanNotWriteToAnInputOnlyStream,
        IncomtibleStreamHostApi,
        BadBufferPtr
    }

    /// <summary>
    /// A type used to specify one or more sample formats. Each value indicates
    /// a possible format for sound data passed to and from the stream callback,
    /// Pa_ReadStream and Pa_WriteStream.
    ///
    /// The standard formats paFloat32, paInt16, paInt32, paInt24, paInt8
    /// and aUInt8 are usually implemented by all implementations.
    ///
    /// The floating point representation (paFloat32) uses +1.0 and -1.0 as the
    /// maximum and minimum respectively.
    ///
    /// paUInt8 is an unsigned 8 bit format where 128 is considered "ground"
    ///
    /// The paNonInterleaved flag indicates that audio data is passed as an array
    /// of pointers to separate buffers, one buffer for each channel. Usually,
    /// when this flag is not used, audio data is passed as a single buffer with
    /// all channels interleaved.
    /// </summary>
    public enum PaSampleFormat : System.UInt32
    {
        Float32 = 0x00000001,
        Int32 = 0x00000002,
        Int24 = 0x00000004,
        Int16 = 0x00000008,
        Int8 = 0x00000010,
        UInt8 = 0x00000020,
        //CUSTOM AND NON INTERLEAVED NOT SUPPORTED BY THE CURRENT WRAPPER
        //CustomFormat = 0x00010000,
        //NonInterleaved = 0x80000000,
    }

    /// <summary>
    /// Flags used to control the behavior of a stream. They are passed as
    /// parameters to Pa_OpenStream or Pa_OpenDefaultStream.Multiple flags may be
    /// ORed together.
    /// </summary>
    public enum PaStreamFlags : System.UInt32
    {
        NoFlag = 0,
        /// <summary>
        /// Disable default clipping of out of range samples.
        /// </summary>
        ClipOff = 0x00000001,

        /// <summary>
        /// Disable default dithering.
        /// </summary>
        DitherOff = 0x00000002,

        /// <summary>
        /// Flag requests that where possible a full duplex stream will not discard
        /// overflowed input samples without calling the stream callback. This flag is
        /// only valid for full duplex callback streams and only when used in combination
        /// with the paFramesPerBufferUnspecified (0) framesPerBuffer parameter. Using
        /// this flag incorrectly results in a paInvalidFlag error being returned from
        /// Pa_OpenStream and Pa_OpenDefaultStream.
        ///
        /// </summary>
        NeverDropInput = 0x00000004,

        /// <summary>
        /// Call the stream callback to fill initial output buffers, rather than the
        /// default behavior of priming the buffers with zeros (silence). This flag has
        /// no effect for input-only and blocking read/write streams.
        /// </summary>
        PrimeOutputBuffersUsingStreamCallback = 0x00000008,

        /// <summary>
        /// A mask specifying the platform specific bits.
        /// </summary>
        PlatformSpecificFlags = 0xFFFF0000,
    }

    /// <summary>
    /// Allowable return values for the PaStreamCallback.
    /// </summary>
    public enum PaStreamCallbackResult
    {
        /// <summary>
        /// Signal that the stream should continue invoking the callback and processing audio. 
        /// </summary>
        paContinue = 0,

        /// <summary>
        /// Signal that the stream should stop invoking the callback and finish once all output samples have played.
        /// </summary>
        paComplete = 1,

        /// <summary>
        /// Signal that the stream should stop invoking the callback and finish as soon as possible. 
        /// </summary>
        paAbort = 2
    }

    /// <summary>
    /// Flag bit constants for the statusFlags to PaStreamCallback.
    /// </summary>
    public enum PaStreamCallbackFlags : System.UInt32
    {
        /// <summary>
        /// In a stream opened with paFramesPerBufferUnspecified, indicates that
        /// input data is all silence (zeros) because no real data is available. In a
        /// stream opened without paFramesPerBufferUnspecified, it indicates that one or
        /// more zero samples have been inserted into the input buffer to compensate
        /// for an input underflow.
        /// </summary>
        InputUnderflow = 0x00000001,

        /// <summary>
        /// In a stream opened with paFramesPerBufferUnspecified, indicates that data
        /// prior to the first sample of the input buffer was discarded due to an
        /// overflow, possibly because the stream callback is using too much CPU time.
        /// Otherwise indicates that data prior to one or more samples in the
        /// input buffer was discarded.
        /// </summary>
        InputOverflow = 0x00000002,

        /// <summary>
        /// Indicates that output data (or a gap) was inserted, possibly because the
        /// stream callback is using too much CPU time.
        /// </summary>
        OutputUnderflow = 0x00000004,

        /// <summary>
        /// Indicates that output data will be discarded because no room is available.
        /// </summary>
        OutputOverflow = 0x00000008,

        /// <summary>
        /// Some of all of the output data will be used to prime the stream, input
        /// data may be zero.
        /// </summary>
        PrimingOutput = 0x00000010
    }

    /// <sumary>Unchanging unique identifiers for each supported host API. This type
    /// is used in the PaHostApiInfo structure. The values are guaranteed to be
    /// unique and to never change, thus allowing code to be written that
    /// conditionally uses host API specific extensions.

    /// New type ids will be allocated when support for a host API reaches
    /// "public alpha" status, prior to that developers should use the
    /// paInDevelopment type id.
    ///</summary>

    enum PaHostApiTypeId
    {
        paInDevelopment = 0,
        paDirectSound = 1,
        paMME = 2,
        paASIO = 3,
        paSoundManager = 4,
        paCoreAudio = 5,
        paOSS = 7,
        paALSA = 8,
        paAL = 9,
        paBeOS = 10,
        paWDMKS = 11,
        paJACK = 12,
        paWASAPI = 13,
        paAudioScienceHPI = 14
    }
}