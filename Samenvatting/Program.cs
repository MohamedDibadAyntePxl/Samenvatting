// =====================================================================
//  PE-TOOLKIT  -  bouwstenen voor een C# console-PE
//  Gebruik: zoek met Ctrl+F op het nummer of het woord (bv. "TOOL 3"),
//  kopieer de methode naar je PE-project en pas namen/teksten aan.
//
//  INHOUD
//   TOOL 1  Methodekop uit een zin van de opdracht (spiekregels)
//   TOOL 2  Bool-methode            (bv. CanPlaceBet, IsValidAge)
//   TOOL 3  Getal inlezen tot geldig (int / decimal, met min-grens)
//   TOOL 4  Tekst inlezen + stoppen met Q
//   TOOL 5  Spellus met rondes ("Round X") en stoppen met Q
//   TOOL 6  Random: kansverdeling en willekeurig getal
//   TOOL 7  Methode die een getal berekent en teruggeeft
//   TOOL 8  Keuze uit menu met switch
//   TOOL 9  Gekleurde tekst + uitlijning/formattering
//   TOOL 10 Ja/nee-vraag (Play again? y/n)
//   TOOL 11 Teller, totaal, gemiddelde, maximum bijhouden in een lus
//   TOOL 12 Veelvoorkomende foutmeldingen in Visual Studio
// =====================================================================

