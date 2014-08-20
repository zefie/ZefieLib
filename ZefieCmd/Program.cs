﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Reflection;

namespace ZefieCmd
{
    class Program
    {
        private static void exit(int code)
        {
            Environment.Exit(code);
        }
        private static string getExecFilename(bool noext = false)
        {
            if (noext)
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            else
                return Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase);
        }
        private static void showHelp(bool shorthelp = false)
        {
            if (shorthelp)
            {
                wl("Zefie.dll Command Interface");
                wl("Usage: " + getExecFilename() + " [options] command value");
                wl("See --help for more information");
            }
            else
            {
                wl("Zefie.dll Command Interface");
                we("Usage: " + getExecFilename() + " [options] command value");
                wl("Available Commands:");
                wl(null);
                wl("Hashing:");
                wl("\tmd5 sha1 sha256 sha384 sha512");
                wl(null);
                wl("Options:");
                wl("\t-q, --quiet\tDo not print filename with result");
                wl(null);
                wl("Acceptable Values:");
                wl("\t[filename, blank for stdin]");
                wl(null);
                wl("Encryption:");
                wl("\tencrypt decrypt");
                wl(null);
                wl("Options:");
                wl("\t-r, --raw\tRaw binary data, instead of Base64");
                wl(null);
                wl("Acceptable Values:");
                wl("\tpassword [filename or text, blank for stdin]");
                wl(null);
                wl("File Browsing:");
                wl("\topenfile openfiles savefile");
                wl(null);
                wl("Acceptable Values:");
                wl("\t[title] [starting folder] [filter]");
                wl(null);
                wl("Folder Browsing:");
                wl("\tfolder");
                wl(null);
                wl("Acceptable Values:");
                wl("\t[title] [show new folder button (yes/y/true/1 or no/n/false/0)]");
                wl(null);
                wl("Windows Form Dialogs:");
                wl("\tmsgbox errordialog confirmdialog promptdialog");
                wl(null);
                wl("Acceptable Values:");
                wl("\t(message text) [title]");
                wl(null);
                wl("Random Hex String:");
                wl("\tgenhex");
                wl(null);
                wl("Acceptable Values:");
                wl("\tlength");
                wl(null);
                wl("Random String:");
                wl("\tgenstr");
                wl(null);
                wl("Acceptable Values:");
                wl("\tlength [characters]");
                wl(null);
                wl("Random Number:");
                wl("\tgennum");
                wl(null);
                wl("Acceptable Values:");
                wl("\t[max] or [min] [max]");
                wl(null);
                wl("Math:");
                wl("\tpercent eval");
                wl(null);
                wl("Acceptable Values:");
                wl("\tpercent: value max");
                wl("\teval takes an expression, for example: 9*9/2");
                wl(null);
                wl("Other stuff:");
                wl("substring (string) (start) (length)");
                wl("contains (haystack) (needle) - Returns bool");
                wl("drivelabel (label) - Returns X:\\, where X = Drive Letter. null if not found.");
            }
            exit(0);
        }
        private static void we(string str)
        {
            using (Stream s = Console.OpenStandardError())
            {
                byte[] b = Encoding.UTF8.GetBytes(str+"\r\n");
                s.Write(b, 0, b.Length);
            }
        }
        private static void w(string s)
        {
            Console.Write(s);
        }
        private static void w(int n)
        {
            Console.Write(n);
        }
        private static void w(byte[] b)
        {
            using (Stream s = Console.OpenStandardOutput())
            {
                s.Write(b, 0, b.Length);
            }
        }
        private static void wl(string s)
        {
            w(s + "\r\n");
        }
        private static void wl(int n)
        {
            w(n.ToString() + "\r\n");
        }

