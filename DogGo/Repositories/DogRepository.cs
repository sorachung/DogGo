using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public DogRepository(IConfiguration config)
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

        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, 
                                               [Name],
                                               Breed,
                                               Notes,
                                               ImageUrl,
                                               ownerId
                                          FROM Dog"
;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Dog> dogs = new List<Dog>();
                        while (reader.Read())
                        {
                            Dog dog = new Dog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            };

                            if (reader.IsDBNull(reader.GetOrdinal("Notes")))
                            {
                                dog.Notes = null;
                            }
                            else
                            {
                                dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                            }
                            if (reader.IsDBNull(reader.GetOrdinal("ImageUrl")))
                            {
                                dog.ImageUrl = null;
                            }
                            else
                            {
                                dog.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
                            }

                            dogs.Add(dog);
                        }
                        return dogs;
                    }
                }
            }
        }
        public Dog GetDogById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, 
                                               [Name],
                                               Breed,
                                               Notes,
                                               ImageUrl,
                                               ownerId
                                          FROM Dog
                                         WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Dog dog = new Dog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            };

                            if (reader.IsDBNull(reader.GetOrdinal("Notes")))
                            {
                                dog.Notes = null;
                            }
                            else
                            {
                                dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                            }
                            if (reader.IsDBNull(reader.GetOrdinal("ImageUrl")))
                            {
                                dog.ImageUrl = null;
                            }
                            else
                            {
                                dog.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
                            }

                            return dog;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public List<Dog> GetDogsByOwner(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, 
                                               [Name],
                                               Breed,
                                               Notes,
                                               ImageUrl,
                                               OwnerId
                                          FROM Dog
                                         WHERE OwnerId = @ownerId";
                    cmd.Parameters.AddWithValue("@ownerId", ownerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Dog> dogs = new List<Dog>();
                        while (reader.Read())
                        {
                            Dog dog = new Dog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            };

                            if (reader.IsDBNull(reader.GetOrdinal("Notes")))
                            {
                                dog.Notes = null;
                            }
                            else
                            {
                                dog.Notes = reader.GetString(reader.GetOrdinal("Notes"));
                            }
                            if (reader.IsDBNull(reader.GetOrdinal("ImageUrl")))
                            {
                                dog.ImageUrl = null;
                            }
                            else
                            {
                                dog.ImageUrl = reader.GetString(reader.GetOrdinal("ImageUrl"));
                            }
                            dogs.Add(dog);
                        }
                        return dogs;
                    }
                }
            }
        }
    }
}
