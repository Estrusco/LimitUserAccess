using Serenity.Data;
using System.Collections.Generic;
using System.Linq;

namespace Test.Membership
{
    public static class LoginLimit
    {
        public static bool IsYourLoginStillTrue(string userName, string sid)
        {
            var q = new SqlQuery().Select("*").From(LoginLimitRow.Fields.TableName, new Alias("T0"))
                    .Where(new Criteria(LoginLimitRow.Fields.LoggedIn) == 1 &&
                          new Criteria(LoginLimitRow.Fields.UserName) == userName &&
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

        public static bool IsUserLoggedOnElsewhere(string userName, string sid)
        {
            var q = new SqlQuery().Select("*").From(LoginLimitRow.Fields.TableName, new Alias("T0"))
                    .Where(new Criteria(LoginLimitRow.Fields.LoggedIn) == 1 &&
                          new Criteria(LoginLimitRow.Fields.UserName) == userName &&
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

        public static void LogEveryoneElseOut(string userName, string sid)
        {
            var q = new SqlQuery().Select("*").From(LoginLimitRow.Fields.TableName, new Alias("T0"))
                    .Where(new Criteria(LoginLimitRow.Fields.LoggedIn) == 1 &&
                          new Criteria(LoginLimitRow.Fields.UserName) == userName &&
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
                                UserName = item.UserName,
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
