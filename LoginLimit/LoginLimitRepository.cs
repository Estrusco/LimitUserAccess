namespace Test.Membership
{
    using Serenity.Data;
    using System.Data;
    using System.Linq;
    using MyRow = LoginLimitRow;

    public class LoginLimitRepository
    {
        private static MyRow.RowFields fld { get { return MyRow.Fields; } }

        public void CreateOrUpdate(MyRow request)
        {
            var connection = SqlConnections.NewFor<MyRow>();

            var q = new SqlQuery().Select("*").From(MyRow.Fields.TableName, new Alias("T0"))
                    .Where(new Criteria(MyRow.Fields.UserId) == request.UserId.Value &&
                           new Criteria(MyRow.Fields.SessionId) == request.SessionId);

            var result = connection.Query<MyRow>(q);
            if (result != null)
            {
                var list = result.ToList();
                if (list.Count > 0)
                {
                    Update(connection, request);
                    return;
                }
            }

            Create(connection, request);

            connection.Close();
        }

        public void Create(IDbConnection connection, MyRow request)
        {
            UnitOfWork uow = new UnitOfWork(connection);

            new SqlInsert(MyRow.Fields.TableName)
                    .Set(fld.UserId, request.UserId)
                    .Set(fld.SessionId, request.SessionId)
                    .Set(fld.LoggedIn, request.LoggedIn)
                    .Execute(connection);

            uow.Commit();
        }

        public void Update(IDbConnection connection, MyRow request)
        {
            UnitOfWork uow = new UnitOfWork(connection);

            new SqlUpdate(MyRow.Fields.TableName)
                    .Set(MyRow.Fields.LoggedIn, request.LoggedIn)
                    .Where(new Criteria(MyRow.Fields.UserId) == request.UserId.Value &&
                           new Criteria(MyRow.Fields.SessionId) == request.SessionId)
                    .Execute(uow.Connection, ExpectedRows.Ignore);

            uow.Commit();
        }

        public void Delete(MyRow request)
        {
            var connection = SqlConnections.NewFor<MyRow>();
            UnitOfWork uow = new UnitOfWork(connection);

            new SqlDelete(MyRow.Fields.TableName)
                .Where(new Criteria(MyRow.Fields.UserId) == request.UserId.Value &&
                       new Criteria(MyRow.Fields.SessionId) == request.SessionId)
                .Execute(uow.Connection, ExpectedRows.Ignore);

            uow.Commit();
            connection.Close();
        }
    }
}
