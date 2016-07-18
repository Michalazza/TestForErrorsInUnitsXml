using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestForErrorsInUnitsXml
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlReaderAndWriter tester = new XmlReaderAndWriter();
            Console.WriteLine("1.Check All");
            Console.WriteLine("2.Check by packaging");
            Console.WriteLine("3.Check Individual");
            var errorCount = 0;
            StreamWriter writetext = new StreamWriter("write.txt");
            int x = int.Parse(Console.ReadLine());

            if (x == 1)
            {
                for (char one = 'A'; one <= 'Z'; one++)
                {
                    for (char two = 'A'; two <= 'Z'; two++)
                    {
                        for (char three = 'A'; three <= 'Z'; three++)
                        {
                            for (char four = 'A'; four <= 'Z'; four++)
                            {
                                for (char five = 'A'; five <= 'Z'; five++)
                                {
                                    for (char six = 'A'; six <= 'Z'; six++)
                                    {
                                        for (int count = 0; count < 1000; count++)
                                        {
                                            var ifValidSite = true;
                                            string countNumber ="";
                                            if(count < 10)
                                            {
                                                countNumber = "00" + count.ToString();
                                            }
                                            else if(count < 100)
                                            {
                                                countNumber = "0" + count.ToString();
                                            }
                                            else
                                            {
                                                countNumber = count.ToString();
                                            }
                                            string builder = one.ToString() + two + three + four + five + six + countNumber;
                                            string linkOne = "http://training.gov.au/TrainingComponentFiles/" + one + two + three + "/" + builder + "_R1.xml";
                                            string linkTwo = "http://training.gov.au/TrainingComponentFiles/" + one + two + three + "/" + builder + "_AssessmentRequirements_R1.xml";

                                            Console.WriteLine(builder);

                                            try
                                            {
                                                tester.CombiningXmlLinksData(linkOne, linkTwo);
                                            }
                                            catch (WebException)
                                            {
                                                ifValidSite = false;
                                            }
                                            if (ifValidSite)
                                            {
                                                try
                                                {
                                                    var XmlData = tester.AddXmlDataToUnit(linkOne, linkTwo);
                                                }
                                                catch (ArgumentException ex)
                                                {
                                                    writetext.WriteLine(builder + "  //---------" + ex.Message + "--------" + ex.InnerException.Message);
                                                    errorCount++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Console.WriteLine(errorCount.ToString());
                writetext.Close();
                Console.ReadKey();
            }
            else if (x == 2)
            {                
                var listOfPackages = new List<string>();
                var listOfUnitPackages = new List<string>();
                
                
                string[] packages = { "ICT", "SIRX" };
                string[] unitPackages = { "WHS", "NWK", "ICT", "PRG", "PMG" };
                for (var count = 0; count < packages.Length; count++)
                {
                    listOfPackages.Add(packages[count]);
                }

                for (var count = 0; count < unitPackages.Length; count++)
                {
                    listOfUnitPackages.Add(unitPackages[count]);
                }

                foreach (var one in listOfPackages)
                {
                    foreach (var two in listOfUnitPackages)
                    {
                        for (var count = 0; count < 1000; count++)
                        {
                            var ifValidSite = true;
                            string countNumber = "";
                            if (count < 10)
                            {
                                countNumber = "00" + count.ToString();
                            }
                            else if (count < 100)
                            {
                                countNumber = "0" + count.ToString();
                            }
                            else
                            {
                                countNumber = count.ToString();
                            }
                            string builder = one + two + countNumber;
                            string linkOne = "http://training.gov.au/TrainingComponentFiles/" + one + "/" + builder + "_R1.xml";
                            string linkTwo = "http://training.gov.au/TrainingComponentFiles/" + one + "/" + builder + "_AssessmentRequirements_R1.xml";

                            Console.Write(builder);

                            try
                            {
                                tester.CombiningXmlLinksData(linkOne, linkTwo);
                            }
                            catch (WebException)
                            {
                                ifValidSite = false;
                            }
                            if (ifValidSite)
                            {
                                try
                                {
                                    var XmlData = tester.AddXmlDataToUnit(linkOne, linkTwo);
                                    Console.Write("-Valid");                
                                }
                                catch (XmlException ex)
                                {
                                    writetext.WriteLine(builder + "  //---------" + ex.Message);
                                    errorCount++;
                                    Console.Write("-Invalid");
                                }
                            }
                            Console.WriteLine();
                        }
                    }
                }
                Console.WriteLine(errorCount.ToString());
                writetext.Close();
                Console.ReadKey();
            }
            else if(x == 3)
            {
                var ifValidSite = true;
                string builder = Console.ReadLine();
                string linkOne = "http://training.gov.au/TrainingComponentFiles/" + builder.Substring(0, 3) + "/" + builder + "_R1.xml";
                string linkTwo = "http://training.gov.au/TrainingComponentFiles/" + builder.Substring(0, 3) + "/" + builder + "_AssessmentRequirements_R1.xml";

                Console.Write(builder);

                try
                {
                    tester.CombiningXmlLinksData(linkOne, linkTwo);
                }
                catch (WebException)
                {
                    ifValidSite = false;
                }
                if (ifValidSite)
                {
                    try
                    {
                        var XmlData = tester.AddXmlDataToUnit(linkOne, linkTwo);
                        Console.Write("-Valid");
                    }
                    catch (XmlException ex)
                    {
                        writetext.WriteLine(builder + "  //---------" + ex.Message);
                        errorCount++;
                        Console.Write("-Invalid");
                    }
                }
                Console.WriteLine();
                writetext.Close();
            }
            else
            {
                writetext.Close();
            }
        }        
    }
}
