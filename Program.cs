using System;

namespace HCode21PracticeRound
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=========== HCode21PracticeRound BEGIN ===========");

            _ = new NaiveSolver("inputs/a_an_example.in.txt",  "a.out", ' ');
            _ = new NaiveSolver("inputs/b_basic.in.txt",       "b.out", ' ');
            _ = new NaiveSolver("inputs/c_coarse.in.txt",      "c.out", ' ');
            _ = new NaiveSolver("inputs/d_difficult.in.txt",   "d.out", ' ');
            _ = new NaiveSolver("inputs/e_elaborate.in.txt",   "e.out", ' ');

            Console.WriteLine("=========== HCode21PracticeRound END =============");
        }
    }
}
