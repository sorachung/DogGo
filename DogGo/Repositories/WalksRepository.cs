using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;

namespace DogGo.Repositories
{
    public class WalksRepository : IWalksRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalksRepository(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walks> GetAllWalks()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT wk.Id AS wkId, 
                                               Date,
                                               Duration,
                                               WalkerId,
                                               DogId,
                                               w.[Name] as wName,
                                               d.[Name] as dName,
                                               o.[Name] as oName,
                                               OwnerId
                                          FROM Walks wk
                                               JOIN Walker w ON w.Id = WalkerId
                                               JOIN Dog d ON d.Id = DogId
                                               JOIN Owner o ON o.Id = OwnerId"
;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Walks> walks = new List<Walks>();
                        while (reader.Read())
                        {
                            Walks walk = new Walks()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("wkId")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                                DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Dog = new Dog()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                    Name = reader.GetString(reader.GetOrdinal("dName")),
                                    Owner = new Owner()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                        Name = reader.GetString(reader.GetOrdinal("oName"))
                                    }
                                },
                                Walker = new Walker()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("wName")),
                                }

                            };

                            walks.Add(walk);
                        }
                        return walks;
                    }
                }
            }
        }
        //public Walks GetWalkById()
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT Id, 
        //                                       [Name],
        //                                       Breed,
        //                                       Notes,
        //                                       ImageUrl,
        //                                       ownerId
        //                                  FROM Dog
        //                                 WHERE Id = @id";
        //            cmd.Parameters.AddWithValue("@id", id);

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    Dog dog = new Dog()
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                        Name = reader.GetString(reader.GetOrdinal("Name")),
        //                        Breed = reader.GetString(reader.GetOrdinal("Breed")),
        //                        OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
        //                        Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
        //                        ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl"))
        //                    };

        //                    return dog;
        //                }
        //                else
        //                {
        //                    return null;
        //                }
        //            }
        //        }
        //    }
        //}

        //public List<Walks> GetWalksByWalkerId(int walkerId)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"SELECT Id, 
        //                                       [Name],
        //                                       Breed,
        //                                       Notes,
        //                                       ImageUrl,
        //                                       OwnerId
        //                                  FROM Dog
        //                                 WHERE OwnerId = @ownerId";
        //            cmd.Parameters.AddWithValue("@ownerId", ownerId);

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                List<Dog> dogs = new List<Dog>();
        //                while (reader.Read())
        //                {
        //                    Dog dog = new Dog()
        //                    {
        //                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        //                        Name = reader.GetString(reader.GetOrdinal("Name")),
        //                        Breed = reader.GetString(reader.GetOrdinal("Breed")),
        //                        OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
        //                    };

        //                    if (reader.IsDBNull(reader.GetOrdinal("Notes")))
        //                    {
        //                        dog.Notes = null;
        //                    }
        //                    else
        //                    {
        //                        dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
        //                    }
        //                    if (reader.IsDBNull(reader.GetOrdinal("ImageUrl")))
        //                    {
        //                        dog.ImageUrl = null;
        //                    }
        //                    else
        //                    {
        //                        dog.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
        //                    }
        //                    dogs.Add(dog);
        //                }
        //                return dogs;
        //            }
        //        }
        //    }
        //}

        public void AddWalk(Walks walk)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Walks (Date, Duration, WalkerId, DogId)
                                        OUTPUT INSERTED.ID
                                        VALUES (@date, @duration, @walkerId, @dogId)";

                    cmd.Parameters.AddWithValue("@date", walk.Date);
                    cmd.Parameters.AddWithValue("@duration", walk.Duration);
                    cmd.Parameters.AddWithValue("@walkerId", walk.WalkerId);
                    cmd.Parameters.AddWithValue("@dogId", walk.DogId);

                    int id = (int)cmd.ExecuteScalar();
                    walk.Id = id;

                }
            }
        }

        //public void UpdateWalk(Walks walk)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();

        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"
        //                    UPDATE Dog
        //                    SET 
        //                        [Name] = @name, 
        //                        Breed = @breed, 
        //                        Notes = @notes, 
        //                        ImageUrl = @imageUrl, 
        //                        OwnerId = @ownerId
        //                    WHERE Id = @id";

        //            cmd.Parameters.AddWithValue("@name", dog.Name);
        //            cmd.Parameters.AddWithValue("@breed", dog.Breed);
        //            cmd.Parameters.AddWithValue("@notes", dog.Notes == null ? DBNull.Value : dog.Notes);
        //            cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl == null ? DBNull.Value : dog.ImageUrl);
        //            cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
        //            cmd.Parameters.AddWithValue("@id", dog.Id);

        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        //public void DeleteWalk(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = @"DELETE FROM Dog
        //                              WHERE Id = @id";
        //            cmd.Parameters.AddWithValue("@id", id);
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}
    }
}
