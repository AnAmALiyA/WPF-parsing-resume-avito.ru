using Finisar.SQLite;
using System;
using System.Collections.Generic;
using WPF_Parsing_resume_avito_ru.Entitties;

namespace WPF_Parsing_resume_avito_ru.Concrete
{
    public class ApplicationContext
    {
        // We use these three SQLite objects:
        SQLiteConnection sqlite_conn;
        SQLiteCommand sqlite_cmd;
        SQLiteDataReader sqlite_datareader;        

        public ApplicationContext()
        {
            // create a new database connection:
            //sqlite_conn = new SQLiteConnection("Data Source=database.db;Version=3;New=True;Compress=True;");
            sqlite_conn = new SQLiteConnection("Data Source="+ Environment.CurrentDirectory+"\\database.db;Version=3;Compress=True;");
        }

        public void CreateDB()
        {
            // open the connection: D:\Education_materials\Test Task\WPF Parsing resume avito_ru\WPF Parsing resume avito_ru\bin\Debug\Database\database.db
            sqlite_conn.Open();

            // create a new SQL command:
            sqlite_cmd = sqlite_conn.CreateCommand();

            // Let the SQLiteCommand object know our SQL-Query:
            sqlite_cmd.CommandText = "CREATE TABLE IF NOT EXISTS CV (id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE, 'Specialty' TEXT, 'Pay' INTEGER, 'Sex' TEXT, 'Age' INTEGER, 'AgeText' TEXT, 'Experience' INTEGER, 'ExperienceText' TEXT, 'Education' TEXT, 'Location' TEXT, 'DateWork' TEXT, 'SpecialityWork' TEXT, 'LocationWork' TEXT);";

            // Now lets execute the SQL ;D
            sqlite_cmd.ExecuteNonQuery();
            
            // Let the SQLiteCommand object know our SQL-Query:
            sqlite_cmd.CommandText = "CREATE TABLE IF NOT EXISTS URL (id INTEGER PRIMARY AUTOINCREMENT KEY UNIQUE, 'StrURL' TEXT NOT NULL);";

            // And execute this again ;D
            sqlite_cmd.ExecuteNonQuery();

            // We are ready, now lets cleanup and close our connection:
            sqlite_conn.Close();
        }

        public void AddCV(CV resume)
        {
            // open the connection:
            sqlite_conn.Open();

            // create a new SQL command:
            sqlite_cmd = sqlite_conn.CreateCommand();            
            
            // Lets insert something into our new table:
            sqlite_cmd.CommandText = "INSERT INTO CV (Specialty, Pay, Sex, Age, AgeText, Experience, ExperienceText, Education, Location, DateWork, SpecialityWork, LocationWork) VALUES ('" + resume.Speciality + "'," + resume.Pay + ",'" + resume.Sex + "'," + resume.Age + ",'" + resume.AgeText + "'," + resume.Experience + ",'" + resume.ExperienceText + "','" + resume.Education + "','" + resume.Location + "','" + resume.DateWork + "','" + resume.SpecialityWork + "','" + resume.LocationWork + "');";

            // And execute this again ;D
            sqlite_cmd.ExecuteNonQuery();

            // We are ready, now lets cleanup and close our connection:
            sqlite_conn.Close();
        }

        public void AddCVList(IEnumerable<CV> resumeList)
        {
            // open the connection:
            sqlite_conn.Open();           

            foreach (CV resume in resumeList)
            {
                // Lets insert something into our new table:
                sqlite_cmd.CommandText = "INSERT INTO CV (Specialty, Pay, Sex, Age, AgeText, Experience, ExperienceText, Education, Location, DateWork, SpecialityWork, LocationWork) VALUES ('" + resume.Speciality + "'," + resume.Pay + ",'" + resume.Sex + "'," + resume.Age + ",'" + resume.AgeText + "'," + resume.Experience + ",'" + resume.ExperienceText + "','" + resume.Education + "','" + resume.Location + "','" + resume.DateWork + "','" + resume.SpecialityWork + "','" + resume.LocationWork + "');";

                // And execute this again ;D
                sqlite_cmd.ExecuteNonQuery();
            }
            
            // We are ready, now lets cleanup and close our connection:
            sqlite_conn.Close();
        }

        public void AddURL(string strUrl)
        {
            // open the connection:
            sqlite_conn.Open();            
            
            // Lets insert something into our new table:
            sqlite_cmd.CommandText = "INSERT INTO URL (StrURL) VALUES ('" + strUrl + "');";

            // And execute this again ;D
            sqlite_cmd.ExecuteNonQuery();

            // We are ready, now lets cleanup and close our connection:
            sqlite_conn.Close();
        }

