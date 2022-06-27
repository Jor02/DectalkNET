using System;
using System.Runtime.InteropServices;
using DectalkNET.invokes;

namespace DectalkNET
{
    /// <summary>
    /// Allows you to use the Dectalk voice sythesizer in .NET applications
    /// </summary>
    public static class Dectalk
    {
        #region Dll Imports
        [DllImport("dectalk.dll", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern MMRESULT TextToSpeechStartup(IntPtr HWND, IntPtr* LPTTS_HANDLE_T, uint device, int i);

        [DllImport("dectalk.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern MMRESULT TextToSpeechShutdown(IntPtr LPTTS_HANDLE_T);

        [DllImport("dectalk.dll", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern MMRESULT TextToSpeechSpeak(IntPtr LPTTS_HANDLE_T, [MarshalAs(UnmanagedType.LPStr)] string LPSTR, long DWORD);
        [DllImport("dectalk.dll", CallingConvention = CallingConvention.Cdecl)]
        private unsafe static extern MMRESULT TextToSpeechOpenWaveOutFile(IntPtr LPTTS_HANDLE_T, [MarshalAs(UnmanagedType.LPStr)] string LPSTR, long DWORD);
        [DllImport("dectalk.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern MMRESULT TextToSpeechSync(IntPtr LPTTS_HANDLE_T);
        [DllImport("dectalk.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern MMRESULT TextToSpeechCloseWaveOutFile(IntPtr LPTTS_HANDLE_T);
        [DllImport("dectalk.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern MMRESULT TextToSpeechSetVolume(IntPtr LPTTS_HANDLE_T, int type, int volume);
        #endregion

        private static IntPtr ttsHandlePtr = defines.NULL;
        public static MMRESULT status { get; private set; }

        /// <summary>
        /// Starts the voice synthesizer
        /// </summary>
        public static void Startup() => Startup(true, defines.WAVE_MAPPER);
        /// <summary>
        /// Starts the voice synthesizer
        /// </summary>
        public static void Startup(uint device) => Startup(true, device);
        /// <summary>
        /// Starts the voice synthesizer
        /// </summary>
        public static void Startup(bool useAudioDevice) => Startup(useAudioDevice, defines.WAVE_MAPPER);
        /// <summary>
        /// Starts the voice synthesizer
        /// </summary>
        /// <param name="useAudioDevice">Should audio device be used</param>
        /// <param name="device">Output device that should be used</param>
        /// public static void Startup(bool useAudioDevice)
        public static void Startup(bool useAudioDevice, uint device)
        {
            IntPtr HandlePtr = defines.NULL;
            unsafe
            {
                status = TextToSpeechStartup(defines.NULL, &HandlePtr, device, 0);
            }
            ttsHandlePtr = HandlePtr;
        }

        public static void Say(string text) => Say(text, true);
        
        public static void Say(string text, bool force)
        {
            unsafe
            {
                status = TextToSpeechSpeak(ttsHandlePtr, text, force ? 1 : 0);
            }
        }

        public static void WaveOut(string text, string filename)
        {
            unsafe
            {
                status = TextToSpeechOpenWaveOutFile(ttsHandlePtr, filename, 1);
                status = TextToSpeechSpeak(ttsHandlePtr, text, 1);

                /* Sync to make sure everything has come out */
                status = TextToSpeechSpeak(ttsHandlePtr, "        ", 1);
                status = TextToSpeechSync(ttsHandlePtr);

                status = TextToSpeechCloseWaveOutFile(ttsHandlePtr);
            }
        }

        public static void SetVolume(int volume)
        {
            status = TextToSpeechSetVolume(ttsHandlePtr, 2, volume);
        }

        public static void WaitForSpeech()
        {
            status = TextToSpeechSync(ttsHandlePtr);
        }

        public static void Shutdown()
        {
            status = TextToSpeechShutdown(ttsHandlePtr);
        }
    }
}
