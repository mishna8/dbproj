
//using System;
//using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System;
using System.Security.Principal;
//using System.IO;
//using System.Linq.Expressions;
using System.Text.RegularExpressions;
//using System.Transactions;
//using System.Xml.Linq;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;


namespace PROJ
{
    class Program
    {
        //global variables:
        //server name
        public static string serverName = System.Environment.MachineName;
        public static string connectionString = $"Server={serverName},1433;Integrated Security=True;TrustServerCertificate=True;";


        //this is the user interface it will guide the user to all the commands to see all the functionallity of the system 
        static void Main(string[] args)
        {
            string DBNAME;
            //when setting up the user gives the DB name to allow multiple setups 

            Console.WriteLine("welcom ");
            Console.WriteLine($"sql server is: {serverName}");
            while (true)
            {
                Console.WriteLine("Enter a commands: [start, load, stats, search, view, data mine,  exit ] ");
                string command = Console.ReadLine();
                if (!string.IsNullOrEmpty(command))
                {
                    switch (command)
                    {
                        //calls the setup function to create a new db with all tables in the name of the user's choosing 
                        case "start":
                            {
                                Console.WriteLine("enter database name:");
                                DBNAME = Console.ReadLine();
                                if (!string.IsNullOrEmpty(DBNAME))
                                {
                                    //will call the function only if the chosen name is approved 
                                    if (!DatabaseExists(DBNAME)) Setup(DBNAME); 
                                    else Console.WriteLine("database exists, try again");
                                }
                                else Console.WriteLine("Invalid input. Please provide a non-empty database name.");

                                break;
                            }

                        //calls the load function to populate all the tables in the desired database from the given dorectory 
                        case "load":
                            {
                                Console.WriteLine("enter the database:");
                                DBNAME = Console.ReadLine();
                                Console.WriteLine("enter directory path:");
                                string DirPath = Console.ReadLine();                               
                                if (!string.IsNullOrEmpty(DBNAME) && !string.IsNullOrEmpty(DirPath))
                                {
                                    //will call the function only if the chosen database exists to be used 
                                    if (DatabaseExists(DBNAME)) 
                                    { 
                                        loadFiles(DBNAME, DirPath);                                     
                                    }
                                    else Console.WriteLine("database doesnt exists, please enter 'start'");         
                                }
                                else Console.WriteLine("Invalid input. Please provide a non-empty directory or database name.");

                                break;
                            }

                        //calls the stats function, the user will be guided from the function 
                        case "stats":
                            {
                                stats();
                                break;
                            }

                        //this functionallity will give all option to serch items 
                        case "search":
                            {
                                Console.WriteLine("enter what: [file, word, group, expression ]");
                                string input = Console.ReadLine();
                                if (!string.IsNullOrEmpty(input))
                                {
                                    switch (input)
                                    {
                                        //first search for files , by the word value and by the metadata value
                                        case "file":
                                            {
                                                List<string> printlist;
                                                Console.WriteLine("Enter a methide: [word, metadata]");
                                                string minput = Console.ReadLine();
                                                if (!string.IsNullOrEmpty(minput))
                                                {
                                                    switch (minput)
                                                    {
                                                        case "word":
                                                            {
                                                                // Get user input 
                                                                Console.WriteLine("Enter a word:");
                                                                string word = Console.ReadLine();
                                                                if (!string.IsNullOrEmpty(word))
                                                                {
                                                                    //get the result list
                                                                    printlist = FindFileByWord(word);
                                                                    print(printlist);
                                                                }
                                                                else Console.WriteLine("Invalid input. Please provide a non-empty parameters ");
                                                                break;
                                                            }
                                                        case "metadata":
                                                            {
                                                                // Get user input 
                                                                Console.WriteLine("Enter a metadata type number:[any=0,Patient=1,Doctor=2,Diagnosis=3,Treatment=4]");
                                                                string MTD =  Console.ReadLine();
                                                                Console.WriteLine("Enter a word:");
                                                                string word = Console.ReadLine();
                                                                if (!string.IsNullOrEmpty(MTD) && !string.IsNullOrEmpty(word))
                                                                {
                                                                    int MTDNum = Convert.ToInt32(MTD);
                                                                    //get the result list
                                                                    printlist = FindFileByMTD(MTDNum, word);
                                                                    print(printlist);
                                                                }
                                                                else Console.WriteLine("Invalid input. Please provide a non-empty parameters ");
                                                                break;
                                                            }
                                                        default:
                                                            {
                                                                // Code to execute when command doesn't match any case
                                                                Console.WriteLine("Command is not recognized.");
                                                                break;
                                                            }
                                                    }
                                                }
                                                else Console.WriteLine("Invalid input. Please provide a non-empty command ");
                                                break;
                                            }
                                        // second search for words by file, by two indexes
                                        case "word":
                                            {
                                                List<string> printlist;
                                                //the function will guid the user to filter the list
                                                printlist = ListWords();
                                                print(printlist);                                               
                                                break;
                                            }

                                        // third search by group , allows to create a new group or print an existing group's members
                                        case "group":
                                            {
                                                Console.WriteLine("enter command: [add, list ]");
                                                string ginput = Console.ReadLine();
                                                if (!string.IsNullOrEmpty(ginput))
                                                {
                                                    switch (ginput)
                                                    {
                                                        case "add":
                                                            {
                                                                // Get user input for a list of words as a single string
                                                                Console.WriteLine("Enter a list of words separated by spaces:");
                                                                string userInput = Console.ReadLine();

                                                                // Get user input for the group name
                                                                Console.WriteLine("Enter a group name:");
                                                                string groupName = Console.ReadLine();

                                                                if (!string.IsNullOrEmpty(userInput) && !string.IsNullOrEmpty(groupName))
                                                                {
                                                                    // Split the user input into individual words
                                                                    List<string> wordList = userInput.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                                                                    // Call the GroupCreate function with the word list and group name
                                                                    GroupCreate(wordList, groupName);
                                                                }
                                                                else Console.WriteLine("Invalid input. Please provide a non-empty parameters ");
                                                                break;
                                                            }
                                                        case "list":
                                                            {
                                                                List<string> printlist;
                                                                // Get user input for the group name
                                                                Console.WriteLine("Enter a group name:");
                                                                string groupName = Console.ReadLine();
                                                                if (!string.IsNullOrEmpty(groupName))
                                                                {
                                                                    //get the result list
                                                                    printlist = GroupSearch(groupName);
                                                                    print(printlist);
                                                                }
                                                                else Console.WriteLine("Invalid input. Please provide a non-empty parameters ");
                                                                break;
                                                            }
                                                        default:
                                                            {
                                                                // Code to execute when command doesn't match any case
                                                                Console.WriteLine("Command is not recognized.");
                                                                break;
                                                            }
                                                    }
                                                }
                                                else Console.WriteLine("Invalid input. Please provide a non-empty command ");

                                                break;
                                            }
                                        //fourth search by expression
                                        case "expression":
                                            {
                                                List<string> printlist;
                                                // Get user input for the group name
                                                Console.WriteLine("Enter an expression:");
                                                string exprs = Console.ReadLine();
                                                if (!string.IsNullOrEmpty(exprs))
                                                {
                                                    //get the result list
                                                    //the list here is the index list of the starting point of the expression
                                                    //this function will handle the creation of new expressions as well
                                                    printlist = returnExpression(exprs);
                                                    print(printlist);
                                                }
                                                else Console.WriteLine("Invalid input. Please provide a non-empty parameters ");
                                                break;
                                            }
                                        default:
                                            {
                                                // Code to execute when command doesn't match any case
                                                Console.WriteLine("Command is not recognized.");
                                                break;
                                            }
                                    }
                                }
                                else Console.WriteLine("Invalid input. Please provide a non-empty command ");
                                break;
                            }

                        //this command will show a 3 sentence context of a chosen word 
                        case "view":
                            {
                                Console.WriteLine("Enter a word to view:");
                                string word = Console.ReadLine();
                                if (!string.IsNullOrEmpty(word))
                                {
                                    //the function will handle the specific location and printing
                                    view(word);
                                }
                                else Console.WriteLine("Invalid input. Please provide a non-empty parameters ");
                                break;
                            }
                        //apriory data mine on the tables , will calculate and print the results 
                        case "data mine":
                            {
                                ExecuteAprioriOnMetaData();
                                break;
                            }

                        default:
                            {
                                // Code to execute when command doesn't match any case
                                Console.WriteLine("Command is not recognized.");
                                break;
                            }

                        case "exit":
                            {
                                Console.WriteLine("Exiting the program.");
                                return; // Exit the program
                            }

                    }
                }
                else Console.WriteLine("Invalid input. Please provide a non-empty command ");
            }
        }

