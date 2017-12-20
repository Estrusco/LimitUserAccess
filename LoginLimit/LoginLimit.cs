using Serenity.Data;
using System.Collections.Generic;
using System.Linq;

namespace Test.Membership
{
    public static class LoginLimit
    {
        public static bool IsYourLoginStillTrue(int userId, string sid)
        {
            var q = new SqlQuery().Select("*").From(LoginLimitRow.Fields.TableName, new Alias("T0"))
                    .Where(new Criteria(LoginLimitRow.Fields.LoggedIn) == 1 &&
                          new Criteria(LoginLimitRow.Fields.UserId) == userId &&
                          new Criteria(LoginLimitRow.Fields.SessionId) == sid);

            var connection = SqlConnections.NewFor<LoginLimitRow>();
            var result = connection.Query<LoginLimitRow>(q);
            if (result != null)
            {
                var list = result.ToList();
                if (list.Count > 0)
                {
                    connection.Close();
                    return true;
                }
            }

            connection.Close();
            return false;
        }

        public static bool IsUserLoggedOnElsewhere(int userId, string sid)
        {
            var q = new SqlQuery().Select("*").From(LoginLimitRow.Fields.TableName, new Alias("T0"))
                    .Where(new Criteria(LoginLimitRow.Fields.LoggedIn) == 1 &&
                          new Criteria(LoginLimitRow.Fields.UserId) == userId &&
                          new Criteria(LoginLimitRow.Fields.SessionId) != sid);

            var connection = SqlConnections.NewFor<LoginLimitRow>();
            var result = connection.Query<LoginLimitRow>(q);
            if (result != null)
            {
                var list = result.ToList();
                if (list.Count > 0)
                {
                    connection.Close();
                    return true;
                }
            }

            connection.Close();
            return false;
        }

        public static void LogEveryoneElseOut(int userId, string sid)
        {
            var q = new SqlQuery().Select("*").From(LoginLimitRow.Fields.TableName, new Alias("T0"))
                    .Where(new Criteria(LoginLimitRow.Fields.LoggedIn) == 1 &&
                          new Criteria(LoginLimitRow.Fields.UserId) == userId &&
                          new Criteria(LoginLimitRow.Fields.SessionId) != sid);

            List<LoginLimitRow> list = new List<LoginLimitRow>();
            var connection = SqlConnections.NewFor<LoginLimitRow>();
            var result = connection.Query<LoginLimitRow>(q);
            if (result != null)
            {
                list = result.ToList();
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        new LoginLimitRepository().Update(connection,
                            new LoginLimitRow()
                            {
                                UserId = item.UserId,
                                SessionId = item.SessionId,
                                LoggedIn = false
                            }
                        );
                    }
                }
            }

            connection.Close();
        }
    }
}