namespace PeToolkit
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Demo: run dit project om alle tools te zien werken.
            ShowColoredText("=== PE TOOLKIT DEMO ===", ConsoleColor.Magenta, true);

            decimal credits = ReadDecimal("Met hoeveel credits wil je spelen? ", 1);
            PlayGameDemo(credits);

            ShowColoredText("Einde demo. Druk op een toets...", ConsoleColor.DarkGray, true);
            Console.ReadKey();
        }

        // =================================================================
        // TOOL 1 - VAN ZIN NAAR METHODEKOP
        // =================================================================
        // "methode X ... geeft een boolean terug"       -> static bool X(...)
        // "... geeft een char terug ('A')"              -> static char X(...)   return 'A';
        // "... geeft een getal/bedrag terug"            -> static int / decimal / double X(...)
        // "... geeft een tekst terug"                   -> static string X(...)
        // "... toont / voert uit" (niks terug)          -> static void X(...)
        // "met 2 parameters: credits en stake"          -> (decimal credits, decimal stake)
        // "met 1 parameter van het type Random"         -> (Random rng)
        // "3 parameters van het type char: a, b, c"     -> (char a, char b, char c)
        //
        // Type niet vermeld? Zoek verderop in de opdracht (vaak bij een andere methode).
        // Geld/bedragen -> decimal.  Aantal/teller -> int.  Ja/nee -> bool.
        //
        // Skelet eerst, met tijdelijke return (geen rode lijnen, dan pushen):
        //   static bool X(decimal a)   { return false; }
        //   static char Y(Random rng)  { return 'A'; }
        //   static int Z(int a)        { return 0; }

        // =================================================================
        // TOOL 2 - BOOL-METHODE (ja/nee-vraag)
        // =================================================================
        // Elke weg moet een return hebben (anders: "not all code paths return a value")
        static bool CanPlaceBet(decimal credits, decimal stake)
        {
            if (stake > 0 && stake <= credits)      // <= : alles inzetten mag
            {
                return true;
            }
            return false;
        }

        // Variant: tussen 2 grenzen (bv. leeftijd 0-120, getal 1-5)
        static bool IsInRange(int value, int min, int max)
        {
            return value >= min && value <= max;    // korte vorm: de vergelijking IS al true/false
        }

        // =================================================================
        // TOOL 3 - GETAL INLEZEN TOT HET GELDIG IS
        // =================================================================
        // do-while: minstens 1 keer vragen. TryParse crasht nooit.
        // Belgische laptop: kommagetallen met KOMMA typen (3,5).
        static decimal ReadDecimal(string question, decimal minimum)
        {
            decimal value;
            bool isValid;

            do
            {
                Console.Write(question);
                isValid = decimal.TryParse(Console.ReadLine(), out value) && value >= minimum;

                if (!isValid)
                {
                    ShowColoredText("Ongeldig bedrag!", ConsoleColor.Red, true);
                }
            }
            while (!isValid);

            return value;
        }

        static int ReadInt(string question, int min, int max)
        {
            int value;
            bool isValid;

            do
            {
                Console.Write(question);
                isValid = int.TryParse(Console.ReadLine(), out value) && value >= min && value <= max;

                if (!isValid)
                {
                    ShowColoredText($"Geef een geheel getal tussen {min} en {max}.", ConsoleColor.Red, true);
                }
            }
            while (!isValid);

            return value;
        }

        // =================================================================
        // TOOL 4 - TEKST INLEZEN + STOPPEN MET Q
        // =================================================================
        // Trim = spaties weg, ToUpper = hoofdletters -> "q", " Q " werken ook
        static bool IsQuit(string input)
        {
            return input != null && input.Trim().ToUpper() == "Q";
        }

        // =================================================================
        // TOOL 5 - SPELLUS MET RONDES EN Q
        // =================================================================
        // Patroon: variabelen VOOR de lus, lus zolang er credits zijn,
        // invoer vragen, Q = break, ongeldig = opnieuw vragen, methode aanroepen
        // en het resultaat OPVANGEN, round++ pas NA een gespeelde ronde.
        static void PlayGameDemo(decimal credits)
        {
            Random rng = new Random();      // 1 Random, hergebruiken
            int round = 1;
            bool stop = false;

            while (credits > 0 && !stop)
            {
                Console.WriteLine();
                Console.WriteLine($"Round {round}, place your bets!");

                // Inzet vragen: herhalen tot geldig of Q
                decimal stake = 0;
                bool validStake = false;
                while (!validStake)
                {
                    Console.Write("Inzet (Q = stoppen): ");
                    string input = Console.ReadLine();

                    if (IsQuit(input))
                    {
                        stop = true;
                        break;                          // uit de VRAAG-lus
                    }

                    if (decimal.TryParse(input, out stake) && CanPlaceBet(credits, stake))
                    {
                        validStake = true;
                    }
                    else
                    {
                        ShowColoredText("Ongeldig bedrag!", ConsoleColor.Red, true);
                    }
                }

                if (stop)
                {
                    break;                              // uit de SPEL-lus
                }

                credits = PlayRoundDemo(credits, stake, rng);   // resultaat opvangen!
                Console.WriteLine($"Credits: {credits:F2}");
                round++;
            }

            ShowColoredText(credits <= 0 ? "Game over!" : $"Je stopt met {credits:F2} credits.",
                            ConsoleColor.Magenta, true);
        }

        // Voorbeeld van "1 ronde spelen en de nieuwe waarde teruggeven"
        static decimal PlayRoundDemo(decimal credits, decimal stake, Random rng)
        {
            if (!CanPlaceBet(credits, stake))
            {
                ShowColoredText("Onvoldoende credits.", ConsoleColor.Red, true);
                return credits;                         // ongewijzigd terug
            }

            credits -= stake;                           // 1. inzet aftrekken

            char a = SpinSymbol(rng);                   // 2. resultaat maken en tonen
            char b = SpinSymbol(rng);
            char c = SpinSymbol(rng);
            ShowColoredText($"| {a} | {b} | {c} |", ConsoleColor.Magenta, true);

            int multiplier = CalculateMultiplier(a, b, c);   // 3. berekenen
            decimal win = stake * multiplier;
            Console.WriteLine($"Winst: {win:F2}");

            credits += win;                             // 4. bijtellen en teruggeven
            return credits;
        }

        // =================================================================
        // TOOL 6 - RANDOM
        // =================================================================
        // rng.Next(100)    -> 0 t.e.m. 99      (tweede getal = de deur, doet niet mee)
        // rng.Next(1, 7)   -> 1 t.e.m. 6       (dobbelsteen)
        // rng.Next(1, 11)  -> 1 t.e.m. 10
        // Random NIET in elke methode opnieuw maken: 1 keer in Main, meegeven als parameter.
        static char SpinSymbol(Random rng)
        {
            int number = rng.Next(100);     // 0-99

            if (number < 50)                // 0-49  -> 50%
            {
                return 'A';
            }
            else if (number < 85)           // 50-84 -> 35%
            {
                return 'B';
            }
            else                            // 85-99 -> 15%
            {
                return 'C';
            }
        }

        // =================================================================
        // TOOL 7 - METHODE DIE EEN GETAL BEREKENT EN TERUGGEEFT
        // =================================================================
        // Strengste voorwaarde EERST (anders wint "2 gelijk" van "3 gelijk").
        static int CalculateMultiplier(char a, char b, char c)
        {
            if (a == 'A' && b == 'A' && c == 'A') return 10;
            if (a == 'B' && b == 'B' && c == 'B') return 20;
            if (a == 'C' && b == 'C' && c == 'C') return 50;
            if (a == b || b == c) return 2;     // 2 opeenvolgende gelijk
            return 0;
        }

        // Voorbeeld prijs/korting (kassa-stijl)
        static decimal CalculatePrice(int age, decimal basePrice)
        {
            if (age < 3)
            {
                return 0;                       // gratis
            }
            else if (age < 12 || age >= 65)
            {
                return basePrice * 0.5m;        // 50% korting (m = decimal!)
            }
            return basePrice;
        }

        // =================================================================
        // TOOL 8 - MENU MET SWITCH
        // =================================================================
        // break in een switch verlaat enkel de SWITCH, niet de lus eromheen.
        static void MenuDemo()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("1) Spelen  2) Regels  9) Stoppen");
                int choice = ReadInt("Keuze: ", 1, 9);

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Spelen...");
                        break;
                    case 2:
                        Console.WriteLine("Regels...");
                        break;
                    case 9:
                        running = false;        // zo stopt de while
                        break;
                    default:
                        ShowColoredText("Ongeldige keuze.", ConsoleColor.Red, true);
                        break;
                }
            }
        }

        // =================================================================
        // TOOL 9 - KLEUR + FORMATTERING
        // =================================================================
        // appendLine = false: cursor blijft op dezelfde regel (voor een vraag)
        static void ShowColoredText(string text, ConsoleColor color, bool appendLine = false)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            if (appendLine)
            {
                Console.WriteLine();
            }
            Console.ResetColor();
        }
        // Formattering in $"...":
        //   {x:F2}   -> 2 decimalen        12,50
        //   {x:C}    -> valuta             € 12,50
        //   {x,8}    -> rechts uitlijnen in 8 tekens   "   12,50"
        //   {x,-8}   -> links uitlijnen in 8 tekens    "12,50   "
        //   {d:dd/MM/yyyy} -> datum (MM = maand, mm = minuten!)
        // Lege regel: Console.WriteLine();
        // Venster leegmaken: Console.Clear();

        // =================================================================
        // TOOL 10 - JA/NEE-VRAAG
        // =================================================================
        static bool AskYesNo(string question)
        {
            while (true)
            {
                Console.Write(question + " (y/n): ");
                string answer = Console.ReadLine().Trim().ToLower();
                if (answer == "y") return true;
                if (answer == "n") return false;
                ShowColoredText("Typ y of n.", ConsoleColor.Red, true);
            }
        }

        // =================================================================
        // TOOL 11 - TELLER, TOTAAL, GEMIDDELDE, MAXIMUM
        // =================================================================
        // Alles VOOR de lus declareren, anders bestaan ze na de lus niet (scope).
        static void StatsDemo()
        {
            int count = 0;
            decimal total = 0;
            decimal highest = 0;

            for (int i = 1; i <= 3; i++)
            {
                decimal amount = ReadDecimal($"Bedrag {i}: ", 0);
                count++;
                total += amount;
                highest = Math.Max(highest, amount);
            }

            decimal average = count > 0 ? total / count : 0;   // nooit delen door 0
            Console.WriteLine($"Aantal: {count}, totaal: {total:F2}, gemiddelde: {average:F2}, max: {highest:F2}");
        }

        // =================================================================
        // TOOL 12 - FOUTMELDINGEN IN VISUAL STUDIO
        // =================================================================
        // "does not exist in the current context"  -> typfout in de naam, of scope:
        //      variabele uit Main gebruikt in een andere methode -> meegeven als parameter
        // "not all code paths return a value"       -> return vergeten op een pad (onderaan return x;)
        // "Cannot implicitly convert type 'double' to 'decimal'" -> m achter het getal: 0.5m
        // "Cannot implicitly convert type 'string' to 'int'"     -> int.Parse / TryParse gebruiken
        // "An object reference is required ..."     -> static vergeten voor je methode
        // "; expected" / "} expected"               -> puntkomma of accolade vergeten
        // "Use of unassigned local variable"        -> variabele een beginwaarde geven (= 0)
        // Programma blijft hangen (oneindige lus)   -> Shift+F5, check ++ / -= / stopvoorwaarde
        // Waarde bekijken: tijdelijk Console.WriteLine(variabele); toevoegen
    }
}