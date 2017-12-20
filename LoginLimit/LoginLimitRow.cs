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
        [DisplayName("User Name")]
        public String UserName
        {
            get { return Fields.UserName[this]; }
            set { Fields.UserName[this] = value; }
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
            public StringField UserName;
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
