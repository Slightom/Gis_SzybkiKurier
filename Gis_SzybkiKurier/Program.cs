//new version
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Gis_SzybkiKurier
{
    public class edge // needed for list of incidence
    {
        public int end { get; set; }
        public short cost { get; set; }
    }
    public class Heap // priority Queue, needed for dijkstry
    {
        int[] h;
        int heapSize;
        int heapLength;

        public Heap(int n)          //               0
        {                           //             /   \
            h = new int[n];         //            1     2
            for (int i = 0; i < n; i++)  //           / \   / \
            { h[i] = i; }           //          3   4 5   6
            heapSize = n;           //
            heapLength = n;         //
        }                           //

        public void repair1(int v) // putting our top on the heap top
        {
            h[0] = h[v];
            h[v] = 0;
        }

        public bool empty()
        {
            return heapSize == 0 ? true : false;
        }

        public int top()
        {
            if (!empty())
            {
                return h[0];
            }
            return -1;
        }

        public void step()
        {
            h[0] = h[--heapSize];
        }

        public void repair(int[] hp, short[,] d, int i)
        {
            int left, right, parent = 0, dmin, pmin, x;
            while (true)
            {
                left = parent + parent + 1;
                right = left + 1;
                if (left >= heapSize) break;
                dmin = d[i, h[left]];
                pmin = left;
                if ((right < heapSize) && (dmin > d[i, h[right]])) // if right son exists and has shorter route
                {
                    dmin = d[i, h[right]];
                    pmin = right;
                }
                if (d[i, h[parent]] <= dmin) break;
                x = h[parent]; h[parent] = h[pmin]; h[pmin] = x;
                hp[h[parent]] = parent; hp[h[pmin]] = pmin;
                parent = pmin;
            }
        }

        public void repair3(int[] hp, List<edge>[] li, short[,] d, int i, int v)
        {
            int child, parent, x;

            for (child = hp[v]; child > 0; child = parent)
            {
                parent = child / 2;
                if (d[i, h[parent]] <= d[i, h[child]]) break;
                x = h[parent]; h[parent] = h[child]; h[child] = x;
                hp[h[parent]] = parent; hp[h[child]] = child;
            }
        }


    }
    class Program
    {
        public static int bestRoute = int.MaxValue;

        static void Main(string[] args)
        {
            int n = 0, m = 0, k = 0, g = 0, i, j;
            string name = "duzy.txt";
            List<edge>[] li;               // list of incidence 
            short[,] d;                    // [n,n] dijkstry, shortest routs from every top to every top 
            int[,] gg;                     // [g,2] conditions
            List<int> Q = new List<int>(); // collection of MustSeeCity



            readFile1(name, ref n, ref m, ref k, ref g); // load n, m, k, g

            li = new List<edge>[n];
            for (i = 0; i < n; i++)
            {
                li[i] = new List<edge>();
            }

            d = new short[n, n];
            gg = new int[g, 2];

            for (i = 1; i <= k; i++)
            {
                Q.Add(i);
            }

            readFile2(name, n, m, k, g, li, gg); // load List of incidence and list of conditions

            
            List<int[]> permutations = determinePermutations2(Q); // determine all permutations

            //DateTime start = DateTime.Now;
            //TimeSpan time; 
            dijkstry(d, li, n, k); // determine shortest routes from 0,1,2..k,n-1 tops to every top    
            //time = DateTime.Now - start;

            //Console.WriteLine("Czas: " + time.Seconds + "." + time.Milliseconds);
            bestRoute = find(permutations, d, n, gg, g);
            Console.WriteLine(bestRoute);



            // I implemented second version of dijkstry alghoritm where minimum is determinated linear
            // Turned out that for small tests linear searching is faster but for big tests using heap is faster. 

            //start = DateTime.Now;
            //dijkstry2(d, li, n, k); // determine shortest routes from every top to every top
            //time = DateTime.Now - start;
            //Console.WriteLine("Czas: " + time.Seconds + "." + time.Milliseconds);
            //bestRoute = int.MaxValue;
            //bestRoute = find(permutations, d, n, gg, g);
            //Console.WriteLine(bestRoute);
        }

        private static void readFile1(string name, ref int n, ref int m, ref int k, ref int g) // read n, m, k, g
        {
            string[] fileLine = System.IO.File.ReadAllLines(name).First().Split();

            n = Int32.Parse(fileLine[0]);
            m = Int32.Parse(fileLine[1]);
            k = Int32.Parse(fileLine[2]);

            g = Int32.Parse(System.IO.File.ReadAllLines(name).ElementAt(m + 1));

        }
        private static void readFile2(string name, int n, int m, int k, int g, List<edge>[] li, int[,] gg)
        {
            int i, j, w1, w2, s, aa = 0;
            string[] fileLines = System.IO.File.ReadAllLines(name);
            string[] line;
            edge edge = new edge();

            line = fileLines[0].Split();
            n = Int32.Parse(line[0]);
            m = Int32.Parse(line[1]);
            k = Int32.Parse(line[2]);


            // load list of incidence
            for (i = 1; i <= m; i++)
            {
                line = fileLines[i].Split();
                w1 = Int32.Parse(line[0]) - 1;
                w2 = Int32.Parse(line[1]) - 1;

                edge = new edge();
                edge.end = w2;
                edge.cost = Int16.Parse(line[2]);
                li[w1].Add(edge);

                edge = new edge();
                edge.end = w1;
                edge.cost = Int16.Parse(line[2]);
                li[w2].Add(edge);
            }

            #region show list of incidence
            //foreach (List<edge> l in li)
            //{
            //    Console.Write(aa++ + " | ");
            //    foreach (edge e in l)
            //    {
            //        Console.Write(e.end + " (" + e.cost + ") ");
            //    }
            //    Console.WriteLine();
            //}
            #endregion


            // load conditions
            for (i = 0; i < g; i++)
            {
                line = fileLines[i + m + 2].Split();
                w1 = Int32.Parse(line[0]) - 1;
                w2 = Int32.Parse(line[1]) - 1;

                gg[i, 0] = w1;
                gg[i, 1] = w2;
            }

            #region show conditions
            //for (i = 0; i < g; i++)
            //{
            //    Console.WriteLine(gg[i + m + 2, 0] + " " + gg[i + m + 2, 1]);
            //}
            #endregion
        }

        private static void dijkstry2(short[,] d, List<edge>[] li, int n, int k)
        {
            int j, v, indexOfMin = 0;
            short min;
            int[] determinated = new int[n];

            int[] specialTops = new int[k + 2];
            for (int i = 0; i < k + 1; i++)
            {
                specialTops[i] = i;
            }
            specialTops[k + 1] = n - 1;

            foreach (int i in specialTops) // for every top
            {
                for (j = 0; j < n; j++)
                {
                    d[i, j] = short.MaxValue;
                    determinated[j] = 0;
                }

                d[i, i] = 0;
                for (j = 1; j < n; j++)
                {
                    min = short.MaxValue;
                    for (int j2 = 0; j2 < n; j2++) //find minimum
                    {
                        if (determinated[j2] == 0 && d[i, j2] < min)
                        {
                            min = d[i, j2];
                            indexOfMin = j2;
                        }
                    }
                    determinated[indexOfMin] = 1;

                    foreach (edge e in li[indexOfMin]) //update
                    {
                        if (determinated[e.end] == 0 && (d[i, e.end] > min + e.cost))
                        {
                            d[i, e.end] = Int16.Parse((min + e.cost).ToString());
                        }
                    }
                }

            }

            #region show shourtest routs
            //for (int i = 0; i < n; i++)  // show shortest routes
            //{
            //    for (j = 0; j < n; j++)
            //    {
            //        Console.Write("{0,4}", d[i, j]);
            //    }
            //    Console.WriteLine("\n");
            //}
            #endregion
        }

        private static void dijkstry(short[,] d, List<edge>[] li, int n, int k)
        {
            int j, u;
            Heap heap;
            bool[] QS;
            int[] hp;

            int[] specialTops = new int[k + 2];
            for (int i = 0; i < k + 1; i++)
            {
                specialTops[i] = i;
            }
            specialTops[k + 1] = n - 1;

            foreach (int i in specialTops) // for every top
            {

                heap = new Heap(n);
                QS = new bool[n];
                hp = new int[n];
                for (j = 0; j < n; j++)
                {
                    QS[j] = false;
                    d[i, j] = short.MaxValue;
                    hp[j] = j;
                }

                d[i, i] = 0; //  step 4
                heap.repair1(i); // set v in HeapTop and repair Heap
                hp[0] = i;
                hp[i] = 0;

                for (j = 1; j < n; j++)
                {
                    u = heap.top(); // we take the minimum, HeapTop
                    heap.step(); // remove heaptop, insert there the last top
                    hp[heap.top()] = i;
                    heap.repair(hp, d, i);

                    QS[u] = true;


                    foreach (edge edge in li[u])
                    {
                        if (!QS[edge.end] && (d[i, edge.end] > d[i, u] + edge.cost))
                        {
                            d[i, edge.end] = (short)(d[i, u] + edge.cost);

                            heap.repair3(hp, li, d, i, edge.end);
                        }
                    }
                }
            }

            #region show shourtest routs
            //for (int i = 0; i < n; i++)  // show shortest routes
            //{
            //    for (j = 0; j < n; j++)
            //    {
            //        Console.Write("{0,4}", d[i, j]);
            //    }
            //    Console.WriteLine("\n");
            //}
            #endregion
        }

    

        private static void insertNumber(List<int> Q, int[] permutation, int n, List<int[]> permutations)
        {
            int i, j;
            List<int> newQ;
            int[] copy;


            if (Q.Count == 1)
            {
                permutation[n - Q.Count] = Q[0];
                copy = new int[n];
                for (i = 0; i < n; i++)
                    copy[i] = permutation[i];
                permutations.Add(copy);
            }
            else
            {
                foreach (int number in Q)
                {
                    permutation[n - Q.Count] = number;
                    newQ = new List<int>();
                    for (i = 0; i < Q.Count; i++) // make collection without choosen number
                        newQ.Add(Q[i]);           //
                    newQ.Remove(number);          //

                    insertNumber(newQ, permutation, n, permutations);
                }
            }
        }
        private static List<int[]> determinePermutations2(List<int> Q)
        {
            int n = Q.Count;
            int[] permutation = new int[n];
            List<int[]> permutations = new List<int[]>();

            insertNumber(Q, permutation, n, permutations);

            return permutations;
        }

        private static int find(List<int[]> permutations, short[,] d, int n, int[,] gg, int g)
        {
            int sum = 0, i, best = int.MaxValue;
            int[] permutationExtra = permutations[0];

            foreach (int[] permutation in permutations)
            {
                if (!allowed(permutation, gg, g))
                {
                    continue;
                }

                sum += d[0, permutation[0]];
                for (i = 1; i < permutation.Length; i++)
                {
                    sum += d[permutation[i - 1], permutation[i]];
                }
                sum += d[permutation[i - 1], n - 1];

                if (sum < best)
                {
                    permutationExtra = permutation;
                    best = sum;
                }
                sum = 0;
            }

            //foreach(int x in permutationExtra)
            //{
            //    Console.Write(x + "-");
            //}
            //Console.WriteLine("\n");
            return best;
        }

        private static bool allowed(int[] permutation, int[,] gg, int g)
        {
            int i, j, indexOfA = -1, indexOfB = -1;

            for (i = 0; i < g; i++) // for all condition
            {
                for (j = 0; j < permutation.Length; j++)
                {
                    if (permutation[j] == gg[i, 0]) indexOfA = j;
                    if (permutation[j] == gg[i, 1]) indexOfB = j;
                }
                if (indexOfA > indexOfB) return false;
            }

            return true;
        }
    }
}

