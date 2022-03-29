using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IConfiguration _config;
        private IDogRepository _dogRepo;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public OwnerRepository(IConfiguration config)
        {
            _config = config;
            _dogRepo = new DogRepository(config);
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT o.Id, 
                                               Email,
                                               o.[Name],
                                               Address,
                                               NeighborhoodId,
                                               n.Id as nId,
                                               n.[Name] as NeighborhoodName,
                                               Phone
                                          FROM Owner o
                                               LEFT JOIN Neighborhood n ON n.Id = NeighborhoodId"
;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Owner> owners = new List<Owner>();
                        while (reader.Read())
                        {
                            Owner owner = new Owner()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                NeighborhoodId = reader.IsDBNull(reader.GetOrdinal("NeighborhoodId")) ? 0 : reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Neighborhood = new Neighborhood()
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("nId")) ? 0 : reader.GetInt32(reader.GetOrdinal("nId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("NeighborhoodName")) ? null : reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                                },
                                Phone = reader.GetString(reader.GetOrdinal("Phone"))
                            };

                            owners.Add(owner);
                        }
                        return owners;
                    }
                }
            }
        }
        public Owner GetOwnerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Owner.Id as oId,
                                               Owner.Email, 
                                               Owner.Name as oName,
                                               Owner.Address,
                                               Owner.NeighborhoodId,
                                               Owner.Phone,
                                               n.Id as nId,
                                               n.[Name] as NeighborhoodName,
                                               Dog.Id as dId, 
                                               Dog.Name dName,
                                               Dog.Breed,
                                               Dog.Notes,
                                               Dog.ImageUrl,
                                               Dog.OwnerId
                                          FROM Owner
                                               LEFT JOIN Dog ON Dog.OwnerId = Owner.Id
                                               LEFT JOIN Neighborhood n ON n.Id = NeighborhoodId
                                         WHERE Owner.Id = @id
;";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Owner owner = null;

                        while (reader.Read())
                        {
                            if (owner == null)
                            {
                                owner = new Owner()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("oId")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Name = reader.GetString(reader.GetOrdinal("oName")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    NeighborhoodId = reader.IsDBNull(reader.GetOrdinal("nId")) ? 0 : reader.GetInt32(reader.GetOrdinal("nId")),
                                    Neighborhood = new Neighborhood()
                                    {
                                        Id = reader.IsDBNull(reader.GetOrdinal("nId")) ? 0 : reader.GetInt32(reader.GetOrdinal("nId")),
                                        Name = reader.IsDBNull(reader.GetOrdinal("NeighborhoodName")) ? null : reader.GetString(reader.GetOrdinal("NeighborhoodName"))
                                    },
                                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                    Dogs = new List<Dog>()
                                };
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("dId")))
                            {


                                Dog dog = new Dog()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("dId")),
                                    Name = reader.GetString(reader.GetOrdinal("dName")),
                                    Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                    OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                    ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl"))
                                };

                                owner.Dogs.Add(dog);
                            }
                        }
                        return owner;
                    }
                }
            }
        }

        public Owner GetOwnerByEmail(string email)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Email, Address, Phone, NeighborhoodId
                        FROM Owner
                        WHERE Email = @email";

                    cmd.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Owner owner = new Owner()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                NeighborhoodId = reader.IsDBNull(reader.GetOrdinal("NeighborhoodId")) ? 0 : reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                            };

                            return owner;
                        }

                        return null;
                    }
                }
            }
        }

        public void AddOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Owner ([Name], Email, Phone, Address, NeighborhoodId)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @email, @phoneNumber, @address, @neighborhoodId);
                ";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@phoneNumber", owner.Phone);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId == 0 ? DBNull.Value : owner.NeighborhoodId);

                    int id = (int)cmd.ExecuteScalar();

                    owner.Id = id;
                }
            }
        }

        public void UpdateOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Owner
                            SET 
                                [Name] = @name, 
                                Email = @email, 
                                Address = @address, 
                                Phone = @phone, 
                                NeighborhoodId = @neighborhoodId
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@phone", owner.Phone);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId == 0 ? DBNull.Value : owner.NeighborhoodId);
                    cmd.Parameters.AddWithValue("@id", owner.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteOwner(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Owner
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", ownerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}