        //function to print result lists 
        static void print(List<string> printlist)
        {
            // Check if the returned list is not null and not empty
            if (printlist != null && printlist.Count > 0)
            {
                // print the populated printlist list
                foreach (string item in printlist)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                Console.WriteLine("No matching data found.");
            }
        }


        ///----------------------------------------------------------------------------------------------------/

        //function that returnes true or false if database already exists in the sql instance
        static bool DatabaseExists(string dbName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if entry exists in the master table
                    // BY USING A COUNT QUERY AND SEE IF THE NUMBER IS 0 OR 1 
                    string CheckQuery = $"SELECT COUNT(*) FROM {"sys.databases"} WHERE {"name"} = @name";
                    SqlCommand CheckCommand = new SqlCommand(CheckQuery, connection);
                    CheckCommand.Parameters.AddWithValue("@name", dbName);
                    int Count = (int)CheckCommand.ExecuteScalar();
                    return Count != 0;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking database existence: " + ex.Message);
                return false;
            }
        }

        ///creates the databse for the first time with all tables
        static void Setup(string databaseName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    //the database itself creation 
                    string createDatabaseQuery = $"CREATE DATABASE {databaseName}";
                    SqlCommand createDatabaseCommand = new SqlCommand(createDatabaseQuery, connection);
                    createDatabaseCommand.ExecuteNonQuery();

                    Console.WriteLine("Database created successfully!");

                    connection.ChangeDatabase(databaseName);

                    ///1
                    //create the FILES table
                    //THE COLUMS:  FileName, FilePath, WordCount, LineCount, ParagCount
                    string createFilesTableQuery = "CREATE TABLE [Files] ([FileName] NVARCHAR(MAX), [FilePath] NVARCHAR(MAX), [WordCount] INT, [LineCount] INT, [ParagCount] INT)";
                    SqlCommand createFilesTableCommand = new SqlCommand(createFilesTableQuery, connection);
                    createFilesTableCommand.ExecuteNonQuery();

                    Console.WriteLine("Files table created successfully!");

                    ///2
                    //create the METADATA id by file table
                    //THE COLUMS:  FileName, 
                    //Patient, Doctor, Diag, Treat as id numbers of the metadata found
                    string createMTDTableQuery = "CREATE TABLE [MetaData] ([FileName] NVARCHAR(MAX), [Patient] NVARCHAR(MAX), [Doctor] NVARCHAR(MAX), [Diag] NVARCHAR(MAX), [Treat] NVARCHAR(MAX))";
                    SqlCommand createMTDTableCommand = new SqlCommand(createMTDTableQuery, connection);
                    createMTDTableCommand.ExecuteNonQuery();

                    Console.WriteLine("MetaData table created successfully!");

                    ///3
                    //create the matadata values table 
                    //columns: ID , value 
                    string createMTDregTableQuery = "CREATE TABLE [MTDREG] ([ID] INT, [value] NVARCHAR(MAX))";
                    SqlCommand createMTDregTableCommand = new SqlCommand(createMTDregTableQuery, connection);
                    createMTDregTableCommand.ExecuteNonQuery();

                    Console.WriteLine("MetaData Values table created successfully!");

                    ///4
                    //create the Content table
                    //THE COLUMS:  WordId,WordValue, File, charCount, paragNum , lineInParagNum, lineNum, charInLineNum, wordInLineNum, Exprs
                    string createWORDTableQuery = $"CREATE TABLE [Content] ([WordId] INT, [WordValue] NVARCHAR(MAX), [File] NVARCHAR(MAX), [charCount] INT, [paragNum] INT, [lineInParagNum] INT, [lineNum] INT, [charInLineNum] INT, [wordInLineNum] INT)";
                    SqlCommand createWORDTableCommand = new SqlCommand(createWORDTableQuery, connection);
                    createWORDTableCommand.ExecuteNonQuery();

                    Console.WriteLine("Content table created successfully!");

                    ///5
                    //create the Tags table
                    //THE COLUMS:  Group, Word
                    string creatTAGTableQuery = "CREATE TABLE [Tags] ([Group] NVARCHAR(MAX), [Word] NVARCHAR(MAX))";
                    SqlCommand createTAGTableCommand = new SqlCommand(creatTAGTableQuery, connection);
                    createTAGTableCommand.ExecuteNonQuery();

                    Console.WriteLine("Tags table created successfully!");

                    ///6
                    //create the Expression table
                    //THE COLUMS:  Sentence, ID
                    string creatExprsTableQuery = "CREATE TABLE [Expression] ([Sentence] NVARCHAR(MAX), [ID] INT)";
                    SqlCommand createExprsTableCommand = new SqlCommand(creatExprsTableQuery, connection);
                    createExprsTableCommand.ExecuteNonQuery();

                    Console.WriteLine("Expression table created successfully!");

                    ///7
                    //the expression location storage table 
                    //THE COLUMNS: file , lineNum, wordNum, ID                  
                    string creatExprslocationTableQuery = "CREATE TABLE [PhraseLocation] ([file] NVARCHAR(MAX), [lineNum] INT, [wordNum] INT, [ID] INT)";
                    SqlCommand createExprslocationTableCommand = new SqlCommand(creatExprslocationTableQuery, connection);
                    createExprslocationTableCommand.ExecuteNonQuery();

                    Console.WriteLine("Expression location table created successfully!");
                }
                catch (Exception ex)
                {                    
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }



