using System;
using System.Net;
using System.Runtime.InteropServices;

namespace PortAudioSharp.Native
{
    public static partial class Interop
    {
        /// <summary>
        /// Translate the supplied PortAudio error code into a human readable
        /// message.
        /// </summary>
        /// <param name="errorCode">Error code to translate to text.</param>
        /// <returns>Error Text.</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.LPTStr)]
        public static extern string Pa_GetErrorText([MarshalAs(UnmanagedType.I4)] PaErrorCode errorCode);

        /// <summary>
        /// Library initialization function - call this before using PortAudioSharp
        /// This function initializes internal data structures and prepares underlying
        /// host APIs for use.  With the exception of Pa_GetVersion(), Pa_GetVersionText(),
        /// and Pa_GetErrorText(), this function MUST be called before using any other
        /// PortAudio API functions.
        /// 
        /// If Pa_Initialize() is called multiple times, each successful 
        /// call must be matched with a corresponding call to Pa_Terminate(). 
        /// Pairs of calls to Pa_Initialize()/Pa_Terminate() may overlap, and are not 
        /// required to be fully nested.
        /// Note that if Pa_Initialize() returns an error code, Pa_Terminate() should
        /// NOT be called.
        /// 
        /// </summary>
        /// <returns>PaErrorCode.paNoError if successful, otherwise an error code indicating the cause</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_Initialize();

        /// <summary>
        /// Library termination function - call this when finished using PortAudioSharp
        /// This function deallocates all resources allocated by PortAudio since it was
        /// initialized by a call to Pa_Initialize(). In cases where Pa_Initialise() has
        /// been called multiple times, each call must be matched with a corresponding call
        /// to Pa_Terminate(). The final matching call to Pa_Terminate() will automatically
        /// close any PortAudio streams that are still open.
        ///
        /// Pa_Terminate() MUST be called before exiting a program which uses PortAudioSharp
        /// Failure to do so may result in serious resource leaks, such as audio devices
        /// not being available until the next reboot.
        /// </summary>
        /// <returns>paNoError if successful, otherwise an error code indicating the cause
        /// of failure.</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_Terminate();

        /// <summary>
        /// Retrieve the index of the default output device. The result can be
        /// used in the outputDevice parameter to Pa_OpenStream().
        /// </summary>
        /// <returns>The default output device index for the default host API, or paNoDevice
        /// if no default output device is available or an error was encountered.</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern System.Int32 Pa_GetDefaultOutputDevice();

        /// <summary>
        /// Retrieve a pointer to a PaDeviceInfo structure containing information
        /// about the specified device.
        /// </summary>
        /// <param name="deviceIndex"></param>
        /// <returns>A pointer to an immutable PaDeviceInfo structure. If the device
        /// parameter is out of range the function returns NULL.</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern IntPtr Pa_GetDeviceInfo(System.Int32 deviceIndex);

        /// <summary>
        /// Retrieve the number of available devices. The number of available devices
        /// may be zero.
        /// </summary>
        /// <returns>A non-negative value indicating the number of available devices or,
        /// a PaErrorCode (which are always negative) if PortAudio is not initialized
        /// or an error is encountered.</returns>returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern System.Int32 Pa_GetDeviceCount();

        /// <summary> Retrieve the index of the default host API. The default host API will be
        /// the lowest common denominator host API on the current platform and is
        /// unlikely to provide the best performance.</summary>
        /// <returns> A non-negative value ranging from 0 to (Pa_GetHostApiCount()-1)
        /// indicating the default host API index or, a PaErrorCode(which are always
        /// negative) if PortAudio is not initialized or an error is encountered.</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern System.Int32 Pa_GetDefaultHostApi();

        /// <summary>
        /// Put the caller to sleep for at least 'msec' milliseconds.This function is
        /// provided only as a convenience for authors of portable code(such as the tests
        /// and examples in the PortAudio distribution.)
        /// 
        /// The function may sleep longer than requested so don't rely on this for accurate
        /// musical timing.
        /// </summary>
        /// <param name="msec"></param>
        [DllImport(NATIVE_LIBRARY_NAME)]
        public static extern void Pa_Sleep(System.Int32 msec);

