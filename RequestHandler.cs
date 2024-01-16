using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using MySql.Data.MySqlClient;


namespace Api {
    public class RequestHandler {
        public static void ProcessRequest(HttpListenerContext context, List<Article> articles , List<User> users, DatabaseConnection data) {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string responseString = "";

            switch(request.HttpMethod) {

                case "POST":
                    // Add Article
                    if (request.Url.LocalPath == "/articles/add") {
                        Article A1 = new Article();
                        // name
                        Console.WriteLine("Enter a Name for your product: ");
                        A1.Name = Console.ReadLine();
                        // Quantity
                        Console.WriteLine("Enter the Quantity remaining of the product");
                        string tempQuantity = Console.ReadLine();
                        A1.Quantity = int.Parse(tempQuantity);
                        var sql = "INSERT INTO api_csharp.article (name, quantity) VALUES ( @name, @quantity)";
                        var command = new MySqlCommand(sql, data.connection);
                        command.Parameters.AddWithValue("@name", A1.Name);
                        command.Parameters.AddWithValue("@quantity", A1.Quantity);
                        command.ExecuteNonQuery();
                    }
                        
                    // Add User
                    if (request.Url.LocalPath == "/users/add") {
                        User U1 = new User();
                        // Username
                        Console.WriteLine("Enter a Username for the user: ");
                        U1.Username = Console.ReadLine();
                        var sql = "INSERT INTO apiCsharp.user (username) VALUES (@username)";
                        var command = new MySqlCommand(sql, data.connection);
                        command.Parameters.AddWithValue("@username", U1.Username);
                        command.ExecuteNonQuery();
                    }
                    break;
                
                case "GET":
                    // Get Article
                    if (request.Url.LocalPath.StartsWith("/articles/")) {
                        int id;
                        if (int.TryParse(request.Url.LocalPath.Substring("/articles/".Length), out id)) {
                            Article A1 = new Article();
                            var sql = "SELECT * FROM articles WHERE id = @id";
                            using (var cmd = new MySqlCommand(sql, data.connection)) {
                                cmd.Parameters.AddWithValue("@id", id);
                                using (var reader = cmd.ExecuteReader()) {
                                    if (reader.Read()) {
                                        if (reader.IsDBNull(1)) {
                                            responseString = "Endpoint non pris en charge";
                                        }
                                        else {
                                            A1.Id = reader.GetInt32("id");
                                            A1.Name = reader.GetString("name");
                                            A1.Quantity = reader.GetInt32("quantity");
                                            responseString = JsonSerializer.Serialize(A1);
                                        }
                                    }
                                }
                            }  
                        }      
                    }

                    // Get all articles
                    else if (request.Url.LocalPath == "/articles") {
                        string query = "SELECT * FROM articles";
                        MySqlCommand command = new MySqlCommand(query, data.connection);
                        MySqlDataReader reader = command.ExecuteReader();

                        List<Article> getAllarticles = new List<Article>(); // Créer une liste pour stocker les articles

                        while (reader.Read())
                        {
                            Article Article1 = new Article();
                            Article1.Id = reader.GetInt32("id");
                            Article1.Name = reader.GetString("name");
                            Article1.Quantity = reader.GetInt32("quantity");
                            getAllarticles.Add(Article1); // Ajouter chaque article à la liste
                        }
                        reader.Close();

                        responseString = JsonSerializer.Serialize(getAllarticles);
                    }

                    // Get User
                    if (request.Url.LocalPath.StartsWith("/users/")) {
                        int id;
                        if (int.TryParse(request.Url.LocalPath.Substring("/users/".Length), out id)) {
                            User User1 = new User();
                            var sql = "SELECT * FROM users WHERE id = @id";
                            using (var cmd = new MySqlCommand(sql, data.connection)) {
                                cmd.Parameters.AddWithValue("@id", id);
                                using (var reader = cmd.ExecuteReader()) {
                                    if (reader.Read()) {
                                        if (reader.IsDBNull(1)) {
                                            responseString = "Endpoint non pris en charge";
                                        }
                                        else {
                                            User1.Id = reader.GetInt32("id");
                                            User1.Username = reader.GetString("username");
                                            responseString = JsonSerializer.Serialize(User1);
                                        }
                                    }
                                }
                            } 
                        }       
                    }

                    // Get all users
                    else if (request.Url.LocalPath == "/users") {
                        string query = "SELECT * FROM users";
                        MySqlCommand command = new MySqlCommand(query, data.connection);
                        MySqlDataReader reader = command.ExecuteReader();

                        List<User> getAllUsers = new List<User>(); // Créer une liste pour stocker les Users

                        while (reader.Read())
                        {
                            User U1 = new User();
                            U1.Id = reader.GetInt32("id");
                            U1.Username = reader.GetString("username");;
                            getAllUsers.Add(U1); // Ajouter chaque User à la liste
                        }
                        reader.Close();

                        responseString = JsonSerializer.Serialize(getAllUsers); 
                    }
                    break;
                
                case "PUT":
                    // Update Article
                    if (request.Url.LocalPath.StartsWith("/articles/update/")) {
                        int id;
                        if (int.TryParse(request.Url.LocalPath.Substring("/articles/update/".Length), out id)) {
                            string sql = $"SELECT * FROM articles WHERE id = {id}";
                            MySqlCommand command = new MySqlCommand(sql, data.connection);
                            MySqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            Article item = new Article();
                            item.Id = reader.GetInt32(0);
                            item.Name = reader.GetString(1);
                            item.Quantity = reader.GetInt32(3);
                            reader.Close();
                            Console.WriteLine("What do you want to update? Name, Description, Quantity");
                            string updateString = Console.ReadLine();
                            switch (updateString){
                                case "Name":
                                    Console.WriteLine("Enter the new value");
                                    item.Name = Console.ReadLine();
                                    string sql_name = $"UPDATE articles SET name = '{item.Name}' WHERE id = {id}";
                                    MySqlCommand command_name = new MySqlCommand(sql_name, data.connection);
                                    MySqlDataReader reader_name = command_name.ExecuteReader();
                                    reader_name.Close();
                                    Console.WriteLine("Value Modified");
                                    break;
                                case "Quantity":
                                    Console.WriteLine("Enter the new value");
                                    item.Quantity = int.Parse(Console.ReadLine());
                                    string sql_quantity = $"UPDATE articles SET quantity = '{item.Quantity}' WHERE id = {id}";
                                    MySqlCommand command_quantity = new MySqlCommand(sql_quantity, data.connection);
                                    MySqlDataReader reader_quantity = command_quantity.ExecuteReader();
                                    reader_quantity.Close();
                                    Console.WriteLine("Value Modified");
                                    break;
                                default:
                                    Console.WriteLine("It doesn't exit!");
                                    break;
                            }
                        } 
                    }

                    // Update User
                    if (request.Url.LocalPath.StartsWith("/users/update/")) {
                        int id;
                        if (int.TryParse(request.Url.LocalPath.Substring("/users/update/".Length), out id)) {
                            string sql = $"SELECT * FROM users WHERE id = {id}";
                            MySqlCommand command = new MySqlCommand(sql, data.connection);
                            MySqlDataReader reader = command.ExecuteReader();
                            reader.Read();
                            User item = new User();
                            item.Id = reader.GetInt32(0);
                            item.Username = reader.GetString(1);
                            reader.Close();
                            Console.WriteLine("What do you want to update? Username, Email, Password");
                            string updateString = Console.ReadLine();
                            switch (updateString){
                                case "Username":
                                    Console.WriteLine("Enter the new value");
                                    item.Username = Console.ReadLine();
                                    string sql_username = $"UPDATE users SET username = '{item.Username}' WHERE id = {id}";
                                    MySqlCommand command_username = new MySqlCommand(sql_username, data.connection);
                                    MySqlDataReader reader_username = command_username.ExecuteReader();
                                    reader_username.Close();
                                    Console.WriteLine("Value Modified");
                                    break;
                                default:
                                    Console.WriteLine("It doesn't exit!");
                                    break;
                            }
                        } 
                    }
                    break;
                
                case "DELETE":
                    // Delete Article
                    int deleteId;
                    if (request.Url.LocalPath.StartsWith("/articles/delete")) {
                        Console.WriteLine("Enter the id of the products you want to delete");
                        deleteId = int.Parse(Console.ReadLine());
                        string sql = $"DELETE FROM articles WHERE id = {deleteId}";
                        MySqlCommand command = new MySqlCommand(sql, data.connection);
                        MySqlDataReader reader = command.ExecuteReader();
                        reader.Close();
                        responseString = "Article deleted";

                    }
                    // Delete User
                    if (request.Url.LocalPath.StartsWith("/users/delete")) {
                        Console.WriteLine("Enter the id of the products you want to delete");
                        deleteId = int.Parse(Console.ReadLine());

                        string sql = $"DELETE FROM users WHERE id = {deleteId}";
                        MySqlCommand command = new MySqlCommand(sql, data.connection);
                        MySqlDataReader reader = command.ExecuteReader();
                        reader.Close();
                        responseString = "User deleted";
                    }
                    break;
                




                default:
                    responseString = "Endpoint non pris en charge";
                    break;

            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }
    }   
}
