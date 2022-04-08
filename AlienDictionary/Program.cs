using System;
using System.Collections.Generic;
using System.Linq;

namespace AlienDictionary
{
    public class DNode
    {
        public string Char;
        public DNode prev = null;
        public DNode next = null;
        public DNode(string character) => Char = character;
    }

    public class AlienCharacters
    {
        public AlienCharacters(int k) => MaxChars = k;

        private int MaxChars; 
        private DNode head = null;
        private Dictionary<string, DNode> index = new Dictionary<string, DNode>(); 

        // As we use Dictionary for indexing, the time complexity for inserting 
        // charactes in order will take O(1)
        // Time: O(1)
        // Space: O(c), where 'c' is the unique character count. 
        public bool UpdateCharacterOrdering(string predChar, string succChar)
        {
            DNode pNode = null, sNode = null;
            bool isSNodeNew = false, isPNodeNew = false; 
            if(!index.TryGetValue(predChar, out pNode))
            {
                pNode = new DNode(predChar);
                index[predChar] = pNode;
                isPNodeNew = true;
            }
            if (!index.TryGetValue(succChar, out sNode))
            {
                sNode = new DNode(succChar);
                index[succChar] = sNode;
                isSNodeNew = true;
            }

            // before ordering is formed, validate if both the nodes are already present
            if (!isSNodeNew && !isPNodeNew)
            {
                if (!Validate(predChar, succChar))
                    return false;
            }
            else if ((isPNodeNew && !isSNodeNew) || (isPNodeNew && isSNodeNew))
                InsertNodeBefore(ref pNode, ref sNode);
            else
                InsertNodeAfter(ref pNode, ref sNode);

            if (pNode.prev == null)
                head = pNode;

            return true;
        }

        // Time: O(1)
        private void InsertNodeAfter(ref DNode pNode, ref DNode sNode)
        {
            sNode.next = pNode?.next;
            if (pNode.next != null)
                pNode.next.prev = sNode;
            
            pNode.next = sNode;
            sNode.prev = pNode;
        }

        // Time: O(1)
        private void InsertNodeBefore(ref DNode pNode, ref DNode sNode)
        {
            // insert pnode before snode
            pNode.prev = sNode?.prev;

            if (sNode.prev != null)
                sNode.prev.next = pNode;
            sNode.prev = pNode;
            pNode.next = sNode;
        }

        // Time: O(1)
        private bool Validate(string predChar, string succChar)
        {
            // this is the first level of validation 
            // validate if predChar node actually occures before succCharNode. 
            DNode sNode = index[succChar];

            while(sNode != null)
            {
                if (sNode.Char != predChar)
                    sNode = sNode.prev;
                else
                    return true; // validation successful
            }

            // if we have reached the end and not found the predChar before succChar
            // something is not right! 
            return false;
        }

        public override string ToString()
        {
            string res = "";
            int count = 0;
            DNode currNode = head; 

            while(currNode != null)
            {
                res += currNode.Char + " - ";
                count++;
                currNode = currNode.next;
            }

            // second level of validation
            if (count != MaxChars) // something went wrong!
                res = "ERROR!!! Input words not enough to find all k unique characters.";

            return res;
        }
    }

    class Program
    {
        static int k = 3;
        static AlienCharacters alienCharacters = new AlienCharacters(k);
        static List<string> vocabulary = new List<string>(); 

        static void Main(string[] args)
        {
            //vocabulary.Add("baa");
            //vocabulary.Add("abcd");
            //vocabulary.Add("abca");
            //vocabulary.Add("cab");
            //vocabulary.Add("cad");

            vocabulary.Add("aa");
            vocabulary.Add("aab");
            vocabulary.Add("aac");

            ProcessVocabulary(0);

            Console.WriteLine(alienCharacters.ToString());

            Console.ReadLine();
        }

        // Time: O(vocabulary.Count + max(word.Length))
        // Space: O(c)
        static void ProcessVocabulary(int startIndex)
        {
            // compare word at startIndex and startIndex + 1
            // idea is that, we compare two words, one caracter at a time. 
            // if both caracters are different, we stop the comarison
            // and the character at startIndex comes before the other. 
            // class AlienCharacters takes care of the overall ordering.
            // we then recurse to compare words at startIndex + 1 and startIndex + 2. 

            // boundry conditions: 
            // the startIndex must be within range
            // when comparing 2 words, if we exhaust on one 
            // (one is smaller in length than the other), 
            // compare only uptil either one exhausts

            if (startIndex >= vocabulary.Count - 1)
                return;

            // following is O(max(str1.Length, str2.Length))
            var res = GetPredSuccChar(vocabulary.ElementAt(startIndex), vocabulary.ElementAt(startIndex + 1)); 
            if (res != null)
            { 
                if (!alienCharacters.UpdateCharacterOrdering(res.Item1, res.Item2))
                {
                    Console.WriteLine("ERROR!!! Invalid input data, the words maybe in wrong order");
                    return;
                }
            }

            ProcessVocabulary(startIndex + 1);
        }

        //Time: O(max(str1.Length, str2.Length)
        //Space: O(1)
        static Tuple<string, string> GetPredSuccChar(string str1, string str2)
        {
            Tuple<string, string> result = null;

            if (str1.Length == 0 || str2.Length == 0)
                return null; // invalid condition. 

            if(str1[0] != str2[0]) // found an ordering
            {
                result = new Tuple<string, string>(str1[0].ToString(), str2[0].ToString());
                return result;
            }

            string s1 = str1.Substring(1, str1.Length - 1);
            string s2 = str2.Substring(1, str2.Length - 1);

            if (s1.Length == 0 || s2.Length == 0)
                return null; // recursion can stop now. 

            return GetPredSuccChar(s1, s2); 
        }
    }
}
