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

        public static int LOGIN_TYPE_LOGIN  = 0;
        public static int LOGIN_TYPE_LOGOUT = 1;

        private readonly int id;
        private string username;
        private string password;
        private string email;
        private DateTime regDate;
        private int status;

        public List<object> USERDATA_SET_MESSAGE;

        public User(int id)
        {
            this.id = id;

            USERDATA_SET_MESSAGE = SetUserData();
        }

        public override string ToString()
        {
            return email;
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

        public int Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
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
                            this.status   = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("UserStatus")));
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


            string sqlCmd = "update_user";

            using(SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };

                    SqlParameter usernameParam = new SqlParameter("@username", this.Username)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 32
                    };

                    SqlParameter emailParam = new SqlParameter("@email", this.Email)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 64
                    };

                    SqlParameter statusParam = new SqlParameter("@status", this.Status)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    SqlParameter pwdParam = new SqlParameter("@pwdHash", this.Password)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 64
                    };

                    SqlParameter idParam = new SqlParameter("@id", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(usernameParam);
                    cmd.Parameters.Add(emailParam);
                    cmd.Parameters.Add(statusParam);
                    cmd.Parameters.Add(pwdParam);
                    cmd.Parameters.Add(idParam);

                    cmd.Prepare();
                    cmd.ExecuteNonQuery();

                    return new List<object>() { 0, "User has successfully been updated in database." }; ;
                }
                catch(Exception e) when(e is SqlException || e is InvalidOperationException)
                {
                    Console.WriteLine(
                           System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                           System.Reflection.MethodBase.GetCurrentMethod().Name +
                           " -> " + e.Message);
                    return new List<object>() { -1, e.Message };
                }
            }
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
                sqlCmd = "SELECT Users.ID AS ID FROM Users INNER JOIN UserPasswords ON Users.ID = UserPasswords.UserID WHERE Username = @username AND PasswordHash = @password AND UserPasswords.CreatedAt = (SELECT TOP(1) CreatedAt FROM UserPasswords WHERE UserID = Users.ID ORDER BY CreatedAt DESC)";


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


                    if(password != null)
                    {
                        SqlParameter passwordParam = new SqlParameter("@password", CreateHashedPassword(password))
                        {
                            DbType = System.Data.DbType.String,
                            Size = 64
                        };
                        cmd.Parameters.Add(passwordParam);
                    }


                    cmd.Parameters.Add(usernameParam);

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
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name + 
                        " -> " + e.Message);
                    return null;
                }
            }
            return retArr;
        }

        public List<FriendRequest> GetFriendRequests()
        {
            List<FriendRequest> retArr = new List<FriendRequest>();
            string sqlCmd = "SELECT * FROM FriendRequests WHERE User2 = @userid AND Accepted = 0";


            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter idParam = new SqlParameter("@userid", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(idParam);

                    cmd.Prepare();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id           = (int)reader.GetValue(reader.GetOrdinal("ID"));
                            int user1        = (int)reader.GetValue(reader.GetOrdinal("User1"));
                            int user2        = (int)reader.GetValue(reader.GetOrdinal("User2"));
                            DateTime reqDate = DateTime.Parse(reader.GetValue(reader.GetOrdinal("RequestDate")).ToString());

                            User userObj1 = new User(user1);
                            User userObj2 = new User(user2);

                            retArr.Add(new FriendRequest(id, userObj1, userObj2, reqDate));
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

        public bool RemoveFriend(User other)
        {
            string sqlCmd = "DELETE FROM Friends WHERE (User1 = @user1 AND User2 = @user2) OR (User1 = @user2 AND User2 = @user1)";

            using(SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter user1Param = new SqlParameter("@user1", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    SqlParameter user2Param = new SqlParameter("@user2", other.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(user1Param);
                    cmd.Parameters.Add(user2Param);

                    cmd.Prepare();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
                catch(Exception e) when(e is SqlException || e is InvalidOperationException)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                    return false;
                }
            }
        }

        public List<User> GetFriends()
        {
            List<User> retArr = new List<User>();
            string sqlCmd = "SELECT * FROM Friends WHERE User1 = @userid OR User2 = @userid";


            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter idParam = new SqlParameter("@userid", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(idParam);

                    cmd.Prepare();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int user1 = (int)reader.GetValue(reader.GetOrdinal("User1"));
                            int user2 = (int)reader.GetValue(reader.GetOrdinal("User2"));

                            if (user1 == this.Id)
                                retArr.Add(new User(user2));
                            else
                                retArr.Add(new User(user1));
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

        public bool AddFriendRequest(int userId)
        {
            using(SqlConnection conn = new SqlConnection(connStr))
            {
                try 
                {
                    conn.Open();

                    string sqlCmd = "new_friend_req";

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };

                    SqlParameter user1Param = new SqlParameter("@user1", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    SqlParameter user2Param = new SqlParameter("@user2", userId)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(user1Param);
                    cmd.Parameters.Add(user2Param);

                    var returnParam = cmd.Parameters.Add("@ReturnVal", System.Data.SqlDbType.Int);
                    returnParam.Direction = System.Data.ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();

                    return ((int)returnParam.Value) != -1;
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

        public static int IsEmailAddrExists(string email)
        {
            List<User> retArr = new List<User>();
            string sqlCmd = "SELECT ID FROM Users WHERE EmailAddr = @email";

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

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            int userID = (int)reader.GetValue(reader.GetOrdinal("ID"));
                            return userID;
                        }
                    }
                    return 0;
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                    return 0;
                }
            }
        }

        public void AddUserLog(int type)
        {
            if (type != 0 && type != 1)
                return;

            string sqlCmd = "INSERT INTO UserLogins(UserID, LoginType) VALUES(@userid, @logintype)";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    cmd.Parameters.Add(new SqlParameter("@userid", this.id));
                    cmd.Parameters.Add(new SqlParameter("@logintype", type));

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                }
            }
        }
        public List<string> GetFriendRequestHistory(int type) // 0: who, 1: whom
        {
            List<string> retArr = new List<string>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string sqlCmd = "";

                    if(type == 0)
                        sqlCmd = "SELECT * FROM FriendRequests WHERE User1 = @userid ORDER BY ID DESC";
                    else
                        sqlCmd = "SELECT * FROM FriendRequests WHERE User2 = @userid ORDER BY ID DESC";

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter param = new SqlParameter("@userid", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(param);
                    cmd.Prepare();

                    StringBuilder sb = new StringBuilder();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader.GetValue(reader.GetOrdinal("ID"));
                            int user1 = (int)reader.GetValue(reader.GetOrdinal("User1"));
                            int user2 = (int)reader.GetValue(reader.GetOrdinal("User2"));
                            string reqDate = reader.GetValue(reader.GetOrdinal("RequestDate")).ToString();

                            User userObj1 = new User(user1);
                            User userObj2 = new User(user2);

                            sb.Clear();

                            if (type == 0)
                                sb.Append("You");
                            else
                                sb.Append(userObj1.Username);


                            sb.Append(" sent a friend request to ");
                            

                            if (type == 0)
                                sb.Append(userObj2.Username);
                            else
                                sb.Append("you");

                            sb.Append(" at ");
                            sb.Append(reqDate);


                            retArr.Add(sb.ToString());
                        }
                        return retArr;
                    }
                }
                catch (Exception e) when (e is SqlException || e is InvalidOperationException)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                    return null;
                }
            }
        }

        public List<string> GetPasswordHistory()
        {
            List<string> retArr = new List<string>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string sqlCmd = "SELECT CreatedAt FROM UserPasswords WHERE UserID = @userid ORDER BY ID DESC";

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter param = new SqlParameter("@userid", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(param);
                    cmd.Prepare();

                    StringBuilder sb = new StringBuilder();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string date = reader.GetValue(reader.GetOrdinal("CreatedAt")).ToString();

                            sb.Clear();

                            sb.Append("You changed your password at ");
                            sb.Append(date);

                            retArr.Add(sb.ToString());
                        }
                        return retArr;
                    }
                }
                catch (Exception e) when (e is SqlException || e is InvalidOperationException)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                    return null;
                }
            }
        }

        public List<string> GetUserLogins()
        {
            List<string> retArr = new List<string>();

            using(SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string sqlCmd = "SELECT * FROM UserLogins WHERE UserID = @userid ORDER BY ID DESC";

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter param = new SqlParameter("@userid", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(param);
                    cmd.Prepare();

                    StringBuilder sb = new StringBuilder();
                    
                    using(SqlDataReader reader = cmd.ExecuteReader()) 
                    {
                        while(reader.Read())
                        {
                            int type = (int)reader.GetValue(reader.GetOrdinal("LoginType"));
                            string date = reader.GetValue(reader.GetOrdinal("LoginDate")).ToString();

                            sb.Clear();

                            sb.Append(this.Username);

                            if (type == 0)
                                sb.Append(" logged in at ");
                            else
                                sb.Append(" logged out at ");

                            sb.Append(date);

                            retArr.Add(sb.ToString());
                        }
                        return retArr;
                    }
                }
                catch(Exception e) when(e is SqlException || e is InvalidOperationException)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                    return null;
                }
            }
        }

        public void Delete()
        {
            using(SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string sqlCmd = "DELETE FROM Users WHERE ID = @id";

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter idParam = new SqlParameter("@id", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(idParam);
                    cmd.Prepare();

                    cmd.ExecuteNonQuery();
                }
                catch(Exception e) when(e is SqlException || e is InvalidOperationException)
                {
                    Console.WriteLine(
                        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                        System.Reflection.MethodBase.GetCurrentMethod().Name +
                        " -> " + e.Message);
                }
            }
        }

        public static string CreateHashedPassword(string password)
        {
            using(MD5 md5 = MD5.Create())
            {
                return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}
