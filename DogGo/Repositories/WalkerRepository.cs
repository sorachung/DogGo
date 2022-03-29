using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkerRepository : IWalkerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public WalkerRepository(IConfiguration config)
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

        public List<Walker> GetAllWalkers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id as wId, w.[Name] as wName, ImageUrl, NeighborhoodId, n.Id as nId, n.[Name] as nName
                          FROM Walker w
                               LEFT JOIN Neighborhood n ON n.Id = NeighborhoodId
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Walker> walkers = new List<Walker>();
                        while (reader.Read())
                        {
                            Walker walker = new Walker
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("wId")),
                                Name = reader.GetString(reader.GetOrdinal("wName")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                                NeighborhoodId = reader.IsDBNull(reader.GetOrdinal("NeighborhoodId")) ? 0 : reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Neighborhood = new Neighborhood()
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("nId")) ? 0 : reader.GetInt32(reader.GetOrdinal("nId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("nName")) ? null : reader.GetString(reader.GetOrdinal("nName"))
                                }

                            };

                            walkers.Add(walker);
                        }

                        return walkers;
                    }
                }
            }
        }

        public Walker GetWalkerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id as wId, w.[Name] as wName, ImageUrl, NeighborhoodId, n.Id as nId, n.[Name] as nName
                          FROM Walker w
                               LEFT JOIN Neighborhood n ON n.Id = NeighborhoodId
                         WHERE w.Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Walker walker = new Walker
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("wId")),
                                Name = reader.GetString(reader.GetOrdinal("wName")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl")),
                                NeighborhoodId = reader.IsDBNull(reader.GetOrdinal("NeighborhoodId")) ? 0 : reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Neighborhood = new Neighborhood()
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("nId")) ? 0 : reader.GetInt32(reader.GetOrdinal("nId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("nName")) ? null : reader.GetString(reader.GetOrdinal("nName"))
                                }

                            };

                            return walker;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public void AddWalker(Walker walker)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Walker ([Name], ImageUrl, NeighborhoodId)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @imageUrl, @neighborhoodId);
                ";

                    cmd.Parameters.AddWithValue("@name", walker.Name);
                    cmd.Parameters.AddWithValue("@imageUrl", walker.ImageUrl == null ? DBNull.Value : walker.ImageUrl);
                    cmd.Parameters.AddWithValue("@neighborhoodId", walker.NeighborhoodId == 0 ? DBNull.Value : walker.NeighborhoodId);

                    int id = (int)cmd.ExecuteScalar();

                    walker.Id = id;
                }
            }
        }

        public void UpdateWalker(Walker walker)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Walker
                                           SET [Name] = @name,
                                               ImageUrl = @imageUrl,
                                               NeighborhoodId = @neighborhoodId
                                         WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", walker.Id);
                    cmd.Parameters.AddWithValue("@name", walker.Name);
                    cmd.Parameters.AddWithValue("@imageUrl", walker.ImageUrl == null ? DBNull.Value : walker.ImageUrl);
                    cmd.Parameters.AddWithValue("@neighborhoodId", walker.NeighborhoodId == 0 ? DBNull.Value : walker.NeighborhoodId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteWalker(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Walker
                                         WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}