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
        private int read;
        private DateTime mailDate;

        public List<object> MAILDATA_SET_MESSAGE;

        public Mail(int id)
        {
            this.id = id;

            MAILDATA_SET_MESSAGE = SetMailData();
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

        public int Read
        {
            get
            {
                return read;
            }
            set
            {
                read = value;
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

                            this.mailSubject = reader.GetValue(reader.GetOrdinal("MailSubject")).ToString();
                            this.mailBody = reader.GetValue(reader.GetOrdinal("MailBody")).ToString();
                            this.mailDate = DateTime.Parse(reader.GetValue(reader.GetOrdinal("MailDate")).ToString());
                            this.read = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("MailReadStatus")));

                            senderID = (int)reader.GetValue(reader.GetOrdinal("SenderID"));
                            addresseeID = (int)reader.GetValue(reader.GetOrdinal("AddresseeID"));

                            object parent = reader.GetValue(reader.GetOrdinal("ParentMail"));

                            if (parent == System.DBNull.Value)
                                this.parentMail = null;
                            else
                                this.parentMail = new Mail((int)parent);

                            this.sender = new User(senderID);
                            this.addressee = new User(addresseeID);
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
                    return new List<object>() { -1, e.Message };
                }
            }

            return new List<object>() { 0, "Mail has successfully been created." };
        }
        public List<object> Update()
        {
            if (id == 0)
                return new List<object>() { -1, "ID of Mail instance is not set. Set the ID and try again." };

            StringBuilder sb = new StringBuilder();

            string sqlCmd = "UPDATE Mails SET SenderID = @senderID, AddresseeID = @addresseeID, MailSubject = @mailSubject, MailBody = @mailBody, ParentMail = @parentMail, MailReadStatus = @read WHERE ID = @mailID";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter senderidParam = new SqlParameter("@senderID", this.sender.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    SqlParameter addresseeidParam = new SqlParameter("@addresseeID", this.addressee.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    SqlParameter subjectParam = new SqlParameter("@mailSubject", this.mailSubject)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 128
                    };

                    SqlParameter bodyParam = new SqlParameter("@mailBody", this.mailBody)
                    {
                        DbType = System.Data.DbType.String,
                        Size = 8000
                    };

                    SqlParameter parentmailParam;

                    if(this.parentMail == null)
                    {
                        parentmailParam = new SqlParameter("@parentMail", DBNull.Value)
                        {
                            DbType = System.Data.DbType.Int32,
                            Size = 11
                        };
                    } else
                    {
                        parentmailParam = new SqlParameter("@parentMail", this.parentMail.Id)
                        {
                            DbType = System.Data.DbType.Int32,
                            Size = 11
                        };
                    }
                    
                    SqlParameter readParam = new SqlParameter("@read", this.read)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };
                    SqlParameter mailidParam = new SqlParameter("@mailID", this.id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(senderidParam);
                    cmd.Parameters.Add(addresseeidParam);
                    cmd.Parameters.Add(subjectParam);
                    cmd.Parameters.Add(bodyParam);
                    cmd.Parameters.Add(parentmailParam);
                    cmd.Parameters.Add(readParam);
                    cmd.Parameters.Add(mailidParam);

                    cmd.Prepare();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                        return new List<object>() { -1, "Mail has not been updated. Unknown error occurred." };
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
            return new List<object>() { 0, "Mail has successfully been updated in database." };
        }

        public static Mail AddNew(int senderID, int addresseeID, string mailSubject, string mailBody, int? parentMail)
        {
            string sqlCmd = "add_new_mail";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn) { CommandType = System.Data.CommandType.StoredProcedure };

                    cmd.Parameters.Add(new SqlParameter("@senderid", senderID));
                    cmd.Parameters.Add(new SqlParameter("@addresseeid", addresseeID));
                    cmd.Parameters.Add(new SqlParameter("@body", mailBody));


                    if(mailSubject == null || mailSubject.Length == 0)
                        cmd.Parameters.Add(new SqlParameter("@subject", DBNull.Value));
                    else
                        cmd.Parameters.Add(new SqlParameter("@subject", mailSubject));

                    if (parentMail.HasValue)
                        cmd.Parameters.Add(new SqlParameter("@parent", parentMail));
                    else
                        cmd.Parameters.Add(new SqlParameter("@parent", DBNull.Value));

                    var returnParam = cmd.Parameters.Add("@ReturnVal", System.Data.SqlDbType.Int);
                    returnParam.Direction = System.Data.ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();

                    return new Mail((int)returnParam.Value);
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

        public static List<Mail> FindBy(int? senderid = null, int? addresseeid = null)
        {
            List<Mail> retArr = new List<Mail>();
            string sqlCmd;

            if (senderid.HasValue && !addresseeid.HasValue)
                sqlCmd = "SELECT * FROM Mails WHERE SenderID = @senderid ORDER BY ID DESC";
            else if(!senderid.HasValue && addresseeid.HasValue)
                sqlCmd = "SELECT * FROM Mails WHERE AddresseeID = @addresseeid ORDER BY ID DESC";
            else if(senderid.HasValue && addresseeid.HasValue)
                sqlCmd = "SELECT * FROM Mails WHERE SenderID = @senderid AND AddresseeID = @addresseeid ORDER BY ID DESC";
            else
                sqlCmd = "SELECT * FROM Mails ORDER BY ID DESC";


            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn);

                    SqlParameter senderParam = new SqlParameter("@senderid", senderid)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    SqlParameter addresseeParam = new SqlParameter("@addresseeid", addresseeid)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    if (senderid.HasValue)
                        cmd.Parameters.Add(senderParam);
                    if (addresseeid.HasValue)
                        cmd.Parameters.Add(addresseeParam);


                    cmd.Prepare();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int mailID = (int)reader.GetValue(reader.GetOrdinal("ID"));

                            Mail mailObj = new Mail(mailID);
                            retArr.Add(mailObj);
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

        public bool Delete()
        {
            using(SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string sqlCmd = "delete_mail";

                SqlCommand cmd = new SqlCommand(sqlCmd, conn) { CommandType = System.Data.CommandType.StoredProcedure };

                SqlParameter idParam = new SqlParameter("@currentid", this.Id)
                {
                    DbType = System.Data.DbType.Int32,
                    Size = 11
                };

                cmd.Parameters.Add(idParam);

                SqlParameter id2Param = new SqlParameter("@startid", this.Id)
                {
                    DbType = System.Data.DbType.Int32,
                    Size = 11
                };
                
                cmd.Parameters.Add(id2Param);
                
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
