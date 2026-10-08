# Metal Gear Solid V: Ground Zeroes — Ελληνικό Patch / Greek Translation Patch

Ανεπίσημη ελληνική μετάφραση για το **Metal Gear Solid V: Ground Zeroes** (Steam): μενού, ρυθμίσεις, iDroid, αποστολές,
συμβουλές φόρτωσης, όπλα και αντικείμενα, και **όλοι οι υπότιτλοι** — σκηνές, ασύρματος, κασέτες, ατάκες φρουρών
(~2.360 κείμενα περιβάλλοντος + 3.821 υπότιτλοι). Μαζί και ελληνικά γράμματα σε όλες τις γραμματοσειρές του παιχνιδιού.

*Unofficial Greek translation for **Metal Gear Solid V: Ground Zeroes** (Steam): menus, options, iDroid, missions, loading tips,
weapons and items, and **all subtitles** — cutscenes, radio, cassette tapes, guard chatter (~2,360 UI strings + 3,821 subtitles).
Greek letters are added to every font the game uses.*

## Πώς δουλεύει / How it works

Η μετάφραση βρίσκεται μέσα σε δύο μεγάλα αρχεία του παιχνιδιού (`data_01.g0s` 1,1 GB και `data_02.g0s` 1,6 GB). Το πακέτο
**δεν περιέχει αυτά τα αρχεία**: περιέχει ένα μικρό αρχείο διαφορών (`Data/greek_patch.dat`, ~2 MB) και ένα εργαλείο που το
εφαρμόζει πάνω στα **δικά σου** αρχεία. Το εργαλείο ελέγχει ότι τα αρχεία είναι τα αυθεντικά πριν αλλάξει οτιδήποτε, προσθέτει
τα αλλαγμένα κομμάτια στο τέλος κάθε αρχείου, ελέγχει το αποτέλεσμα και κρατά ένα μικρό αντίγραφο ασφαλείας
(`data_0X.g0s.greek_backup`) για την απεγκατάσταση. Διαρκεί λίγα δευτερόλεπτα.

*The translation lives inside two large game archives. This package does **not** contain them: it has a small delta file
(`Data/greek_patch.dat`, ~2 MB) and a tool that applies it to **your own** files. The originals are verified first, the changed
entries are appended to each archive, the result is verified, and a small backup (`data_0X.g0s.greek_backup`) is kept for uninstalling.*

Η μετάφραση μπαίνει στη θέση της **αγγλικής** γλώσσας. Οι φωνές μένουν αγγλικές.
*The Greek text replaces the **English** language slot; voices stay English.*

## Εγκατάσταση / Install (Windows)

Το mod έρχεται σε **δύο μέρη**: τα **δεδομένα μετάφρασης** (φάκελος `Data`, στο Nexus) και το **πρόγραμμα** που τα εφαρμόζει (εδώ, στο GitHub).

1. Κατέβασε το πρόγραμμα από τα [Releases](../../releases) (`MGSVGZGreekPatcher_v1.0_Windows.zip`) και αποσυμπίεσέ το σε έναν φάκελο.
2. Κατέβασε το `Data` από τη σελίδα του mod στο Nexus και αποσυμπίεσέ το **στον ίδιο φάκελο**, ώστε δίπλα στο
   `MGSVGZGreekPatcher.exe` να υπάρχει ο φάκελος `Data` (με μέσα το `greek_patch.dat`). *(Ο φάκελος `Data` υπάρχει και σε αυτό το repo.)*
3. Κλείσε το παιχνίδι και τρέξε το **`Install.bat`** (διπλό κλικ). Βρίσκει μόνο του το Steam· αν όχι, θα σου ζητήσει τον φάκελο,
   π.χ. `C:\Program Files (x86)\Steam\steamapps\common\Metal Gear Solid Ground Zeroes`.
4. Κράτα τη γλώσσα του παιχνιδιού στα **English**.

*1. Download `MGSVGZGreekPatcher_v1.0_Windows.zip` from [Releases](../../releases) and extract it. 2. Extract the `Data` folder from the
Nexus mod page into the same folder. 3. Close the game and run `Install.bat` (it finds Steam, or asks for the game folder).
4. Keep the game language set to English.*

### Linux / Steam Deck (Proton)

Κατέβασε το `MGSVGZGreekPatcher_v1.0_Linux.tar.gz`, βγάλ' το, βάλε δίπλα τον φάκελο `Data` και τρέξε `./MGSVGZGreekPatcher`
(ή `./MGSVGZGreekPatcher "/path/to/steamapps/common/Metal Gear Solid Ground Zeroes"`).

### Επιλογές / Options

