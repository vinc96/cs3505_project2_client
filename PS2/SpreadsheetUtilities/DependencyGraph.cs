// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Code implemented by Josh Christensen (u0978248)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// A dependent member object, consisting of a couple of HashSets
        /// </summary>
        private struct DepenedentMember
        {
            public HashSet<String> dependees;
            public HashSet<String> dependents;
        }
        
        /// <summary>
        /// Dictionary, containing all the pairs in this DependencyGraph
        /// </summary>
        Dictionary<String, DepenedentMember> pairs = new Dictionary<String, DepenedentMember>();

        /// <summary>
        /// Represents how many ordered pairs there are in this Dependency Graph.
        /// </summary>
        int size;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get {
                if (pairs.ContainsKey(s))
                {
                    return pairs[s].dependees.Count;
                }
                else
                {
                    return 0;
                }
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (pairs.ContainsKey(s))
            {
                return pairs[s].dependents.Count != 0;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (pairs.ContainsKey(s))
            {
                return pairs[s].dependees.Count != 0;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (pairs.ContainsKey(s))
            {
                return pairs[s].dependents.AsEnumerable();
            }
            else
            {
                return Enumerable.Empty<String>();
            }
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (pairs.ContainsKey(s))
            {
                return pairs[s].dependees.AsEnumerable();
            }
            else
            {
                return Enumerable.Empty<String>();
            }
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            if (pairs.ContainsKey(s))
            {
                if (pairs[s].dependents.Contains(t))
                {
                    //Do nothing
                }
                else
                {
                    pairs[s].dependents.Add(t);
                    size++;
                }
                
            }
            else
            {
                //Initialize DependentMember
                DepenedentMember member = new DepenedentMember();
                member.dependees = new HashSet<string>();
                member.dependents = new HashSet<string>(); 
                pairs.Add(s, member);
                pairs[s].dependents.Add(t);
                size++;
            }
            if (pairs.ContainsKey(t))
            {
                pairs[t].dependees.Add(s);
            }
            else
            {
                DepenedentMember member = new DepenedentMember();
                member.dependees = new HashSet<string>();
                member.dependents = new HashSet<string>();
                pairs.Add(t, member);
                pairs[t].dependees.Add(s);
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (pairs.ContainsKey(s))
            {
                if (pairs[s].dependents.Contains(t))
                {
                    pairs[s].dependents.Remove(t);
                    //We assume that, if we have an item listed as a dependent, that item has a corresponding value in the dictionary.
                    pairs[t].dependees.Remove(s);
                    //If either s or t is now a complete orphan (no dependents or dependees), remove it to save memory.
                    if (!(this.HasDependees(s) || this.HasDependents(s)))
                    {
                        pairs.Remove(s);
                    }
                    if (!(this.HasDependees(t) || this.HasDependents(t)))
                    {
                        pairs.Remove(t);
                    }

                    size--;
                } 
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            //REMOVE LATER: WHAT HAPPENS WHEN s DOES NOT EXIST?
            if(pairs.ContainsKey(s))
            {
                List<string> dependents = pairs[s].dependents.ToList();
                //Remove all the existing dependents of s
                foreach (string t in dependents)
                {
                    this.RemoveDependency(s, t);
                }
                //Add all the new dependents of s
                foreach (string t in newDependents)
                {
                    this.AddDependency(s, t);
                }
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            //REMOVE LATER: WHAT HAPPENS WHEN s DOES NOT EXIST?
            if (pairs.ContainsKey(s))
            {
                List<string> dependees = pairs[s].dependees.ToList();
                //Remove all the dependencies of the form (r, s) where r is any dependee of s
                foreach (string t in dependees)
                {
                    this.RemoveDependency(t, s);
                }
                //Add in the new dependency pairs.
                foreach (string t in newDependees)
                {
                    this.AddDependency(t, s);
                }

            }
        }
    }
}


