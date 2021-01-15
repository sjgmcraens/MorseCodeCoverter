using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExtensionMethods;

namespace MorseCodeCoverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Convert_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Right.Text = TextBox_Left.Text.ToMorseCode();
        }


        private void Button_Mode_Click(object sender, RoutedEventArgs e)
        {
            Morse.SwitchConvertMode();
            Button_Mode.Content = "Mode: " + Morse.GetConvertModeString(); 
            Button_Convert_Click(null, null);
        }

        private void Button_Spacing_Click(object sender, RoutedEventArgs e)
        {
            Morse.SwitchSpacingMode();
            Button_Spacing.Content = "Spacing: " + Morse.GetSpacingModeString();
            Button_Convert_Click(null, null);
        }
    }
}

namespace ExtensionMethods
{
    
    public static class Morse
    {
        // International morse code: https://en.wikipedia.org/wiki/Morse_code

        // The length of a dot is one unit.
        // A dash is three units.
        // The space between parts of the same letter is one unit.
        // The space between letters is three units.
        // The space between words is seven units.

        static string LetterSpace = "   ", WordSpace = "       ";

        static Dictionary<char, string> DotDashConversion = new Dictionary<char, string>()
        {
            {'a', "• −"},
            {'b', "− • • •"},
            {'c', "− • − •"},
            {'d', "− • •"},
            {'e', "•"},
            {'f', "• • − •"},
            {'g', "− − •"},
            {'h', "• • • •"},
            {'i', "• •"},
            {'j', "• − − −"},
            {'k', "− • −"},
            {'l', "• − • •"},
            {'m', "− −"},
            {'n', "− •"},
            {'o', "− − −"},
            {'p', "• − − •"},
            {'q', "− − • −"},
            {'r', "• − •"},
            {'s', "• • •"},
            {'t', "−"},
            {'u', "• • −"},
            {'v', "• • • −"},
            {'w', "• − −"},
            {'x', "− • • −"},
            {'y', "− • − −"},
            {'z', "− − • •"},
            {'1', "• − − − −"},
            {'2', "• • − − −"},
            {'3', "• • • − −"},
            {'4', "• • • • −"},
            {'5', "• • • • •"},
            {'6', "− • • • •"},
            {'7', "− − • • •"},
            {'8', "− − − • •"},
            {'9', "− − − − •"},
            {'0', "− − − − −"}
        };

        static Dictionary<char, string> CurrentConversion = DotDashConversion;

        public static string convertMode = "DotDash";
        public static byte letterSpacing = 0;
        public static string letterSpace = "";
        public static byte wordSpacing = 1;
        public static string wordSpace = " ";

        public static string ToMorseCode(this string str)
        {
            /// This method returns the string converted into international morse code.
            /// It supports the 26 English letters and 10 numerals.
            /// Unknown characters become "*".
            /// Dots => "•", Dash => "−".
            /// See also: static class Morse.

            // Convert string to lowercase
            string rStr = "";

            //
            bool someNotConverted = false;

            // For each word in str
            foreach (string word in str.ToLower().Trim().Split(' '))
            {
                string w = "";
                // For each caracter in word
                foreach (char c in word)
                {
                    // KeyNotFound
                    if (!Morse.DotDashConversion.ContainsKey(c))
                    {
                        w += c;
                        someNotConverted = true;
                    }
                    else
                    {
                        w += Morse.CurrentConversion[c];
                    }
                    w += LetterSpace;
                }
                // Add word
                rStr += w;
                rStr += WordSpace;
            }
            rStr = rStr.Trim();

            if (someNotConverted)
            {
                rStr += "\n\nWarning: Some caracters were not converted.";
            }

            return rStr;
        }

        public static Dictionary<char, string> BinairyConversion = new Dictionary<char, string>();

        static Morse()
        {
            // Compute BinairyConversion dictionary from DotDashConversion (just to save me some time)
            foreach (KeyValuePair<char, string> p in DotDashConversion)
            {
                BinairyConversion.Add(p.Key, p.Value.Replace('•', '0').Replace('−', '1'));
            }
        }

        public static void SwitchConvertMode()
        {
            if (convertMode == "DotDash")
            {
                convertMode = "Binairy";
                CurrentConversion = BinairyConversion;
            }
            else
            {
                convertMode = "DotDash";
                CurrentConversion = DotDashConversion;
            }
        }

        public static string GetConvertModeString()
        {
            if (convertMode == "DotDash")
            {
                return "•−";
            }
            else
            {
                return "01";
            }
        }

        public static void SwitchLetterSpacingMode()
        {
            if (letterSpacing < 3)
            {
                letterSpacing++;
                LetterSpace = "".PadRight(letterSpacing, ' ');
            }
            else
            {
                letterSpacing = 0;
                LetterSpace = "";
            }
        }

        public static void SwitchWordSpacingMode()
        {
            if (wordSpacing < 7)
            {
                wordSpacing++;
                wordSpace = "".PadRight(wordSpacing, ' ');
            }
            else
            {
                wordSpacing = 0;
                wordSpace = "";
            }
        }
    }
}