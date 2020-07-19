/* Mark Wagner
   7/17/2020
   This is a remake of a Google Page Rank program that was done in C++ for
   COP 3520: Data Structures. The program will read an input file. The first
   line of the input file will declare the number of lines that will follow and
   the number of power iterations that will be performed for the page rank.
*/


using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        public class Page
        {
            // A Page is an intialization of a website that has links inward and outward and a calculated page rank
            private string Name;
            private readonly List<Page> OutLinks = new List<Page>();
            private readonly List<Page> InLinks = new List<Page>();
            private double Rank = 0.0;

            // Page constructor with no connections
            public Page(string name)
            {
                this.Name = name;
            }

            // Returns the name of the page
            public string GetName()
            {
                return Name;
            }

            // Returns all of the connections of this page
            public List<Page> GetOutLinks()
            {
                return this.OutLinks;
            }

            // Returns a specific connection based on the index
            public Page GetOutLinks(int index)
            {
                return OutLinks[index];
            }

            // Returns all of the connections of this page
            public List<Page> GetInLinks()
            {
                return this.InLinks;
            }

            // Returns a specific connection based on the index
            public Page GetInLinks(int index)
            {
                return InLinks[index];
            }

            // Adds a new page to the list of out pages
            public void AddOutLink(Page newPage)
            {
                OutLinks.Add(newPage);
                SortPages(OutLinks);
            }

            // Adds a new page to the list of pages
            public void AddInLink(Page newPage)
            {
                InLinks.Add(newPage);
                SortPages(InLinks);
            }

            // Sets the rank of the page - used following a page rank calculation
            public void SetRank(double rank)
            {
                this.Rank = rank;
            }

            // Returns the page's rank
            public double GetRank()
            {
                return Rank;
            }
        }

        // Returns the index of a given page in a list of page or if
        // the page is not found in the list, a negative number is returned
        static int FindPage(List<Page> from, Page to)
        {
            for (int ii = 0; ii < from.Count; ii++)
            {
                if (from[ii].GetName() == to.GetName())
                {
                    return ii;
                }
            }
            return -1;
        }

        // This will read the inputFile, determine the power iterations to complete
        // and generate the pages
        static List<Page> Input(string[] args, System.IO.StreamReader inputFile, int numberOfLines)
        {
            //// Takes an inputFile at a given path and reads the first line, expection two integers
            List<Page> Pages = new List<Page>();
            for (int ii = 0; ii < numberOfLines; ii++)
            {
                string line = "";
                string[] splitLines = new string[2];
                // Since there is a chance for incorrect input, the text reading is wrapped in a try-catch statement
                // This will print an error message if an exception is thrown by Split
                try
                {
                    line = inputFile.ReadLine();
                    splitLines = line.Split(' ');
                }
                catch (Exception)
                {
                    Console.WriteLine("File error: Cannot read past end of file");
                    Console.Write("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(0);

                }
                Page fromPage = new Page(splitLines[0]);
                Page toPage = new Page(splitLines[1]);

                // 4 possibilites: The 'from' page already exists and the 'to' page exists,
                // the 'from' page exists but not the 'to' page, the 'to' page exists but not
                // the 'from' page, or both pages do not exist
                if (FindPage(Pages, fromPage) >= 0)
                {
                    // If both pages exist
                    int fromPageIndex = FindPage(Pages, fromPage);
                    if (FindPage(Pages, toPage) >= 0)
                    {
                        int toPageIdex = FindPage(Pages, toPage);
                        Pages[fromPageIndex].AddOutLink(Pages[toPageIdex]);
                        Pages[toPageIdex].AddInLink(Pages[fromPageIndex]);
                    }
                    // If 'from' exists and 'to' does not
                    else
                    {
                        toPage.AddInLink(Pages[fromPageIndex]);
                        Pages.Add(toPage);
                        Pages[fromPageIndex].AddOutLink(Pages[^1]);
                    }
                }
                else
                {
                    // If 'to' exists and 'from' does not
                    if (FindPage(Pages, toPage) >= 0)
                    {
                        int toPageIdex = FindPage(Pages, toPage);
                        Pages.Add(fromPage);
                        fromPage.AddOutLink(Pages[toPageIdex]);
                        Pages[toPageIdex].AddInLink(Pages[^1]);
                    }
                    // If neither page exists
                    else
                    {
                        toPage.AddInLink(fromPage);
                        fromPage.AddOutLink(toPage);
                        Pages.Add(fromPage);
                        Pages.Add(toPage);
                    }
                }
            }
            // Closes the input File and returns the list of pages
            inputFile.Close();
            return Pages;
        }

        // Uses a selection sort method to sort through the pages based on names
        static List<Page> SortPages(List<Page> pageList)
        {
            int min;
            for (int ii = 0; ii < pageList.Count - 1; ii++)
            {
                min = ii;
                for (int jj = ii + 1; jj < pageList.Count; jj++)
                {
                    if (String.Compare(pageList[min].GetName(), pageList[jj].GetName()) > 0)
                    {
                        min = jj;
                    }
                }
                // Swaps the minimum page and current page
                Page temp = pageList[min];
                pageList[min] = pageList[ii];
                pageList[ii] = temp;
            }
            return pageList;
        }

        static void PageRank(List<Page> pageList, int iterations)
        {
            // Set up ranks - initial page ranking
            for (int ii = 0; ii < pageList.Count; ii++)
            {
                pageList[ii].SetRank(1.0 / (double)pageList.Count);
            }

            // ALTERNATIVE to Matrix Multiplication
            //  Uses the inLinks and outLinks to correctly get the page ranks without the use of a sparse matrix
            double[] newRank = new double[pageList.Count];
            for (int power = 1; power < iterations; power++)
            {
                for (int ii = 0; ii < pageList.Count; ii++)
                {
                    newRank[ii] = 0.0;
                    for (int jj = 0; jj < pageList[ii].GetInLinks().Count; jj++)
                    {
                        newRank[ii] += (1.0 / (double)pageList[ii].GetInLinks(jj).GetOutLinks().Count) * pageList[ii].GetInLinks(jj).GetRank();
                    }
                }

                //At the end of each matrix multiplication, the ranks of each page are updated
                for (int ii = 0; ii < pageList.Count; ii++)
                {
                    pageList[ii].SetRank(newRank[ii]);
                }
            }
        }

        static void Main(string[] args)
        {
            //var path = @"C:\Users\markw\OneDrive\Documents\School Work\UF\Year 2\Summer 2019\CDA 3520\PA2\inputFile.txt";
            // Takes an inputFile from arrguments and reads the first line, expection two integers
            var inputFile = new System.IO.StreamReader(args[0]);
            string line = inputFile.ReadLine();
            string[] splitLines = line.Split(' ');
            // Initializes these to zero, in case the input is invalid
            int numberOfLines = 0;
            int powerIterations = 0;

            // Tries to convert the strings to integers, if it is invalid, the program will exit after an error message
            try
            {
                numberOfLines = Convert.ToInt32(splitLines[0]);
                powerIterations = Convert.ToInt32(splitLines[1]);
            }
            catch (Exception)
            {
                Console.WriteLine("Improper Input File Format on line 1: '{0}'", line);
                Console.Write("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }


            // Uses the Input function to get the rest of the pages, sorts all of the pages, and then ranks them
            List<Page> allPages = Input(args, inputFile, numberOfLines);
            allPages = SortPages(allPages);
            PageRank(allPages, powerIterations);

            // Prints out all of the ranks for each page in alphabetical order
            for (int ii = 0; ii < allPages.Count; ii++)
            {
                Console.WriteLine("{0} {1}", allPages[ii].GetName(), String.Format("{0:0.00}", allPages[ii].GetRank()));
            }

            // Exit message
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