        public IEnumerable<CV> GetAllCV()
        {
            // open the connection:
            sqlite_conn.Open();

            // create a new SQL command:
            sqlite_cmd = sqlite_conn.CreateCommand();

            // But how do we read something out of our table ?
            // First lets build a SQL-Query again:
            sqlite_cmd.CommandText = "SELECT * FROM CV";

            // Now the SQLiteCommand object can give us a DataReader-Object:
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            List<CV> allCV = new List<CV>();

            // The SQLiteDataReader allows us to run through the result lines:
            while (sqlite_datareader.Read()) // Read() returns true if there is still a result line to read
            {
                CV tempCV= new CV();

                tempCV.Id = (int)sqlite_datareader["Id"];
                tempCV.Speciality = (string)sqlite_datareader["Speciality"];
                tempCV.Pay = (int)sqlite_datareader["Pay"];
                tempCV.Sex = (string)sqlite_datareader["Sex"];
                tempCV.Age = (int)sqlite_datareader["Age"];
                tempCV.AgeText = (string)sqlite_datareader["AgeText"];
                tempCV.Experience = (int)sqlite_datareader["Experience"];
                tempCV.ExperienceText = (string)sqlite_datareader["ExperienceText"];
                tempCV.Education = (string)sqlite_datareader["Education"];
                tempCV.Location = (string)sqlite_datareader["Location"];
                tempCV.DateWork = (string)sqlite_datareader["DateWork"];
                tempCV.SpecialityWork = (string)sqlite_datareader["SpecialityWork"];
                tempCV.LocationWork = (string)sqlite_datareader["LocationWork"];

                allCV.Add(tempCV);               
            }

            // We are ready, now lets cleanup and close our connection:
            sqlite_conn.Close();

            return (IEnumerable<CV>) allCV;
        }

        public IEnumerable<URL> GetURL()
        {
            // open the connection:
            sqlite_conn.Open();

            // create a new SQL command:
            sqlite_cmd = sqlite_conn.CreateCommand();

            // But how do we read something out of our table ?
            // First lets build a SQL-Query again:
            sqlite_cmd.CommandText = "SELECT * FROM URL";

            // Now the SQLiteCommand object can give us a DataReader-Object:
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            List<URL> allURL = new List<URL>();

            // The SQLiteDataReader allows us to run through the result lines:
            while (sqlite_datareader.Read()) // Read() returns true if there is still a result line to read
            {
                URL tempURL = new URL();

                tempURL.StrURL = (string)sqlite_datareader["StrURL"];

                allURL.Add(tempURL);
            }

            // We are ready, now lets cleanup and close our connection:
            sqlite_conn.Close();

            return (IEnumerable<URL>)allURL;
        }

        public CV GetCV(int id)
        {
            // open the connection:
            sqlite_conn.Open();

            // create a new SQL command:
            sqlite_cmd = sqlite_conn.CreateCommand();

            // But how do we read something out of our table ?
            // First lets build a SQL-Query again:
            sqlite_cmd.CommandText = "SELECT * FROM CV WHERE Id="+id.ToString();

            // Now the SQLiteCommand object can give us a DataReader-Object:
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            CV tempCV = new CV();

            // The SQLiteDataReader allows us to run through the result lines:
            while (sqlite_datareader.Read()) // Read() returns true if there is still a result line to read
            {
                tempCV.Id = (int)sqlite_datareader["Id"];
                tempCV.Speciality = (string)sqlite_datareader["Speciality"];
                tempCV.Pay = (int)sqlite_datareader["Pay"];
                tempCV.Sex = (string)sqlite_datareader["Sex"];
                tempCV.Age = (int)sqlite_datareader["Age"];
                tempCV.AgeText = (string)sqlite_datareader["AgeText"];
                tempCV.Experience = (int)sqlite_datareader["Experience"];
                tempCV.ExperienceText = (string)sqlite_datareader["ExperienceText"];
                tempCV.Education = (string)sqlite_datareader["Education"];
                tempCV.Location = (string)sqlite_datareader["Location"];
                tempCV.DateWork = (string)sqlite_datareader["DateWork"];
                tempCV.SpecialityWork = (string)sqlite_datareader["SpecialityWork"];
                tempCV.LocationWork = (string)sqlite_datareader["LocationWork"];                
            }

            // We are ready, now lets cleanup and close our connection:
            sqlite_conn.Close();

            return tempCV;
        }

        public void DeleteCV(int id)
        {
            // open the connection:
            sqlite_conn.Open();

            // create a new SQL command:
            sqlite_cmd = sqlite_conn.CreateCommand();

            // But how do we read something out of our table ?
            // First lets build a SQL-Query again:
            sqlite_cmd.CommandText = "DELETE CV WHERE Id=" + id + "SELECT* FROM CV";
            
            // We are ready, now lets cleanup and close our connection:
            sqlite_conn.Close();            
        }
    }
}
