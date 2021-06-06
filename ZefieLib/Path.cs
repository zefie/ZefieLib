﻿using System.IO;
using System.Runtime.InteropServices;

namespace ZefieLib
{
    public class Path
    {
        /// <summary>
        /// Finds the drive letter of the drive with the specified label
        /// </summary>
        /// <param name="label">Label of the drive to search for (case sensitive)</param>
        /// <returns>Drive letter in format of X:\, or null if no results</returns>
        public static string GetDriveLetterFromLabel(string label)
        {
            foreach (DriveInfo DI in DriveInfo.GetDrives())
            {
                try
                {
                    if (DI.VolumeLabel.Length > 0)
                    {
                        if (DI.VolumeLabel == label)
                        {
                            return DI.Name;
                        }
                    }
                }
                catch { }
            }
            return null;
        }

        [DllImport("kernel32.dll")]

        /// <summary>
        /// Creates an NTFS Symlink (Junction)
        /// </summary>
        /// <param name="lpSymlinkFileName">Location for the new symlink</param>
        /// <param name="lpTargetFileName">Location of the file to be linked to</param>
        /// <param name="dwFlags">SymbolicLink Flags (File or Directory)</param>
        /// <returns>True if successful, false if not</returns>
        public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        public enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }
    }
}