        ///_______________________________________________________________________________________/

        ///this function will receive the desired database and a files directory to load files from to the table 
        //my test envoironment : C:\Users\me\source\test
        static void loadFiles(string databaseName, string directoryPath)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    connection.ChangeDatabase(databaseName);

                    Console.WriteLine("starting....");

                    //get a list of all txt files in the given directory
                    string[] fileEntries = Directory.GetFiles(directoryPath, "*.txt");

                    foreach (string filePath in fileEntries)
                    {
                        // Check if file entry exists in the files table
                        // BY USING A COUNT QUERY AND SEE IF THE NUMBER OF FILE ARE 0 OR 1 
                        string fileCheckQuery = $"SELECT COUNT(*) FROM {"Files"} WHERE {"FilePath"} = @Path";
                        SqlCommand fileCheckCommand = new SqlCommand(fileCheckQuery, connection);
                        fileCheckCommand.Parameters.AddWithValue("@Path", filePath);
                        int fileCount = (int)fileCheckCommand.ExecuteScalar();

                        /*if the file doesnt exists we must 
                        1. create a file properties entry in file table 
                        2. load all words in the content table
                        3. load all new metadata in the registery and for each file
                        4. updata the expression locations
                         */
                        if (fileCount == 0)
                        {
                            // handle files properties instert
                            // start getting the properties 
                            Console.WriteLine("adding the file....");

                            //name
                            string fileName = Path.GetFileName(filePath);
                            // get file stats  
                            int paragraphCount, lineCount, wordCount;
                            CountFileStats(filePath, out paragraphCount, out lineCount, out wordCount);
                            // the query 
                            string insertFileQuery = $"INSERT INTO Files (FileName, FilePath, WordCount, LineCount, ParagCount )" +
                                                      $"VALUES (@fileName, @filePath, @wordCount, @lineCount, @paragraphCount)";
                            using (SqlCommand insertFileCommand = new SqlCommand(insertFileQuery, connection))
                            {
                                insertFileCommand.Parameters.AddWithValue("@fileName", fileName);
                                insertFileCommand.Parameters.AddWithValue("@filePath", filePath);
                                insertFileCommand.Parameters.AddWithValue("@wordCount", wordCount);
                                insertFileCommand.Parameters.AddWithValue("@lineCount", lineCount);
                                insertFileCommand.Parameters.AddWithValue("@paragraphCount", paragraphCount);                 
                                insertFileCommand.ExecuteNonQuery();
                            }

                            //handle metadata mapping entry for the file 
                            string insertMTDQuery = $"INSERT INTO MetaData (FileName, Patient, Doctor ,Diag ,Treat )" +
                                                      $"VALUES (@fileName, 0,0,0,0)";
                            using (SqlCommand insertMTDCommand = new SqlCommand(insertMTDQuery, connection))
                            {
                                insertMTDCommand.Parameters.AddWithValue("@fileName", fileName);
                                insertMTDCommand.ExecuteNonQuery();
                            }
                            //handle the word and metadata inserts & mapping
                            wordInsert(filePath);
                        }
                    }
                    //after the new words have been added need to update the locations of saved expressions in the new file
                    updateExpression(); 
                    Console.WriteLine("Entries added successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        //reads the file to extract overall stats for the files table
        static void CountFileStats(string filePath, out int paragraphCount, out int lineCount, out int wordCount)
        {
            string content = File.ReadAllText(filePath);

            paragraphCount = Regex.Split(content, @"\r\n\r\n|\n\n").Length;
            lineCount = content.Split('\n').Length;
            wordCount = content.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        //the function is called to read the words of the file and populate the other tables with its content 
        public static void wordInsert(string filePath)
        {
            Console.WriteLine("reading the words....");
            // Initialize counters
            int paragNum = 0;
            int lineNum = 0;
            int lineInParagNum = 0;
            int charInLineNum = 0;
            int wordInLineNum = 0;
            int charInWordNum = 0;


            // Open the file for reading
            using (StreamReader reader = new StreamReader(filePath))
            {
                bool mtd =false; //a flag to remind that the next word is a metadata value that should be handeled
                string mtd_type = "";
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Increment counters
                    //when we get to each new line in total file the line counter increase
                    lineNum++;
                    // when we get to each new line in the current paragraph the counter increase
                    lineInParagNum++;
                    //the word count in the new line will reset to start counting
                    wordInLineNum = 0;
                    //the char count in the new line will reset to start counting
                    charInLineNum = 0;

                    // Check for paragraph break (empty line)
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        //when we get to new paragraph in total file the paragraph counter increase
                        paragNum++;
                        //and the line count in the new paragraphe will reset to start counting
                        lineInParagNum = 0;
                        continue;
                    }

                    // Split the line into words
                    string[] words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in words)
                    {
                        //in the previous word the mtd flag was set so in the current word is sent to be added as metadata
                        //we assume that the files are correct and there is no metadata on teh first word, in first iteration the flag is false 
                        if (mtd) MTDHandler(word, mtd_type, filePath);

                        //get the word's charecter number (length)
                        charInWordNum = word.Length;
                        //when we get to new word in the current line the counter increase
                        wordInLineNum++;

                        // Insert word data into the "content" table
                        InsertWordData(word, filePath, charInWordNum, paragNum, lineInParagNum, lineNum, charInLineNum, wordInLineNum);

                        // Check for special words and call the corresponding function                   
                        if(word == "patient" || word == "doctor" || word == "diagnosis" || word == "treatment")
                        {
                            mtd = true; //set the flag fo the next word
                            mtd_type = word; //hold the word 

                        }
                                               
                        //after the new word the charecter in line count will prepare to the next word
                        //counter wil move up by the number of charecters in the word we past plus one for the space between them;
                        charInLineNum += charInWordNum + 1;
                    }
                }
            }
        }

        private static void InsertWordData(string word, string filePath, int charInWordNum, int paragNum, int lineInParagNum, int lineNum, int charInLineNum, int wordInLineNum)
        {
            // Create a SqlConnection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();
                Console.WriteLine("inserting the words....");
                //create new id for the word
                int MaxID = 0;
                string GetMaxID = $"SELECT MAX(WordId) FROM Content";
                using (SqlCommand IDcommand = new SqlCommand(GetMaxID, connection))
                {
                    var result = IDcommand.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        MaxID = Convert.ToInt32(result);
                    }
                }
                //prep the new ID 
                int num = MaxID + 1;

                // Define your SQL query to insert data into the "content" table
                string insertQuery = $"INSERT INTO Content (WordId, WordValue, [File], charCount, paragNum, lineInParagNum, lineNum, charInLineNum, wordInLineNum ) " +
                    $"VALUES (@WordId, @WordValue, @File, @charCount, @paragNum, @lineInParagNum, @lineNum, @charInLineNum, @wordInLineNum)";
                //string insertFileQuery = $"INSERT INTO Files (FileName, FilePath, WordCount, LineCount, ParagCount )" +
                  //                                    $"VALUES (@fileName, @filePath, @wordCount, @lineCount, @paragraphCount)";
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    // Set parameter values
                    command.Parameters.AddWithValue("@WordId", num);
                    command.Parameters.AddWithValue("@WordValue", word);
                    command.Parameters.AddWithValue("@File", filePath);
                    command.Parameters.AddWithValue("@charCount", charInWordNum);
                    command.Parameters.AddWithValue("@paragNum", paragNum);
                    command.Parameters.AddWithValue("@lineInParagNum", lineInParagNum);
                    command.Parameters.AddWithValue("@lineNum", lineNum);
                    command.Parameters.AddWithValue("@charInLineNum", charInLineNum);
                    command.Parameters.AddWithValue("@wordInLineNum", wordInLineNum);

                    // Execute the insert query
                    command.ExecuteNonQuery();
                }

                // Close the connection
                connection.Close();
            }
        }


        //this function inserts the id of the metadata it got to the correct column in the table
        public static void MTDHandler(string word, string type, string fileName)
        {
            Console.WriteLine("reading special characters....");
            // Get the WordId using the GetOrCreateEntryId function
            int wordId = GetOrCreateEntryId(word);


            // Update the specified column in the table
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string updateQuery = $"UPDATE MetaData SET {type} = @wordId WHERE FileName = @fileName";

                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@wordId", wordId);
                    command.Parameters.AddWithValue("@fileName", fileName);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        //this function gets the value and metadata type as number and return the id of the metadata combo (existing or create new)
        public static int GetOrCreateEntryId(string word)
        {
            int entryId = -1; // Default value if not found

            // Create a SqlConnection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();
                // Create a SqlCommand to check if the combination exists
                string checkQuery = "SELECT id FROM MTDREG WHERE value = @word ";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@word", word);

                    // Execute the query
                    using (SqlDataReader reader = checkCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Entry already exists, get the ID
                            entryId = reader.GetInt32(0);
                        }
                    }
                }

                // If entryId is still -1, it means the combination doesn't exist, so we insert a new entry
                if (entryId == -1)
                {
                    string insertQuery = "INSERT INTO MTDREG (value) VALUES (@word); SELECT SCOPE_IDENTITY();";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@word", word);

                        // Execute the insert query and get the newly generated ID
                        entryId = Convert.ToInt32(insertCommand.ExecuteScalar());
                    }
                }

                // Close the connection
                connection.Close();
            }

            return entryId;
        }


        ///--------------------------------------------------------------------------------------------///

        ///shows the user the statistics of each level they choose to view
        static void stats()
        {
            //first loop is file level with option to drill down to paragraph and line level
            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Select a File");
                Console.WriteLine("2. Exit");
                string option = Console.ReadLine();

                switch (option)
                {
                    //here user inputs the file and gets the stats 
                    case "1":
                        {
                            Console.WriteLine("Enter a file name:");
                            string fileName = Console.ReadLine();
                            //first check if file exists
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                int paragCounter = 0;
                                // Create a parameterized SQL command to check if the file exists
                                string query = "SELECT COUNT(*) FROM Files WHERE FileName = @fileName OR FilePath = @fileName";
                                using (SqlCommand command = new SqlCommand(query, connection))
                                {
                                    command.Parameters.AddWithValue("@fileName", fileName);

                                    int fileCount = Convert.ToInt32(command.ExecuteScalar());
                                    //if it does , find its stats and present
                                    if (fileCount > 0)
                                    {
                                        string countquery = "SELECT ParagCount, LineCount, WordCount FROM Files WHERE FileName = @fileName";
                                        using (SqlCommand countcommand = new SqlCommand(countquery, connection))
                                        {
                                            countcommand.Parameters.AddWithValue("@fileName", fileName);

                                            SqlDataReader reader = countcommand.ExecuteReader();
                                            if (reader.Read())
                                            {
                                                paragCounter = Convert.ToInt32(reader["ParagCount"]);
                                                int sentenceCounter = Convert.ToInt32(reader["LineCount"]);
                                                int wordCounter = Convert.ToInt32(reader["WordCount"]);

                                                Console.WriteLine($"paragraphs in {fileName}: {paragCounter}");
                                                Console.WriteLine($"Sentences in {fileName}: {sentenceCounter}");
                                                Console.WriteLine($"Words in {fileName}: {wordCounter}");
                                            }
                                        }
                                        //give the user option to drill down 
                                        Console.WriteLine("do you wish to continue? Y/N");
                                        string paragoption = Console.ReadLine();
                                        switch (paragoption)
                                        {
                                            case "Y":
                                                //go to a loop to view stats within the file
                                                drill(fileName);
                                                break;
                                            case "N": break; //if not return to the first loop
                                            default:
                                                Console.WriteLine("Invalid option. Please try again.");
                                                break;
                                        }
                                    }
                                    //if not print warning and return to loop
                                    else Console.WriteLine($"File '{fileName}' not found in the 'Files' table.");
                                }
                            }

                            break;
                        }
                    //here you exit the loop
                    case "2":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }


        static void drill(string fileName)
        {
            //you enter the second loop where you can drill down to see paragraph/line level
            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Select a paragraph");
                Console.WriteLine("2. select a sentence");
                Console.WriteLine("3. go back");
                string option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        //sends to function that will print the paragraph stats
                        Console.WriteLine("choose a paragaraph");
                        int parag = Convert.ToInt32(Console.ReadLine());
                        statsParagraphs(fileName, parag);
                        break;
                    case "2":
                        {
                            //sends to function that will print the line stats
                            Console.WriteLine("choose a line");
                            int line = Convert.ToInt32(Console.ReadLine());
                            statsline(fileName, line);
                            //give the user option to drill down 
                            Console.WriteLine("choose a word ? Y/N");
                            string wordoption = Console.ReadLine();
                            switch (wordoption)
                            {
                                case "Y":
                                    {
                                        Console.WriteLine("choose a word number in the sentence");
                                        int word = Convert.ToInt32(Console.ReadLine());
                                        //get the number of characters in the word 
                                        using (SqlConnection connection = new SqlConnection(connectionString))
                                        {
                                            connection.Open(); 
                                            //get charecter number
                                            //query the charNum in the entry of the word/line/file combo
                                            string wordquery = "SELECT WordValue, charCount FROM Content WHERE wordInLineNum = @word AND lineNum = @lineNum AND  File = @fileName ";
                                            using (SqlCommand wordcommand = new SqlCommand(wordquery, connection))
                                            {
                                                wordcommand.Parameters.AddWithValue("@word", word);
                                                wordcommand.Parameters.AddWithValue("@lineNum", line);
                                                wordcommand.Parameters.AddWithValue("@FileName", fileName);

                                                SqlDataReader reader = wordcommand.ExecuteReader();
                                                if (reader.Read())
                                                {
                                                    string value = reader["WordValue"].ToString();
                                                    int chars = Convert.ToInt32(reader["charCount"]);
                                                    Console.WriteLine($"Word number {word} is {value} and has {chars} charecters");
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case "N": break;
                                default:
                                    Console.WriteLine("Invalid option. Please try again.");
                                    break;
                            }
                            break;
                        }
                    case "3": return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        // Query to retrieve paragraph stats for the selected file and given paragraoh number
        static void statsParagraphs(string fileName, int num)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int sentenceCounter = 0; int wordCounter = 0;
                //get sentence number
                //count the unique line numbers in the given paragraph nunmer of the file
                //or get the max line in paragraph to indicate how many line there is 
                string linequery = "SELECT MAX(lineInParagNum) AS lineCount FROM Content WHERE paragNum = @num AND File = @fileName  ";
                using (SqlCommand linecommand = new SqlCommand(linequery, connection))
                {
                    linecommand.Parameters.AddWithValue("@num", num);
                    linecommand.Parameters.AddWithValue("@FileName", fileName);

                    SqlDataReader reader = linecommand.ExecuteReader();
                    if (reader.Read())
                    {
                        sentenceCounter = Convert.ToInt32(reader["lineCount"]);
                    }
                }

                //get word number
                //query to sum the last word number in each sentence in the given paragraph nunmer of the file
                string wordquery = "SELECT SUM(MaxWordInLineNum) AS wordCount" +
                    "    FROM (SELECT MAX(wordInLineNum) AS MaxWordInLineNum" +
                    "        FROM Content  WHERE paragNum = @num AND File = @fileName GROUP BY lineNum" +
                    "    ) AS Subquery ";
                using (SqlCommand wordcommand = new SqlCommand(wordquery, connection))
                {
                    wordcommand.Parameters.AddWithValue("@num", num);
                    wordcommand.Parameters.AddWithValue("@FileName", fileName);

                    SqlDataReader reader = wordcommand.ExecuteReader();
                    if (reader.Read())
                    {
                        wordCounter = Convert.ToInt32(reader["wordCount"]);
                    }
                }

                //present
                Console.WriteLine($"paragraph number is : {num}");
                Console.WriteLine($"Sentences : {sentenceCounter}");
                Console.WriteLine($"Words : {wordCounter}");
            }
        }
        // Query to retrieve line stats for the selected file and given line number
        static void statsline(string fileName, int num)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int lineCounter = 0;
                //get word number
                //query the last word number in the sentence in the given file
                string wordquery = "SELECT MAX(wordInLineNum) AS LineCount FROM Content WHERE lineNum = @num AND File = @fileName ";
                using (SqlCommand wordcommand = new SqlCommand(wordquery, connection))
                {
                    wordcommand.Parameters.AddWithValue("@num", num);
                    wordcommand.Parameters.AddWithValue("@FileName", fileName);

                    SqlDataReader reader = wordcommand.ExecuteReader();
                    if (reader.Read())
                    {
                        lineCounter = Convert.ToInt32(reader["LineCount"]);
                    }
                }

                //present
                Console.WriteLine($"line  number is : {num}");
                Console.WriteLine($"Words : {lineCounter}");
            }
        }

        ///--------------------------------------------------------------------/

        ///outputs files that contain the inputed word
        static List<string> FindFileByWord(string word)
        {
            //the list of files 
            List<string> fileList = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query to search for records containing the specified word
                string query = $"SELECT File FROM Content WHERE WordValue = @word ";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@word", word);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string fileName = reader["File"].ToString();
                            //if the file we got is not null and not already in the list 
                            if (!string.IsNullOrEmpty(fileName) && !fileList.Contains(fileName))
                            {
                                fileList.Add(fileName);
                                Console.WriteLine($"Found file: {fileName}");
                            }
                        }
                    }
                }
            }
            return fileList;
        }

        ///outputs files that contain the inputed word as a specified metadata value 
        static List<string> FindFileByMTD(int number, string word)
        {
            //the list of files 
            List<string> fileList = new List<string>();

            int entryId = -1; // Default value if not found

            // Create a SqlConnection
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Open the connection
                connection.Open();

                // Create a SqlCommand to check if the combination exists
                string checkQuery = "SELECT id FROM MTDREG WHERE value = @word";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@word", word);

                    // Execute the query
                    using (SqlDataReader reader = checkCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Entry already exists, get the ID
                            entryId = reader.GetInt32(0);
                        }
                    }
                }

                // If entryId is still -1, it means the combination doesn't exist, so we insert a new entry
                if (entryId == -1)
                {
                    Console.WriteLine($"This metadata was not found");
                    return fileList;
                }

                //if the combination was found find the file according to the ID found
                //get the column
                string columnName = "";
                switch (number)
                {
                    case 1:
                        columnName = "Patient";
                        break;
                    case 2:
                        columnName = "Doctor";
                        break;
                    case 3:
                        columnName = "Diag";
                        break;
                    case 4:
                        columnName = "Treat";
                        break;
                    case 0:
                        columnName = "ALL";
                        break;
                    default:
                        throw new ArgumentException("Invalid number.");
                }

                //find files matching the entryId in the specified column
                string query;
                if (columnName == "ALL")
                {
                    query = $"SELECT FileName FROM MetaData WHERE {"Patient"} = @entryId OR {"Doctor"} = @entryId OR {"Diag"} = @entryId OR {"Treat"} = @entryId";
                }
                else  query = $"SELECT FileName FROM MetaData WHERE {columnName} = @entryId";
                using (SqlCommand fileQueryCommand = new SqlCommand(query, connection))
                    {
                        fileQueryCommand.Parameters.AddWithValue("@entryId", entryId);

                        using (SqlDataReader reader = fileQueryCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string fileName = reader["FileName"].ToString();
                                //if the file we got is not null and not already in the list 
                                if (!string.IsNullOrEmpty(fileName) && !fileList.Contains(fileName))
                                {
                                    fileList.Add(fileName);
                                    Console.WriteLine($"Found file: {fileName}");
                                }
                            }
                        }
                    }               
            }
            return fileList;
        }

        ///-----------------------------------------------------------------------------------/

        ///outputs all words according to filters(few or none)    
        static List<string> ListWords()
        {
            //the list of words
            List<string> resultList = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //as defualt the function will query all the words to list 
                string baseQuery = $"SELECT WordValue FROM Content ";

                //find out what filters can be used 
                //get a string list that holds parts of query according to the user's filters to the words
                List<string> filters = new List<string>();
                
                //file filter
                Console.WriteLine("do you wish to filter by file? Y/N");
                string Foption = Console.ReadLine();
                switch (Foption)
                {
                    case "Y":
                        //get the file and add to the query
                        Console.WriteLine("Enter a file:");
                        string file = Console.ReadLine();
                        if (!string.IsNullOrEmpty(file))
                        {
                            filters.Add($"File = {file} ");
                        }
                        else Console.WriteLine("Invalid input. Please provide a non-empty parameters, will be skipped ");
                        break;
                    case "N": break; //if not skip                   
                }

                //choose filter 
                Console.WriteLine("do you wish to filter by index? [1. by line 2. by paragraph 3.no]");
                string index = Console.ReadLine();
                switch (index)
                {
                    case "1":
                        //get the FIRST INDEX (LINE & CHARECTER) and add to the query
                        Console.WriteLine("Enter an sentence number in file:");
                        int x;
                        if (!int.TryParse(Console.ReadLine(), out x)) x = -1;
                        Console.WriteLine("Enter a character number in sentence:");
                        int y;
                        if (!int.TryParse(Console.ReadLine(), out y)) y = -1;
                        // must have both indexes in order to filter the file for the correct word
                        if (x != -1 && y != -1)
                        {
                            filters.Add($"lineNum = {x} AND charInLineNum = {y}");
                        }
                        else Console.WriteLine("Invalid input. Please provide a non-empty parameters, will be skipped ");
                        break;
                    case "2":
                        //get the SECOND INDEX (PARAG & PARAG LINE) and add to the query
                        Console.WriteLine("Enter a paragraph number in file:");
                        int a;
                        if (!int.TryParse(Console.ReadLine(), out a)) a = -1;
                        Console.WriteLine("Enter a sentence number in paragraph:");
                        int b;
                        if (!int.TryParse(Console.ReadLine(), out b)) b = -1;
                        // must have both indexes in order to filter the file for the correct word

                        if (a != -1 && b != -1)
                        {
                            filters.Add($"paragNum = {a} AND lineInParagNum = {b}");
                        }
                        else Console.WriteLine("Invalid input. Please provide a non-empty parameters, will be skipped ");
                        break;
                    case "3":
                        break;
                    default:
                        Console.WriteLine("Invalid option. will be skiped.");
                        break;
                }
                              
                //build the query from the filters 
                string addon = string.Join(" AND ", filters);

                if (!string.IsNullOrEmpty(addon))
                {
                    baseQuery += $"WHERE " + addon;
                }

                //execute the query 
                using (SqlCommand command = new SqlCommand(baseQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string result = reader["WordValue"].ToString();
                            resultList.Add(result);
                        }
                    }
                }
            }

            return resultList;
        }

        ///outputs the 3 sentences surrounding the given word
        static void view(string givenWord)
        {

            List<string> wordsInRange = new List<string>();
            //get the location of the word 
            Console.WriteLine("Enter a file:");
            string file = Console.ReadLine();

            if (!string.IsNullOrEmpty(file))
            {
                //find all sentence numbers that has the given word in it 
                int x = getLine(givenWord, file);
                if (x != -1) 
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        // Retrieve words within the specified sentence range from the same file
                        string selectQuery = "SELECT WordValue FROM Content " +
                                         "WHERE File = @file " +
                                         "AND lineNum BETWEEN @minSentence AND @maxSentence " +
                                         "ORDER BY lineNum, wordInLineNum";

                        int sentenceRange = 3;
                        int minSentence = Math.Max(1, x - sentenceRange);
                        int maxSentence = x + sentenceRange;

                        using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                        {
                            selectCommand.Parameters.AddWithValue("@file", file);
                            selectCommand.Parameters.AddWithValue("@minSentence", minSentence);
                            selectCommand.Parameters.AddWithValue("@maxSentence", maxSentence);

                            SqlDataReader wordReader = selectCommand.ExecuteReader();
                            while (wordReader.Read())
                            {
                                string word = wordReader["WordValue"].ToString();
                                wordsInRange.Add(word);
                            }

                        }
                    }
                }

            }
            else Console.WriteLine("Invalid input. Please provide a non-empty parameters ");

            //print the list as a peragraph to show the part where the word was 
            Console.WriteLine(string.Join(" ", wordsInRange));
        }
        //gets the word and file and print all line numbers that has this word 
        //asks user to pick a line and reeturnes this line to view the context of this word
        static int getLine(string givenWord, string file)
        {
            List<string> lineList = new List<string>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                // Retrieve words within the specified sentence range from the same file
                string selectQuery = "SELECT lineNum FROM Content WHERE File = @file AND WordValue = @givenword";
                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@file", file);
                    selectCommand.Parameters.AddWithValue("@givenword", givenWord);

                    SqlDataReader wordReader = selectCommand.ExecuteReader();
                    while (wordReader.Read())
                    {
                        string lineNum = wordReader["lineNum"].ToString();
                        lineList.Add(lineNum);
                    }
                }
            }
            print(lineList);
            Console.WriteLine("please choose a line number to view: [or type -1]");
            string num = Console.ReadLine();
            if (!string.IsNullOrEmpty(num))
            {
                int n = Convert.ToInt32(num);
                return n;
            }
            return -1;
        }
        ///--------------------------------------------------------------------------------------------///
        
        /// gets a list of words and a group name and records all those ords as in this group
        static void GroupCreate(List<string> wordList, string groupName)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (string word in wordList)
                {
                    // Check if the word-group pair already exists
                    string checkQuery = $"SELECT COUNT(*) FROM Tags WHERE Word = @Word AND Group = @GroupName";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Word", word);
                        checkCommand.Parameters.AddWithValue("@GroupName", groupName);

                        int count = (int)checkCommand.ExecuteScalar();

                        //if theres no existing combo
                        if (count == 0)
                        {
                            // Add the word to the group
                            string insertQuery = $"INSERT INTO Tags (Word, Group) VALUES (@Word, @GroupName)";
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@Word", word);
                                insertCommand.Parameters.AddWithValue("@GroupName", groupName);

                                insertCommand.ExecuteNonQuery();
                            }
                            Console.WriteLine($"Value '{groupName}' updated.");
                        }
                    }
                }
            }
        }

        ///gets a group and returns all words in this group with thier location (files and indexes)
        static List<string> GroupSearch(string group)
        {
            //the list of the general words 
            List<string> wordsInGroup = new List<string>();
            //the list of the words with thier locations
            List<string> wordslocations = new List<string>();

            //find the generakl word list from the tags table
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = $"SELECT Word FROM Tags WHERE Group = @GroupName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GroupName", group);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string word = reader["Word"].ToString();
                            if (!string.IsNullOrEmpty(word))
                            {
                                //have a list of all words in the tags table that has the given group tag
                                wordsInGroup.Add(word);
                            }
                        }
                    }
                }
            }
            //get the specific words with the locations 
            wordslocations = GetWordslocations(wordsInGroup);

            return wordslocations;
        }
        //returnes the words with thier location from content table from the taged word list 
        static List<string> GetWordslocations(List<string> words)
        {
            List<string> wordsWithIndexes = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Query to search for records containing the specified word
                string query = $"SELECT WordValue, File, paragNum , lineInParagNum, lineNum, charInLineNum, FROM Content WHERE WordValue IN ({string.Join(",", words)})";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string wordValue = reader["WordValue"].ToString();
                            string File = reader["File"].ToString();
                            int paragNum = int.Parse(reader["paragNum"].ToString());
                            int lineInParagNum = int.Parse(reader["lineInParagNum"].ToString());
                            int lineNum = int.Parse(reader["lineNum"].ToString());
                            int charInLineNum = int.Parse(reader["charInLineNum"].ToString());

                            string wordWithIndex = $"{wordValue} | File: {File}, File Line: {lineNum}, Char Index: {charInLineNum}, Paragraph: {paragNum}, Part Line: {lineInParagNum}";
                            wordsWithIndexes.Add(wordWithIndex);
                        }
                    }
                }
            }
            return wordsWithIndexes;
        }

        /// -------------------------------------------------------------------------------------------///

        ///return a list of locations of the expression given 
        static List<string> returnExpression(string expression)
        {
            List<string> exprsIndexes = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int ID = 0;//if not exists

                //step 1: find the expression ID 
                string GetID = $"SELECT ID FROM Expression WHERE Sentence = @Phrase";
                using (SqlCommand IDcommand = new SqlCommand(GetID, connection))
                {
                    IDcommand.Parameters.AddWithValue("@Phrase", expression);
                    var result = IDcommand.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        ID = Convert.ToInt32(result);
                    }
                }

                //if prase not found in the table , means will not be in the location table and needs to be search in the files
                if (ID == 0)
                {
                    Console.WriteLine("expression was not found ");
                    exprsIndexes = newExpression(expression);
                }
                //if prase exists return the locations
                else
                {
                    // Query to search for records containing the specified word
                    string query = $"SELECT File, lineNum, wordInLineNum, FROM PhraseLocation WHERE Exprs = @ID )";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                command.Parameters.AddWithValue("@ID", ID);
                                string File = reader["File"].ToString();
                                int lineNum = int.Parse(reader["lineNum"].ToString());
                                int wordInLineNum = int.Parse(reader["wordInLineNum"].ToString());

                                string wordWithIndex = $" File: {File}, File Line: {lineNum}, word Index: {wordInLineNum}";
                                exprsIndexes.Add(wordWithIndex);
                            }
                        }
                    }
                }
            }
            return exprsIndexes;
        }

        //searches for the expressions accross the files in the 
        static List<string> newExpression(string expression)
        {
            //every string in this list looks like this: File: {File}, File Line: {lineNum}, word Index: {wordInLineNum}
            List<string> exprsIndexes = new List<string>();
            // Split the phrase into words
            string[] words = expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //get the first word 
            string exprWord = words.FirstOrDefault();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //find the properties of the first word that 
                string file="";
                int firstlineNum = 0; int firstWordNum=0;

                //find in the content table of all words possible points that can continue to the phrase 
                string query = $"SELECT File, lineNum, wordInLineNum FROM Content WHERE WordValue=@first";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@first", exprWord);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            file = reader["File"].ToString();
                            firstlineNum = int.Parse(reader["lineNum"].ToString());
                            firstWordNum = int.Parse(reader["wordInLineNum"].ToString());

                            //for each possible point check if its the expression and return the list of IDs
                            bool found;
                            found = matchCheck(words, file, firstlineNum, firstWordNum);
                            //if it is then add the properties, if not skip
                            if (found)
                            {
                                //1/ add the properties to the list that wil retuen to the user for the current request
                                string exprsIndex = $" File: {file}, File Line: {firstlineNum}, word Index: {firstWordNum}";
                                exprsIndexes.Add(exprsIndex);
                                //2/ add the properties to the expression table for future requests 
                                addExpression(expression, file, firstlineNum, firstWordNum);
                            }

                        }
                    }
                }
                return exprsIndexes;
            }
        }

        static public void addExpression(string expression, string file, int firstlineNum, int firstWordNum)
        {
            //first step: add or get the expression id from the expression table
            int id = exprsID(expression);
            //second step: add the properties to the locations tagble
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertQuery = $"INSERT INTO PhraseLocation (ID, File, lineNum, wordInLineNum) VALUES (@id, @file, @lineNum, @WordNum)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@id", id);
                    insertCommand.Parameters.AddWithValue("@file", file);
                    insertCommand.Parameters.AddWithValue("@firstlineNum", firstlineNum);
                    insertCommand.Parameters.AddWithValue("@WordNum", firstWordNum);
        
                    insertCommand.ExecuteNonQuery();
                }
            }
        }
        static int exprsID(string expression)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int ID = 0;

                string GetID = $"SELECT ID FROM Expression WHERE Sentence = @Phrase";
                using (SqlCommand IDcommand = new SqlCommand(GetID, connection))
                {
                    IDcommand.Parameters.AddWithValue("@Phrase", expression);
                    var result = IDcommand.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        ID = Convert.ToInt32(result);
                    }
                }

                //if prase not found in the table , create it to find it and look for it again
                if (ID == 0)
                {

                    int MaxID = 0;// If the table is empty, return 1 as the starting ID  
                                  //if the table is not empty get the last ID 
                    string GetMaxID = $"SELECT MAX(ID) FROM Expression";
                    using (SqlCommand IDcommand = new SqlCommand(GetMaxID, connection))
                    {
                        var result = IDcommand.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            MaxID = Convert.ToInt32(result);
                        }
                    }
                    //prep the new ID 
                    ID = MaxID + 1;

                    //insert the new expression with the new ID 
                    string insertQuery = $"INSERT INTO Expression (Sentence,ID) VALUES (@Expression, @ID);";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Expression", expression);
                        command.Parameters.AddWithValue("@ID", ID);

                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine($"Value '{expression}' inserted with ID '{ID}'.");
                }
                return ID;
            }
        }

        //get a expression and a starting point in the table , get each next word and compare
        static public bool matchCheck(string[] expression, string file, int firstlineNum, int firstWordNum)
        {
            //set the current table word index
            int currWordInLineNum =  firstWordNum;
            int currLineNum = firstlineNum;

            int last = 0;
            int nextWordInLine = 0;
            int nextLineNum = 0;

            //for the length of the expression, starting from the second word as we entered the loop from the first word match
            for (int i = 0; i < expression.Length; i++)
            {
                //get the expression word
                string expWord = expression[i];
                //get the table word 
                string nextWord = null;     
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //check if its last word                   
                    string checklastquery = "SELECT MAX(wordInLineNum) AS last FROM Content WHERE LineNum = @currLineNum AND File = @file  ";
                    using (SqlCommand checklast = new SqlCommand(checklastquery, connection))
                    {
                        checklast.Parameters.AddWithValue("@num", currLineNum);
                        checklast.Parameters.AddWithValue("@FileName", file);

                        SqlDataReader reader = checklast.ExecuteReader();
                        if (reader.Read())
                        {
                            last = Convert.ToInt32(reader["last"]);
                        }
                    }
                    //get the next index
                    if (currWordInLineNum == last)
                    {
                        //the word is the first in the next sentence
                        nextWordInLine =  1;
                        nextLineNum = currLineNum + 1;
                    }
                    else
                    {
                        //the word is the next in the same sentence
                        nextWordInLine = currWordInLineNum + 1;
                        nextLineNum = currLineNum;
                    }
                    //get the next word 
                    string query = @"SELECT Word FROM Content WHERE File = @file AND (LineNum = @nextLineNum AND WordInLineNum = @nextWordInLine)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@file", file);
                        command.Parameters.AddWithValue("@nextLineNum", nextLineNum);
                        command.Parameters.AddWithValue("@nextWordInLine", nextWordInLine);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                nextWord = reader.GetString(0);
                            }
                        }
                    }
                }
               
                //if they mismatch (or a next word was not found), break and return false 
                if (expWord != nextWord) return false;

                //set for the next word
                currWordInLineNum = nextWordInLine;
                currLineNum = nextLineNum;
            }
            //if we finished the loop passing all the expression word and didnt break
            return true;
        }

        //when a new file is added the old expressions that exissts needs to update 
        //use the newExpression function on a partial view of the content table ,
        //for each file from the new wordId look for existing expressions
        static void updateExpression()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //get all the expressions in the expressions table  
                string query = $"SELECT Sentence FROM Expression ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //get the expression string 
                            string expr = reader["Sentence"].ToString();

                            //for each expression add new location from the file that was just added
                            newExpression(expr);

                        }
                    }
                }
            }

        }

        /// --------------------------------------------------------------------------------------------//

        static void ExecuteAprioriOnMetaData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Retrieve data from the "MetaData" table
                string query = "SELECT Patient,Doctor,Diag,Treat FROM MetaData";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    List<List<string>> transactions = new List<List<string>>();

                    while (reader.Read())
                    {
                        string patient = reader["Patient"].ToString();
                        string doctor = reader["Doctor"].ToString();
                        string diag = reader["Diag"].ToString();
                        string treat = reader["Treat"].ToString();

                        // Add attributes of interest to the transaction
                        List<string> transaction = new List<string> { patient, doctor, diag, treat};
                        transactions.Add(transaction);
                    }

                    // Implement your Apriori algorithm logic here to mine association rules
                    // For simplicity, let's assume you have mined association rules and stored them in a list
                    List<string> associationRules = FindAssociationRules(transactions);

                    // Print the association rules
                    Console.WriteLine("Association Rules:");
                    foreach (string rule in associationRules)
                    {
                        Console.WriteLine(rule);
                    }
                }
            }
        }

        static List<string> FindAssociationRules(List<List<string>> transactions)
        {
            List<string> associationRules = new List<string>();
            // Implement the Apriori algorithm logic here to mine association rules
            // This is a simplified example; a complete Apriori implementation is more complex
            // You may need to implement support for generating frequent itemsets, confidence, and pruning

            // Example: Association rule {A} => {B} with a minimum support and confidence threshold
            double minSupport = 0.2; // Adjust as needed
            double minConfidence = 0.5; // Adjust as needed

            // Iterate over all possible rules
            foreach (List<string> transaction in transactions)
            {
                // Check if the transaction meets the minimum support threshold
                if (MeetMinSupport(transaction, transactions, minSupport))
                {
                    // Generate candidate rules
                    List<string> candidates = GenerateCandidateRules(transaction);

                    // Iterate over candidate rules
                    foreach (string candidateRule in candidates)
                    {
                        // Calculate confidence
                        double confidence = CalculateConfidence(candidateRule, transaction, transactions);

                        // Check if the confidence meets the minimum confidence threshold
                        if (confidence >= minConfidence)
                        {
                            associationRules.Add(candidateRule);
                        }
                    }
                }
            }

            return associationRules;
        }

        static bool MeetMinSupport(List<string> itemset, List<List<string>> transactions, double minSupport)
        {
            // Calculate the support for an itemset
            int count = transactions.Count(t => itemset.All(item => t.Contains(item)));
            double support = (double)count / transactions.Count;

            return support >= minSupport;
        }

        static List<string> GenerateCandidateRules(List<string> transaction)
        {
            // Generate all possible candidate rules for a transaction
            List<string> candidates = new List<string>();

            for (int i = 0; i < transaction.Count; i++)
            {
                for (int j = 0; j < transaction.Count; j++)
                {
                    if (i != j)
                    {
                        candidates.Add($"{transaction[i]} => {transaction[j]}");
                    }
                }
            }

            return candidates;
        }

        static double CalculateConfidence(string rule, List<string> transaction, List<List<string>> transactions)
        {
            // Calculate the confidence for a rule
            string[] items = rule.Split(" => ");
            string antecedent = items[0];
            string consequent = items[1];

            int antecedentCount = transactions.Count(t => t.Contains(antecedent));
            int ruleCount = transactions.Count(t => t.Contains(antecedent) && t.Contains(consequent));

            return (double)ruleCount / antecedentCount;
        }

    }
}