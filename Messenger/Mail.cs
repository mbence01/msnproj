using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Messenger
{
    internal class Mail
    {
        private static string connStr = "Server=localhost;Database=Messenger;Integrated Security=SSPI;";

        private readonly int id;
        private User sender;
        private User addressee;
        private string mailSubject;
        private string mailBody;
        private Mail parentMail;
        private DateTime mailDate;

        public Mail(int id)
        {
            this.id = id;

            SetMailData();
        }

        public int Id
        {
            get
            {
                return id;
            }
        }
        public User Sender
        {
            get
            {
                return sender;
            }
            set
            {
                sender = value;
            }
        }
        public User Addressee
        {
            get
            {
                return addressee;
            }
            set
            {
                addressee = value;
            }
        }
        public string MailSubject
        {
            get
            {
                return mailSubject;
            }
            set
            {
                mailSubject = value;
            }
        }
        public string MailBody
        {
            get
            {
                return mailBody;
            }
            set
            {
                mailBody = value;
            }
        }

        public Mail ParentMail
        {
            get
            {
                return parentMail;
            }
            set
            {
                parentMail = value;
            }
        }

        public DateTime MailDate
        {
            get
            {
                return mailDate;
            }
        }

        private List<object> SetMailData()
        {
            if (id == 0)
                return new List<object>() { -1, "ID of Mail instance is not set. Set the ID and try again." };

            string sqlCmd = "SELECT * FROM Mails WHERE ID = @mailid";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter useridParam = new SqlParameter("@mailid", this.id)
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
                            int senderID;
                            int addresseeID;
                            int? parentMail;

                            this.mailSubject = reader.GetValue(reader.GetOrdinal("MailSubject")).ToString();
                            this.mailBody = reader.GetValue(reader.GetOrdinal("MailBody")).ToString();
                            this.mailDate = DateTime.Parse(reader.GetValue(reader.GetOrdinal("MailDate")).ToString());

                            senderID = (int)reader.GetValue(reader.GetOrdinal("SenderID"));
                            addresseeID = (int)reader.GetValue(reader.GetOrdinal("AddresseeID"));
                            parentMail = (int)reader.GetValue(reader.GetOrdinal("ParentMail"));

                            this.sender = new User(senderID);
                            this.addressee = new User(addresseeID);

                            if(parentMail.HasValue)
                            {
                                this.parentMail = new Mail(parentMail.Value);
                            }
                        }
                    }
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

            return new List<object>() { 0, "Mail has successfully been created." };
        }

        //TODO: Finish class
        /*
        public List<object> Update()
        {
            if (id == 0)
                return new List<object>() { -1, "ID of User instance is not set. Set the ID and try again." };

            string sqlCmd = "UPDATE " +
                            "   Users " +
                            "SET " +
                            "   Users.Username = @username," +
                            "   Users.EmailAddr = @email," +
                            "   UserPasswords.PasswordHash = @hash " +
                            "FROM " +
                            "   Users " +
                            "   INNER JOIN " +
                            "       UserPasswords " +
                            "   ON  " +
                            "       Users.ID = UserPasswords.UserID " +
                            "WHERE Users.ID = @userid";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    cmd.Parameters.Add(new SqlParameter("@userid", this.id));
                    cmd.Parameters.Add(new SqlParameter("@username", this.username));
                    cmd.Parameters.Add(new SqlParameter("@email", this.email));
                    cmd.Parameters.Add(new SqlParameter("@hash", this.password));

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

            using (SqlConnection conn = new SqlConnection(connStr))
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
                catch (Exception e)
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
        }*/
    }
}
