using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Messenger
{
    internal class User
    {
        private static string connStr = "Server=localhost;Database=Messenger;Integrated Security=SSPI;";

        private readonly int id;
        private string username;
        private string password;
        private string email;
        private DateTime regDate;

        public User(int id)
        {
            this.id = id;

            SetUserData();
        }

        public int Id { 
            get 
            {
                return id;
            } 
        }
        public string Username { 
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }
        public string Password { 
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }
        public string Email {
            get
            {
                return email;
            }
            set 
            {
                email = value;
            } 
        }
        public DateTime RegDate { 
            get
            {
                return regDate;
            }    
        }

        private List<object> SetUserData()
        {
            if (id == 0)
                return new List<object>() { -1, "ID of User instance is not set. Set the ID and try again." };

            string sqlCmd = "SELECT TOP(1) * FROM UsersWithPassword WHERE UserID = @userid ORDER BY CreatedAt DESC";

            using(SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter useridParam = new SqlParameter("@userid", this.id)
                    {
                        SqlDbType = System.Data.SqlDbType.Int,
                        Size = 11
                    };

                    cmd.Parameters.Add(useridParam);

                    cmd.Prepare();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            this.username = reader.GetValue(reader.GetOrdinal("Username")).ToString();
                            this.password = reader.GetValue(reader.GetOrdinal("UserPassword")).ToString();
                            this.email    = reader.GetValue(reader.GetOrdinal("EmailAddr")).ToString();
                            this.regDate  = DateTime.Parse(reader.GetValue(reader.GetOrdinal("RegisterDate")).ToString());
                        }
                    }
                } catch(Exception e)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                    return new List<object>() { -1, e.Message };
                }
            }

            return new List<object>() { 0, "User has successfully been created." };
        }

        public List<object> Update()
        {
            if (id == 0)
                return new List<object>() { -1, "ID of User instance is not set. Set the ID and try again." };

            string sqlCmd = "UPDATE Users SET Users.Username = @username, Users.EmailAddr = @email, UserPasswords.PasswordHash = @hash FROM Users INNER JOIN UserPasswords ON Users.ID = UserPasswords.UserID WHERE Users.ID = @userid";
            
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter useridParam = new SqlParameter("@userid", this.id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    SqlParameter usernameParam = new SqlParameter("@username", this.username)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 32
                    };

                    SqlParameter emailParam = new SqlParameter("@email", this.email)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 64
                    };

                    SqlParameter pwdhashParam = new SqlParameter("@hash", this.password)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 64
                    };

                    cmd.Parameters.Add(useridParam);
                    cmd.Parameters.Add(usernameParam);
                    cmd.Parameters.Add(emailParam);
                    cmd.Parameters.Add(pwdhashParam);

                    cmd.Prepare();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        return new List<object>() { -1, "User has not been updated. Unknown error occurred." };
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                    return new List<object>() { -1, e.Message };
                }
            }
            return new List<object>() { 0, "User has successfully been updated in database." };
        }

        public static User AddNew(string username, string password, string email)
        {
            string sqlCmd = "add_new_user";
            string hashedPwd = CreateHashedPassword(password);

            using(SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn) { CommandType = System.Data.CommandType.StoredProcedure };

                    cmd.Parameters.Add(new SqlParameter("@username", username));
                    cmd.Parameters.Add(new SqlParameter("@password", hashedPwd));
                    cmd.Parameters.Add(new SqlParameter("@email", email));
                    
                    var returnParam = cmd.Parameters.Add("@ReturnVal", System.Data.SqlDbType.Int);
                    returnParam.Direction = System.Data.ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();

                    return new User((int)returnParam.Value);
                }
                catch(Exception e)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                    return null;
                }
            }
        }

        public static List<User> FindBy(string username, string password = null)
        {
            List<User> retArr = new List<User>();
            string sqlCmd;

            if (password == null)
                sqlCmd = "SELECT Users.ID AS ID FROM Users WHERE Username = @username";
            else
                sqlCmd = "SELECT Users.ID AS ID FROM Users INNER JOIN UserPasswords ON Users.ID = UserPasswords.UserID WHERE Username = @username AND PasswordHash = @password";


            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter usernameParam = new SqlParameter("@username", username)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 32
                    };

                    SqlParameter passwordParam = new SqlParameter("@password", CreateHashedPassword(password))
                    {
                        DbType = System.Data.DbType.String,
                        Size = 64
                    };

                    cmd.Parameters.Add(usernameParam);

                    if (password != null)
                        cmd.Parameters.Add(passwordParam);

                    cmd.Prepare();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userID = (int)reader.GetValue(reader.GetOrdinal("ID"));

                            User userObj = new User(userID);
                            retArr.Add(userObj);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name + 
                        " -> " + e.Message);
                    return null;
                }
            }
            return retArr;
        }

        public static bool IsEmailAddrExists(string email)
        {
            List<User> retArr = new List<User>();
            string sqlCmd = "SELECT * FROM Users WHERE EmailAddr = @email";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter emailParam = new SqlParameter("@email", email)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 64                    
                    };

                    cmd.Parameters.Add(emailParam);
                    cmd.Prepare();

                    var foundRecords = cmd.ExecuteScalar();

                    return foundRecords != null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                    return false;
                }
            }
        }

        private static string CreateHashedPassword(string password)
        {
            using(MD5 md5 = MD5.Create())
            {
                return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}