        private static bool quiet = false;
        private static int readblock = 4096;
        static void Main(string[] args)
        {
            // This allows you to rename the exe to any available command within
            // and it will execute that command on default.
            // For example, if you named this exe "md5.exe",
            // Then executed "echo test | md5.exe", you would get
            // a55ab7512f0d0ff4527d898d06afd5c5        -

            if (!doCmd(args,true))
            {
                List<string> mew = new List<string>();
                mew.Add(getExecFilename(true));
                foreach (string arg in args)
                        mew.Add(arg);
                
                doCmd(mew.ToArray());
            }
        }
        private static bool doCmd(string[] args, bool nohelp = false)
        {
            string arg1 = null;
            bool rawdata = false;
            if (args.Count() >= 1)
            {
                List<string> mew = new List<string>();
                List<string> opt = new List<string>();
                foreach (string arg in args)
                {
                    if (arg.Substring(0, 1) == "-") 
                        opt.Add(arg);
                    else
                        mew.Add(arg);
                }
                args = mew.ToArray();

                foreach (string o in opt)
                {
                    if (o == "-n" || o == "--no-sort")
                        DirFlags.sort = false;
                    if (o == "-r" || o == "--recursive")
                        DirFlags.recursive = true;
                    if (o == "-s" || o == "--system")
                        DirFlags.system = true;
                    if (o == "-h" || o == "--hidden")
                        DirFlags.hidden = true;
                    if (o == "-r" || o == "--raw")
                        rawdata = true;
                    if (o == "-q" || o == "--quiet")
                        quiet = true;
                    if (o == "-?" || o == "--help")
                        showHelp();

                }
                if (args.Count() >= 2)
                    arg1 = args[1];

                switch (args[0].ToLower())
                {
                    case "md5":
                    case "sha1":
                    case "sha256":
                    case "sha384":
                    case "sha512":
                        {
                            if (args.Count() < 1)
                            {
                                for (int i = 1; i < args.Count(); i++)
                                    hashcmd(args[i], args[0]);
                            }
                            else
                            {
                                hashcmd(args[0]);
                            }
                            break;
                        }
                    case "dir":
                        {
                            string path = null;
                            string search = null;
                            string fname = null;
                            if (args.Count() > 2)
                                search = args[2];

                            if (Directory.Exists(arg1))
                                path = arg1;
                            else
                            {
                                path = Directory.GetCurrentDirectory();
                                if (arg1 != null)
                                    search = arg1;
                            }
                            try
                            {
                                string[] dirs = Zefie.Directory.GetDirectories(path, search, DirFlags.sort, DirFlags.recursive);
                                string[] files = Zefie.Directory.GetFiles(path, search, DirFlags.sort, DirFlags.recursive);

                                foreach (string dir in dirs)
                                {
                                    DirectoryInfo d = new DirectoryInfo(dir);
                                    if (d.Attributes.ToString().Contains("Hidden") && !DirFlags.hidden)
                                        continue;
                                    if (d.Attributes.ToString().Contains("System") && !DirFlags.system)
                                        continue;

                                    if (DirFlags.recursive)
                                        fname = d.FullName;
                                    else
                                        fname = d.Name;

                                    wl(d.CreationTime.ToString("MM/dd/yy  hh:mm tt") + "\t<DIR>\t\t" + fname);
                                }

                                foreach (string file in files)
                                {
                                    FileInfo f = new FileInfo(file);
                                    if (f.Attributes.ToString().Contains("Hidden") && !DirFlags.hidden)
                                        continue;
                                    if (f.Attributes.ToString().Contains("System") && !DirFlags.system)
                                        continue;

                                    if (DirFlags.recursive)
                                        fname = f.FullName;
                                    else
                                        fname = f.Name;

                                    wl(f.CreationTime.ToString("MM/dd/yy  hh:mm tt") + "\t\t" + Zefie.Math.calcBytes(f.Length) + "\t" + fname);

                                }
                            }
                            catch (Exception e)
                            {
                                wl("ERROR: " + e.Message);
                            }
                            break;
                        }
                    case "encrypt":
                        {
                            if (arg1 != null)
                            {
                                if (args.Count() >= 3)
                                    encrypt(arg1, args[2], rawdata);
                                else
                                    encrypt(arg1, null, rawdata);
                            }
                            else
                            {
                                we("Usage: " + getExecFilename() + " "+args[0]+" password [text or file, blank for stdin]");
                            }
                            break;
                        }
                    case "decrypt":
                        {
                            if (arg1 != null)
                            {
                                if (args.Count() >= 3)
                                    decrypt(arg1, args[2], rawdata);
                                else
                                    decrypt(arg1, null, rawdata);
                            }
                            else
                            {
                                we("Usage: " + getExecFilename() + " "+args[0]+" password [text or file, blank for stdin]");
                            }
                            break;
                        }
                    case "openfile":
                        {
                            string arg2 = null;
                            string arg3 = null;
                            if (args.Count() >= 3)
                                arg2 = args[2];
                            if (args.Count() >= 4)
                                arg3 = args[3];

                            try { wl(Zefie.Prompts.browseOpenFile(arg1, arg2, arg3)); }
                            catch (Exception e)
                            {
                                wl("ERROR: " + e.Message);
                            }
                            break;
                        }
                    case "openfiles":
                        {
                            string arg2 = null;
                            string arg3 = null;
                            if (args.Count() >= 3)
                                arg2 = args[2];
                            if (args.Count() >= 4)
                                arg3 = args[3];
                            try
                            {
                                string[] result = Zefie.Prompts.browseOpenFiles(arg1, arg2, arg3);
                                foreach (string r in result)
                                    wl(r);
                            }
                            catch (Exception e)
                            {
                                wl("ERROR: " + e.Message);
                            }
                            break;
                        }
                    case "savefile":
                        {
                            string arg2 = null;
                            string arg3 = null;
                            if (args.Count() >= 3)
                                arg2 = args[2];
                            if (args.Count() >= 4)
                                arg3 = args[3];
                            try
                            {
                                wl(Zefie.Prompts.browseSaveFile(arg1, arg2, arg3));
                            }
                            catch (Exception e)
                            {
                                wl("ERROR: " + e.Message);
                            }
                            break;
                        }
                    case "folder":
                        {
                            bool arg2 = true;
                            if (args.Count() >= 3)
                            {
                                try
                                {
                                    arg2 = Convert.ToBoolean(args[2]);
                                }
                                catch
                                {
                                    if (args[2] == "0" || args[2].ToLower() == "no" || args[2].ToLower() == "n")
                                        arg2 = false;
                                    if (args[2] == "1" || args[2].ToLower() == "yes" || args[2].ToLower() == "y")
                                        arg2 = true;
                                }
                            }
                            try
                            {
                                wl(Zefie.Prompts.browseFolder(arg1, arg2));
                            }
                            catch (Exception e)
                            {
                                wl("ERROR: " + e.Message);
                            }
                            break;
                        }
                    case "msgbox":
                        {
                            string arg2 = null;
                            if (args.Count() >= 3)
                                arg2 = args[2];
                            if (arg1 != null)
                                Zefie.Prompts.ShowMsg(arg1, arg2);
                            else
                            {
                                we("Usage: " + getExecFilename() + " " + args[0] + " message [title]");
                                exit(1);
                            }
                            break;
                        }
                    case "errordialog":
                        {
                            string arg2 = null;
                            if (args.Count() >= 3)
                                arg2 = args[2];
                            if (arg1 != null)
                                Zefie.Prompts.ShowError(arg1, arg2);
                            else
                            {
                                we("Usage: " + getExecFilename() + " " + args[0] + " message [title]");
                                exit(1);
                            }                            
                            break;
                        }
                    case "confirmdialog":
                        {
                            string arg2 = null;
                            if (args.Count() >= 3)
                                arg2 = args[2];
                            if (arg1 != null)
                                wl(Zefie.Prompts.ShowConfirm(arg1, arg2).ToString());
                            else
                            {
                                we("Usage: " + getExecFilename() + " " + args[0] + " message [title]");
                                exit(1);
                            }
                            break;
                        }
                    case "promptdialog":
                        {
                            string arg2 = null;
                            if (args.Count() >= 3)
                                arg2 = args[2];
                            if (arg1 != null)
                                wl(Zefie.Prompts.ShowPrompt(arg1, arg2));
                            else
                            {
                                we("Usage: " + getExecFilename() + " " + args[0] + " message [title]");
                                exit(1);
                            }                                
                            break;
                        }

                    case "genhex":
                        {
                            if (args.Count() < 2)
                            {
                                we("Usage: " + getExecFilename() + " " + args[0] + " length");
                                exit(1);
                            }
                            try { wl(Zefie.Strings.genHexString(Convert.ToInt32(arg1))); }
                            catch (Exception e) { wl("ERROR: " + e.Message); }
                            break;
                        }
                    case "genstr":
                        {
                            string arg2 = null;
                            if (args.Count() < 2)
                            {
                                we("Usage: " + getExecFilename() + " " + args[0] + " length [characters]");
                                exit(1);
                            }
                            if (args.Count() >= 3)
                                arg2 = args[2];
                            try { wl(Zefie.Strings.genString(Convert.ToInt32(arg1), arg2)); }
                            catch (Exception e) { wl("ERROR: " + e.Message); }
                            break;
                        }
                    case "gennum":
                        {
                            string arg2 = null;
                            if (args.Count() < 3)
                            {
                                we("Usage: " + getExecFilename() + " " + args[0] + " max OR min max");
                                exit(1);
                            }
                            if (args.Count() >= 3)
                            {
                                arg2 = args[2];
                                try { wl(Zefie.Math.random(Convert.ToInt32(arg1), Convert.ToInt32(arg2))); }
                                catch (Exception e) { wl("ERROR: " + e.Message); }
                            }
                            else
                            {
                                try { wl(Zefie.Math.random(Convert.ToInt32(arg1))); }
                                catch (Exception e) { wl("ERROR: " + e.Message); }
                            }
                            break;
                        }
                    case "percent":
                        {
                            string arg2 = null;
                            if (args.Count() >= 3)
                            {
                                arg2 = args[2];
                                try { wl(Zefie.Math.calcPercent(Convert.ToDouble(arg1), Convert.ToDouble(arg2)).ToString() + "%"); }
                                catch (Exception e) { wl("ERROR: " + e.Message); }
                            }
                            else
                            {
                                we("Usage: " + getExecFilename() + " " + args[0] + " value max");
                                exit(1);
                            }
                            break;
                        }
                    case "eval":
                        {
                            string expression = "";
                            for (int i = 1; i < args.Count(); i++)
                                expression += args[i];
                            try
                            {
                                var loDataTable = new DataTable();
                                var loDataColumn = new DataColumn("Eval", typeof(double), expression);
                                loDataTable.Columns.Add(loDataColumn);
                                loDataTable.Rows.Add(0);
                                wl(((double)(loDataTable.Rows[0]["Eval"])).ToString());
                            }
                            catch (Exception e) { wl("ERROR: " + e.Message); }
                            break;
                        }
                    case "substring":
                        {
                            if (args.Count() >= 4)
                            {
                                string arg2 = args[2];
                                string arg3 = args[3];
                                try { wl(arg1.Substring(Convert.ToInt32(arg2),Convert.ToInt32(arg3))); }
                                catch (Exception e) { wl("ERROR: " + e.Message); }
                            }
                            else if (args.Count() >= 3)
                            {
                                string arg2 = args[2];
                                try { wl(arg1.Substring(Convert.ToInt32(arg2))); }
                                catch (Exception e) { wl("ERROR: " + e.Message); }
                            }
                            else
                            {
                                we("Usage: " + getExecFilename() + " " + args[0] + " string start [length]");
                                exit(1);
                            }
                                
                            break;
                        }
                    case "contains":
                        {
                            if (args.Count() >= 3)
                            {
                                string arg2 = args[2];
                                try { wl(arg1.Contains(arg2).ToString()); }
                                catch (Exception e) { wl("ERROR: " + e.Message); }
                            }
                            else
                            {
                                we("Usage: " + getExecFilename() + " "+args[0]+" haystack needle");
                                exit(1);
                            }
                                
                            break;
                        }
                    case "scaleimage":
                        {
                            if (args.Count() >= 4)
                            {
                                string arg2 = args[2];
                                string arg3 = args[3];
                                if (File.Exists(arg1))
                                {
                                    try
                                    {
                                        Image i = Image.FromFile(arg1);
                                        Image n;
                                        if (arg2.IndexOf('x') > 0)
                                        {
                                            string[] size = arg2.Split('x');
                                            n = Zefie.Imaging.Scale(i, Convert.ToInt32(size[0]), Convert.ToInt32(size[1]));
                                            n.Save(arg3);
                                        }
                                        else if (arg2.IndexOf('%') > 0)
                                        {
                                            n = Zefie.Imaging.Scale(i, Convert.ToInt32(arg2.Remove(arg2.Length-1)));
                                            n.Save(arg3);
                                        }
                                        else
                                        {
                                            n = Zefie.Imaging.Scale(i, Convert.ToInt32(arg2));
                                            n.Save(arg3);
                                        }                                        
                                        if (File.Exists(arg3))
                                        {
                                            FileInfo f = new FileInfo(arg3);
                                            wl("Saved " + arg3 + " (" + Zefie.Math.calcBytes(f.Length) + ")");
                                        }
                                        else
                                        {
                                            we("There was an unknown error saving " + arg3);
                                            exit(1);
                                        }
                                    }
                                    catch (Exception e) { wl("ERROR: " + e.Message); }
                                }
                            }
                            else
                            {
                                we("Usage: " + getExecFilename() + " "+args[0]+" infile (WxH OR Width OR #%) outfile ");
                                exit(1);
                            }
                            break;
                        }
                    case "drivelabel":
                        {
                            if (args.Count() == 2)
                            {
                                try { wl(Zefie.Path.getDriveLetterFromLabel(arg1)); }
                                catch (Exception e) { wl("ERROR: " + e.Message); }
                            }
                            else
                            {
                                we("Usage: " + getExecFilename() + " "+args[0]+" (drive label)");
                                exit(1);
                            }
                            break;
                        }
                    default:
                        {
                            if (!nohelp)
                                showHelp(true);
                            return false;
                        }
                }
                return true;
            }
            else
            {
                if (!nohelp)
                    showHelp(true);
            }
            return false;
        }
        private static void hashcmd(string file, string hashtype)
        {
            Zefie.Data.BlockSize = readblock;
            if (file != null)
            {
                if (File.Exists(file))
                {
                    w(doHash(file, hashtype));
                    if (quiet)
                        w("\n");
                    else
                        w("\t" + file + "\n");
                }
                else
                {
                    wl(getExecFilename() + ": " + hashtype + ": "+file+": No such file or directory");
                    return;
                }
            }
        }
        private static void hashcmd(string hashtype)
        {
            string file = "-";
            byte[] data = Zefie.Data.readFromStdin();
            w(doHash(data, hashtype));
            data = null;
            if (quiet)
                w("\n");
            else
                w("\t" + file + "\n");
        }
        private static string doHash(byte[] data, string hashtype)
        {
            switch (hashtype)
            {
                case "md5":
                    return Zefie.Cryptography.Hash.MD5(data);
                case "sha1":
                    return Zefie.Cryptography.Hash.SHA1(data);
                case "sha256":
                    return Zefie.Cryptography.Hash.SHA256(data);
                case "sha384":
                    return Zefie.Cryptography.Hash.SHA384(data);
                case "sha512":
                    return Zefie.Cryptography.Hash.SHA512(data);
            }
            return null;
        }
        private static string doHash(string file, string hashtype)
        {
            switch (hashtype)
            {
                case "md5":
                    return Zefie.Cryptography.Hash.MD5(file);
                case "sha1":
                    return Zefie.Cryptography.Hash.SHA1(file);
                case "sha256":
                    return Zefie.Cryptography.Hash.SHA256(file);
                case "sha384":
                    return Zefie.Cryptography.Hash.SHA384(file);
                case "sha512":
                    return Zefie.Cryptography.Hash.SHA512(file);
            }
            return null;
        }
        private static void encrypt(string passwd, string data, bool rawdata)
        {
            byte[] buffer;
            if (passwd != null)
            {
                if (data != null)
                {
                    if (File.Exists(data))
                    {
                        using (FileStream f = File.OpenRead(data))
                        {
                            buffer = new byte[f.Length];
                            f.Read(buffer, 0, buffer.Length);
                            f.Close();
                        }
                        if (rawdata)
                            w(Zefie.Cryptography.encrypt(buffer, passwd));
                        else
                            wl(Zefie.Data.base64Encode(Zefie.Cryptography.encrypt(buffer, passwd)));
                    }
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes(data);
                        if (rawdata)
                            w(Zefie.Cryptography.encrypt(buffer, passwd));
                        else
                            wl(Zefie.Data.base64Encode(Zefie.Cryptography.encrypt(buffer, passwd)));
                    }
                }
                else
                {
                    if (rawdata)
                        w(Zefie.Cryptography.encrypt(Zefie.Data.readFromStdin(), passwd));
                    else
                        wl(Zefie.Data.base64Encode(Zefie.Cryptography.encrypt(Zefie.Data.readFromStdin(), passwd)));
                }
            }
        }
        private static void decrypt(string passwd, string data, bool rawdata)
        {
            byte[] buffer;
            if (passwd != null)
            {
                if (data != null)
                {
                    if (File.Exists(data))
                    {
                        using (FileStream f = File.OpenRead(data))
                        {
                            buffer = new byte[f.Length];
                            f.Read(buffer, 0, buffer.Length);
                            f.Close();
                        }
                        if (rawdata)
                            w(Zefie.Cryptography.decrypt(buffer, passwd));
                        else
                            w(Zefie.Cryptography.decrypt(Zefie.Data.base64Decode(Encoding.UTF8.GetString(buffer)), passwd));
                    }
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes(data);
                        if (rawdata)
                            w(Zefie.Cryptography.decrypt(buffer, passwd));
                        else
                            w(Zefie.Cryptography.decrypt(Zefie.Data.base64Decode(Encoding.UTF8.GetString(buffer)), passwd));
                    }
                }
                else
                {
                    if (rawdata)
                        w(Zefie.Cryptography.decrypt(Zefie.Data.readFromStdin(), passwd));
                    else
                        w(Zefie.Cryptography.decrypt(Zefie.Data.base64Decode(Encoding.UTF8.GetString(Zefie.Data.readFromStdin())), passwd));
                }
            }
        }
    }
    class DirFlags
    {
        public static bool recursive = false;
        public static bool sort = true;
        public static bool hidden = false;
        public static bool system = false;
    }
}
