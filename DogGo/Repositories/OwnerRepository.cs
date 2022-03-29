using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
                                Neighborhood = new Neighborhood()
                                {
                                    Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
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
                                    Neighborhood = new Neighborhood()
                                    {
                                        Name = reader.GetString(reader.GetOrdinal("NeighborhoodName"))
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
                                owner.Dogs.Add(dog);
                            }
                        }
                        return owner;
                    }
                }
            }
        }
    }
}