        /// <summary>
        /// Opens a stream for either input, output or both.
        /// </summary>
        /// <param name="stream">stream The address of a PaStream pointer which will receive
        /// a pointer to the newly opened stream.</param>
        /// <param name="inputParameters">A structure that describes the input parameters used by
        /// the opened stream.See PaStreamParameters for a description of these parameters.
        /// inputParameters must be NULL for output-only streams.</param>
        /// <param name="outputParameters">A structure that describes the output parameters used by
        /// the opened stream.See PaStreamParameters for a description of these parameters.
        /// outputParameters must be NULL for input-only streams.</param>
        ///  <param name="sampleRate">The desired sampleRate. For full-duplex streams it is the
        /// sample rate for both input and output</param>
        ///  <param name="framesPerBuffer">The number of frames passed to the stream callback
        /// function, or the preferred block granularity for a blocking read/write stream.
        /// The special value paFramesPerBufferUnspecified (0) may be used to request that
        /// the stream callback will receive an optimal(and possibly varying) number of
        /// frames based on host requirements and the requested latency settings.
        /// Note: With some host APIs, the use of non-zero framesPerBuffer for a callback
        /// stream may introduce an additional layer of buffering which could introduce
        /// additional latency.PortAudio guarantees that the additional latency
        /// will be kept to the theoretical minimum however, it is strongly recommended
        /// that a non-zero framesPerBuffer value only be used when your algorithm
        /// requires a fixed number of frames per stream callback.</param>
        ///  <param name="streamFlags">Flags which modify the behavior of the streaming process.
        /// This parameter may contain a combination of flags ORed together.Some flags may
        /// only be relevant to certain buffer formats.</param>
        ///  <param name="streamCallback">A pointer to a client supplied function that is responsible
        /// for processing and filling input and output buffers.If this parameter is NULL
        /// the stream will be opened in 'blocking read/write' mode.In blocking mode,
        /// the client can receive sample data using Pa_ReadStream and write sample data
        /// using Pa_WriteStream, the number of samples that may be read or written
        /// without blocking is returned by Pa_GetStreamReadAvailable and
        /// Pa_GetStreamWriteAvailable respectively.</param>
        /// <param name="userData">A client supplied pointer which is passed to the stream callback
        /// function.It could for example, contain a pointer to instance data necessary
        /// for processing the audio buffers. This parameter is ignored if streamCallback
        /// is NULL.</param>
        /// <returns>Upon success Pa_OpenStream() returns paNoError and places a pointer to a
        /// valid PaStream in the stream argument.The stream is inactive (stopped).
        /// If a call to Pa_OpenStream() fails, a non-zero error code is returned (see
        /// PaError for possible error codes) and the value of stream is invalid.</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_OpenStream(out IntPtr stream, IntPtr inputParameters, IntPtr outputParameters, double sampleRate,
            System.UInt32 framesPerBuffer, PaStreamFlags streamFlags, IntPtr streamCallback, IntPtr userData);

        /// <summary>
        /// Functions of type PaStreamCallback are implemented by PortAudio clients.
        /// They consume, process or generate audio in response to requests from an
        /// active PortAudio stream.
        ///
        /// When a stream is running, PortAudio calls the stream callback periodically.
        /// The callback function is responsible for processing buffers of audio samples 
        /// passed via the input and output parameters.
        ///
        /// The PortAudio stream callback runs at very high or real-time priority.
        /// It is required to consistently meet its time deadlines. Do not allocate 
        /// memory, access the file system, call library functions or call other functions 
        /// from the stream callback that may block or take an unpredictable amount of
        /// time to complete.
        ///
        /// In order for a stream to maintain glitch-free operation the callback
        /// must consume and return audio data faster than it is recorded and/or
        /// played. PortAudio anticipates that each callback invocation may execute for 
        /// a duration approaching the duration of frameCount audio frames at the stream 
        /// sample rate. It is reasonable to expect to be able to utilise 70% or more of
        /// the available CPU time in the PortAudio callback. However, due to buffer size 
        /// adaption and other factors, not all host APIs are able to guarantee audio 
        /// stability under heavy CPU load with arbitrary fixed callback buffer sizes. 
        /// When high callback CPU utilisation is required the most robust behavior 
        /// can be achieved by using paFramesPerBufferUnspecified as the 
        /// Pa_OpenStream() framesPerBuffer parameter.
        /// </summary>
        /// <param name="input">input and output are either arrays of interleaved samples or;
        /// if non-interleaved samples were requested using the paNonInterleaved sample
        /// format flag, an array of buffer pointers, one non-interleaved buffer for 
        /// each channel.
        ///
        /// The format, packing and number of channels used by the buffers are
        /// determined by parameters to Pa_OpenStream().</param>
        /// <param name="output">input and output are either arrays of interleaved samples or;
        /// if non-interleaved samples were requested using the paNonInterleaved sample
        /// format flag, an array of buffer pointers, one non-interleaved buffer for 
        /// each channel.
        ///
        /// The format, packing and number of channels used by the buffers are
        /// determined by parameters to Pa_OpenStream().</param>
        /// <param name="frameCount">The number of sample frames to be processed by
        /// the stream callback.</param>
        /// <param name="timeInfo"></param>
        /// <param name="statusFlags"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I4)]
        public delegate PaStreamCallbackResult PaStreamCallback(IntPtr input, IntPtr output, System.UInt32 frameCount, ref PaStreamCallbackTimeInfo timeInfo, 
            PaStreamCallbackFlags statusFlags, IntPtr userData);

