﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class ComplaintPersistenceService
    {
        private MySqlConnection sqlConnection;

        public ComplaintPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Complaints Call
        public List<Complaint> GetComplaints()
        {
            MySqlDataReader mySQLReader = null;
            List<Complaint> Complaints = new List<Complaint>();

            string slqCommandString = "SELECT * FROM Complaints";
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Complaint Complaint = new Complaint();
                    Complaint.complaintID = mySQLReader.GetInt32(0);
                    Complaint.guestUserID = mySQLReader.GetInt32(1);
                    Complaint.hostUserID = mySQLReader.GetInt32(2);
                    Complaint.csrUserID = mySQLReader.GetInt32(3);
                    Complaint.content = mySQLReader.GetString(4);
                    Complaint.resolution = mySQLReader.IsDBNull(5) ? String.Empty : mySQLReader.GetString(5);
                    Complaints.Add(Complaint);
                }
                mySQLReader.Close();
                return Complaints;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Complaints call in ComplaintPersistenceService(GetComplaints): " + ex);
                return null;
            }
        }

        // GET Complaint Call
        public Complaint GetComplaint(long id)
        {
            Complaint Complaint = new Complaint();
            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM Complaints WHERE ComplaintID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    Complaint.complaintID = mySQLReader.GetInt32(0);
                    Complaint.guestUserID = mySQLReader.GetInt32(1);
                    Complaint.hostUserID = mySQLReader.GetInt32(2);
                    Complaint.csrUserID = mySQLReader.GetInt32(3);
                    Complaint.content = mySQLReader.GetString(4);
                    Complaint.resolution = mySQLReader.IsDBNull(5) ? String.Empty : mySQLReader.GetString(5);

                    mySQLReader.Close();
                    return Complaint;
                }
                mySQLReader.Close();
                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Complaint call in ComplaintPersistenceService (GetComplaint): " + ex);
                return null;
            }
        }

        // POST Complaint
        public long SaveComplaint(Complaint complaint)
        {
            string sqlCommandString;
            if (String.IsNullOrEmpty(complaint.resolution)) {
                sqlCommandString = "INSERT INTO Complaints VALUES(null, " + complaint.guestUserID + "," +
                complaint.hostUserID + "," + complaint.csrUserID + ",'" + complaint.content + "', null)";
            }
            else
            {
                sqlCommandString = "INSERT INTO Complaints VALUES(null, " + complaint.guestUserID + "," +
                complaint.hostUserID + "," + complaint.csrUserID + ",'" + complaint.content + "'," + complaint.resolution + ")";
            }          
            Debug.WriteLine(sqlCommandString);

            MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
            try
            {
                sqlCommand.ExecuteNonQuery();
                long id = sqlCommand.LastInsertedId;
                return id;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a POST Complaint call in ComplaintPersistenceService (SaveComplaint): " + ex);
                return -1;
            }
        }

        // PATCH or PUT Accommodation
        public bool UpdateComplaint(long id, String resolution)
        {
            MySqlDataReader mySQLReader = null;
            string slqCommandString = "SELECT * FROM Complaints WHERE ComplaintID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();


                if (mySQLReader.Read())
                {
                    // If we are to update the email, we will need to reference the old email, as it is the PK in the child table.
                    // AGAIN, updating the PK of a table is a BAD idea, but in our design, we want the user to be able to update Email.
                    var oldEmail = mySQLReader.GetString(1);
                    
                    mySQLReader.Close();

                    string sqlUpdateCommandString = "UPDATE Complaints"
                        + " SET resolution='" + resolution
                        + "' WHERE ComplaintID=" + id.ToString();

                    MySqlCommand sqlUpdateCommand = new MySqlCommand(sqlUpdateCommandString, sqlConnection);

                    sqlUpdateCommand.ExecuteNonQuery();
                    return true;
                }

                mySQLReader.Close();
                return false;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a PUT Complaint call in ComplaintPersistenceService: " + ex);
                return false;
            }
        }
}
}