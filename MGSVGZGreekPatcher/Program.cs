// Metal Gear Solid V: Ground Zeroes - Greek translation installer.
// The translation lives inside data_01.g0s / data_02.g0s (1.1 + 1.6 GB). This tool rebuilds the changed entries from the
// user's own files plus Data/greek_patch.dat (deltas, made by tools/make_delta.py), appends them where the entry table
// used to be and writes a new table - the rest of each archive is not touched. The original table is kept in a small
// *.greek_backup file, which is all Uninstall needs.
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

static class Program
{
    const string Version = "1.0";
    const string GameDirName = "Metal Gear Solid Ground Zeroes";
    const string RepoUrl = "https://github.com/x4tz1s/MGSVGroundZeroes-GreekPatch";
    const uint Magic2 = 0xA0F8EFE6;
    static readonly byte[] DataMagic = Encoding.ASCII.GetBytes("GZGREEK1");
    static readonly byte[] BackupMagic = Encoding.ASCII.GetBytes("GZBACKUP");

    class Op { public bool Copy; public int Off, Len; public byte[] Bytes; }
    class Rep { public int Index; public ulong Hash; public byte[] OldMd5, NewMd5; public int NewLen; public List<Op> Ops = new(); public byte[] Stored; }
    class Arch
    {
        public string Name; public long OrigSize, TablePos, NewSize; public byte[] OrigTailMd5, NewTailMd5; public List<Rep> Reps = new();
        public string Path; public byte[] Tail; public bool HadBackup;
    }
    class Fail : Exception { public Fail(string m) : base(m) { } }

    static int Main(string[] args)
    {
        try { Console.OutputEncoding = Encoding.UTF8; } catch { }
        bool uninstall = args.Any(a => a.Equals("--uninstall", StringComparison.OrdinalIgnoreCase));
        bool noPause = args.Any(a => a.Equals("--no-pause", StringComparison.OrdinalIgnoreCase));
        string dirArg = args.FirstOrDefault(a => !a.StartsWith("--"));
        Console.WriteLine($"Metal Gear Solid V: Ground Zeroes - Ελληνική μετάφραση / Greek translation - installer v{Version}");
        Console.WriteLine();
        int rc = 0;
        try
        {
            string game = FindGame(dirArg, !noPause);
            Console.WriteLine("Φάκελος παιχνιδιού / Game folder: " + game);
            Console.WriteLine();
            if (uninstall) Uninstall(game); else Install(game);
        }
        catch (Fail e) { Console.WriteLine(); Console.WriteLine("ΣΦΑΛΜΑ / ERROR: " + e.Message); rc = 1; }
        catch (Exception e) { Console.WriteLine(); Console.WriteLine("ΑΠΡΟΣΔΟΚΗΤΟ ΣΦΑΛΜΑ / UNEXPECTED ERROR: " + e); rc = 2; }
        if (!noPause) { Console.WriteLine(); Console.WriteLine("Πάτησε Enter για έξοδο / Press Enter to exit."); Console.ReadLine(); }
        return rc;
    }

    // ---------------------------------------------------------------- game folder
    static bool IsGame(string d)
    {
        try { return !string.IsNullOrWhiteSpace(d) && File.Exists(Path.Combine(d, "data_01.g0s")) && File.Exists(Path.Combine(d, "data_02.g0s")); }
        catch { return false; }
    }

