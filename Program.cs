using System;                      // Allows to type method names of members of the System namespace without typing the word System every time
using System.Collections.Generic; // Contains interfaces and classes that define generic collections
using System.Linq;               // It provides classes and interfaces that support queries that use Language-Integrated Query (LINQ) 
using System.Threading.Tasks;   // It provides types that simplify the work of writing concurrent and asynchronous code
using System.Windows.Forms;    // It contains classes for creating Windows-based applications that take full advantage of the rich user interface 

namespace Assignment5_Sudoku
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();                      // Enables visual styles for the application
            Application.SetCompatibleTextRenderingDefault(false); // Sets the application-wide default for the UseCompatibleTextRendering property defined on certain controls.
            Application.Run(new Form1());                        // Begins running a standard application message loop on the current thread.
        }
    }
}