        /// <summary>
        /// Closes an audio stream. If the audio stream is active it
        /// discards any pending buffers as if Pa_AbortStream() had been called.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_CloseStream(IntPtr stream);

        /// <summary>
        /// Register a stream finished callback function which will be called when the 
        /// stream becomes inactive. See the description of PaStreamFinishedCallback for 
        /// further details about when the callback will be called.
        /// </summary>
        /// <param name="stream">a pointer to a PaStream that is in the stopped state - if the
        /// stream is not stopped, the stream's finished callback will remain unchanged 
        /// and an error code will be returned.</param>
        /// <param name="streamFinishedCallback">a pointer to a function with the same signature
        /// as PaStreamFinishedCallback, that will be called when the stream becomes
        /// inactive.Passing NULL for this parameter will un-register a previously
        /// registered stream finished callback function.</param>
        /// <returns>on success returns paNoError, otherwise an error code indicating the cause
        /// of the error.</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_SetStreamFinishedCallback(IntPtr stream, FinishedCallback streamFinishedCallback);

        /// <summary>
        /// Functions of type PaStreamFinishedCallback are implemented by PortAudio 
        /// clients. They can be registered with a stream using the Pa_SetStreamFinishedCallback
        /// function. Once registered they are called when the stream becomes inactive
        /// (ie once a call to Pa_StopStream() will not block).
        /// A stream will become inactive after the stream callback returns non-zero,
        /// or when Pa_StopStream or Pa_AbortStream is called. For a stream providing audio
        /// output, if the stream callback returns paComplete, or Pa_StopStream() is called,
        /// the stream finished callback will not be called until all generated sample data
        /// has been played.
        /// </summary>
        /// <param name="userData">The userData parameter supplied to Pa_OpenStream()</param>
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void FinishedCallback(IntPtr userData);

        /// <summary>
        /// Commences audio processing.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_StartStream(IntPtr stream);

        /// <summary>
        /// Terminates audio processing. It waits until all pending
        /// audio buffers have been played before it returns.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_StopStream(IntPtr stream);

        /// <summary>
        /// Terminates audio processing immediately without waiting for pending
        /// buffers to complete.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_AbortStream(IntPtr stream);

        /// <summary>
        /// Determine whether the stream is stopped.
        /// A stream is considered to be stopped prior to a successful call to
        /// Pa_StartStream and after a successful call to Pa_StopStream or Pa_AbortStream.
        /// If a stream callback returns a value other than paContinue the stream is NOT
        /// considered to be stopped.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Returns one (1) when the stream is stopped, zero (0) when
        /// the stream is running or, a PaErrorCode(which are always negative) if
        /// PortAudio is not initialized or an error is encountered.</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_IsStreamStopped(IntPtr stream);

        /// <summary>
        /// Determine whether the stream is active.
        /// A stream is active after a successful call to Pa_StartStream(), until it
        /// becomes inactive either as a result of a call to Pa_StopStream() or
        /// Pa_AbortStream(), or as a result of a return value other than paContinue from
        /// the stream callback. In the latter case, the stream is considered inactive
        /// after the last buffer has finished playing.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Returns one (1) when the stream is active (ie playing or recording
        /// audio), zero (0) when not playing or, a PaErrorCode (which are always negative)
        /// if PortAudio is not initialized or an error is encountered.</returns>
        [DllImport(NATIVE_LIBRARY_NAME)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern PaErrorCode Pa_IsStreamActive(IntPtr stream);
    }
}