    static IEnumerable<string> SteamRoots()
    {
        var roots = new List<string>();
        if (OperatingSystem.IsWindows())
        {
            foreach (var d in DriveInfo.GetDrives())
            {
                string r = d.Name;
                roots.Add(Path.Combine(r, "Program Files (x86)", "Steam")); roots.Add(Path.Combine(r, "Program Files", "Steam"));
                roots.Add(Path.Combine(r, "Steam")); roots.Add(Path.Combine(r, "SteamLibrary")); roots.Add(Path.Combine(r, "Games", "Steam"));
                roots.Add(Path.Combine(r, "Games", "SteamLibrary"));
            }
        }
        else
        {
            string h = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            roots.Add(Path.Combine(h, ".local", "share", "Steam")); roots.Add(Path.Combine(h, ".steam", "steam"));
            roots.Add(Path.Combine(h, ".var", "app", "com.valvesoftware.Steam", ".local", "share", "Steam"));
        }
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var r in roots.ToArray())
        {
            try
            {
                string vdf = Path.Combine(r, "steamapps", "libraryfolders.vdf");
                if (!File.Exists(vdf)) continue;
                foreach (Match m in Regex.Matches(File.ReadAllText(vdf), "\"path\"\\s+\"([^\"]+)\""))
                    roots.Add(m.Groups[1].Value.Replace("\\\\", "\\"));
            }
            catch { }
        }
        foreach (var r in roots) if (seen.Add(r)) yield return r;
    }

    static string FindGame(string arg, bool interactive)
    {
        var cands = new List<string>();
        if (arg != null) cands.Add(arg.Trim('"'));
        cands.Add(Directory.GetCurrentDirectory()); cands.Add(AppContext.BaseDirectory);
        cands.Add(Path.GetDirectoryName(AppContext.BaseDirectory.TrimEnd('\\', '/')));
        foreach (var c in cands) if (IsGame(c)) return Path.GetFullPath(c);
        if (arg != null) throw new Fail($"Δεν βρέθηκαν τα data_01.g0s + data_02.g0s στο «{arg}». / data_01.g0s + data_02.g0s not found in that folder.");
        foreach (var r in SteamRoots())
        {
            string c = Path.Combine(r, "steamapps", "common", GameDirName);
            if (IsGame(c)) return Path.GetFullPath(c);
        }
        while (interactive)
        {
            Console.WriteLine("Δεν βρέθηκε το παιχνίδι. Γράψε (ή σύρε εδώ) τον φάκελο του Metal Gear Solid V: Ground Zeroes και πάτησε Enter:");
            Console.WriteLine("Game not found. Type (or drag here) the Metal Gear Solid V: Ground Zeroes folder and press Enter:");
            string s = Console.ReadLine(); if (s == null) break;
            s = s.Trim().Trim('"', '\''); if (s.Length == 0) break;
            if (IsGame(s)) return Path.GetFullPath(s);
            Console.WriteLine("  -> εκεί δεν υπάρχουν data_01.g0s + data_02.g0s / no data_01.g0s + data_02.g0s there."); Console.WriteLine();
        }
        throw new Fail("Δεν βρέθηκε ο φάκελος του παιχνιδιού (data_01.g0s + data_02.g0s). / Game folder not found.");
    }

    // ---------------------------------------------------------------- patch data
    static List<Arch> LoadData(bool required)
    {
        string p = Path.Combine(AppContext.BaseDirectory, "Data", "greek_patch.dat");
        if (!File.Exists(p))
        {
            if (!required) return null;
            char sep = Path.DirectorySeparatorChar;
            throw new Fail($"Λείπει το Data{sep}greek_patch.dat δίπλα στο πρόγραμμα. Κατέβασε τα δεδομένα της μετάφρασης από τη σελίδα του mod στο Nexus Mods "
                + $"(ή από το {RepoUrl}) και αποσυμπίεσέ τα εδώ, ώστε ο φάκελος Data να είναι δίπλα στο πρόγραμμα.\n"
                + $"Data{sep}greek_patch.dat is missing. Download the translation data from the mod page on Nexus Mods (or from {RepoUrl}) and extract it next to this program.");
        }
        byte[] raw = File.ReadAllBytes(p);
        if (raw.Length < 16 || !raw.Take(8).SequenceEqual(DataMagic)) throw new Fail("Το greek_patch.dat δεν είναι έγκυρο. / greek_patch.dat is not valid.");
        var ms = new MemoryStream();
        try { using var z = new ZLibStream(new MemoryStream(raw, 8, raw.Length - 8), CompressionMode.Decompress); z.CopyTo(ms); }
        catch (InvalidDataException) { throw new Fail("Το greek_patch.dat είναι κατεστραμμένο - κατέβασέ το ξανά. / greek_patch.dat is corrupt - download it again."); }
        ms.Position = 0;
        var r = new BinaryReader(ms);
        string Str() => Encoding.UTF8.GetString(r.ReadBytes(r.ReadByte()));
        string ver = Str();
        Console.WriteLine("Δεδομένα μετάφρασης / Translation data: v" + ver);
        var list = new List<Arch>();
        int na = r.ReadInt32();
        for (int a = 0; a < na; a++)
        {
            var x = new Arch { Name = Str(), OrigSize = r.ReadInt64(), TablePos = r.ReadInt64(), OrigTailMd5 = r.ReadBytes(16), NewSize = r.ReadInt64(), NewTailMd5 = r.ReadBytes(16) };
            int nr = r.ReadInt32();
            for (int i = 0; i < nr; i++)
            {
                var e = new Rep { Index = r.ReadInt32(), Hash = r.ReadUInt64(), OldMd5 = r.ReadBytes(16), NewMd5 = r.ReadBytes(16), NewLen = r.ReadInt32() };
                int no = r.ReadInt32();
                for (int k = 0; k < no; k++)
                {
                    if (r.ReadByte() == 0) e.Ops.Add(new Op { Copy = true, Off = r.ReadInt32(), Len = r.ReadInt32() });
                    else { int n = r.ReadInt32(); e.Ops.Add(new Op { Len = n, Bytes = r.ReadBytes(n) }); }
                }
                x.Reps.Add(e);
            }
            list.Add(x);
        }
        return list;
    }

    // ---------------------------------------------------------------- g0s
    static void QarXor(byte[] d, long pos)
    {
        for (int i = 0; i < d.Length; i++) d[i] ^= (byte)((uint)((ulong)(pos + i) * 6339797UL) >> 16);
    }

    static void Layer2Xor(byte[] d, int start, uint key)
    {
        int n = (d.Length - start) / 4;
        uint v = key | ((key ^ 0xFFFFCDEC) << 16), i = unchecked(69069 * key);
        for (int k = 0; k < n; k++)
        {
            int p = start + 4 * k;
            uint w = BitConverter.ToUInt32(d, p) ^ v;
            d[p] = (byte)w; d[p + 1] = (byte)(w >> 8); d[p + 2] = (byte)(w >> 16); d[p + 3] = (byte)(w >> 24);
            v = unchecked(3 * (i + 23023 * v));
        }
    }

    static byte[] Md5(byte[] d) => MD5.HashData(d);
    static bool Same(byte[] a, byte[] b) => a.AsSpan().SequenceEqual(b);

    static FileStream Open(string path, bool write)
    {
        try { return new FileStream(path, FileMode.Open, write ? FileAccess.ReadWrite : FileAccess.Read, write ? FileShare.None : FileShare.Read); }
        catch (IOException) { throw new Fail($"Το {Path.GetFileName(path)} χρησιμοποιείται - κλείσε το παιχνίδι και δοκίμασε ξανά. / {Path.GetFileName(path)} is in use - close the game and try again."); }
        catch (UnauthorizedAccessException) { throw new Fail($"Δεν επιτρέπεται η εγγραφή στο {Path.GetFileName(path)}. / No write access to {Path.GetFileName(path)}."); }
    }

    static byte[] ReadAt(FileStream f, long pos, int n) { var b = new byte[n]; f.Seek(pos, SeekOrigin.Begin); f.ReadExactly(b); return b; }

    static string WrongFile(string name) =>
        $"Το {name} δεν είναι το αρχικό αρχείο της έκδοσης Steam (άλλη έκδοση ή ήδη τροποποιημένο). Κάνε «Επαλήθευση ακεραιότητας αρχείων» στο Steam και ξαναδοκίμασε.\n"
        + $"{name} is not the original Steam file (different version or already modified). Use \"Verify integrity of game files\" in Steam and try again.";

    /// The original table+footer of an archive: from the backup if the archive was patched before, else from the file.
    static void LoadTail(Arch x, string game)
    {
        x.Path = Path.Combine(game, x.Name);
        string bk = x.Path + ".greek_backup";
        long len = new FileInfo(x.Path).Length;
        if (File.Exists(bk))
        {
            byte[] b = File.ReadAllBytes(bk);
            if (b.Length > 24 && b.Take(8).SequenceEqual(BackupMagic) && BitConverter.ToInt64(b, 8) == x.OrigSize && BitConverter.ToInt64(b, 16) == x.TablePos
                && len > x.TablePos)
            {
                x.Tail = b.Skip(24).ToArray(); x.HadBackup = true;
            }
            else if (len != x.OrigSize) throw new Fail(WrongFile(x.Name));
        }
        if (x.Tail == null)
        {
            if (len != x.OrigSize)
            {
                if (len == x.NewSize)
                {
                    using var f = Open(x.Path, false);
                    if (Same(Md5(ReadAt(f, x.TablePos, (int)(len - x.TablePos))), x.NewTailMd5))
                        throw new Fail($"Το {x.Name} έχει ήδη αυτή την έκδοση της μετάφρασης (χωρίς αντίγραφο ασφαλείας). Δεν χρειάζεται κάτι.\n"
                            + $"{x.Name} already contains this version of the translation (no backup present). Nothing to do.");
                }
                throw new Fail(WrongFile(x.Name));
            }
            using var g = Open(x.Path, false);
            x.Tail = ReadAt(g, x.TablePos, (int)(len - x.TablePos));
        }
        if (!Same(Md5(x.Tail), x.OrigTailMd5)) throw new Fail(WrongFile(x.Name));
    }

    static void Prepare(Arch x)
    {
        byte[] t = x.Tail;
        int count = BitConverter.ToInt32(t, t.Length - 20);
        if ((long)BitConverter.ToInt32(t, t.Length - 12) * 16 != x.TablePos || count * 16 > t.Length) throw new Fail(WrongFile(x.Name));
        using var f = Open(x.Path, false);
        foreach (var e in x.Reps)
        {
            if (e.Index >= count || BitConverter.ToUInt64(t, 16 * e.Index) != e.Hash) throw new Fail(WrongFile(x.Name));
            long off = (long)BitConverter.ToUInt32(t, 16 * e.Index + 8) * 16; int size = (int)BitConverter.ToUInt32(t, 16 * e.Index + 12);
            if (off + size > x.TablePos) throw new Fail(WrongFile(x.Name));
            byte[] d = ReadAt(f, off, size);
            QarXor(d, off);
            bool l2 = d.Length >= 8 && BitConverter.ToUInt32(d, 0) == Magic2; uint key = l2 ? BitConverter.ToUInt32(d, 4) : 0;
            if (l2) { Layer2Xor(d, 8, key); d = d.AsSpan(8).ToArray(); }
            if (!Same(Md5(d), e.OldMd5)) throw new Fail(WrongFile(x.Name));
            byte[] n = new byte[e.NewLen + (l2 ? 8 : 0)]; int p = l2 ? 8 : 0;
            foreach (var op in e.Ops)
            {
                if (op.Copy) Buffer.BlockCopy(d, op.Off, n, p, op.Len); else Buffer.BlockCopy(op.Bytes, 0, n, p, op.Len);
                p += op.Len;
            }
            if (p != n.Length || !Same(MD5.HashData(n.AsSpan(l2 ? 8 : 0)), e.NewMd5))
                throw new Fail("Εσωτερικό σφάλμα: τα δεδομένα μετάφρασης δεν ταιριάζουν. / Internal error: translation data mismatch.");
            if (l2) { BitConverter.GetBytes(Magic2).CopyTo(n, 0); BitConverter.GetBytes(key).CopyTo(n, 4); Layer2Xor(n, 8, key); }
            e.Stored = n;
        }
    }

    static void Write(Arch x)
    {
        string bk = x.Path + ".greek_backup";
        if (!x.HadBackup)
        {
            using (var b = new FileStream(bk + ".tmp", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                b.Write(BackupMagic); b.Write(BitConverter.GetBytes(x.OrigSize)); b.Write(BitConverter.GetBytes(x.TablePos)); b.Write(x.Tail);
                b.Flush(true);
            }
            File.Move(bk + ".tmp", bk, true);
        }
        byte[] t = x.Tail; int count = BitConverter.ToInt32(t, t.Length - 20);
        byte[] table = t.AsSpan(0, 16 * count).ToArray();
        using (var f = Open(x.Path, true))
        {
            f.Seek(x.TablePos, SeekOrigin.Begin);
            foreach (var e in x.Reps.OrderBy(r => r.Index))
            {
                long pos = f.Position;
                byte[] d = (byte[])e.Stored.Clone();
                QarXor(d, pos);
                f.Write(d);
                BitConverter.GetBytes((uint)(pos / 16)).CopyTo(table, 16 * e.Index + 8);
                BitConverter.GetBytes((uint)d.Length).CopyTo(table, 16 * e.Index + 12);
                f.Write(new byte[(int)((16 - f.Position % 16) % 16)]);
            }
            int block = (int)(f.Position / 16);
            f.Write(table);
            uint sum = 0;
            for (int i = 0; i < count; i++) sum = unchecked(sum + BitConverter.ToUInt32(table, 16 * i + 12));
            f.Write(BitConverter.GetBytes(sum));
            f.Write(new byte[(int)((16 - f.Position % 16) % 16)]);
            byte[] foot = t.AsSpan(t.Length - 20).ToArray();
            BitConverter.GetBytes(block).CopyTo(foot, 8);
            f.Write(foot);
            f.SetLength(f.Position);
            f.Flush(true);
            if (f.Length != x.NewSize || !Same(Md5(ReadAt(f, x.TablePos, (int)(f.Length - x.TablePos))), x.NewTailMd5))
                throw new Fail($"Η επαλήθευση του {x.Name} απέτυχε. Τρέξε την απεγκατάσταση (Uninstall) ή επαλήθευση αρχείων στο Steam.\n"
                    + $"Verification of {x.Name} failed. Run Uninstall or verify the game files in Steam.");
        }
    }

    static void Restore(Arch x, byte[] tail, long tablePos, long origSize)
    {
        using var f = Open(x.Path, true);
        f.Seek(tablePos, SeekOrigin.Begin); f.Write(tail); f.SetLength(f.Position); f.Flush(true);
        if (f.Length != origSize) throw new Fail(WrongFile(x.Name));
    }

    static void Install(string game)
    {
        var data = LoadData(true);
        foreach (var x in data) { Console.WriteLine($"Έλεγχος / Checking {x.Name} ..."); LoadTail(x, game); Prepare(x); }
        foreach (var x in data)
        {
            Console.WriteLine($"Εγγραφή / Writing {x.Name} ({x.Reps.Count} αρχεία / entries) ...");
            using (Open(x.Path, true)) { }
            Write(x);
        }
        Console.WriteLine();
        Console.WriteLine("Η ελληνική μετάφραση εγκαταστάθηκε. Κράτα τη γλώσσα του παιχνιδιού στα English.");
        Console.WriteLine("The Greek translation is installed. Keep the game language set to English.");
    }

    static void Uninstall(string game)
    {
        int done = 0;
        foreach (string name in new[] { "data_01.g0s", "data_02.g0s" })
        {
            string path = Path.Combine(game, name), bk = path + ".greek_backup";
            if (!File.Exists(bk)) continue;
            byte[] b = File.ReadAllBytes(bk);
            if (b.Length <= 24 || !b.Take(8).SequenceEqual(BackupMagic)) throw new Fail($"Το {Path.GetFileName(bk)} δεν είναι έγκυρο. / {Path.GetFileName(bk)} is not valid.");
            long orig = BitConverter.ToInt64(b, 8), tp = BitConverter.ToInt64(b, 16);
            var x = new Arch { Name = name, Path = path };
            long len = new FileInfo(path).Length;
            if (len == orig) { using var f = Open(path, false); if (Same(ReadAt(f, tp, (int)(len - tp)), b.Skip(24).ToArray())) { f.Dispose(); File.Delete(bk); continue; } }
            if (len <= tp) throw new Fail(WrongFile(name));
            Console.WriteLine($"Επαναφορά / Restoring {name} ...");
            Restore(x, b.Skip(24).ToArray(), tp, orig);
            File.Delete(bk); done++;
        }
        Console.WriteLine();
        if (done == 0) { Console.WriteLine("Δεν βρέθηκε εγκατεστημένη μετάφραση (κανένα αρχείο .greek_backup). / No installed translation found."); return; }
        Console.WriteLine("Τα αρχικά αρχεία επανήλθαν. / The original files have been restored.");
    }
}
