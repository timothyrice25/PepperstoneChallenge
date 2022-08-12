using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PepperstoneChallenge
{
    class ClassChallengeProcessor
    {
        private string DictionaryFile;
        private string SearchFile;
        public List<string> DictionaryList;
        public List<string> SearchList;
        public List<int> CaseCount;

        public ClassChallengeProcessor()
        {
            Initialize();
        }

        public ClassChallengeProcessor(string dictionary, string search)
        {
            Initialize();

            DictionaryFile = dictionary;
            SearchFile = search;
            ParseDictionaryFile();
            ParseSearchFile();

        }

        public void Initialize()
        {
            DictionaryList = new List<string>();
            SearchList = new List<string>();
            CaseCount = new List<int>();
        }

        public void Process()
        {
            ParseDictionaryFile();
            ParseSearchFile();
            ProcessData();
        }

        public void ProcessData()
        {
            for (var k = 0; k < SearchList.Count; k++)
            {
                for (var i = 0; i < DictionaryList.Count; i++)
                {
                    var probabilityList = GenerateProbabilityList(DictionaryList[i]);
                    for (var j = 0; j < probabilityList.Count; j++)
                    {
                        var found = false;

                        if (SearchList[k].Contains(probabilityList[j]))
                        {
                            CaseCount[k]++;
                            found = true;
                        }

                        if (found)
                            break;
                    }
                }
            }
        }

        public List<string> GenerateProbabilityList(string input)
        {
            var ret = new List<string>();

            var start = input[0].ToString();
            var end = input[input.Length - 1].ToString();

            for (var i = 1; i < input.Length - 1; i++)
            {
                var token = input[i].ToString();
                var others = input.Remove(input.Length - 1, 1);
                others = others.Remove(i, 1);
                others = others.Remove(0, 1);

                var str = string.Format("{0}{1}{2}{3}", start, token, others, end);
                if (!ret.Contains(str))
                {
                    ret.Add(str);
                }

                for (var j = 0; j < others.Length - 1; j++)
                {
                    var substr = others.Remove(others.Length - 1, 1);
                    substr = others[others.Length - 1] + substr;

                    str = string.Format("{0}{1}{2}{3}", start, token, substr, end);
                    if (!ret.Contains(str))
                    {
                        ret.Add(str);
                    }
                }
            }
            return ret;
        }

        public string GenerateResult()
        {
            var ret = new StringBuilder();
            for (var i = 0; i < CaseCount.Count; i++)
            {
                ret.AppendLine(string.Format("Case #{0}: {1}", (i + 1).ToString(), CaseCount[i]));
            }
            return ret.ToString();
        }

        public string CheckDuplicateWordInDictionaryFile()
        {
            var list = new List<string>();
            for (var i = 0; i < DictionaryList.Count; i++)
            {
                if (!list.Contains(DictionaryList[i]))
                {
                    list.Add(DictionaryList[i]);
                }
                else
                    return DictionaryList[i];
            }
            return "";
        }

        public string CheckWordLengthInDictionaryFile()
        {
            for (var i = 0; i < DictionaryList.Count; i++)
            {
                if ((DictionaryList[i].Length >= 105 || DictionaryList[i].Length <= 2) && DictionaryList[i] != "")
                {
                    return DictionaryList[i];
                }

            }
            return "";
        }

        public bool CheckDictionaryFileLength()
        {
            var str = File.ReadAllText(DictionaryFile);
            str = str.Replace(Environment.NewLine, "");
            str = str.Replace(" ", "");
            if (str.Length > 105)
                return false;
            return true;
        }

        public void ParseDictionaryFile()
        {
            DictionaryList.Clear();
            //Read File
            var ar = File.ReadAllLines(DictionaryFile);
            for (var i = 0; i < ar.Length; i++)
            {
                //if (!DictionaryList.Contains(ar[i]))
                //{
                DictionaryList.Add(ar[i]);
                //}
            }
        }

        public void ParseSearchFile()
        {
            CaseCount.Clear();
            var ar = File.ReadAllLines(SearchFile);
            SearchList = ar.ToList();
            for (var i = 0; i < SearchList.Count; i++)
            {
                CaseCount.Add(0);
            }

        }


    }
}
