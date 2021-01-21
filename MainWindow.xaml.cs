using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
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
using MorseCode;

namespace MorseCodeCoverter
{
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Button-related

        private void Button_Convert_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Right.Text = TextBox_Left.Text.ToMorse();
        }
        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            Button_Convert_Click(null, null);
            Morse.PlayMorse(TextBox_Left.Text);
        }
        private void Button_CloseAbout_Click(object sender, RoutedEventArgs e)
        {
            Border_About.Visibility = Visibility.Hidden;
        }
        private void Button_About_Click(object sender, RoutedEventArgs e)
        {
            Border_About.Visibility = Visibility.Visible;
        }

        #endregion

        #region Setting-related
        private void TextBox_Conversion_Dash_TextChanged(object sender, TextChangedEventArgs e)
        {
            Morse.dash = TextBox_Conversion_Dash.Text;
        }

        private void TextBox_Conversion_Dot_TextChanged(object sender, TextChangedEventArgs e)
        {
            Morse.dot = TextBox_Conversion_Dot.Text;
        }

        private void TextBox_Spacing_Morse_TextChanged(object sender, TextChangedEventArgs e)
        {
            Morse.morseSpace = TextBox_Spacing_Morse.Text;
        }

        private void TextBox_Spacing_Chars_TextChanged(object sender, TextChangedEventArgs e)
        {
            Morse.charSpace = TextBox_Spacing_Chars.Text;
        }

        private void TextBox_Spacing_Words_TextChanged(object sender, TextChangedEventArgs e)
        {
            Morse.wordSpace = TextBox_Spacing_Words.Text;
        }

        #endregion

        private void Button_Changelog_Click(object sender, RoutedEventArgs e)
        {
            Border_Changelog.Visibility = Visibility.Visible;
        }

        private void Button_CloseChangelog_Click(object sender, RoutedEventArgs e)
        {
            Border_Changelog.Visibility = Visibility.Hidden;
        }
    }
}

namespace MorseCode
{
    /// <summary>
    /// The class "Morse" contains two public functions:
    /// 
    /// ToMorse(this string) which return the string in morse code according to the settings: 
    /// dot, dash, morseSpace, charSpace, wordSpace. An unsupported character will not be converted
    /// and will be treated as a regular character in terms of spacing. When the converter detects 
    /// an unsupported character, the return string will end with a warning message.
    /// 
    /// The other public function is PlayMorse(this string) which will play audio of the morse code with beeps and boops.
    /// </summary>
    public static class Morse
    {
        // International morse code: https://en.wikipedia.org/wiki/Morse_code

        #region Conversion

        // Dictionairy for conversion.
        // False == Dot == "•" && True == Dash == "−"
        private static readonly Dictionary<char, bool[]> ConversionDict = new Dictionary<char, bool[]>()
        {
            {'a', new bool[] {false, true } },
            {'b', new bool[] {true , false, false, false} },
            {'c', new bool[] {true , false, true , false} },
            {'d', new bool[] {true , false, false} },
            {'e', new bool[] {false} },
            {'f', new bool[] {false, false, true , false} },
            {'g', new bool[] {true , true , false } },
            {'h', new bool[] {false, false, false, false, } },
            {'i', new bool[] {false, false} },
            {'j', new bool[] {false, true , true , true } },
            {'k', new bool[] {true , false, true } },
            {'l', new bool[] {false, true , false, false} },
            {'m', new bool[] {true , true } },
            {'n', new bool[] {true , false} },
            {'o', new bool[] {true , false} },
            {'p', new bool[] {false, true , true , false} },
            {'q', new bool[] {true , true , false, true } },
            {'r', new bool[] {false, true , false} },
            {'s', new bool[] {false, false, false} },
            {'t', new bool[] {true } },
            {'u', new bool[] {false, false, true } },
            {'v', new bool[] {false, false, false, true } },
            {'w', new bool[] {false, true , true } },
            {'x', new bool[] {true , false, false, true } },
            {'y', new bool[] {true , false, true , true } },
            {'z', new bool[] {true , true , false, false} },
            {'1', new bool[] {false, true , true , true , true } },
            {'2', new bool[] {false, false, true , true , true } },
            {'3', new bool[] {false, false, false, true , true } },
            {'4', new bool[] {false, false, false, false, true } },
            {'5', new bool[] {false, false, false, false, false} },
            {'6', new bool[] {true , false, false, false, false} },
            {'7', new bool[] {true , true , false, false, false} },
            {'8', new bool[] {true , true , true , false, false} },
            {'9', new bool[] {true , true , true , true , false} },
            {'0', new bool[] {true , true , true , true , true } }
        };

        // These are public since they are settings that can be changed
        public static string dot = "•", dash = "−";
        public static string morseSpace = "", charSpace = " ", wordSpace = "   ";

