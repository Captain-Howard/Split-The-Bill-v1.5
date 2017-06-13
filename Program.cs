using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_Bill_Solution_v1
{
    public class Program
    {
        static void Main(string[] args)
        {

            /*
             *
             *   _________      .__  .__  __      __  .__             __________.__.__  .__           ____      .________
             *  /   _____/_____ |  | |__|/  |_  _/  |_|  |__   ____   \______   \__|  | |  |   ___  _/_   |     |   ____/
             *  \_____  \\____ \|  | |  \   __\ \   __\  |  \_/ __ \   |    |  _/  |  | |  |   \  \/ /|   |     |____  \ 
             *  /        \  |_> >  |_|  ||  |    |  | |   Y  \  ___/   |    |   \  |  |_|  |__  \   / |   |     /       \
             * /_______  /   __/|____/__||__|    |__| |___|  /\___  >  |______  /__|____/____/   \_/  |___| /\ /______  /
             *         \/|__|                              \/     \/          \/                            \/        \/ 
             *
             * 
             * Hello, and welcome to my program! I haven't done C# in over two years now, so I apologize for inevitably 
             * poor coding practices. I'll try my hardest however!
             * 
             * (Hope you don't mind random and useless comments)
             * 
             * Author:  Austin Howard
             * Date:    June 11, 2017
             * 
             * 
             * Notes:
             * ------------------------------------------
             * 1.5  - Store values into new text file
             * 
             *      - Base Directory for the file is:
             *      
             *                  C:\File_Reader\
             *      
             *      - Cleaning code
             *-------------------------------------------
             * 1.0  - Implementation of text file reading
             *      - Segregate values read in
             */

            Console.WriteLine("Split the Bill v1.5\n-------------------"); //Simple console title
            Console.WriteLine("\n\nWelcome User,\nPlease enter your file-name: ");
            var fileName = Console.ReadLine(); //Grabbing the file name from the user

            //Now going into the file reader section... here we go.
            string fileBase = @"C:\File_Reader\"; //Base directory for the file
            try
            {
                using (StreamReader reader = File.OpenText(fileBase + fileName))
                {
                    string s; //I wanted to read each line in
                    List<string> results = new List<string>(); //Result from the parse

                    while ((s = reader.ReadLine()) != null)
                    {
                        results.Add(s); //Reading values in and storing those into the list
                    }

                    string[] resultsArray = results.ToArray(); //Changing that list into the string array
                    OrganizeValues(resultsArray, (fileBase + fileName)); //Now this is where the magic happens
                }
            }
            catch (Exception ex) //Whoops... you screwed something up if you're in here.
            {
                Console.WriteLine(ex.ToString()); //Did you create the file correctly? Type it in correctly?
            }
            Console.WriteLine("Calculation Successful!\nHope you had a fun camping trip!"); //Fun message, because who doesn't like fun messages
            Console.Write("Press any key to continue..."); //Momentary latch so that we can at least see the messages on the screen
            Console.ReadKey(true); //Okay sorry, TECHNICALLY this is the latch... whatever.
        }
        private static void OrganizeValues(string[] array, string file_name) // Yay for classes that do things
        {
            //Create a new file
            file_name += ".out";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(file_name)){} //So in the event that something went wrong last time, this overwrites that bad file to make way for a new better one
            //Variable declaration
            int n; //Number of people participating in the camping trip
            int p; //Number of receipts/charges for a specific participant
            double tempTotal; //The total of the participant
            double total; //The total of the group
            for(int i = 0; i < array.Length; i++) //Now this was an interesting choice... would I go with a for loop if I tried this again? Probably not... Maybe a do while instead.
            {
                List<double> results = new List<double>(); //Ahh... the glorious results list
                total = 0.0; //Gotta re-initialize that value before we go again for some more values
                n = Int32.Parse(array[i]); //Changing that values into an integer [I should have used tryParse in hindsight...]
                if(n != 0) //Checking to see if that first value is a zero (EOF) well... not really, I look for NULL instead of 0. Hindsight tells me I should have done the do while with the EOF being a 0. This works for now.
                {
                    i++; //Incrementing inside of a for loop feels so wrong. This is to move me up to that next value for how many receipts a participant has
                    for (int ii = 0; ii < n; ii++) //So for the time that we still have participants in a group do the following
                    {
                        p = Int32.Parse(array[i]); //Check to see how many reciepts for the participant [Once again should have done tryParse]
                        i++; //Increment it again (there's gotta be a better way...)
                        tempTotal = 0.0; //Have to reset it for each participant
                        for (int iii = 0; iii < p; iii++) //For the time that each participant still has receipts left do the following
                        {
                            tempTotal += Convert.ToDouble(array[i]); //Convert that value over into double and add it into the participants complete receipt total
                            i++; //Increment the array so that we can see the next value for the double
                        }
                        results.Add(tempTotal); //Adds the participants total into the next slot of the results list
                        total += tempTotal; //Add that participants total to the overall total
                    }
                    tempTotal = total / n; //Now this is where I re-use some variables for other purposes. I figure out what the price per person is this time
                    double[] group = results.ToArray(); //Make an array of doubles from the list of results that was inputted earlier
                    for(int iv = 0; iv < group.Length; iv++) //That's right, all the for loops! We are now spitting those values out into the new text document
                    {
                        double t = tempTotal - group[iv]; //This is the offset of a person from the group (What you're wanting to find in the first place)
                        if (t < 0) //If it's a negative number (money should be collected from the group)
                        {
                            var abs = Math.Abs(t); //Take the absolute value
                            var value = Math.Round(abs, 3, MidpointRounding.AwayFromZero); //Some values had an odd rounding effect where the number would have to get rounded twice. (Ex.) Number would be 0.97 instead of 0.98 because of the rounding error)
                            var value1 = Math.Round(value, 2, MidpointRounding.AwayFromZero); //Finally I can implement the actual two decimal place rounded value
                            var final = String.Format("{0:0.00}", value1);
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(file_name, true))
                            {
                                file.WriteLine("($" + final + ")"); //Finally writing that value into the .out file using the outlined formatting 
                            }

                        }
                        else //In this case the value is clearly positive, so we get rid of the first line, but the rest stay the same
                        {
                            var value = Math.Round(t, 3, MidpointRounding.AwayFromZero);
                            var value1 = Math.Round(value, 2, MidpointRounding.AwayFromZero);
                            var final = String.Format("{0:0.00}", value1);
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(file_name, true))
                            {
                                file.WriteLine("$" + final); 
                            }
                        }
                    
                    }
                    i--; //This line corrects the automatic adding incrementing of the i value from the for loop... once again if I did it again, I think I would use a do while instead
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(file_name, true))
                    {

                        file.WriteLine("\n"); //Moving onto the next group I need to include that blank line in order to seperate them
                    }
                }
                
            }
        }
    }
}
