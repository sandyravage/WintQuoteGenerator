using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InspirationalQuoteGeneratorV2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string all = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.txt"));
            string[] separatingChars = { "~Delimiter~" };
            string[] quotes = all.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
            var newsen = new List<string>();
            List<Word> words = new List<Word>();
            foreach (var q in quotes)
            {
                var sen = q.Split(' ');
                var twosen = new string[sen.Length];
                for (int n = 0; n < sen.Length; n++)
                {
                    twosen[n] = Regex.Replace(sen[n].Trim(), @"\t\n\r", "");
                }
                sen = twosen;
                var sb1 = new StringBuilder();
                for (int j = 0; j < sen.Length; j++)
                {
                    var sb2 = new StringBuilder();
                    for (int z = 0; z < sen[j].Length; z++)
                    {
                        if (sen[j].Length > 4 && z == 0)
                        {
                            var isHttp = (sen[j][0].ToString() + sen[j][1].ToString() + sen[j][2].ToString() + sen[j][3].ToString()).ToString().ToLower();
                            if (isHttp == "http" || isHttp == "&amp")
                            {
                                break;
                            }
                        }
                        if (sen[j][z] == '@' && z == 0)
                        {
                            break;
                        }
                        else if (!char.IsPunctuation(sen[j][z]) || sen[j][z] == '\'')
                        {
                            sb2.Append(sen[j][z]);
                        }
                    }
                    if (sb2.ToString().ToLower() == "i")
                    {
                        sb1.Append(sb2.ToString().ToUpper() + ' ');
                    }
                    else if(sb2.ToString() != "")
                    {
                        sb1.Append(sb2.ToString().ToLower() + ' ');
                    }
                }
                newsen.Add(sb1.ToString().Trim());
            }

            foreach (var s in newsen)
            {
                var sen = s.Split(' ');
                for (int i = 0; i < sen.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        if (!words.Exists(x => x.text == sen[i]))
                        {
                            words.Add(new Word() { text = sen[i], isFirst = true, isEnd = false });
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = false, isFirst = false });
                        }
                        else
                        {
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = false, isFirst = false });
                        }
                    }
                    else if (i == sen.Length - 2)
                    {
                        if (!words.Exists(x => x.text == sen[i]))
                        {
                            words.Add(new Word() { text = sen[i], isFirst = false, isEnd = false });
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = true, isFirst = false });
                            words.Add(new Word() { text = sen[i + 1], isEnd = true, isFirst = false });
                        }
                        else
                        {
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = true, isFirst = false });
                            words.Add(new Word() { text = sen[i + 1], isEnd = true, isFirst = false });
                        }
                    }
                    else
                    {
                        if (!words.Exists(x => x.text == sen[i]))
                        {
                            words.Add(new Word() { text = sen[i], isFirst = false, isEnd = false });
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = false, isFirst = false });
                        }
                        else
                        {
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = false, isFirst = false });
                        }
                    }
                }
            }
            var r = new Random();
            var sb3 = new StringBuilder();
            var sb4 = new StringBuilder();
            var currentWord = new Word();
            var wordList = words.Where(x => x.isFirst == true).ToList();
            var randomStart = wordList[r.Next(0, wordList.Count)];
            for (int z = 0; z < randomStart.text.Length; z++)
            {
                if (z == 0)
                {
                    sb3.Append(randomStart.text[z].ToString().ToUpper());
                }
                else
                {
                    sb3.Append(randomStart.text[z]);
                }
            }
            sb4.Append(sb3.ToString() + ' ');
            sb3.Clear();
            currentWord = randomStart;
            while (!currentWord.isEnd)
            {
                var next = currentWord.followedBy[r.Next(0, currentWord.followedBy.Count)].text;
                var nextWord = words.First(x => x.text == next);
                sb4.Append(nextWord.text + ' ');
                currentWord = nextWord;
            }
            richTextBox1.Text = sb4.ToString().Trim() + '.';
        }
    }

    public class Word
    {
        public string text;
        public List<Word> followedBy = new List<Word>();
        public bool isFirst;
        public bool isEnd;
    }
}