```
Install.bat ["<φάκελος παιχνιδιού>"] [--no-pause]                       (Windows)
./MGSVGZGreekPatcher [<φάκελος παιχνιδιού>] [--uninstall] [--no-pause]   (Linux)
```

## Απεγκατάσταση / Uninstall

`Uninstall.bat` (Linux: `./MGSVGZGreekPatcher --uninstall`): επαναφέρει τα αρχεία ακριβώς όπως ήταν.
Εναλλακτικά: Steam → *Επαλήθευση ακεραιότητας αρχείων παιχνιδιού* (ξανακατεβάζει τα δύο αρχεία).
Η επαλήθευση και οι ενημερώσεις του παιχνιδιού αναιρούν το patch — τρέξε ξανά το `Install.bat`.

*`Uninstall.bat` restores the files exactly. Steam's "Verify integrity of game files" also removes the patch — run `Install.bat` again.*

## Συμβατότητα / Compatibility

Φτιάχτηκε και δοκιμάστηκε με την έκδοση Steam (AppID 311340) σε Linux/Proton· το εργαλείο Windows δοκιμάστηκε με Wine.
Το εργαλείο σταματά με μήνυμα αν κάποιο αρχείο δεν είναι το αναμενόμενο (άλλη έκδοση ή ήδη τροποποιημένο) — δεν γράφει ποτέ
πάνω σε άγνωστο περιεχόμενο. Δεν είναι συμβατό με άλλα mods που αλλάζουν τα ίδια αρχεία μέσα στα `data_01.g0s` / `data_02.g0s`.

*Made for and tested with the Steam version on Linux/Proton; the Windows tool was tested under Wine. The tool stops with a message
if a file is not the expected one and never writes over unknown content.*

## Σημειώσεις / Notes

- Η μετάφραση είναι **δική μας, όχι μηχανική**. Αναφορές λαθών: ανοίξτε ένα issue ή γράψτε στο Nexus.
- Ονόματα χαρακτήρων με ελληνικά γράμματα (Σνέικ, Μίλερ, Τσίκο, Παζ, Σκαλ Φέις). Μένουν λατινικά: Cipher, ZEKE, XOF, FOX, CQC,
  iDroid, MSF, μάρκες και κωδικοί όπλων, τίτλοι τραγουδιών, ονόματα πλήκτρων, ονόματα συντελεστών.
- Τα ελληνικά γράμματα πήραν τη θέση των τονούμενων λατινικών στις γραμματοσειρές, γι' αυτό τα ονόματα στους τίτλους τέλους
  γράφονται χωρίς τόνους και οι άλλες δυτικές γλώσσες (Γαλλικά, Γερμανικά, Ισπανικά, Ιταλικά) δεν εμφανίζονται σωστά όσο είναι
  εγκατεστημένο το patch. *Greek letters replace the accented Latin letters in the fonts, so the other Western languages will
  not display correctly while the patch is installed.*
- Οι φωνές παραμένουν αγγλικές (μόνο κείμενο).

## Δημιουργία από τον πηγαίο κώδικα / Build

```
dotnet publish MGSVGZGreekPatcher -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Απαιτεί .NET 8 SDK.

## Τεχνικές σημειώσεις / Technical notes

- Fox Engine. `.g0s`: δεδομένα, πίνακας (hash u64, offset/16, μέγεθος) και footer 20 bytes· κάθε byte είναι XOR με κλειδί που
  εξαρτάται μόνο από τη θέση του στο αρχείο, και πολλά αρχεία έχουν δεύτερο στρώμα. Το εργαλείο γράφει τα νέα αρχεία στη θέση
  του παλιού πίνακα και μετά νέο πίνακα — όλα τα υπόλοιπα bytes μένουν ίδια.
- Κείμενα περιβάλλοντος: `.lng` (v2) μέσα σε `.fpk/.fpkd`. Υπότιτλοι: `.subp` (μονοβάιτο κείμενο + δεύτερο πεδίο με το μήκος
  σε UTF-8, που πρέπει να ξαναϋπολογίζεται). Γραμματοσειρές: `.ffnt` και άτλαντες `.fnt` + `.ftex/.ftexs`.
- Το ελληνικό κείμενο γράφεται με κωδικούς Latin-1 (πεζά 0xE0–0xFE, κεφαλαία 0x20 πιο κάτω) και οι γραμματοσειρές έχουν τα
  ελληνικά σχέδια σε αυτούς τους κωδικούς.
- Χρήσιμο για την αποσυμπίεση: [GzsTool](https://github.com/Atvaark/GzsTool) του Atvaark.

Credits: X4Tz1S. Το παιχνίδι και το αρχικό κείμενο © Konami Digital Entertainment — ανεπίσημη μετάφραση θαυμαστών.
