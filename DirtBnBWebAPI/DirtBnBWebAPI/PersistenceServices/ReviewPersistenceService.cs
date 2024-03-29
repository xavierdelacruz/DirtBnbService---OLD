﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using DirtBnBWebAPI.Models;
using MySql.Data.MySqlClient;

namespace DirtBnBWebAPI.PersistenceServices
{
    public class ReviewPersistenceService
    {
        private MySqlConnection sqlConnection;

        public ReviewPersistenceService()
        {
            sqlConnection = SqlServerConnectionManager.Instance.GetSqlConnection();
        }

        // GET Reviews Call
        public List<Review> GetReviews()
        {
            MySqlDataReader mySQLReader = null;
            List<Review> Reviews = new List<Review>();

            string slqCommandString = "SELECT * FROM Reviews";
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Review Review = new Review();
                    Review.reviewID = mySQLReader.GetInt32(0);
                    Review.accommodationID = mySQLReader.GetInt32(1);
                    Review.reservationID = mySQLReader.GetInt32(2);
                    Review.content = mySQLReader.GetString(3);
                    Review.rating = mySQLReader.GetInt32(4);
                    Reviews.Add(Review);
                }
                mySQLReader.Close();
                return Reviews;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Reviews call in ReviewPersistenceService(GetReviews): " + ex);
                Debug.WriteLine(ex);
                return null;
            }
        }

        // GET Review Call
        public Review GetReview(long id)
        {
            Review Review = new Review();
            MySqlDataReader mySQLReader = null;

            string slqCommandString = "SELECT * FROM Reviews WHERE ReviewID = " + id.ToString();

            try
            {
                MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
                mySQLReader = sqlCommand.ExecuteReader();

                if (mySQLReader.Read())
                {
                    Review.reviewID = mySQLReader.GetInt32(0);
                    Review.accommodationID = mySQLReader.GetInt32(1);
                    Review.reservationID = mySQLReader.GetInt32(2);
                    Review.content = mySQLReader.GetString(3);
                    Review.rating = mySQLReader.GetInt32(4);

                    mySQLReader.Close();
                    return Review;
                }
                mySQLReader.Close();
                return null;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Review call in ReviewPersistenceService (GetReview): " + ex);
                Debug.WriteLine(ex);
                return null;
            }
        }

        // GET Reviews Above Accommodation Average Call
        public List<Review> GetAboveAverageReviews()
        {
            MySqlDataReader mySQLReader = null;
            List<Review> Reviews = new List<Review>();

            string slqCommandString = "SELECT Reviews.reviewID, Reviews.accommodationID, Reviews.reservationID, Reviews.content, Reviews.rating, " +
                "AVGTABLE.avgR as avgRating " +
                "FROM Reviews JOIN(SELECT Reviews.accommodationID, avg(Reviews.rating) as avgR FROM Reviews, Accommodations " +
                "WHERE Reviews.accommodationID = Accommodations.accommodationID GROUP BY Reviews.accommodationID) " +
                "AS AVGTABLE ON Reviews.accommodationID = AVGTABLE.accommodationID WHERE Reviews.rating >= AVGTABLE.avgR";

            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Review Review = new Review();
                    Review.reviewID = mySQLReader.GetInt32(0);
                    Review.accommodationID = mySQLReader.GetInt32(1);
                    Review.reservationID = mySQLReader.GetInt32(2);
                    Review.content = mySQLReader.GetString(3);
                    Review.rating = mySQLReader.GetInt32(4);
                    Reviews.Add(Review);
                }
                mySQLReader.Close();
                return Reviews;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Reviews call in ReviewPersistenceService(GetReviews): " + ex);
                Debug.WriteLine(ex);
                return null;
            }
        }


        // POST Review
        public long SaveReview(Review review)
        {
            string sqlCommandString = "INSERT INTO Reviews VALUES(" + review.reviewID + "," +
                review.accommodationID + "," + review.reservationID + ",'" + review.content + "'," + review.rating + ")";
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
                Console.WriteLine("Found an error when performing a POST Review call in ReviewPersistenceService (SaveReview): " + ex);
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public List<Review> GetSelectReviews(String op, int val)
        {
            MySqlDataReader mySQLReader = null;
            List<Review> Reviews = new List<Review>();

            string slqCommandString = "SELECT * FROM Reviews WHERE rating " + op + " " + val.ToString();
            MySqlCommand sqlCommand = new MySqlCommand(slqCommandString, sqlConnection);
            try
            {
                mySQLReader = sqlCommand.ExecuteReader();

                while (mySQLReader.Read())
                {
                    Review Review = new Review();
                    Review.reviewID = mySQLReader.GetInt32(0);
                    Review.accommodationID = mySQLReader.GetInt32(1);
                    Review.reservationID = mySQLReader.GetInt32(2);
                    Review.content = mySQLReader.GetString(3);
                    Review.rating = mySQLReader.GetInt32(4);
                    Reviews.Add(Review);
                }
                mySQLReader.Close();
                return Reviews;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Found an error when performing a GET Reviews call in ReviewPersistenceService(GetReviews): " + ex);
                Debug.WriteLine(ex);
                return null;
            }
        }

        // DELETE Review
        // Commented out as we decided not to allow deleting reviews
        //public bool DeleteReview(long id)
        //{
        //    string sqlCommandString = "SELECT * FROM Reviews WHERE ReviewID = " + id.ToString();
        //    MySqlCommand sqlCommand = new MySqlCommand(sqlCommandString, sqlConnection);
        //    try
        //    {
        //        sqlCommand.ExecuteNonQuery();
        //        return true;
        //    }
        //    catch (MySqlException ex)
        //    {
        //        Console.WriteLine("Found an error when performing a DELETE Review call in ReviewPersistenceService: " + ex);
        //        return false;
        //    }
        //}
    }
}