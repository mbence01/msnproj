using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Messenger
{
    internal class FriendRequest
    {
        private static string connStr = "Server=localhost;Database=Messenger;Integrated Security=SSPI;";

        private int id;
        private User user1;
        private User user2;
        private DateTime reqDate;
    
        public FriendRequest(int id, User user1, User user2, DateTime reqDate)
        {
            this.id = id;
            this.user1 = user1;
            this.user2 = user2;
            this.reqDate = reqDate;
        }

        public int Id { get { return id; } }
        public User User1 { get { return user1; } }
        public User User2 { get { return user2; } }
        public DateTime RequestDate { get { return reqDate; } }

        public bool Accept()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    string sqlCmd = "accept_friend_req";

                    SqlCommand cmd = new SqlCommand(sqlCmd, conn)
                    {
                        CommandType = System.Data.CommandType.StoredProcedure
                    };

                    SqlParameter param = new SqlParameter("@id", this.Id)
                    {
                        DbType = System.Data.DbType.Int32,
                        Size = 11
                    };

                    cmd.Parameters.Add(param);

                    var returnParam = cmd.Parameters.Add("@ReturnVal", System.Data.SqlDbType.Int);
                    returnParam.Direction = System.Data.ParameterDirection.ReturnValue;

                    cmd.ExecuteNonQuery();

                    return (int)returnParam.Value != -1;
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
    }
}
