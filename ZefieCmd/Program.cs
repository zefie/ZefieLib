using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
                wl("\tmessage text [title]");
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
                wl("contains (haystack) (needle)");
                //wl("contains (haystack) (needle)");



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
                    if (o == "-q" || o == "--quiet")
                        quiet = true;
                    if (o == "-r" || o == "--raw")
                        rawdata = true;
                    if (o == "-h" || o == "--help")
                        showHelp();

                }
                if (args.Count() >= 2)
                    arg1 = args[1];

                switch (args[0].ToLower())
                {
                    case "md5":
                        {
                            for (int i = 1; i < args.Count(); i++)
                                hashcmd(args[i], "md5");
                            break;
                        }
                    case "sha1":
                        {
                            for (int i = 1; i < args.Count(); i++)
                                hashcmd(args[i], "sha1");
                            break;
                        }
                    case "sha256":
                        {
                            for (int i = 1; i < args.Count(); i++)
                                hashcmd(args[i], "sha256");
                            break;
                        }
                    case "sha384":
                        {
                            for (int i = 1; i < args.Count(); i++)
                                hashcmd(args[i], "sha384");
                            break;
                        }
                    case "sha512":
                        {
                            for (int i = 1; i < args.Count(); i++)
                                hashcmd(args[i], "sha512");
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
            string hash = null;
            Zefie.Data.BlockSize = readblock;
            if (file != null)
            {
                if (File.Exists(file))
                {
                    switch (hashtype)
                    {
                        case "md5":
                            hash = Zefie.Data.Hashing.MD5File(file);
                            break;
                        case "sha1":
                            hash = Zefie.Data.Hashing.SHA1File(file);
                            break;
                        case "sha256":
                            hash = Zefie.Data.Hashing.SHA256File(file);
                            break;
                        case "sha384":
                            hash = Zefie.Data.Hashing.SHA384File(file);
                            break;
                        case "sha512":
                            hash = Zefie.Data.Hashing.SHA512File(file);
                            break;
                    }
                }
                else
                {
                    wl(getExecFilename() + ": " + hashtype + ": "+file+": No such file or directory");
                    return;
                }
            }
            else
            {
                byte[] data = Zefie.Data.readFromStdin();
                switch (hashtype)
                {
                    case "md5":
                        hash = Zefie.Data.Hashing.MD5(data);
                        break;
                    case "sha1":
                        hash = Zefie.Data.Hashing.SHA1(data);
                        break;
                    case "sha256":
                        hash = Zefie.Data.Hashing.SHA256(data);
                        break;
                    case "sha384":
                        hash = Zefie.Data.Hashing.SHA384(data);
                        break;
                    case "sha512":
                        hash = Zefie.Data.Hashing.SHA512(data);
                        break;
                }
                file = "-";
                data = null;
            }
            w(hash);
            if (quiet)
                w("\n");
            else
                w("\t" + file + "\n");
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
                            w(Zefie.Crypto.encrypt(buffer, passwd));
                        else
                            wl(Zefie.Data.base64Encode(Zefie.Crypto.encrypt(buffer, passwd)));
                    }
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes(data);
                        if (rawdata)
                            w(Zefie.Crypto.encrypt(buffer, passwd));
                        else
                            wl(Zefie.Data.base64Encode(Zefie.Crypto.encrypt(buffer, passwd)));
                    }
                }
                else
                {
                    if (rawdata)
                        w(Zefie.Crypto.encrypt(Zefie.Data.readFromStdin(), passwd));
                    else
                        wl(Zefie.Data.base64Encode(Zefie.Crypto.encrypt(Zefie.Data.readFromStdin(), passwd)));
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
                            w(Zefie.Crypto.decrypt(buffer, passwd));
                        else
                            w(Zefie.Crypto.decrypt(Zefie.Data.base64Decode(Encoding.UTF8.GetString(buffer)), passwd));
                    }
                    else
                    {
                        buffer = Encoding.UTF8.GetBytes(data);
                        if (rawdata)
                            w(Zefie.Crypto.decrypt(buffer, passwd));
                        else
                            w(Zefie.Crypto.decrypt(Zefie.Data.base64Decode(Encoding.UTF8.GetString(buffer)), passwd));
                    }
                }
                else
                {
                    if (rawdata)
                        w(Zefie.Crypto.decrypt(Zefie.Data.readFromStdin(), passwd));
                    else
                        w(Zefie.Crypto.decrypt(Zefie.Data.base64Decode(Encoding.UTF8.GetString(Zefie.Data.readFromStdin())), passwd));
                }
            }
        }
    }
}