        // Convert string to morse string according to settings
        public static string ToMorse(this string str)
        {
            /// This method returns the string converted into international morse code.
            /// It supports the 26 English letters and 10 numerals.
            /// Unknown characters become "*".
            /// Dots => "•", Dash => "−".
            /// See also: static class Morse.
            /// 
            
            // Preventing errors ...
            if(str == "")
            {
                return "";
            }

            //
            bool someNotConverted = false;

            //

            List<string> lines = new List<string>();
            // For each line
            foreach (string line in str.ToLower().Trim().Split('\n'))
            {
                List<string> words = new List<string>();
                // For each word in line
                foreach (string word in str.ToLower().Trim().Split(' '))
                {
                    List<string> chars = new List<string>();

                    // For each char in word
                    foreach (char c in word)
                    {
                        // KeyNotFound
                        if (!Morse.ConversionDict.ContainsKey(c))
                        {
                            chars.Add(c.ToString());
                            someNotConverted = true;
                        }
                        else
                        {

                            chars.Add(c.ToMorse());
                        }
                    }
                    // Add word
                    words.Add(chars.Aggregate((i, j) => i + charSpace + j));
                }
                // Join words
                lines.Add(words.Aggregate((i, j) => i + wordSpace + j));
            }
            string rStr = lines.Aggregate((i, j) => i + "\n" + j);

            if (someNotConverted)
            {
                rStr += "\n\nWarning: Some caracters were not converted.";
            }

            return rStr;
        }

        // Convert chars to morse string
        private static string ToMorse(this char c)
        {
            List<string> r = new List<string>();
            foreach (bool isDash in ConversionDict[c])
            {
                if (isDash)
                {
                    r.Add(dash);
                }
                else
                {
                    r.Add(dot);
                }
            }
            return r.Aggregate((i, j) => i + morseSpace + j);
        }

        #endregion

        #region Audio

        // Media player
        private static readonly SoundPlayer SP_Beep = new SoundPlayer(MorseCodeConverter.Properties.Resources.Beep);
        private static readonly SoundPlayer SP_Boop = new SoundPlayer(MorseCodeConverter.Properties.Resources.Boop);

        // Playlist
        private static List<byte> PlayList;
        private static int PlayListIndex;

        // string to morse code audio
        public static void PlayMorse(this string str)
        {
            /// The length of a dot is one unit.
            /// A dash is three units.
            /// The space between parts of the same letter is one unit.
            /// The space between letters is three units.
            /// The space between words is seven units.
            /// 
            /// Playlist is a list of bytes
            /// 
            /// 0 == Dot (Beep) duration is 145ms
            /// 1 == Dash (Boop) duration is 435ms (3 x 145)
            /// 2 == 145ms pauze (between dots and dashes)
            /// 3 == 435ms pauze (between letters)
            /// 4 == 1015ms pauze (between words)

            // Reset playlist
            PlayList = new List<byte>();

            string[] words = str.ToLower().Trim().Split(' ');

            // For each word in str
            for (int i=0; i<words.Length; i++)
            {
                char[] chars = words[i].ToCharArray();

                // For each char in word
                for (int j=0; j<chars.Length; j++)
                {
                    if (ConversionDict.ContainsKey(chars[j]))
                    {
                        bool[] b = ConversionDict[chars[j]];

                        // For each morse char in c
                        for (int k = 0; k < b.Length; k++)
                        {
                            // Add morse char to playlist
                            if (b[k])
                            {
                                PlayList.Add(1);
                            }
                            else
                            {
                                PlayList.Add(0);
                            }

                            // Add space
                            if (k != b.Length - 1)
                            {
                                PlayList.Add(2);
                            }
                        }
                        // Add space
                        if (j != chars.Length - 1)
                        {
                            PlayList.Add(3);
                        }
                    }
                    else if (chars[j] == '\n') // KeyNotFound
                    {
                        PlayList.Add(5); // line break
                    }
                    else
                    {
                        PlayList.Add(2);
                    }
                }
                // Add space
                if (i != words.Length - 1)
                {
                    PlayList.Add(4);
                }
            }







            // Play first sound
            PlayListIndex = 0;
            PlayNextInPlaylist();
        }

        // Function that calls itself
        static void PlayNextInPlaylist()
        {
            /// Playlist is a list of bytes
            /// 
            /// 0 == Dot (Beep) duration is 145ms
            /// 1 == Dash (Boop) duration is 435ms (3 x 145)
            /// 2 == 145ms pauze (between dots and dashes)
            /// 3 == 435ms pauze (between letters)
            /// 4 == 1015ms pauze (between words)

            // Playlist ended exception
            if (PlayListIndex >= PlayList.Count)
            {
                return;
            }

            int sleepTime;

            // Play sound/pauze
            switch (PlayList[PlayListIndex])
            {
                case 0:
                    SP_Beep.Play();
                    sleepTime = 145;
                    break;
                case 1:
                    SP_Boop.Play();
                    sleepTime = 145;
                    break;
                case 2:
                    sleepTime = 145;
                    break;
                case 3:
                    sleepTime = 435;
                    break;
                case 4: 
                    sleepTime = 1015;
                    break;
                default: // Really case 5
                    sleepTime = 1000;
                    break;
            }

            // Increment playlist index
            PlayListIndex++;

            // Queue up next sound/pauze
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(sleepTime);
                PlayNextInPlaylist();
            });
        }

        #endregion
    }
}