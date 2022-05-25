﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Sublang
{
    class Keyboard
    {
        private static readonly byte[] DistinctVirtualKeys = Enumerable
           .Range(0, 256)
           .Select(KeyInterop.KeyFromVirtualKey)
           .Where(item => item != Key.None)
           .Distinct()
           .Select(item => (byte)KeyInterop.VirtualKeyFromKey(item))
           .ToArray();

        /// <summary>
        /// Gets all keys that are currently in the down state.
        /// </summary>
        /// <returns>
        /// A collection of all keys that are currently in the down state.
        /// </returns>
        public static List<Key> GetDownKeys()
        {
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);
            Console.WriteLine(string.Join(", ",keyboardState));


            var downKeys = new List<Key>();
            for (var index = 0; index < DistinctVirtualKeys.Length; index++)
            {
                var virtualKey = DistinctVirtualKeys[index];
                if ((keyboardState[virtualKey] & 0x80) != 0)
                {
                    downKeys.Add(KeyInterop.KeyFromVirtualKey(virtualKey));
                }
            }

            return downKeys;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetKeyboardState(byte[] keyState);
    }

   
}
