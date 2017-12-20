namespace Test.Membership
{
    using Serenity.Data;
    using SpeedWE.Administration;
    using System;
    using System.ComponentModel;

    [ConnectionKey("Default")]
    [ReadPermission(PermissionKeys.Security)]
    [ModifyPermission(PermissionKeys.Security)]
    public sealed class LoginLimitRow : Row
    {
        [DisplayName("User Id")]
        public Int32? UserId
        {
            get { return Fields.UserId[this]; }
            set { Fields.UserId[this] = value; }
        }

        [DisplayName("Session Id")]
        public String SessionId
        {
            get { return Fields.SessionId[this]; }
            set { Fields.SessionId[this] = value; }
        }

        [DisplayName("Logged In")]
        public Boolean? LoggedIn
        {
            get { return Fields.LoggedIn[this]; }
            set { Fields.LoggedIn[this] = value; }
        }

        public static readonly RowFields Fields = new RowFields().Init();

        public LoginLimitRow()
            : base(Fields)
        {
        }

        public class RowFields : RowFieldsBase
        {
            public Int32Field UserId;
            public StringField SessionId;
            public BooleanField LoggedIn;

            public RowFields()
                : base("LoginsLimit")
            {
                LocalTextPrefix = "Administration.LoginLimit";
            }
        }
    }
}
